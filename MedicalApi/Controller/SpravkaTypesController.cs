using MedicalApi.Data;
using MedicalApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class SpravkaTypesController : ControllerBase
{
    private readonly MedicalDbContext _db;

    public SpravkaTypesController(MedicalDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<List<SpravkaType>> GetAll()
    {
        return await _db.SpravkaTypes.OrderBy(t => t.Id).ToListAsync();
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<SpravkaType>> GetById(int id)
    {
        SpravkaType? type = await _db.SpravkaTypes.FirstOrDefaultAsync(t => t.Id == id);

        if (type == null)
        {
            return NotFound();
        }

        return Ok(type);
    }

    [HttpPost]
    public async Task<ActionResult<SpravkaType>> Create(SpravkaType type)
    {
        type.Id = 0; // номер выдаёт база

        _db.SpravkaTypes.Add(type);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = type.Id }, type);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<SpravkaType>> Update(int id, SpravkaType type)
    {
        SpravkaType? existingType = await _db.SpravkaTypes.FirstOrDefaultAsync(t => t.Id == id);

        if (existingType == null)
        {
            return NotFound();
        }

        existingType.Name = type.Name;
        existingType.Price = type.Price;

        await _db.SaveChangesAsync();

        return Ok(existingType);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<ActionResult> Delete(int id)
    {
        SpravkaType? type = await _db.SpravkaTypes.FirstOrDefaultAsync(t => t.Id == id);

        if (type == null)
        {
            return NotFound();
        }

        if (await _db.Spravki.AnyAsync(s => s.SpravkaTypeId == id))
        {
            return BadRequest("Нельзя удалить вид справки: по нему уже выписаны справки");
        }

        _db.SpravkaTypes.Remove(type);
        await _db.SaveChangesAsync();

        return NoContent();
    }
}