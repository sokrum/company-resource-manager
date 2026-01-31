using Api.Data;
using Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ResourcesController : ControllerBase
{
    private readonly AppDbContext db;

    public ResourcesController(AppDbContext db)
    {
        this.db = db;
    }

    [HttpGet]
    public async Task<List<ResourceItem>> GetAll()
        => await db.Resources.AsNoTracking().OrderBy(x => x.Id).ToListAsync();

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ResourceItem>> Get(int id)
    {
        var item = await db.Resources.FindAsync(id);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<ResourceItem>> Create(ResourceItem item)
    {
        item.Id = 0;
        item.UpdatedAtUtc = DateTime.UtcNow;

        db.Resources.Add(item);
        await db.SaveChangesAsync();

        return CreatedAtAction(nameof(Get), new { id = item.Id }, item);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, ResourceItem item)
    {
        var existing = await db.Resources.FindAsync(id);
        if (existing is null) return NotFound();

        existing.Name = item.Name;
        existing.Type = item.Type;
        existing.Description = item.Description;
        existing.IsAvailable = item.IsAvailable;
        existing.UpdatedAtUtc = DateTime.UtcNow;

        await db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var existing = await db.Resources.FindAsync(id);
        if (existing is null) return NotFound();

        db.Resources.Remove(existing);
        await db.SaveChangesAsync();
        return NoContent();
    }
}
