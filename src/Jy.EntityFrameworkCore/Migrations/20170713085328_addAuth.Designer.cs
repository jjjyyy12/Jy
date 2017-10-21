using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Jy.EntityFrameworkCore;

namespace Jy.EntityFrameworkCore.Migrations
{
    [DbContext(typeof(JyDbContext))]
    [Migration("20170713085328_addAuth")]
    partial class addAuth
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("Npgsql:PostgresExtension:uuid-ossp", "'uuid-ossp', '', ''")
                .HasAnnotation("ProductVersion", "1.1.2");

            modelBuilder.Entity("Jy.Domain.Entities.Department", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Code")
                        .HasColumnType("VARCHAR(36)");

                    b.Property<string>("ContactNumber")
                        .HasColumnType("VARCHAR(36)");

                    b.Property<DateTime?>("CreateTime");

                    b.Property<Guid>("CreateUserId")
                        .HasColumnType("VARCHAR(36)");

                    b.Property<int>("IsDeleted");

                    b.Property<string>("Manager")
                        .HasColumnType("VARCHAR(36)");

                    b.Property<string>("Name")
                        .HasColumnType("VARCHAR(72)");

                    b.Property<Guid>("ParentId")
                        .HasColumnType("VARCHAR(36)");

                    b.Property<string>("Remarks")
                        .HasColumnType("VARCHAR(200)");

                    b.HasKey("Id");

                    b.ToTable("Departments");
                });

            modelBuilder.Entity("Jy.Domain.Entities.Menu", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Code")
                        .HasColumnType("VARCHAR(36)");

                    b.Property<string>("Icon")
                        .HasColumnType("VARCHAR(36)");

                    b.Property<string>("Name")
                        .HasColumnType("VARCHAR(72)");

                    b.Property<Guid>("ParentId")
                        .HasColumnType("VARCHAR(36)");

                    b.Property<string>("Remarks")
                        .HasColumnType("VARCHAR(200)");

                    b.Property<int>("SerialNumber");

                    b.Property<int>("Type");

                    b.Property<string>("Url")
                        .HasColumnType("VARCHAR(200)");

                    b.HasKey("Id");

                    b.ToTable("Menus");
                });

            modelBuilder.Entity("Jy.Domain.Entities.Role", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Code")
                        .HasColumnType("VARCHAR(36)");

                    b.Property<DateTime?>("CreateTime");

                    b.Property<Guid>("CreateUserId")
                        .HasColumnType("VARCHAR(36)");

                    b.Property<string>("Name")
                        .HasColumnType("VARCHAR(72)");

                    b.Property<string>("Remarks")
                        .HasColumnType("VARCHAR(200)");

                    b.HasKey("Id");

                    b.HasIndex("CreateUserId");

                    b.ToTable("Roles");
                });

            modelBuilder.Entity("Jy.Domain.Entities.RoleMenu", b =>
                {
                    b.Property<Guid>("RoleId");

                    b.Property<Guid>("MenuId");

                    b.HasKey("RoleId", "MenuId");

                    b.HasIndex("MenuId");

                    b.ToTable("RoleMenus");
                });

            modelBuilder.Entity("Jy.Domain.Entities.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime?>("CreateTime");

                    b.Property<Guid>("CreateUserId")
                        .HasColumnType("VARCHAR(36)");

                    b.Property<Guid>("DepartmentId");

                    b.Property<string>("EMail")
                        .HasColumnType("VARCHAR(100)");

                    b.Property<int>("IsDeleted");

                    b.Property<DateTime>("LastLoginTime");

                    b.Property<int>("LoginTimes");

                    b.Property<string>("MobileNumber")
                        .HasColumnType("VARCHAR(36)");

                    b.Property<string>("Name")
                        .HasColumnType("VARCHAR(72)");

                    b.Property<string>("Password")
                        .HasColumnType("VARCHAR(72)");

                    b.Property<string>("Remarks")
                        .HasColumnType("VARCHAR(200)");

                    b.Property<string>("UserName")
                        .HasColumnType("VARCHAR(72)");

                    b.HasKey("Id");

                    b.HasIndex("CreateUserId");

                    b.HasIndex("DepartmentId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Jy.Domain.Entities.UserIndex", b =>
                {
                    b.Property<Guid>("UserId")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("DepartmentId")
                        .HasColumnType("VARCHAR(36)");

                    b.Property<string>("Password")
                        .HasColumnType("VARCHAR(72)");

                    b.Property<Guid?>("UserId1");

                    b.Property<string>("UserName")
                        .HasColumnType("VARCHAR(72)");

                    b.HasKey("UserId");

                    b.HasIndex("DepartmentId");

                    b.HasIndex("UserId1");

                    b.ToTable("UserIndexs");
                });

            modelBuilder.Entity("Jy.Domain.Entities.UserRole", b =>
                {
                    b.Property<Guid>("UserId");

                    b.Property<Guid>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("UserRoles");
                });

            modelBuilder.Entity("Jy.Domain.Entities.Role", b =>
                {
                    b.HasOne("Jy.Domain.Entities.User", "CreateUser")
                        .WithMany()
                        .HasForeignKey("CreateUserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Jy.Domain.Entities.RoleMenu", b =>
                {
                    b.HasOne("Jy.Domain.Entities.Menu", "Menu")
                        .WithMany("MenuRoles")
                        .HasForeignKey("MenuId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Jy.Domain.Entities.Role", "Role")
                        .WithMany("RoleMenus")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Jy.Domain.Entities.User", b =>
                {
                    b.HasOne("Jy.Domain.Entities.User", "CreateUser")
                        .WithMany()
                        .HasForeignKey("CreateUserId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Jy.Domain.Entities.Department", "Department")
                        .WithMany("Users")
                        .HasForeignKey("DepartmentId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Jy.Domain.Entities.UserIndex", b =>
                {
                    b.HasOne("Jy.Domain.Entities.Role", "Department")
                        .WithMany()
                        .HasForeignKey("DepartmentId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Jy.Domain.Entities.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId1");
                });

            modelBuilder.Entity("Jy.Domain.Entities.UserRole", b =>
                {
                    b.HasOne("Jy.Domain.Entities.Role", "Role")
                        .WithMany("RoleUsers")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Jy.Domain.Entities.User", "User")
                        .WithMany("UserRoles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
