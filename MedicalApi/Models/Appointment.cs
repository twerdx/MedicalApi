using System.ComponentModel.DataAnnotations;

namespace MedicalApi.Models;

public class Appointment
{
    public int Id { get; set; }

    public int PatientId { get; set; }

    [Required(ErrorMessage = "Имя врача обязательно")]
    public string DoctorName { get; set; } = "";

    public DateTime AppointmentDate { get; set; }

    public string Status { get; set; } = "Запланирован";
}