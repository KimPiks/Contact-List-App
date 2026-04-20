using Microsoft.EntityFrameworkCore;
using NetPC.Application.Contacts;
using NetPC.Application.DTOs.Contacts;
using NetPC.Domain.Contact;

namespace NetPC.Infrastructure.Contacts;

public class ContactService : IContactService
{
    private readonly AppDbContext _dbContext;

    public ContactService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyCollection<ContactDto>> GetAllAsync()
    {
        var contacts = await _dbContext.Contacts
            .AsNoTracking()
            .Include(contact => contact.Category)
            .Include(contact => contact.Subcategory)
            .ToListAsync();

        return contacts.Select(Map)
            .OrderBy(contact => contact.LastName)
            .ThenBy(contact => contact.FirstName)
            .ToList();
    }

    public async Task<ContactDto?> GetByIdAsync(Guid id)
    {
        var contact = await _dbContext.Contacts
            .AsNoTracking()
            .Include(item => item.Category)
            .Include(item => item.Subcategory)
            .FirstOrDefaultAsync(contact => contact.Id == id);

        return contact is null ? null : Map(contact);
    }

    public async Task<IReadOnlyCollection<CategoryDto>> GetCategoriesAsync()
    {
        return await _dbContext.ContactCategories
            .AsNoTracking()
            .OrderBy(category => category.Name)
            .Select(category => new CategoryDto
            {
                Id = category.Id,
                Name = category.Name
            })
            .ToListAsync();
    }

    public async Task<IReadOnlyCollection<SubcategoryDto>> GetSubcategoriesAsync(int? categoryId = null)
    {
        var query = _dbContext.ContactSubcategories
            .AsNoTracking()
            .AsQueryable();

        if (categoryId.HasValue)
        {
            query = query.Where(subcategory => subcategory.CategoryId == categoryId.Value);
        }

        return await query
            .OrderBy(subcategory => subcategory.Name)
            .Select(subcategory => new SubcategoryDto
            {
                Id = subcategory.Id,
                CategoryId = subcategory.CategoryId,
                Name = subcategory.Name
            })
            .ToListAsync();
    }

    public async Task<ContactDto> CreateAsync(ContactUpsertDto dto)
    {
        var contact = await CreateEntityAsync(dto);

        _dbContext.Contacts.Add(contact);
        await _dbContext.SaveChangesAsync();

        return (await GetByIdAsync(contact.Id))
            ?? throw new InvalidOperationException("Created contact could not be loaded.");
    }

    public async Task<ContactDto?> UpdateAsync(Guid id, ContactUpsertDto dto)
    {
        var contact = await _dbContext.Contacts.FirstOrDefaultAsync(contact => contact.Id == id);
        if (contact is null)
        {
            return null;
        }

        await ApplyFieldsAsync(contact, dto);

        await _dbContext.SaveChangesAsync();
        return await GetByIdAsync(id);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var contact = await _dbContext.Contacts.FirstOrDefaultAsync(contact => contact.Id == id);
        if (contact is null)
        {
            return false;
        }

        _dbContext.Contacts.Remove(contact);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    private async Task<Contact> CreateEntityAsync(ContactUpsertDto dto)
    {
        var contact = new Contact { Id = Guid.NewGuid() };
        await ApplyFieldsAsync(contact, dto);
        return contact;
    }

    private async Task ApplyFieldsAsync(Contact contact, ContactUpsertDto dto)
    {
        var categoryId = dto.CategoryId ?? throw new ArgumentException("CategoryId is required.");
        var dateOfBirth = dto.DateOfBirth ?? throw new ArgumentException("DateOfBirth is required.");

        var categoryExists = await _dbContext.ContactCategories.AnyAsync(category => category.Id == categoryId);
        if (!categoryExists)
        {
            throw new ArgumentException($"Category with id {categoryId} was not found.");
        }

        if (dto.SubcategoryId.HasValue)
        {
            var subcategoryMatchesCategory = await _dbContext.ContactSubcategories
                .AnyAsync(subcategory => subcategory.Id == dto.SubcategoryId.Value && subcategory.CategoryId == categoryId);

            if (!subcategoryMatchesCategory)
            {
                throw new ArgumentException($"Subcategory with id {dto.SubcategoryId.Value} was not found for category {categoryId}.");
            }
        }

        contact.FirstName = dto.FirstName;
        contact.LastName = dto.LastName;
        contact.Email = dto.Email;
        contact.PhoneNumber = dto.PhoneNumber;
        contact.DateOfBirth = dateOfBirth;
        contact.CategoryId = categoryId;
        contact.SubcategoryId = dto.SubcategoryId;
    }

    private static ContactDto Map(Contact contact)
    {
        return new ContactDto
        {
            Id = contact.Id,
            FirstName = contact.FirstName,
            LastName = contact.LastName,
            Email = contact.Email,
            PhoneNumber = contact.PhoneNumber,
            DateOfBirth = contact.DateOfBirth,
            CategoryId = contact.CategoryId,
            CategoryName = contact.Category.Name,
            SubcategoryId = contact.SubcategoryId,
            SubcategoryName = contact.Subcategory?.Name
        };
    }
}

