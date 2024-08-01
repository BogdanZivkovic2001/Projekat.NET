using Microsoft.EntityFrameworkCore;

namespace Prodavnica.Services
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
            
        }
    }
}
