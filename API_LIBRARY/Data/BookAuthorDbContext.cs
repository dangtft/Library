using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
namespace API_LIBRARY.Data
{
    public class BookAuthorDbContext : IdentityDbContext
    {
        public BookAuthorDbContext(DbContextOptions<BookAuthorDbContext> options) : base(options)
        {
        }
    }
}
