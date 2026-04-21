using Microsoft.EntityFrameworkCore;
using NetPC.Application.Contacts;
using NetPC.Domain.Contact;

namespace NetPC.Infrastructure.Contacts;

public class ContactRepository : IContactRepository
{
    private readonly AppDbContext _dbContext;

    public ContactRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyCollection<Contact>> GetAllAsync()
    {
        return await _dbContext.Contacts
            .AsNoTracking()
            .Include(c => c.Category)
            .Include(c => c.Subcategory)
            .ToListAsync();
    }

    public async Task<Contact?> GetByIdAsync(Guid id)
    {
        return await _dbContext.Contacts
            .Include(c => c.Category)
            .Include(c => c.Subcategory)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<bool> EmailExistsAsync(string email, Guid excludeId)
    {
        return await _dbContext.Contacts
            .AnyAsync(c => c.Email == email && c.Id != excludeId);
    }

    public async Task<Category?> GetCategoryByIdAsync(int id)
    {
        return await _dbContext.ContactCategories.FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<IReadOnlyCollection<Category>> GetAllCategoriesAsync()
    {
        return await _dbContext.ContactCategories
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<IReadOnlyCollection<Subcategory>> GetSubcategoriesAsync(int? categoryId = null)
    {
        var query = _dbContext.ContactSubcategories.AsNoTracking().AsQueryable();

        if (categoryId.HasValue)
            query = query.Where(s => s.CategoryId == categoryId.Value);

        return await query.OrderBy(s => s.Name).ToListAsync();
    }

    public async Task<Subcategory?> GetSubcategoryAsync(int id, int categoryId)
    {
        return await _dbContext.ContactSubcategories
            .FirstOrDefaultAsync(s => s.Id == id && s.CategoryId == categoryId);
    }

    public async Task<Subcategory?> GetSubcategoryByNameAsync(string name, int categoryId)
    {
        return await _dbContext.ContactSubcategories
            .FirstOrDefaultAsync(s => s.CategoryId == categoryId && s.Name == name);
    }

    public async Task AddSubcategoryAsync(Subcategory subcategory)
    {
        _dbContext.ContactSubcategories.Add(subcategory);
        await _dbContext.SaveChangesAsync();
    }

    public async Task AddAsync(Contact contact)
    {
        _dbContext.Contacts.Add(contact);
        await _dbContext.SaveChangesAsync();
    }

    public async Task RemoveAsync(Contact contact)
    {
        _dbContext.Contacts.Remove(contact);
        await _dbContext.SaveChangesAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }
}
