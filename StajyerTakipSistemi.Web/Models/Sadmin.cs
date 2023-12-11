namespace StajyerTakipSistemi.Web.Models
{
    public class Sadmin
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public Guid? Guid { get; set; }
        public string? Email { get; set; }
    }
}
