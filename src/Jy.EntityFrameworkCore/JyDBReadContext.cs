using Jy.Domain.Entities;
using Jy.IRepositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jy.EntityFramewordCoreBase;
namespace Jy.EntityFrameworkCore
{
    //http://www.cnblogs.com/CreateMyself/p/6613949.html
    public class JyDBReadContext : DbContext
    {
        public JyDBReadContext(DbContextOptions<JyDBReadContext> options) : base(options)
        {

        }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Menu> Menus { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserIndex> UserIndexs { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<RoleMenu> RoleMenus { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            //http://www.cnblogs.com/CreateMyself/p/6995403.html
            builder.Entity<User>(b =>
            {
                b.ToTable("Users");
                b.Property(e => e.Id).ValueGeneratedOnAdd();
                b.HasKey(e => e.Id);
                b.Property(e => e.CreateTime);
                b.Property(e => e.MobileNumber).HasColumnType("VARCHAR(36)");
                b.Property(e => e.EMail).HasColumnType("VARCHAR(100)");
                b.Property(e => e.IsDeleted);
                b.Property(e => e.LastLoginTime);
                b.Property(e => e.LoginTimes);
                b.Property(e => e.Name).HasColumnType("VARCHAR(72)");
                b.Property(e => e.Password).HasColumnType("VARCHAR(72)");
                b.Property(e => e.UserName).HasColumnType("VARCHAR(72)");
                b.Property(e => e.Remarks).HasColumnType("VARCHAR(200)");
                b.Property(e => e.CreateUserId).HasColumnType("VARCHAR(36)");
            });
            //Department User  一对多
            builder.Entity<Department>(b =>
            {
                b.ToTable("Departments");
                b.Property(e => e.Id).ValueGeneratedOnAdd();
                b.HasKey(e => e.Id);
                b.Property(e => e.Code).HasColumnType("VARCHAR(36)");
                b.Property(e => e.ContactNumber).HasColumnType("VARCHAR(36)");
                b.Property(e => e.CreateTime);
                b.Property(e => e.CreateUserId).HasColumnType("VARCHAR(36)");
                b.Property(e => e.IsDeleted);
                b.Property(e => e.Manager).HasColumnType("VARCHAR(36)");
                b.Property(e => e.Name).HasColumnType("VARCHAR(72)");
                b.Property(e => e.ParentId).HasColumnType("VARCHAR(36)");
                b.Property(e => e.Remarks).HasColumnType("VARCHAR(200)");
                b.HasMany(e => e.Users).WithOne(e => e.Department).HasForeignKey(e => e.DepartmentId);
            });

            builder.Entity<Menu>(b =>
            {
                b.ToTable("Menus");
                b.Property(e => e.Id).ValueGeneratedOnAdd();
                b.HasKey(e => e.Id);
                b.Property(e => e.Code).HasColumnType("VARCHAR(36)");
                b.Property(e => e.Icon).HasColumnType("VARCHAR(36)");
                b.Property(e => e.Name).HasColumnType("VARCHAR(72)");
                b.Property(e => e.ParentId).HasColumnType("VARCHAR(36)");
                b.Property(e => e.SerialNumber);
                b.Property(e => e.Type);
                b.Property(e => e.Url).HasColumnType("VARCHAR(200)");
                b.Property(e => e.Remarks).HasColumnType("VARCHAR(200)");
            });

            builder.Entity<Role>(b =>
            {
                b.ToTable("Roles");
                b.Property(e => e.Id).ValueGeneratedOnAdd();
                b.HasKey(e => e.Id);
                b.Property(e => e.Code).HasColumnType("VARCHAR(36)");
                b.Property(e => e.CreateTime);
                b.Property(e => e.CreateUserId).HasColumnType("VARCHAR(36)");
                b.Property(e => e.Name).HasColumnType("VARCHAR(72)");
                b.Property(e => e.Remarks).HasColumnType("VARCHAR(200)");
            });

            //user role 多对多
            builder.Entity<UserRole>(b =>
            {
                b.ToTable("UserRoles");
                b.Property(e => e.UserId);
                b.Property(e => e.RoleId);
                b.HasKey(p => new { p.UserId, p.RoleId });

                b.HasOne(r => r.User)
                      .WithMany(r => r.UserRoles)
                      .HasForeignKey(r => r.UserId);
                b.HasOne(m => m.Role)
                    .WithMany(m => m.RoleUsers)
                    .HasForeignKey(m => m.RoleId);
            });
            //role menu 多对多
            builder.Entity<RoleMenu>(b =>
            {
                b.ToTable("RoleMenus");
                b.Property(e => e.RoleId);
                b.Property(e => e.MenuId);
                b.HasKey(p => new { p.RoleId, p.MenuId });

                b.HasOne(r => r.Role)
                      .WithMany(r => r.RoleMenus)
                      .HasForeignKey(r => r.RoleId);
                b.HasOne(m => m.Menu)
                    .WithMany(m => m.MenuRoles)
                    .HasForeignKey(m => m.MenuId);
            });
            builder.Entity<UserIndex>(b =>
            {
                b.ToTable("UserIndexs");
                b.Property(e => e.UserId);
                b.HasKey(e => e.UserId);
                b.Property(e => e.UserName).HasColumnType("VARCHAR(72)");
                b.Property(e => e.Password).HasColumnType("VARCHAR(72)");
                b.Property(e => e.DepartmentId).HasColumnType("VARCHAR(36)");
            });

            //启用Guid主键类型扩展
            builder.HasPostgresExtension("uuid-ossp");

            base.OnModelCreating(builder);
        }
        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            base.OnConfiguring(builder);
        }

    }

}

