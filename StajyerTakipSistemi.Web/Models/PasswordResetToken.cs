namespace StajyerTakipSistemi.Web.Models
{
    public class PasswordResetToken
    { 
        public int Id { get; set; }

        public string Guid { get; set; }

        public string Token { get; set; }

        public DateTime ExpirationTime { get; set; }
    }
}
