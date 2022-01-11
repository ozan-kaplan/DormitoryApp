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

            //User studentItem = new User()
            //{
            //    Name = "Ozan",
            //    Lastname = "Kaplan",
            //    Email = "okaplan@st.medipol.edu.tr",
            //    Password = "1",
            //    UserRole = User.Role.Student,
            //    UserGender = User.UserGenderEnum.Male,
            //    UserStatus = User.UserStatusEnum.Active,
            //    IsDeleted = false,
            //    CreatedDate = DateTime.Now,
            //    CreatedUserId = 1
            //};
            context.Users.Add(systemAdmingUserItem);
            //context.Users.Add(studentItem);


            for (int i = 0; i < 20; i++)
            {
                Room room = new Room()
                {
                    RoomName = "Room " + (i + 1),
                    RoomFee = 50,
                    CreatedDate = DateTime.Now,
                    CreatedUserId = -1,
                    RoomCapacity = 4,
                    CurrentCapacity = 0,
                    IsDeleted = false,
                };
                context.Rooms.Add(room);
            }  

            context.SaveChanges();

        }
    }
}