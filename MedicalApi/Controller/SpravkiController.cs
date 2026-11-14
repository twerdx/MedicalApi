using MedicalApi.Data;
using MedicalApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class SpravkiController : ControllerBase
{
    private readonly MedicalDbContext _db;

    public SpravkiController(MedicalDbContext db)
    {
        _db = db;
    }

    // GET /api/spravki?patientId=1&minPrice=200&sort=price
    [HttpGet]
    [ProducesResponseType(typeof(List<Spravka>), 200)]
    public async Task<List<Spravka>> GetAll(int? patientId, decimal? minPrice, string? sort)
    {
        IQueryable<Spravka> query = _db.Spravki;

        if (patientId != null)
        {
            query = query.Where(s => s.PatientId == patientId);
        }

        if (minPrice != null)
        {
            query = query.Where(s => s.Price >= minPrice);
        }

        query = sort == "price"
            ? query.OrderBy(s => s.Price)
            : query.OrderBy(s => s.Id);

        return await query.ToListAsync();
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(Spravka), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<Spravka>> GetById(int id)
    {
        Spravka? spravka = await _db.Spravki.FirstOrDefaultAsync(s => s.Id == id);

        if (spravka == null)
        {
            return NotFound();
        }

        return spravka;
    }

    [HttpPost]
    [ProducesResponseType(typeof(Spravka), 201)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<Spravka>> Create(Spravka spravka)
    {
        SpravkaType? type = await _db.SpravkaTypes
            .FirstOrDefaultAsync(t => t.Id == spravka.SpravkaTypeId);

        if (type == null)
        {
            return BadRequest("Такого типа справки не существует");
        }

        Patient? patient = await _db.Patients
            .FirstOrDefaultAsync(p => p.Id == spravka.PatientId);

        if (patient == null)
        {
            return BadRequest("Пациента с таким PatientId не существует");
        }

        spravka.Id = 0;                  // номер выдаёт база
        spravka.Name = type.Name;
        spravka.PatientName = patient.Name;
        spravka.Price = type.Price;      // цена фиксируется на момент выдачи

        _db.Spravki.Add(spravka);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = spravka.Id }, spravka);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(Spravka), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<Spravka>> Update(int id, Spravka spravka)
    {
        Spravka? existingSpravka = await _db.Spravki.FirstOrDefaultAsync(s => s.Id == id);

        if (existingSpravka == null)
        {
            return NotFound();
        }

        existingSpravka.Name = spravka.Name;
        existingSpravka.PatientName = spravka.PatientName;
        existingSpravka.Price = spravka.Price;

        await _db.SaveChangesAsync();

        return Ok(existingSpravka);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<ActionResult> Delete(int id)
    {
        Spravka? spravka = await _db.Spravki.FirstOrDefaultAsync(s => s.Id == id);

        if (spravka == null)
        {
            return NotFound();
        }

        _db.Spravki.Remove(spravka);
        await _db.SaveChangesAsync();

        return NoContent();
    }
}