using Jy.CRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace Jy.CRM.EntityFrameworkCore
{
    public static class SeedData
    {
        /*
         
            DbMigration使用方法  1、Enable-Migrations -ContextTypeNameLITCS.Data.gmisContext  Enable-Migrations命令创建了一个新的Migrations文件夹，并在该目录下创建了Configuration.cs文件。使用Visual Studio打开Configuration.cs文件，向Seed方法中添加要插入数据表的数据。  2、Add-Migration Initial来创建初始迁移  3、Add-Migration "Adduserrolemanagermenu"   创建新的版本并指定标识符 4、update-database –TargetMigration:"201301230114573_InitPartialDb" 更新到指定的版本，update-database更新到最新版本。  5、如果要最终生成一个版本，可以先删除Migrations文件夹，然后执行1和2即可，把Seed里边的代码贴过了，执行4即可。
             */
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new JyCRMDBContext(serviceProvider.GetRequiredService<DbContextOptions<JyCRMDBContext>>()))
            {

                if (!context.CardTypes.Any())
                {
                    context.CardTypes.Add(CardType.Amex);
                    context.CardTypes.Add(CardType.Visa);
                    context.CardTypes.Add(CardType.MasterCard);
                }

                if (!context.OrderStatus.Any())
                {
                    context.OrderStatus.Add(OrderStatus.Canceled);
                    context.OrderStatus.Add(OrderStatus.InProcess);
                    context.OrderStatus.Add(OrderStatus.Shipped);
                }

                if (!context.Commoditys.Any())
                {
                    //增加商品
                    context.Commoditys.AddRange(
                   new Commodity
                   {
                       Id = Guid.NewGuid(),
                       Name = "花脉精品",
                       Price = 1000,
                       MaxNum = 1000,
                       MaxGouNum = 10,
                       Des = "飘花筋脉石精品"
                   },
                   new Commodity
                   {
                       Id = Guid.NewGuid(),
                       Name = "本地精品",
                       Price = 1000,
                       MaxNum = 1000,
                       MaxGouNum = 10,
                       Des = "内蒙本地筋脉石精品"
                   }
                );
                }

                if (!context.Users.Any())
                {
                    //增加四个基本功能菜单
                    context.Users.AddRange(
                   new User
                   {
                       Id = Guid.NewGuid(),
                       NickName = "jiaoyue",
                       EMail = "jiaoyue2002@163.com",
                       Address = "常营北小街2号院5号楼1407",
                       MobileNumber = "13811177528"
                   },
                    new User
                    {
                        Id = Guid.NewGuid(),
                        NickName = "jiaoyuxin",
                        EMail = "tugongshenle@163.com",
                        Address = "常营北小街2号院5号楼1407",
                        MobileNumber = "13811177528"
                    }
                );
                }
                   
                context.SaveChanges();
            }
        }
    }
}
