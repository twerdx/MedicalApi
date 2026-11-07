using System.ComponentModel.DataAnnotations;

namespace MedicalApi.Models;

public class Spravka
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Название справки обязательно")]
    public string Name { get; set; } = "";

    [Required(ErrorMessage = "Имя пациента обязательно")]
    public string PatientName { get; set; } = "";

    [Range(1, 100000, ErrorMessage = "Цена должна быть от 1 до 100000")]
    public decimal Price { get; set; }

    public int PatientId { get; set; }

    public int SpravkaTypeId { get; set; }
}