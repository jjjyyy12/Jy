using Jy.CRM.Domain.Entities;
using Jy.EntityFramewordCoreBase;
using Jy.IRepositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Jy.CRM.EntityFrameworkCore
{
	//http://www.cnblogs.com/CreateMyself/p/6613949.html
    public class JyCRMDBContext : DbContext, IUnitOfWork
    {
        public JyCRMDBContext(DbContextOptions<JyCRMDBContext> options) : base(options)
        {
           
        }
        public DbSet<Commodity> Commoditys { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<SecKillOrder> SecKillOrders { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<CardType> CardTypes { get; set; }
        public DbSet<OrderStatus> OrderStatus { get; set; }
        public DbSet<Address> Addresss { get; set; }
        public DbSet<UserAddress> UserAddresss { get; set; }
        
        protected override void OnModelCreating(ModelBuilder builder)
        {
            //http://www.cnblogs.com/CreateMyself/p/6995403.html
            //user SecKillOrder  一对多
            builder.Entity<User>(b => 
            {
                b.ToTable("User");
                b.Property(e => e.Id).ValueGeneratedOnAdd();
                b.HasKey(e => e.Id);
                b.Property(e => e.NickName).HasColumnType("VARCHAR(72)"); 
                b.Property(e => e.MobileNumber).HasColumnType("VARCHAR(36)");
                b.Property(e => e.EMail).HasColumnType("VARCHAR(100)");
                b.Property(e => e.Address).HasColumnType("VARCHAR(200)");
                b.HasMany(e => e.SecKillOrders).WithOne(e => e.User).HasForeignKey(e => e.UserId);
            });
            //Commodity SecKillOrder  一对多
            builder.Entity<Commodity>(b =>
            {
                b.ToTable("Commodity");
                b.Property(e => e.Id).ValueGeneratedOnAdd();
                b.HasKey(e => e.Id);
                b.Property(e => e.Name).HasColumnType("VARCHAR(72)");
                b.Property(e => e.MaxNum);
                b.Property(e => e.MaxGouNum);
                b.Property(e => e.Price);
                b.Property(e => e.Des).HasColumnType("VARCHAR(200)");
                b.Property(e => e.Url).HasColumnType("VARCHAR(200)");
                b.HasMany(e => e.SecKillOrders).WithOne(e => e.Commodity).HasForeignKey(e => e.CommodityId);
            });
            //Payment 
            builder.Entity<Payment>(b =>
            {
                b.ToTable("Payment");
                b.Property(e => e.Id).ValueGeneratedOnAdd();
                b.HasKey(e => e.Id);
                b.Property(e => e.ChannelCode).HasColumnType("VARCHAR(36)");
                b.Property(e => e.PaymentMode).HasColumnType("VARCHAR(36)");
                b.Property(e => e.PaymentAccount).HasColumnType("VARCHAR(72)") ;
                b.Property(e => e.PaymentAmount);
            });
            //SecKillOrder Payments  一对多
            builder.Entity<SecKillOrder>(b =>
            {
                b.ToTable("SecKillOrder");
                b.Property(e => e.Id).ValueGeneratedOnAdd();
                b.HasKey(e => e.Id);
                b.Property(e => e.PayOutTime);
                b.Property(e => e.CreatTime);
                b.Property(e => e.PayTime);
                b.Property(e => e.Num);
                b.HasMany(e => e.Payments).WithOne(e => e.SecKillOrder).HasForeignKey(e => e.SecKillOrderId);
            });
            //OrderStatus SecKillOrder  一对多
            builder.Entity<OrderStatus>(b =>
            {
                b.ToTable("OrderStatus");
                b.Property(e => e.Id).ValueGeneratedOnAdd();
                b.HasKey(e => e.Id);
                b.Property(e => e.Name).HasColumnType("VARCHAR(36)");
                b.HasMany(e => e.SecKillOrders).WithOne(e => e.OrderStatus).HasForeignKey(e => e.OrderStatusId);
            });
            //Address seckillorder 一对多
            builder.Entity<Address>(b =>
            {
                b.ToTable("Address");
                b.Property(e => e.Id).ValueGeneratedOnAdd();
                b.HasKey(e => e.Id);
                b.Property(e => e.Country).HasColumnType("VARCHAR(200)");
                b.Property(e => e.City).HasColumnType("VARCHAR(200)");
                b.Property(e => e.Province).HasColumnType("VARCHAR(200)");
                b.Property(e => e.Street).HasColumnType("VARCHAR(200)");
                b.Property(e => e.ZipCode).HasColumnType("VARCHAR(72)");
                b.HasMany(e => e.SecKillOrders).WithOne(e => e.Address).HasForeignKey(e => e.AddressId);
            });
            //Address user 多对多
            builder.Entity<UserAddress>(b =>
            {
                b.ToTable("UserAddress");
                b.Property(e => e.UserId);
                b.Property(e => e.AddressId);
                b.HasKey(p => new { p.UserId, p.AddressId });

                b.HasOne(r => r.User)
                      .WithMany(r => r.UserAddresss)
                      .HasForeignKey(r => r.UserId);
                b.HasOne(m => m.Address)
                    .WithMany(m => m.AddressUsers)
                    .HasForeignKey(m => m.AddressId);
            });

            //CardType Payment  一对多
            builder.Entity<CardType>(b =>
            {
                b.ToTable("CardType");
                b.Property(e => e.Id).ValueGeneratedOnAdd();
                b.HasKey(e => e.Id);
                b.Property(e => e.Name).HasColumnType("VARCHAR(72)");
                b.HasMany(e => e.Payments).WithOne(e => e.CardType).HasForeignKey(e => e.CardTypeId);
            });


            //启用Guid主键类型扩展
            builder.HasPostgresExtension("uuid-ossp");

            base.OnModelCreating(builder);
        }
        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            base.OnConfiguring(builder);
        }

        public int SaveChange()
        {
            return DbContextExtensions.SaveChanges(this, RefreshConflict.MergeClientAndStore);
        }
        public Task SaveChangeAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.Factory.StartNew(SaveChange, cancellationToken);
        }
    }
 
}

