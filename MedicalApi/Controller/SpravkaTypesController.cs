using Microsoft.AspNetCore.Mvc;
using MedicalApi.Models;

[ApiController]
[Route("api/[controller]")]
public class SpravkaTypesController : ControllerBase
{
    private static List<SpravkaType> _types = new List<SpravkaType>
    {
        new SpravkaType { Id = 1, Name = "Справка для бассейна", Price = 500 },
        new SpravkaType { Id = 2, Name = "Медицинская справка", Price = 700 },
        new SpravkaType { Id = 3, Name = "Справка для университета", Price = 600 },
        new SpravkaType { Id = 4, Name = "Справка о состоянии здоровья", Price = 800 }
    };

    [HttpGet]
    public List<SpravkaType> GetAll()
    {
        return _types;
    }

    [HttpGet("{id}")]
    public ActionResult<SpravkaType> GetById(int id)
    {
        SpravkaType? type = _types.FirstOrDefault(t => t.Id == id);

        if (type == null)
        {
            return NotFound();
        }

        return Ok(type);
    }

    [HttpPost]
    public ActionResult<SpravkaType> Create(SpravkaType type)
    {
        type.Id = _types.Count == 0
            ? 1
            : _types.Max(t => t.Id) + 1;        _types.Add(type);

        return CreatedAtAction(
            nameof(GetById),
            new { id = type.Id },
            type
        );
    }

    [HttpPut("{id}")]
    public ActionResult<SpravkaType> Update(int id, SpravkaType type)
    {
        SpravkaType? existingType = _types.FirstOrDefault(t => t.Id == id);

        if (existingType == null)
        {
            return NotFound();
        }

        existingType.Name = type.Name;
        existingType.Price = type.Price;

        return Ok(existingType);
    }

    [HttpDelete("{id}")]
    public ActionResult Delete(int id)
    {
        SpravkaType? type = _types.FirstOrDefault(t => t.Id == id);

        if (type == null)
        {
            return NotFound();
        }

        _types.Remove(type);

        return NoContent();
    }
    public static SpravkaType? GetTypeById(int id)
    {
        return _types.FirstOrDefault(t => t.Id == id);
    }
}