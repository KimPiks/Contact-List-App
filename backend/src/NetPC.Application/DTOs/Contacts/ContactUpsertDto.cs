using System.ComponentModel.DataAnnotations;

namespace NetPC.Application.DTOs.Contacts;

public class ContactUpsertDto
{
    [Required]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required]
    public DateTime? DateOfBirth { get; set; }

    [Required]
    public int? CategoryId { get; set; }

    public int? SubcategoryId { get; set; }
}
