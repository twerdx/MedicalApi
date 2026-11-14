using MedicalApi.Data;
using MedicalApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class AppointmentsController : ControllerBase
{
    private readonly MedicalDbContext _db;

    public AppointmentsController(MedicalDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<List<Appointment>> GetAll()
    {
        return await _db.Appointments.OrderBy(a => a.Id).ToListAsync();
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Appointment>> GetById(int id)
    {
        Appointment? appointment = await _db.Appointments.FirstOrDefaultAsync(a => a.Id == id);

        if (appointment == null)
        {
            return NotFound();
        }

        return Ok(appointment);
    }

    // AppointmentDate отправляйте в UTC с буквой Z: "2026-11-20T04:00:00Z"
    [HttpPost]
    [ProducesResponseType(typeof(Appointment), 201)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<Appointment>> Create(Appointment appointment)
    {
        if (!await _db.Patients.AnyAsync(p => p.Id == appointment.PatientId))
        {
            return BadRequest("Пациента с таким PatientId не существует");
        }

        appointment.Id = 0;

        _db.Appointments.Add(appointment);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = appointment.Id }, appointment);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(Appointment), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<Appointment>> Update(int id, Appointment appointment)
    {
        Appointment? existing = await _db.Appointments.FirstOrDefaultAsync(a => a.Id == id);

        if (existing == null)
        {
            return NotFound();
        }

        if (!await _db.Patients.AnyAsync(p => p.Id == appointment.PatientId))
        {
            return BadRequest("Пациента с таким PatientId не существует");
        }

        existing.PatientId = appointment.PatientId;
        existing.DoctorName = appointment.DoctorName;
        existing.AppointmentDate = appointment.AppointmentDate;
        existing.Status = appointment.Status;

        await _db.SaveChangesAsync();

        return Ok(existing);
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id)
    {
        Appointment? appointment = await _db.Appointments.FirstOrDefaultAsync(a => a.Id == id);

        if (appointment == null)
        {
            return NotFound();
        }

        _db.Appointments.Remove(appointment);
        await _db.SaveChangesAsync();

        return NoContent();
    }

    [HttpGet("search")]
    public async Task<ActionResult<List<Appointment>>> Search(string? doctorName, string? status)
    {
        IQueryable<Appointment> query = _db.Appointments;

        if (!string.IsNullOrEmpty(doctorName))
        {
            query = query.Where(a => EF.Functions.ILike(a.DoctorName, $"%{doctorName}%"));
        }

        if (!string.IsNullOrEmpty(status))
        {
            query = query.Where(a => a.Status == status);
        }

        return Ok(await query.OrderBy(a => a.Id).ToListAsync());
    }

    [HttpGet("statistics")]
    public async Task<IActionResult> GetStatistics()
    {
        int total = await _db.Appointments.CountAsync();
        int planned = await _db.Appointments.CountAsync(a => a.Status == "Запланирован");
        int completed = await _db.Appointments.CountAsync(a => a.Status == "Завершён");
        int cancelled = await _db.Appointments.CountAsync(a => a.Status == "Отменён");

        return Ok(new
        {
            Total = total,
            Planned = planned,
            Completed = completed,
            Cancelled = cancelled
        });
    }
}