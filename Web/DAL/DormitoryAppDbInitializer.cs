using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Web.Models;

namespace Web.DAL
{
    public class DormitoryAppDbInitializer : System.Data.Entity.DropCreateDatabaseIfModelChanges<DormitoryAppDbContext>
    {
        protected override void Seed(DormitoryAppDbContext context)
        {

            User userItem = new User()
            {
                Name = "System",
                Lastname = "Admin",
                Email = "systemadmin@mail.com",
                Password = "1",
                UserRole = User.Role.SystemAdmin,
                UserGender = User.Gender.Unknown,
                CreatedDate = DateTime.Now,
                CreatedUserId = -1
            };
            context.Users.Add(userItem);
            context.SaveChanges();

        }
    }
}