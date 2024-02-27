using StajyerTakipSistemi.Web.Models;

namespace StajyerTakipSistemi.Web
{
    public class UserManagerExist
    {
        private readonly StajyerTakipSistemiDbContext _context;

        public UserManagerExist(StajyerTakipSistemiDbContext context)
        {
            _context = context;
        }

        public bool CheckGuid(string guid)
        {
            var managerUser = _context.SManagers
                    .Where(a => a.Guid.Equals(Guid.Parse(guid)))
                    .FirstOrDefault();

            if (managerUser != null) { return true; }
            else { return false; }
        }
    }
}