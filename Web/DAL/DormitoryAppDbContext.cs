using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;
using Web.Models;

namespace Web.DAL
{
    public class DormitoryAppDbContext : DbContext
    {

        public DormitoryAppDbContext() : base("DormitoryAppDbContext")
        {   
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Room> Rooms { get; set; } 
        public DbSet<UserRoom> UserRooms { get; set; } 
        public DbSet<RoomApplication> RoomApplications { get; set; } 
        public DbSet<DormitoryAnnouncement> DormitoryAnnouncements { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }



    }
}