using StajyerTakipSistemi.Web.Models;

namespace StajyerTakipSistemi.Web
{
    public class UserInternExist
    {
        private readonly StajyerTakipSistemiDbContext _context;

        public UserInternExist(StajyerTakipSistemiDbContext context)
        {
            _context = context;
        }

        public bool CheckGuid(string guid)
        {
            var internUser = _context.SInterns
                    .Where(a => a.Guid.Equals(Guid.Parse(guid)))
                    .FirstOrDefault();

            if (internUser != null) { return true; }
            else { return false; }
        }
    }
}