using NetPC.Domain.Contact;

namespace NetPC.Application.Contacts;

public interface IContactRepository
{
    Task<IReadOnlyCollection<Contact>> GetAllAsync();
    Task<Contact?> GetByIdAsync(Guid id);
    Task<bool> EmailExistsAsync(string email, Guid excludeId);
    Task<Category?> GetCategoryByIdAsync(int id);
    Task<IReadOnlyCollection<Category>> GetAllCategoriesAsync();
    Task<IReadOnlyCollection<Subcategory>> GetSubcategoriesAsync(int? categoryId = null);
    Task<Subcategory?> GetSubcategoryAsync(int id, int categoryId);
    Task<Subcategory?> GetSubcategoryByNameAsync(string name, int categoryId);
    Task AddSubcategoryAsync(Subcategory subcategory);
    Task AddAsync(Contact contact);
    Task RemoveAsync(Contact contact);
    Task SaveChangesAsync();
}
