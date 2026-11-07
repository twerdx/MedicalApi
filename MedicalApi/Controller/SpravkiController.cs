using Microsoft.AspNetCore.Mvc;
using MedicalApi.Models;

[ApiController]
[Route("api/[controller]")]
public class SpravkiController : ControllerBase
{
    private static List<Spravka> _spravki = new List<Spravka>();
    

    [HttpGet]  
    [ProducesResponseType(typeof(List<Spravka>), 200)]

    public List<Spravka> GetAll()
    {
        return _spravki;
    }
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Spravka), 200)]
    [ProducesResponseType(404)]
    public ActionResult<Spravka> GetById(int id)
    {
        Spravka? spravka = _spravki.FirstOrDefault(s => s.Id == id);

        if (spravka == null)
        {
            return NotFound();
        }

        return spravka;
    }

    [HttpPost] 
    [ProducesResponseType(typeof(Spravka), 201)]
    [ProducesResponseType(400)]
    public ActionResult<Spravka> Create(Spravka spravka)
    {
        SpravkaType? type = SpravkaTypesController.GetTypeById(spravka.SpravkaTypeId);

        if (type == null)
        {
            return BadRequest("Такого типа справки не существует");
        }

        spravka.Id = _spravki.Count == 0
            ? 1
            : _spravki.Max(s => s.Id) + 1;
        spravka.Name = type.Name;
        spravka.Price = type.Price;

        _spravki.Add(spravka);

        return CreatedAtAction(
            nameof(GetById),
            new { id = spravka.Id },
            spravka
        );
    }

    [HttpPut("{id}")]  
    [ProducesResponseType(typeof(Spravka), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public ActionResult<Spravka> Update(int id, Spravka spravka)
    {
        Spravka? existingSpravka = _spravki.FirstOrDefault(s => s.Id == id);

        if (existingSpravka == null)
        {
            return NotFound();
        }

        existingSpravka.Name = spravka.Name;
        existingSpravka.PatientName = spravka.PatientName;
        existingSpravka.Price = spravka.Price;

        return Ok(existingSpravka);
    }

    [HttpDelete("{id}")]    
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public ActionResult Delete(int id)
    {
        Spravka? spravka = _spravki.FirstOrDefault(s => s.Id == id);

        if (spravka == null)
        {
            return NotFound();
        }

        _spravki.Remove(spravka);

        return NoContent();
    }
    public static List<Spravka> GetByPatientId(int patientId)
    {
        return _spravki.Where(s => s.PatientId == patientId).ToList();
    }
}