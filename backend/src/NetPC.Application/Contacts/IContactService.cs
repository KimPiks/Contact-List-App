using NetPC.Application.DTOs.Contacts;

namespace NetPC.Application.Contacts;

public interface IContactService
{
    Task<IReadOnlyCollection<ContactDto>> GetAllAsync();
    Task<ContactDto?> GetByIdAsync(Guid id);
    Task<IReadOnlyCollection<CategoryDto>> GetCategoriesAsync();
    Task<IReadOnlyCollection<SubcategoryDto>> GetSubcategoriesAsync(int? categoryId = null);
    Task<ContactDto> CreateAsync(ContactUpsertDto dto);
    Task<ContactDto?> UpdateAsync(Guid id, ContactUpsertDto dto);
    Task<bool> DeleteAsync(Guid id);
}
