using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetPC.Application.Contacts;
using NetPC.Application.DTOs.Contacts;

namespace NetPC.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContactsController : ControllerBase
{
    private readonly IContactService _contactService;

    public ContactsController(IContactService contactService)
    {
        _contactService = contactService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<ContactDto>>> GetAll()
    {
        return Ok(await _contactService.GetAllAsync());
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ContactDto>> GetById(Guid id)
    {
        var contact = await _contactService.GetByIdAsync(id);
        return contact is null ? NotFound() : Ok(contact);
    }

    [HttpGet("categories")]
    public async Task<ActionResult<IReadOnlyCollection<CategoryDto>>> GetCategories()
    {
        return Ok(await _contactService.GetCategoriesAsync());
    }

    [HttpGet("subcategories")]
    public async Task<ActionResult<IReadOnlyCollection<SubcategoryDto>>> GetSubcategories([FromQuery] int? categoryId)
    {
        return Ok(await _contactService.GetSubcategoriesAsync(categoryId));
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<ContactDto>> Create([FromBody] ContactUpsertDto dto)
    {
        try
        {
            var created = await _contactService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize]
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ContactDto>> Update(Guid id, [FromBody] ContactUpsertDto dto)
    {
        try
        {
            var updated = await _contactService.UpdateAsync(id, dto);
            return updated is null ? NotFound() : Ok(updated);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _contactService.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}
