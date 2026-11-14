using MedicalApi.Data;
using MedicalApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class PatientsController : ControllerBase
{
    private readonly MedicalDbContext _db;

    public PatientsController(MedicalDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<List<Patient>> GetAll(string? name)
    {
        IQueryable<Patient> query = _db.Patients;

        if (!string.IsNullOrWhiteSpace(name))
        {
            query = query.Where(p => EF.Functions.ILike(p.Name, $"%{name}%"));
        }

        return await query.OrderBy(p => p.Name).ToListAsync();
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Patient>> GetById(int id)
    {
        Patient? patient = await _db.Patients.FirstOrDefaultAsync(p => p.Id == id);

        if (patient == null)
        {
            return NotFound();
        }

        return patient;
    }

    [HttpPost]
    [ProducesResponseType(typeof(Patient), 201)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<Patient>> Create(Patient patient)
    {
        if (patient.BirthDate > DateOnly.FromDateTime(DateTime.Now))
        {
            return BadRequest("Дата рождения не может быть в будущем");
        }

        // номер выдаёт база, id из тела запроса игнорируем
        patient.Id = null;

        _db.Patients.Add(patient);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = patient.Id }, patient);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(Patient), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<Patient>> Update(int id, Patient patient)
    {
        Patient? existingPatient = await _db.Patients.FirstOrDefaultAsync(p => p.Id == id);

        if (existingPatient == null)
        {
            return NotFound();
        }

        if (patient.BirthDate > DateOnly.FromDateTime(DateTime.Now))
        {
            return BadRequest("Дата рождения не может быть в будущем");
        }

        existingPatient.Name = patient.Name;
        existingPatient.BirthDate = patient.BirthDate;
        existingPatient.Phone = patient.Phone;
        existingPatient.Email = patient.Email;
        existingPatient.Address = patient.Address;
        existingPatient.IsInsured = patient.IsInsured;

        await _db.SaveChangesAsync();

        return Ok(existingPatient);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<ActionResult> Delete(int id)
    {
        Patient? patient = await _db.Patients.FirstOrDefaultAsync(p => p.Id == id);

        if (patient == null)
        {
            return NotFound();
        }

        if (await _db.Spravki.AnyAsync(s => s.PatientId == id))
        {
            return BadRequest("Нельзя удалить пациента: у него есть выписанные справки");
        }

        if (await _db.Appointments.AnyAsync(a => a.PatientId == id))
        {
            return BadRequest("Нельзя удалить пациента: у него есть записи на приём");
        }

        _db.Patients.Remove(patient);
        await _db.SaveChangesAsync();

        return NoContent();
    }

    [HttpGet("{id:int}/spravki")]
    public async Task<ActionResult<List<Spravka>>> GetPatientSpravki(int id)
    {
        bool exists = await _db.Patients.AnyAsync(p => p.Id == id);

        if (!exists)
        {
            return NotFound();
        }

        List<Spravka> spravki = await _db.Spravki
            .Where(s => s.PatientId == id)
            .ToListAsync();

        return Ok(spravki);
    }
}