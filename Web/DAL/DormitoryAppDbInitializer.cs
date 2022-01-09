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

            User systemAdmingUserItem = new User()
            {
                Name = "System",
                Lastname = "Admin",
                Email = "systemadmin@mail.com",
                Password = "1",
                UserRole = User.Role.SystemAdmin,
                UserGender = User.UserGenderEnum.Unknown,
                UserStatus = User.UserStatusEnum.Active,
                IsDeleted = false,
                CreatedDate = DateTime.Now, 
                CreatedUserId = -1
            }; 

            User studentItem = new User()
            {
                Name = "Ozan",
                Lastname = "Kaplan",
                Email = "okaplan@st.medipol.edu.tr",
                Password = "1",
                UserRole = User.Role.Student,
                UserGender = User.UserGenderEnum.Male,
                UserStatus = User.UserStatusEnum.Passive,
                IsDeleted = false,
                CreatedDate = DateTime.Now,
                CreatedUserId = 1
            };
            context.Users.Add(systemAdmingUserItem);
            context.Users.Add(studentItem);
            context.SaveChanges();

        }
    }
}