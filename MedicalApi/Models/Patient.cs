using System.ComponentModel.DataAnnotations;

namespace MedicalApi.Models;

public class Patient
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Имя пациента обязательно")]
    public string Name { get; set; } = "";

    [Required(ErrorMessage = "Дата рождения обязательна")]
    public DateTime BirthDate { get; set; }

    [Required(ErrorMessage = "Номер телефона обязателен")]
    [Phone(ErrorMessage = "Некорректный номер телефона")]
    public string Phone { get; set; } = "";

    [Required(ErrorMessage = "Email обязателен")]
    [EmailAddress(ErrorMessage = "Некорректный email")]
    public string Email { get; set; } = "";

    [Required(ErrorMessage = "Адрес обязателен")]
    public string Address { get; set; } = "";

    public bool IsInsured { get; set; }
    
}