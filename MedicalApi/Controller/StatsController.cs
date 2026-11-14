using MedicalApi.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/stats")]
public class StatsController : ControllerBase
{
    private readonly MedicalDbContext _db;

    public StatsController(MedicalDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        int patientsCount = await _db.Patients.CountAsync();
        int spravkiCount = await _db.Spravki.CountAsync();
        decimal totalAmount = await _db.Spravki.SumAsync(s => s.Price);

        return Ok(new
        {
            PatientsCount = patientsCount,
            SpravkiCount = spravkiCount,
            TotalAmount = totalAmount
        });
    }
}