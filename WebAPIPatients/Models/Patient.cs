namespace WebAPIPatients.Models
{
    public class Patient
    {
        public int PatientId { get; set; }

        public string DocumentType { get; set; } = null!;
        public string DocumentNumber { get; set; } = null!;

        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;

        public DateTime BirthDate { get; set; }

        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
