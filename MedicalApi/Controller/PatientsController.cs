using Microsoft.AspNetCore.Mvc;
using MedicalApi.Models;

[ApiController]
[Route("api/[controller]")]
public class PatientsController : ControllerBase
{
    private static List<Patient> _patients = new List<Patient>();

    [HttpPut("{id}")]
    public ActionResult<Patient> Update(int id, Patient patient)
    {
        Patient? existingPatient = _patients.FirstOrDefault(p => p.Id == id);

        if (existingPatient == null)
        {
            return NotFound();
        }

        existingPatient.Name = patient.Name;
        existingPatient.BirthDate = patient.BirthDate;
        existingPatient.Phone = patient.Phone;
        existingPatient.Email = patient.Email;
        existingPatient.Address = patient.Address;
        existingPatient.IsInsured = patient.IsInsured;

        return Ok(existingPatient);
    }

    [HttpDelete("{id}")]
    public ActionResult Delete(int id)
    {
        Patient? patient = _patients.FirstOrDefault(p => p.Id == id);

        if (patient == null)
        {
            return NotFound();
        }

        _patients.Remove(patient);

        return NoContent();
    }
    
    [HttpGet]
    public List<Patient> GetAll()
    {
        return _patients;
    }

    [HttpGet("{id}")]
    public ActionResult<Patient> GetById(int id)
    {
        Patient? patient = _patients.FirstOrDefault(p => p.Id == id);

        if (patient == null)
        {
            return NotFound();
        }

        return patient;
    }

    [HttpPost]
    public ActionResult<Patient> Create(Patient patient)
    {
        patient.Id = _patients.Count == 0
            ? 1
            : _patients.Max(p => p.Id) + 1;
        _patients.Add(patient);

        return CreatedAtAction(
            nameof(GetById),
            new { id = patient.Id },
            patient
        );
    }
    [HttpGet("{id}/spravki")]
    public ActionResult<List<Spravka>> GetPatientSpravki(int id)
    {
        Patient? patient = _patients.FirstOrDefault(p => p.Id == id);

        if (patient == null)
        {
            return NotFound();
        }

        List<Spravka> spravki = SpravkiController.GetByPatientId(id);

        return Ok(spravki);
    }
}