
using Jy.Application.DepartmentApp;
using Jy.Application.MenuApp;
using Jy.Application.RoleApp;
using Jy.Application.UserApp;
using Jy.ConsumerAuth.Middleware;
using Jy.DistributedLock;
using Jy.Domain.IRepositories;
using Jy.EntityFrameworkCore;
using Jy.EntityFrameworkCore.Repositories;
using Jy.IMessageQueue;
using Jy.Kafka;
using Jy.DistributedLockHandler;
using Jy.RabbitMQ;
using Jy.RabbitMQ.ProcessMessage;
using Jy.Utility.Paged;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using Jy.CacheService;
using Jy.QueueSerivce;
using Jy.Domain.Message;
using Jy.Domain;
using Jy.IRepositories;
using System.Threading.Tasks;
using Jy.IIndex;
using Jy.EntityFramewordCoreBase.Repositories;

namespace Jy.ConsumerAuth
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IServiceCollection services = new ServiceCollection();

            var confbuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();
            IConfigurationRoot Configuration = confbuilder.Build();
            
            services.AddDbContext<JyDbContext>(options => options.UseMySql(Configuration.GetConnectionString("MySql")), ServiceLifetime.Scoped);//mysql
            //services.AddDbContext<JyDBReadContext>(options => options.UseMySql(Configuration.GetConnectionString("MySqlRead")), ServiceLifetime.Scoped);//mysqlread


            //目前使用2个DbContextPool不好弄
            //services.AddDbContextPool<JyDbContext>(
            //        options => options.UseMySql(Configuration.GetConnectionString("MySql"),
            //            mysqlOptions => mysqlOptions.MaxBatchSize(100)));
            services.AddDbContextPool<JyDBReadContext>(
                    options => options.UseMySql(Configuration.GetConnectionString("MySqlRead"),
            mysqlOptions => mysqlOptions.MaxBatchSize(100)));


            services.Configure<SDBSettings>(Configuration.GetSection("SDBSettings"));
            services.Configure<SIndexSettings>(Configuration.GetSection("SIndexSettings"));
            DomainMapper.Initialize();
            //services.AddRawRabbit();
            //分布式锁,redis实现
            services.AddSingleton<Redlock>(new Redlock(Configuration.GetSection("CacheConfig").GetValue<string>("Redis_ConnectionString")));
            services.AddScoped<ProcessMessageLockHandler>();

            //txtlog
            services.AddSingleton<ILog.ILogger, Logger>();

            //依赖注入
            services.AddScoped<PagedHelper>();
            services.AddScoped<ICacheService, Jy.CacheService.CacheService>();
            services.AddScoped<IQueueService, Jy.QueueSerivce.QueueSerivce>();

            services.AddScoped<IRepositoryContext, AuthRepositoryContext>();
            services.AddScoped<IRepositoryReadContext, AuthRepositoryReadContext>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserAppService, UserAppService>();
            services.AddScoped<IMenuRepository, MenuRepository>();
            services.AddScoped<IMenuAppService, MenuAppService>();
            services.AddScoped<IDepartmentRepository, DepartmentRepository>();
            services.AddScoped<IDepartmentAppService, DepartmentAppService>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IRoleAppService, RoleAppService>();

            services.AddScoped<IUserRepositoryRead, UserRepositoryRead>();
            services.AddScoped<IRoleRepositoryRead, RoleRepositoryRead>();
            services.AddScoped<IMenuRepositoryRead, MenuRepositoryRead>();
            services.AddScoped<IDepartmentRepositoryRead, DepartmentRepositoryRead>();
            services.AddScoped<IRepositoryFactory, RepositoryFactory>();
            services.AddScoped<IRepositoryReadFactory, RepositoryReadFactory>();

            services.AddScoped(factory => {
                Func<string, IRepositoryFactory> accesor = (key) =>
                {
                    if (key.Equals("EF"))
                    {
                        return factory.GetService<RepositoryFactory>();
                    }
                    else if (key.Equals("DP"))
                    {
                        return factory.GetService<Jy.Dapper.Repositories.DPRepositoryFactory>();
                    }
                    else
                    {
                        throw new ArgumentException($"Not Support key :{key}");
                    }
                };
                return accesor;
            });
            //MQ 
            //services.AddScoped(typeof(IProcessMessage));
            //handlers
            services.AddScoped(typeof(ProcessUser_delete_deleteuser_normal));
            services.AddScoped(typeof(ProcessUser_delete_deleteuser_normal_2));
            services.AddScoped(typeof(ProcessUser_update_insertupdate_rpc));
            services.AddScoped(typeof(ProcessUser_update_insertupdate_rpc_2));
            services.AddScoped(typeof(ProcessUser_update_userroles_normal));
            services.AddScoped(typeof(ProcessRoleRole_delete_deleterole_normal));
            services.AddScoped(typeof(ProcessRoleRole_delete_others_normal));
            services.AddScoped(typeof(ProcessRoleRole_update_insertupdate_rpc));
            services.AddScoped(typeof(ProcessRoleRole_update_others_normal));
            services.AddScoped(typeof(ProcessRoleRole_update_rolemenus_normal));
            services.AddScoped(typeof(ProcessRoleRole_rolemenus_others_normal));
            services.AddScoped(typeof(ProcessMenuMenu_delete_deletemenu_normal));
            services.AddScoped(typeof(ProcessMenuMenu_delete_others_normal));
            services.AddScoped(typeof(ProcessMenuMenu_update_insertupdate_rpc));
            services.AddScoped(typeof(ProcessMenuMenu_update_others_normal));
            services.AddScoped(typeof(ProcessDepartmentDepartment_delete_deletedepartment_normal));
            services.AddScoped(typeof(ProcessDepartmentDepartment_delete_others_normal));
            services.AddScoped(typeof(ProcessDepartmentDepartment_update_insertupdate_rpc));
            services.AddScoped(typeof(ProcessDepartmentDepartment_update_others_normal));

            services.AddScoped(typeof(ProcessOperateLog));


            services.AddScoped<IProcessMessage, ProcessUser_delete_deleteuser_normal>(); //这里需要注册IProcessMessage接口的默认实例，不然异常
                                                                                         //services.AddScoped<IQueueOperation, QueueOperationRawRabbit>();
                                                                                         //services.AddScoped<IQueueOperationSubscriptionsManager, InMemorySubscriptionsManager>();
                                                                                         //RabbitMQ
            services.AddRabbitMQServices(new RabbitMQOptions()
            {
                HostName = Configuration.GetSection("RabbitMQConfig").GetValue<string>("HostName"),
                UserName = Configuration.GetSection("RabbitMQConfig").GetValue<string>("UserName"),
                Password = Configuration.GetSection("RabbitMQConfig").GetValue<string>("Password"),
                Port = Configuration.GetSection("RabbitMQConfig").GetValue<int>("Port")
            });
            services.AddScoped(typeof(ProcessMessageDecorator<>));

            //------------------------kafka
            services.AddScoped<IBigQueueOperation, QueueOperationRdKafka>();
            //------------------------kafka

            IServiceProvider serviceProvider = services.BuildServiceProvider();

            //-------------------------------------------------------serilog 配置
            ConsLogOptions authLogOptions = new ConsLogOptions()
            {
                LogPath = "D:\\LogFiles_MQ",//Configuration[nameof(AuthLogOptions.LogPath)],
                PathFormat = "MQ_{Date}.log"
            };
            var serilog = new LoggerConfiguration()
               .MinimumLevel.Debug()
               .Enrich.FromLogContext()
               .WriteTo.RollingFile(Path.Combine(authLogOptions.LogPath, authLogOptions.PathFormat),
                   outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {Message}{NewLine}{Exception}");
            ConsLogOptions.EnsurePreConditions(authLogOptions);
            var logger = serilog.CreateLogger();
            //---------------------------------------------------serilog 配置

            logger.Information("-------------ConsumerAuth start,begin init:");
            //IProcessMessage pm = serviceProvider.GetService<IProcessMessage>();
            IQueueOperation qo = serviceProvider.GetService<IQueueOperation>();
            //JyDbContext cntexta = serviceProvider.GetService<JyDbContext>();

            logger.Information("launch ErrorSubscribe");
            //errorHandle
            qo.ErrorSubscribe();


            ProcessMessageLockHandler lockhandler = (ProcessMessageLockHandler)serviceProvider.GetService(typeof(ProcessMessageLockHandler));//分布式锁处理类

            SubscribeTopic<user_delete_deleteuser_normal, ProcessUser_delete_deleteuser_normal>(serviceProvider, lockhandler, logger, qo, "auth.exchange", "user_delete_deleteuser_normal", "user_delete_deleteuser_normal");
            SubscribeTopic<user_delete_deleteuser_normal, ProcessUser_delete_deleteuser_normal_2>(serviceProvider, lockhandler, logger, qo, "auth.exchange", "user_delete_deleteuser_normal", "user_delete_deleteuser_normal");
            ResponseTopic<user_update_insertupdate_rpc, ProcessUser_update_insertupdate_rpc>(serviceProvider, lockhandler, logger, qo, "auth.exchange", "user_update_insertupdate_rpc", "user_update_insertupdate_rpc");
            ResponseTopic<user_update_insertupdate_rpc, ProcessUser_update_insertupdate_rpc_2>(serviceProvider, lockhandler, logger, qo, "auth.exchange", "user_update_insertupdate_rpc", "user_update_insertupdate_rpc");
            SubscribeTopic<user_update_userroles_normal, ProcessUser_update_userroles_normal>(serviceProvider, lockhandler, logger, qo, "auth.exchange", "user_update_userroles_normal", "user_update_userroles_normal");

            SubscribeTopic<role_delete_deleterole_normal, ProcessRoleRole_delete_deleterole_normal>(serviceProvider, lockhandler, logger, qo, "auth.exchange", "role_delete_deleterole_normal", "role_delete_deleterole_normal");
            SubscribeTopic<role_delete_others_normal, ProcessRoleRole_delete_others_normal>(serviceProvider, lockhandler, logger, qo, "auth.exchange", "role_delete_others_normal", "role_rolemenus_others_normal");
            ResponseTopic<role_update_insertupdate_rpc, ProcessRoleRole_update_insertupdate_rpc>(serviceProvider, lockhandler, logger, qo, "auth.exchange", "role_update_insertupdate_rpc", "role_update_insertupdate_rpc");
            SubscribeTopic<role_update_others_normal, ProcessRoleRole_update_others_normal>(serviceProvider, lockhandler, logger, qo, "auth.exchange", "role_update_others_normal", "role_update_others_normal");
            SubscribeTopic<role_update_rolemenus_normal, ProcessRoleRole_update_rolemenus_normal>(serviceProvider, lockhandler, logger, qo, "auth.exchange", "role_update_rolemenus_normal", "role_update_rolemenus_normal");
            SubscribeTopic<role_rolemenus_others_normal, ProcessRoleRole_rolemenus_others_normal>(serviceProvider, lockhandler, logger, qo, "auth.exchange", "role_rolemenus_others_normal", "role_rolemenus_others_normal");

            SubscribeTopic<menu_delete_deletemenu_normal, ProcessMenuMenu_delete_deletemenu_normal>(serviceProvider, lockhandler, logger, qo, "auth.exchange", "menu_delete_deletemenu_normal", "menu_delete_deletemenu_normal");
            SubscribeTopic<menu_delete_others_normal, ProcessMenuMenu_delete_others_normal>(serviceProvider, lockhandler, logger, qo, "auth.exchange", "menu_delete_others_normal", "menu_delete_others_normal");
            ResponseTopic<menu_update_insertupdate_rpc, ProcessMenuMenu_update_insertupdate_rpc>(serviceProvider, lockhandler, logger, qo, "auth.exchange", "menu_update_insertupdate_rpc", "menu_update_insertupdate_rpc");
            ResponseTopic<menu_update_others_normal, ProcessMenuMenu_update_others_normal>(serviceProvider, lockhandler, logger, qo, "auth.exchange", "menu_update_others_normal", "menu_update_others_normal");

            SubscribeTopic<department_delete_deletedepartment_normal, ProcessDepartmentDepartment_delete_deletedepartment_normal>(serviceProvider, lockhandler, logger, qo, "auth.exchange", "department_delete_deletedepartment_normal", "department_delete_deletedepartment_normal");
            SubscribeTopic<department_delete_others_normal, ProcessDepartmentDepartment_delete_others_normal>(serviceProvider, lockhandler, logger, qo, "auth.exchange", "department_delete_others_normal", "department_delete_others_normal");
            ResponseTopic<department_update_insertupdate_rpc, ProcessDepartmentDepartment_update_insertupdate_rpc>(serviceProvider, lockhandler, logger, qo, "auth.exchange", "department_update_insertupdate_rpc", "department_update_insertupdate_rpc");
            ResponseTopic<department_update_others_normal, ProcessDepartmentDepartment_update_others_normal>(serviceProvider, lockhandler, logger, qo, "auth.exchange", "department_update_others_normal", "department_update_others_normal");

            //kafaka
            IBigQueueOperation bqo = serviceProvider.GetService<IBigQueueOperation>();
            ProcessOperateLog pol = (ProcessOperateLog)serviceProvider.GetService(typeof(ProcessOperateLog));
            ProcessMessageDecorator<MessageBase> pold = new ProcessMessageDecorator<MessageBase>(pol, lockhandler);//分布式锁装饰类
            bqo.SubscribeTopic<MessageBase, ProcessMessageDecorator<MessageBase>>(() => { return pold; }, "", "auth", new List<string> { "auth.operate" });


            logger.Information("-------------ConsumerAuth start,end init");

            #region old init
            //ProcessUser User Operation Listener    
            //ProcessUser_delete_deleteuser_normal pddn = serviceProvider.GetRequiredService<ProcessUser_delete_deleteuser_normal>();
            //ProcessMessageDecorator<user_delete_deleteuser_normal> pddnd = new ProcessMessageDecorator<user_delete_deleteuser_normal>(pddn, lockhandler);//分布式锁装饰类
            //logger.Information("launch SubscribeTopic ProcessUser_delete_deleteuser_normal");
            //qo.SubscribeTopic<user_delete_deleteuser_normal, ProcessMessageDecorator<user_delete_deleteuser_normal>>(() => { return pddnd; }, "", "auth.exchange", "user_delete_deleteuser_normal", "user_delete_deleteuser_normal"); //普通消息

            //ProcessUser_delete_deleteuser_normal_2 pddn2 = serviceProvider.GetRequiredService<ProcessUser_delete_deleteuser_normal_2>();
            //ProcessMessageDecorator<user_delete_deleteuser_normal> pddnd2 = new ProcessMessageDecorator<user_delete_deleteuser_normal>(pddn2, lockhandler);//分布式锁装饰类
            //logger.Information("launch SubscribeTopic ProcessUser_delete_deleteuser_normal2");
            //qo.SubscribeTopic<user_delete_deleteuser_normal, ProcessMessageDecorator<user_delete_deleteuser_normal>>(() => { return pddnd2; }, "", "auth.exchange", "user_delete_deleteuser_normal", "user_delete_deleteuser_normal"); //普通消息

            //ProcessUser_update_insertupdate_rpc puir = serviceProvider.GetRequiredService<ProcessUser_update_insertupdate_rpc>();
            //ProcessMessageDecorator<user_update_insertupdate_rpc> puird = new ProcessMessageDecorator<user_update_insertupdate_rpc>(puir, lockhandler);//分布式锁装饰类
            //logger.Information("launch SubscribeTopic user_update_insertupdate_rpc");
            //qo.ResponseTopic<user_update_insertupdate_rpc, ProcessMessageDecorator<user_update_insertupdate_rpc>>(() => { return puird; }, "auth.exchange", "user_update_insertupdate_rpc", "user_update_insertupdate_rpc"); //普通消息

            //ProcessUser_update_insertupdate_rpc_2 puir2 = serviceProvider.GetRequiredService<ProcessUser_update_insertupdate_rpc_2>();
            //ProcessMessageDecorator<user_update_insertupdate_rpc> puird2 = new ProcessMessageDecorator<user_update_insertupdate_rpc>(puir2, lockhandler);//分布式锁装饰类
            //logger.Information("launch SubscribeTopic user_update_insertupdate_rpc2");
            //qo.ResponseTopic<user_update_insertupdate_rpc, ProcessMessageDecorator<user_update_insertupdate_rpc>>(() => { return puird2; }, "auth.exchange", "user_update_insertupdate_rpc", "user_update_insertupdate_rpc"); //普通消息

            //ProcessUser_update_userroles_normal puun = serviceProvider.GetRequiredService<ProcessUser_update_userroles_normal>();
            //ProcessMessageDecorator<user_update_userroles_normal> puund = new ProcessMessageDecorator<user_update_userroles_normal>(puun, lockhandler);//分布式锁装饰类
            //logger.Information("launch SubscribeTopic ProcessUser_update_userroles_normal");
            //qo.SubscribeTopic<user_update_userroles_normal, ProcessMessageDecorator<user_update_userroles_normal>>(() => { return puund; }, "", "auth.exchange", "user_update_userroles_normal", "user_update_userroles_normal"); //普通消息


            ////ProcessRole Role Operation Listener
            //ProcessRoleRole_delete_deleterole_normal prddn = serviceProvider.GetRequiredService<ProcessRoleRole_delete_deleterole_normal>();
            //ProcessMessageDecorator<role_delete_deleterole_normal> prddnd = new ProcessMessageDecorator<role_delete_deleterole_normal>(prddn, lockhandler);//分布式锁装饰类
            //logger.Information("launch SubscribeTopic ProcessRoleRole_delete_deleterole_normal");
            //qo.SubscribeTopic<role_delete_deleterole_normal, ProcessMessageDecorator<role_delete_deleterole_normal>>(() => { return prddnd; }, "", "auth.exchange", "role_delete_deleterole_normal", "role_delete_deleterole_normal"); //普通消息

            //ProcessRoleRole_delete_others_normal prdon = serviceProvider.GetRequiredService<ProcessRoleRole_delete_others_normal>();
            //ProcessMessageDecorator<role_delete_others_normal> prdond = new ProcessMessageDecorator<role_delete_others_normal>(prdon, lockhandler);//分布式锁装饰类
            //logger.Information("launch SubscribeTopic ProcessRoleRole_delete_others_normal");
            //qo.SubscribeTopic<role_delete_others_normal, ProcessMessageDecorator<role_delete_others_normal>>(() => { return prdond; }, "", "auth.exchange", "role_delete_others_normal", "role_delete_others_normal"); //普通消息

            //ProcessRoleRole_update_insertupdate_rpc pruir = serviceProvider.GetRequiredService<ProcessRoleRole_update_insertupdate_rpc>();
            //ProcessMessageDecorator<role_update_insertupdate_rpc> pruird = new ProcessMessageDecorator<role_update_insertupdate_rpc>(pruir, lockhandler);//分布式锁装饰类
            //logger.Information("launch SubscribeTopic ProcessRoleRole_update_insertupdate_rpc");
            //qo.ResponseTopic<role_update_insertupdate_rpc, ProcessMessageDecorator<role_update_insertupdate_rpc>>(() => { return pruird; }, "auth.exchange", "role_update_insertupdate_rpc", "role_update_insertupdate_rpc"); //普通消息

            //ProcessRoleRole_update_others_normal pruon = serviceProvider.GetRequiredService<ProcessRoleRole_update_others_normal>();
            //ProcessMessageDecorator<role_update_others_normal> pruond = new ProcessMessageDecorator<role_update_others_normal>(pruon, lockhandler);//分布式锁装饰类
            //logger.Information("launch SubscribeTopic ProcessRoleRole_update_others_normal");
            //qo.SubscribeTopic<role_update_others_normal, ProcessMessageDecorator<role_update_others_normal>>(() => { return pruond; }, "", "auth.exchange", "role_update_others_normal", "role_update_others_normal"); //普通消息

            //ProcessRoleRole_update_rolemenus_normal prurn = serviceProvider.GetRequiredService<ProcessRoleRole_update_rolemenus_normal>();
            //ProcessMessageDecorator<role_update_rolemenus_normal> prurnd = new ProcessMessageDecorator<role_update_rolemenus_normal>(prurn, lockhandler);//分布式锁装饰类
            //logger.Information("launch SubscribeTopic ProcessRoleRole_update_rolemenus_normal");
            //qo.SubscribeTopic<role_update_rolemenus_normal, ProcessMessageDecorator<role_update_rolemenus_normal>>(() => { return prurnd; }, "", "auth.exchange", "role_update_rolemenus_normal", "role_update_rolemenus_normal"); //普通消息

            //ProcessRoleRole_rolemenus_others_normal pron = serviceProvider.GetRequiredService<ProcessRoleRole_rolemenus_others_normal>();
            //ProcessMessageDecorator<role_rolemenus_others_normal> prond = new ProcessMessageDecorator<role_rolemenus_others_normal>(pron, lockhandler);//分布式锁装饰类
            //logger.Information("launch SubscribeTopic ProcessRoleRole_rolemenus_others_normal");
            //qo.SubscribeTopic<role_rolemenus_others_normal, ProcessMessageDecorator<role_rolemenus_others_normal>>(() => { return prond; }, "", "auth.exchange", "role_rolemenus_others_normal", "role_rolemenus_others_normal"); //普通消息


            //ProcessMenu Menu Operation Listener
            //ProcessMenuMenu_delete_deletemenu_normal pmddn = serviceProvider.GetRequiredService<ProcessMenuMenu_delete_deletemenu_normal>();
            //ProcessMessageDecorator<menu_delete_deletemenu_normal> pmddnd = new ProcessMessageDecorator<menu_delete_deletemenu_normal>(pmddn, lockhandler);//分布式锁装饰类
            //logger.Information("launch SubscribeTopic ProcessMenuMenu_delete_deletemenu_normal");
            //qo.SubscribeTopic<menu_delete_deletemenu_normal, ProcessMessageDecorator<menu_delete_deletemenu_normal>>(() => { return pmddnd; }, "", "auth.exchange", "menu_delete_deletemenu_normal", "menu_delete_deletemenu_normal"); //普通消息

            //ProcessMenuMenu_delete_others_normal pdon = serviceProvider.GetRequiredService<ProcessMenuMenu_delete_others_normal>();
            //ProcessMessageDecorator<menu_delete_others_normal> pdond = new ProcessMessageDecorator<menu_delete_others_normal>(pdon, lockhandler);//分布式锁装饰类
            //logger.Information("launch SubscribeTopic ProcessMenuMenu_delete_others_normal");
            //qo.SubscribeTopic<menu_delete_others_normal, ProcessMessageDecorator<menu_delete_others_normal>>(() => { return pdond; }, "", "auth.exchange", "menu_delete_others_normal", "menu_delete_others_normal"); //普通消息

            //ProcessMenuMenu_update_insertupdate_rpc pmuir = serviceProvider.GetRequiredService<ProcessMenuMenu_update_insertupdate_rpc>();
            //ProcessMessageDecorator<menu_update_insertupdate_rpc> pmuird = new ProcessMessageDecorator<menu_update_insertupdate_rpc>(pmuir, lockhandler);//分布式锁装饰类
            //logger.Information("launch SubscribeTopic ProcessMenuMenu_update_insertupdate_rpc");
            //qo.ResponseTopic<menu_update_insertupdate_rpc, ProcessMessageDecorator<menu_update_insertupdate_rpc>>(() => { return pmuird; }, "auth.exchange", "menu_update_insertupdate_rpc", "menu_update_insertupdate_rpc"); //普通消息

            //ProcessMenuMenu_update_others_normal pmuon = serviceProvider.GetRequiredService<ProcessMenuMenu_update_others_normal>();
            //ProcessMessageDecorator<menu_update_others_normal> pmuond = new ProcessMessageDecorator<menu_update_others_normal>(pmuon, lockhandler);//分布式锁装饰类
            //logger.Information("launch SubscribeTopic ProcessMenuMenu_update_others_normal");
            //qo.ResponseTopic<menu_update_others_normal, ProcessMessageDecorator<menu_update_others_normal>>(() => { return pmuond; }, "auth.exchange", "menu_update_others_normal", "menu_update_others_normal"); //普通消息


            //ProcessDepartment Department Operation Listener
            //ProcessDepartmentDepartment_delete_deletedepartment_normal pdddn = serviceProvider.GetRequiredService<ProcessDepartmentDepartment_delete_deletedepartment_normal>();
            //ProcessMessageDecorator<department_delete_deletedepartment_normal> pdddnd = new ProcessMessageDecorator<department_delete_deletedepartment_normal>(pdddn, lockhandler);//分布式锁装饰类
            //logger.Information("launch SubscribeTopic ProcessDepartmentDepartment_delete_deletedepartment_normal");
            //qo.SubscribeTopic<department_delete_deletedepartment_normal, ProcessMessageDecorator<department_delete_deletedepartment_normal>>(() => { return pdddnd; }, "", "auth.exchange", "department_delete_deletedepartment_normal", "department_delete_deletedepartment_normal"); //普通消息

            //ProcessDepartmentDepartment_delete_others_normal pddon = serviceProvider.GetRequiredService<ProcessDepartmentDepartment_delete_others_normal>();
            //ProcessMessageDecorator<department_delete_others_normal> pddond = new ProcessMessageDecorator<department_delete_others_normal>(pddon, lockhandler);//分布式锁装饰类
            //logger.Information("launch SubscribeTopic ProcessDepartmentDepartment_delete_others_normal");
            //qo.SubscribeTopic<department_delete_others_normal, ProcessMessageDecorator<department_delete_others_normal>>(() => { return pddond; }, "", "auth.exchange", "department_delete_others_normal", "department_delete_others_normal"); //普通消息

            //ProcessDepartmentDepartment_update_insertupdate_rpc pduir = serviceProvider.GetRequiredService<ProcessDepartmentDepartment_update_insertupdate_rpc>();
            //ProcessMessageDecorator<department_update_insertupdate_rpc> pduird = new ProcessMessageDecorator<department_update_insertupdate_rpc>(pduir, lockhandler);//分布式锁装饰类
            //logger.Information("launch SubscribeTopic ProcessDepartmentDepartment_update_insertupdate_rpc");
            //qo.ResponseTopic<department_update_insertupdate_rpc, ProcessMessageDecorator<department_update_insertupdate_rpc>>(() => { return pduird; }, "auth.exchange", "department_update_insertupdate_rpc", "department_update_insertupdate_rpc"); //普通消息

            //ProcessDepartmentDepartment_update_others_normal pduon = serviceProvider.GetRequiredService<ProcessDepartmentDepartment_update_others_normal>();
            //ProcessMessageDecorator<department_update_others_normal> pduond = new ProcessMessageDecorator<department_update_others_normal>(pduon, lockhandler);//分布式锁装饰类
            //logger.Information("launch SubscribeTopic ProcessDepartmentDepartment_update_others_normal");
            //qo.ResponseTopic<department_update_others_normal, ProcessMessageDecorator<department_update_others_normal>>(() => { return pduond; }, "auth.exchange", "department_update_others_normal", "department_update_others_normal"); //普通消息
            #endregion
        }

        private static void ResponseTopic<T,TH>(IServiceProvider serviceProvider
            ,ProcessMessageLockHandler lockhandler
            ,Serilog.Core.Logger logger
            ,IQueueOperation qo
            ,string exchange
            ,string queue
            ,string topic)
            where TH : IProcessMessage<T>
            where T : MessageBase
        {
            //Task.Run(() => { 
                TH pddon = serviceProvider.GetRequiredService<TH>();
                ProcessMessageDecorator<T> pddond = new ProcessMessageDecorator<T>(pddon, lockhandler);//分布式锁装饰类
                logger.Information($"launch SubscribeTopic{typeof(TH).Name}");
                qo.ResponseTopic<T, ProcessMessageDecorator<T>>(() => { return pddond; }, exchange, queue, topic); //普通消息
            //});

        }

        private static void SubscribeTopic<T, TH>(IServiceProvider serviceProvider
            , ProcessMessageLockHandler lockhandler
            , Serilog.Core.Logger logger
            , IQueueOperation qo
            , string exchange
            , string queue
            , string topic)
        where TH : IProcessMessage<T>
        where T : MessageBase
        {
            //Task.Run(() => {
                TH pddon = serviceProvider.GetRequiredService<TH>();
                ProcessMessageDecorator<T> pddond = new ProcessMessageDecorator<T>(pddon, lockhandler);//分布式锁装饰类
                logger.Information($"launch SubscribeTopic{typeof(TH).Name}");
                qo.SubscribeTopic<T, ProcessMessageDecorator<T>>(() => { return pddond; }, "", exchange, queue, topic); //普通消息
            //});
        }

    }
}

//  Autofac注入
//  //直接指定实例类型
//var builder = new ContainerBuilder();
//builder.RegisterType<DBBase>();
//builder.RegisterType<SqlRepository>().As<IRepository>();
//using (var container = builder.Build())
//{
//    var manager = container.Resolve<DBBase>();
//    manager.Search("SELECT * FORM USER");
//}

////通过配置文件实现对象的创建
//var builder2 = new ContainerBuilder();
//builder2.RegisterType<DBBase>();
//builder2.RegisterModule(new ConfigurationSettingsReader("autofac"));
//using (var container = builder2.Build())
//{
//    var manager = container.Resolve<DBBase>();
//    manager.Search("SELECT * FORM USER");
//}
////通过配置文件，配合Register方法来创建对象
//var builder3 = new ContainerBuilder();
//builder3.RegisterModule(new ConfigurationSettingsReader("autofac"));
//builder3.Register(c => new DBBase(c.Resolve<IRepository>()));
//using (var container = builder3.Build())
//{
//    var manager = container.Resolve<DBBase>();
//    manager.Search("SELECT * FORM USER");
//}

//  #region Autofac注入
//var builder = new ContainerBuilder();
//builder.RegisterControllers(Assembly.GetExecutingAssembly());
//builder.RegisterGeneric(typeof(Repository<>)).As(typeof(IRepository<>)).InstancePerHttpRequest();

//builder.RegisterType<Web_ExceptionLogManager>().As<IWeb_ExceptionLogManager>().InstancePerHttpRequest(); //从HTTP请求中重到注入点

//IContainer container = builder.Build();
//DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
//#endregion


//JyDbContext cntexta= serviceProvider.GetService<JyDbContext>();
////var efOptionsBuilder = new DbContextOptionsBuilder();
////var par = efOptionsBuilder.UseMySql(sqlConnectionMySqlString)  ;

//builder.RegisterInstance(cntexta).As<JyDbContext>().SingleInstance();
//builder.RegisterType<DbContextOptions<JyDbContext>>();
//builder.Register(c => new JyDbContext(c.Resolve<DbContextOptions<JyDbContext>>()));
//builder.RegisterType<JyDbContext>().AsImplementedInterfaces().InstancePerLifetimeScope();
//builder.RegisterType<UserRepository>().As<IUserRepository>();
//builder.RegisterType<MenuRepository>().As<IMenuRepository>();
//builder.RegisterType<DepartmentRepository>().As<IDepartmentRepository>();
//builder.RegisterType<RoleRepository>().As<IRoleRepository>();

//builder.RegisterType<MessageBase>();
//builder.RegisterType<ProcessUserRole>();
//builder.RegisterType<ProcessUserRole>().As<IProcessMessage>();
//builder.RegisterType<QueueOperation>();
//builder.RegisterType<QueueOperation>().As<IQueueOperation>();
//builder.RegisterType<Logger>();
//builder.RegisterType<Logger>().As<ILogger>();

//builder.RegisterAssemblyTypes(Assembly.Load(new AssemblyName("Jy.EntityFrameworkCore")));
//var asb = Assembly.GetEntryAssembly();
//builder.RegisterAssemblyTypes(asb);

//builder.RegisterType<DbContextOptionsBuilder>();


//using (var container = builder.Build())
//{
//    IProcessMessage pm = container.Resolve<ProcessUserRole>();
//    IQueueOperation qo = container.Resolve<QueueOperation>();

//    qo.SubscribeTopic(pm, "user.update.userroles", "user.update.userroles");

//    UserRepository ddd = container.Resolve<UserRepository>();
//    ddd.GetUserRoles(new Guid());

// }
