namespace registration_api.Models
{
    public class StudentPublic
    {
        public string Nome { get; set; }
        public string CPF { get; set; }
        public string Email { get; set; }
        public string RA { get; set; }
        public string HashId { get; set; }

        public StudentPublic() { }

        public StudentPublic(Student student)
        {
            Nome = student.Nome;
            CPF = student.CPF;
            Email = student.Email;
            RA = student.RA.ToString();
            HashId = student.HashId;
        }
    }
}
