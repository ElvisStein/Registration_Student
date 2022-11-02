namespace registration_api.Models
{
    public class Student
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string CPF { get; set; }
        public string Email { get; set; }
        public int RA { get; set; }
        public string HashId { get; set; }
    }
}
