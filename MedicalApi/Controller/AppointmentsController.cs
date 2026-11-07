using Microsoft.AspNetCore.Mvc;
using MedicalApi.Models;

[ApiController]
[Route("api/[controller]")]
public class AppointmentsController : ControllerBase
{
    private static List<Appointment> _appointments = new List<Appointment>();

    [HttpGet]
    public List<Appointment> GetAll()
    {
        return _appointments;
    }

    [HttpGet("{id}")]
    public ActionResult<Appointment> GetById(int id)
    {
        Appointment? appointment = _appointments.FirstOrDefault(a => a.Id == id);

        if (appointment == null)
        {
            return NotFound();
        }

        return Ok(appointment);
    }

    [HttpPost]
    public ActionResult<Appointment> Create(Appointment appointment)
    {
        appointment.Id = _appointments.Count == 0
            ? 1
            : _appointments.Max(a => a.Id) + 1;        _appointments.Add(appointment);

        return CreatedAtAction(
            nameof(GetById),
            new { id = appointment.Id },
            appointment
        );
    }

    [HttpPut("{id}")]
    public ActionResult<Appointment> Update(int id, Appointment appointment)
    {
        Appointment? existing = _appointments.FirstOrDefault(a => a.Id == id);

        if (existing == null)
        {
            return NotFound();
        }

        existing.PatientId = appointment.PatientId;
        existing.DoctorName = appointment.DoctorName;
        existing.AppointmentDate = appointment.AppointmentDate;
        existing.Status = appointment.Status;

        return Ok(existing);
    }

    [HttpDelete("{id}")]
    public ActionResult Delete(int id)
    {
        Appointment? appointment = _appointments.FirstOrDefault(a => a.Id == id);

        if (appointment == null)
        {
            return NotFound();
        }

        _appointments.Remove(appointment);

        return NoContent();
    }
    [HttpGet("search")]
    public ActionResult<List<Appointment>> Search(
        string? doctorName,
        string? status)
    {
        List<Appointment> result = _appointments;

        if (!string.IsNullOrEmpty(doctorName))
        {
            result = result
                .Where(a => a.DoctorName.Contains(
                    doctorName,
                    StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        if (!string.IsNullOrEmpty(status))
        {
            result = result
                .Where(a => a.Status == status)
                .ToList();
        }

        return Ok(result);
    }
    [HttpGet("statistics")]
    public IActionResult GetStatistics()
    {
        int total = _appointments.Count;

        int planned = _appointments.Count(
            a => a.Status == "Запланирован");

        int completed = _appointments.Count(
            a => a.Status == "Завершён");

        int cancelled = _appointments.Count(
            a => a.Status == "Отменён");

        return Ok(new
        {
            Total = total,
            Planned = planned,
            Completed = completed,
            Cancelled = cancelled
        });
    }
}