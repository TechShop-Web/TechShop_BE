using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserService.Repository.Models;
using System.Security.Cryptography;
using System.Text;


namespace UserService.Repository.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
          
            builder.HasData(
                new User
                {
                    Id = 1,
                    Email = "admin@techshop.vn",
                    Password = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                    FullName = "Nguyễn Văn Admin",
                    Role = "Admin",
                },
                new User
                {
                    Id = 2,
                    Email = "manager@techshop.vn",
                    Password = BCrypt.Net.BCrypt.HashPassword("Manager@123"),
                    FullName = "Trần Thị Manager",
                    Role = "Manager",
                },
                new User
                {
                    Id = 3,
                    Email = "user1@gmail.com",
                    Password = BCrypt.Net.BCrypt.HashPassword("User@123"),
                    FullName = "Lê Văn User",
                    Role = "User",
                },
                new User
                {
                    Id = 4,
                    Email = "user2@gmail.com",
                    Password = BCrypt.Net.BCrypt.HashPassword("User@123"),
                    FullName = "Phạm Thị Hương",
                    Role = "User",
                },
                new User
                {
                    Id = 5,
                    Email = "customer@techshop.vn",
                    Password = BCrypt.Net.BCrypt.HashPassword("Customer@123"),
                    FullName = "Hoàng Minh Khách",
                    Role = "User",
                }
            );
        }

    }
}