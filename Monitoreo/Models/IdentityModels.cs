using Microsoft.AspNet.Identity.EntityFramework;
using Monitoreo.Models.DAL;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace Monitoreo.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        //public int PersonalId { get; set; }
        //public virtual Personal PersonalAsociado { get; set; }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", false)
        {
        }

        //static ApplicationDbContext()
        //{
        //    System.Data.Entity.Database.SetInitializer(new ApplicationDbContextInitializer());
            
        //}
    }
}