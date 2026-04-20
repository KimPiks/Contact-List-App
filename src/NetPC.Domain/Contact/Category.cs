namespace NetPC.Domain.Contact;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public ICollection<Subcategory> Subcategories { get; set; } = new List<Subcategory>();
}