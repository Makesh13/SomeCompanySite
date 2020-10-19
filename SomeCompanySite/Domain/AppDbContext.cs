using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Net.Sockets;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SomeCompanySite.Domain.Entities;

namespace SomeCompanySite.Domain
{
    public class AppDbContext : IdentityDbContext<IdentityUser>//IdentityUser содержит все необходимые свойства, вроде телефонных номеров и т.д
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options){}

        public AppDbContext()
        {

        }
        public DbSet<TextField> TextFields { get; set; }//Создаем таблицы, проецирую классы на базу
        public DbSet<ServiceItem> ServiceItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<IdentityRole>().HasData(new IdentityRole   //Добавляем пользователя
            {
                Id = "ada7cfae-c6fe-4865-b581-30836cf4442a",
                Name = "admin",
                NormalizedName = "ADMIN"
            });

            modelBuilder.Entity<IdentityUser>().HasData(new IdentityUser
            {
                Id = "4b042a86-c77f-4b7d-ad70-85cbfbbf9181",
                UserName = "admin",
                NormalizedUserName = "ADMIN",
                EmailConfirmed = true,
                NormalizedEmail = "My@EMAIL.COM",
                PasswordHash = new PasswordHasher<IdentityUser>().HashPassword(null, "supperpassword"),//Так хешируем пароль
                SecurityStamp = string.Empty
            });

            modelBuilder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string>
            {
                RoleId = "ada7cfae-c6fe-4865-b581-30836cf4442a",
                UserId = "4b042a86-c77f-4b7d-ad70-85cbfbbf9181"
            });

            modelBuilder.Entity<TextField>().HasData(new TextField
            {
                Id = new Guid("d4182680-e973-4dbd-a408-92586f4dbf21"),
                CodeWord = "PageIndex",
                Title = "Главная",
            });
            modelBuilder.Entity<TextField>().HasData(new TextField
            {
                Id = new Guid("e79f5437-145f-45cb-92d6-ccf04f77628f"),
                CodeWord = "PageServices",
                Title = "Наши услуги"
            });
            modelBuilder.Entity<TextField>().HasData(new TextField
            {
                Id = new Guid("ac8634a8-f1d8-45ef-9aeb-3ce3d7c9a6f4"),
                CodeWord = "PageContacts",
                Title = "Контакты"
            });
        }
    }
}
