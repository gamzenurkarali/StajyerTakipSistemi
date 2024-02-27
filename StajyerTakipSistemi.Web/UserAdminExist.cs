using StajyerTakipSistemi.Web.Models;

namespace StajyerTakipSistemi.Web
{
    public class UserAdminExist
    {
        private readonly StajyerTakipSistemiDbContext _context;

        public UserAdminExist(StajyerTakipSistemiDbContext context)
        {
            _context = context;
        }

        public bool CheckGuid(string guid)
        {
            var adminUser = _context.admin
                    .Where(a => a.Guid.Equals(Guid.Parse(guid)))
                    .FirstOrDefault();

            if (adminUser != null) { return true; }
            else { return false; }
        }
    }
}