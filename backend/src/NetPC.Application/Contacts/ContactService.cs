using NetPC.Application.Contacts;
using NetPC.Application.DTOs.Contacts;
using NetPC.Application.Encryption;
using NetPC.Domain.Contact;

namespace NetPC.Application.Contacts;

/// <summary>
/// Service for managing contacts, including CRUD operations and password handling.
/// </summary>
public class ContactService : IContactService
{
    private readonly IContactRepository _repository;
    private readonly IEncryptionService _encryption;

    public ContactService(IContactRepository repository, IEncryptionService encryption)
    {
        _repository = repository;
        _encryption = encryption;
    }

    public async Task<IReadOnlyCollection<ContactDto>> GetAllAsync()
    {
        var contacts = await _repository.GetAllAsync();

        return contacts.Select(ToDto)
            .OrderBy(c => c.LastName)
            .ThenBy(c => c.FirstName)
            .ToList();
    }

    public async Task<ContactDto?> GetByIdAsync(Guid id)
    {
        var contact = await _repository.GetByIdAsync(id);
        return contact is null ? null : ToDto(contact);
    }

    public async Task<IReadOnlyCollection<CategoryDto>> GetCategoriesAsync()
    {
        var categories = await _repository.GetAllCategoriesAsync();
        return categories.Select(c => new CategoryDto { Id = c.Id, Name = c.Name }).ToList();
    }

    public async Task<IReadOnlyCollection<SubcategoryDto>> GetSubcategoriesAsync(int? categoryId = null)
    {
        var subcategories = await _repository.GetSubcategoriesAsync(categoryId);
        return subcategories.Select(s => new SubcategoryDto
        {
            Id = s.Id,
            CategoryId = s.CategoryId,
            Name = s.Name
        }).ToList();
    }

    public async Task<ContactDto> CreateAsync(CreateContactDto dto)
    {
        var contact = new Contact { Id = Guid.NewGuid() };
        await ToEntity(contact, dto);

        await _repository.AddAsync(contact);

        return (await GetByIdAsync(contact.Id))
            ?? throw new InvalidOperationException("Created contact could not be loaded.");
    }

    public async Task<ContactDto?> UpdateAsync(Guid id, CreateContactDto dto)
    {
        var contact = await _repository.GetByIdAsync(id);
        if (contact is null)
            return null;

        await ToEntity(contact, dto);
        await _repository.SaveChangesAsync();

        return await GetByIdAsync(id);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var contact = await _repository.GetByIdAsync(id);
        if (contact is null)
            return false;

        await _repository.RemoveAsync(contact);
        return true;
    }

    public async Task<string?> GetPasswordAsync(Guid id)
    {
        var contact = await _repository.GetByIdAsync(id);
        if (contact is null)
            return null;

        return _encryption.Decrypt(contact.EncryptedPassword);
    }

    private async Task ToEntity(Contact contact, CreateContactDto dto)
    {
        var categoryId = dto.CategoryId ?? throw new ArgumentException("CategoryId is required.");
        var dateOfBirth = dto.DateOfBirth ?? throw new ArgumentException("DateOfBirth is required.");

        // Check if contact exists
        if (await _repository.EmailExistsAsync(dto.Email, contact.Id))
            throw new ArgumentException("A contact with this email already exists.");

        // Check if category exists
        var category = await _repository.GetCategoryByIdAsync(categoryId)
            ?? throw new ArgumentException($"Category with id {categoryId} was not found.");

        // If subcategory is required for the category, check if it is passed
        if (category.Name == "Business" && !dto.SubcategoryId.HasValue)
            throw new ArgumentException("Subcategory is required for Business contacts.");

        int? subcategoryId = dto.SubcategoryId;

        // If category is Custom and custom subcategory is provided, check if it exists or create it
        if (category.Name == "Custom" && !string.IsNullOrWhiteSpace(dto.CustomSubcategory))
        {
            var existing = await _repository.GetSubcategoryByNameAsync(dto.CustomSubcategory.Trim(), categoryId);

            if (existing is not null)
            {
                subcategoryId = existing.Id;
            }
            else
            {
                var newSub = new Subcategory
                {
                    Name = dto.CustomSubcategory.Trim(),
                    CategoryId = categoryId
                };
                await _repository.AddSubcategoryAsync(newSub);
                subcategoryId = newSub.Id;
            }
        }

        // If subcategory is provided, check if it exists and belongs to the category
        if (subcategoryId.HasValue)
        {
            var sub = await _repository.GetSubcategoryAsync(subcategoryId.Value, categoryId);
            if (sub is null)
                throw new ArgumentException($"Subcategory with id {subcategoryId.Value} was not found for category {categoryId}.");
        }

        // Validate password
        PasswordValidator.ValidatePasswordComplexity(dto.Password);

        // Map fields
        contact.FirstName = dto.FirstName;
        contact.LastName = dto.LastName;
        contact.Email = dto.Email;
        contact.EncryptedPassword = _encryption.Encrypt(dto.Password);
        contact.PhoneNumber = dto.PhoneNumber;
        contact.DateOfBirth = DateTime.SpecifyKind(dateOfBirth, DateTimeKind.Utc);
        contact.CategoryId = categoryId;
        contact.SubcategoryId = subcategoryId;
    }
    
    private static ContactDto ToDto(Contact contact)
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
