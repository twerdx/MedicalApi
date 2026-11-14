using System.ComponentModel.DataAnnotations;

namespace MedicalApi.Models;

public class SpravkaType
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Название вида справки обязательно")]
    public string Name { get; set; } = "";

    [Range(1, 100000, ErrorMessage = "Цена должна быть от 1 до 100000")]
    public decimal Price { get; set; }
}