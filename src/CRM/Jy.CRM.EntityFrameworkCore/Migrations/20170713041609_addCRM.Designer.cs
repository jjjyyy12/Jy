using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Jy.CRM.EntityFrameworkCore;

namespace Jy.CRM.EntityFrameworkCore.Migrations
{
    [DbContext(typeof(JyCRMDBContext))]
    [Migration("20170713041609_addCRM")]
    partial class addCRM
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("Npgsql:PostgresExtension:uuid-ossp", "'uuid-ossp', '', ''")
                .HasAnnotation("ProductVersion", "1.1.2");

            modelBuilder.Entity("Jy.CRM.Domain.Entities.Address", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("City")
                        .HasColumnType("VARCHAR(200)");

                    b.Property<string>("Country")
                        .HasColumnType("VARCHAR(200)");

                    b.Property<string>("Province")
                        .HasColumnType("VARCHAR(200)");

                    b.Property<string>("Street")
                        .HasColumnType("VARCHAR(200)");

                    b.Property<string>("ZipCode")
                        .HasColumnType("VARCHAR(72)");

                    b.HasKey("Id");

                    b.ToTable("Address");
                });

            modelBuilder.Entity("Jy.CRM.Domain.Entities.CardType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .HasColumnType("VARCHAR(72)");

                    b.HasKey("Id");

                    b.ToTable("CardType");
                });

            modelBuilder.Entity("Jy.CRM.Domain.Entities.Commodity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Des")
                        .HasColumnType("VARCHAR(200)");

                    b.Property<int>("MaxGouNum");

                    b.Property<int>("MaxNum");

                    b.Property<string>("Name")
                        .HasColumnType("VARCHAR(72)");

                    b.Property<decimal>("Price");

                    b.Property<string>("Url")
                        .HasColumnType("VARCHAR(200)");

                    b.HasKey("Id");

                    b.ToTable("Commodity");
                });

            modelBuilder.Entity("Jy.CRM.Domain.Entities.OrderStatus", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .HasColumnType("VARCHAR(36)");

                    b.HasKey("Id");

                    b.ToTable("OrderStatus");
                });

            modelBuilder.Entity("Jy.CRM.Domain.Entities.Payment", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("CardTypeId");

                    b.Property<string>("ChannelCode")
                        .HasColumnType("VARCHAR(36)");

                    b.Property<string>("PaymentAccount")
                        .HasColumnType("VARCHAR(72)");

                    b.Property<decimal>("PaymentAmount");

                    b.Property<string>("PaymentMode")
                        .HasColumnType("VARCHAR(36)");

                    b.Property<Guid>("SecKillOrderId");

                    b.HasKey("Id");

                    b.HasIndex("CardTypeId");

                    b.HasIndex("SecKillOrderId");

                    b.ToTable("Payment");
                });

            modelBuilder.Entity("Jy.CRM.Domain.Entities.SecKillOrder", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AddressId");

                    b.Property<Guid>("CommodityId");

                    b.Property<DateTime>("CreatTime");

                    b.Property<int>("Num");

                    b.Property<int>("OrderStatusId");

                    b.Property<DateTime>("PayOutTime");

                    b.Property<DateTime>("PayTime");

                    b.Property<Guid>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("AddressId");

                    b.HasIndex("CommodityId");

                    b.HasIndex("OrderStatusId");

                    b.HasIndex("UserId");

                    b.ToTable("SecKillOrder");
                });

            modelBuilder.Entity("Jy.CRM.Domain.Entities.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Address")
                        .HasColumnType("VARCHAR(200)");

                    b.Property<string>("EMail")
                        .HasColumnType("VARCHAR(100)");

                    b.Property<string>("MobileNumber")
                        .HasColumnType("VARCHAR(36)");

                    b.Property<string>("NickName")
                        .HasColumnType("VARCHAR(72)");

                    b.HasKey("Id");

                    b.ToTable("User");
                });

            modelBuilder.Entity("Jy.CRM.Domain.Entities.UserAddress", b =>
                {
                    b.Property<Guid>("UserId");

                    b.Property<int>("AddressId");

                    b.HasKey("UserId", "AddressId");

                    b.HasIndex("AddressId");

                    b.ToTable("UserAddress");
                });

            modelBuilder.Entity("Jy.CRM.Domain.Entities.Payment", b =>
                {
                    b.HasOne("Jy.CRM.Domain.Entities.CardType", "CardType")
                        .WithMany("Payments")
                        .HasForeignKey("CardTypeId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Jy.CRM.Domain.Entities.SecKillOrder", "SecKillOrder")
                        .WithMany("Payments")
                        .HasForeignKey("SecKillOrderId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Jy.CRM.Domain.Entities.SecKillOrder", b =>
                {
                    b.HasOne("Jy.CRM.Domain.Entities.Address", "Address")
                        .WithMany("SecKillOrders")
                        .HasForeignKey("AddressId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Jy.CRM.Domain.Entities.Commodity", "Commodity")
                        .WithMany("SecKillOrders")
                        .HasForeignKey("CommodityId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Jy.CRM.Domain.Entities.OrderStatus", "OrderStatus")
                        .WithMany("SecKillOrders")
                        .HasForeignKey("OrderStatusId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Jy.CRM.Domain.Entities.User", "User")
                        .WithMany("SecKillOrders")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Jy.CRM.Domain.Entities.UserAddress", b =>
                {
                    b.HasOne("Jy.CRM.Domain.Entities.Address", "Address")
                        .WithMany("AddressUsers")
                        .HasForeignKey("AddressId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Jy.CRM.Domain.Entities.User", "User")
                        .WithMany("UserAddresss")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
