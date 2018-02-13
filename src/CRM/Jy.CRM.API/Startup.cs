using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Jy.CRM.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Jy.CRM.Domain;
using Jy.CRM.API.Swagger;
using Jy.IRepositories;
using Jy.Cache;
using Jy.ICache;
using Microsoft.Extensions.Caching.Memory;
using RawRabbit.Extensions.Client;
using Jy.IMessageQueue;
using Jy.RabbitMQ;
using Jy.Kafka;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Jy.TokenService;
using Jy.Utility.Paged;
using Jy.CacheService;
using Jy.QueueSerivce;
using Jy.Utility;
using Jy.CRM.Domain.IRepositories;
using Jy.CRM.EntityFrameworkCore.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Jy.CRM.API.Middleware;
using Serilog;
using System.IO;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.FileProviders;
using Jy.CRM.API.Filter;
using Jy.AuthService;
using Jy.EntityFramewordCoreBase.Repositories;

namespace Jy.CRM.API
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
            //初始化映射关系
            DomainMapper.Initialize();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.DescribeAllEnumsAsStrings();
                options.SwaggerDoc("v1", new Swashbuckle.AspNetCore.Swagger.Info
                {
                    Title = "AuthAdmin HTTP API",
                    Version = "v1",
                    Description = "The AuthAdmin Service HTTP API",
                    TermsOfService = "Terms Of Service"
                });
                options.OperationFilter<AuthorizationHeaderParameterOperationFilter>(); // 添加httpHeader参数
            });

            //添加数据上下文
            //services.AddDbContext<JyDbContext>(options => options.UseNpgsql(sqlConnectionString)); //PostgreSQL
            services.AddDbContext<JyCRMDBContext>(options => options.UseMySql(Configuration.GetConnectionString("MySql")), ServiceLifetime.Scoped);//mysql
            //services.AddDbContext<JyCRMDBReadContext>(options => options.UseMySql(Configuration.GetConnectionString("MySqlRead")), ServiceLifetime.Scoped);//mysqlread
            services.AddDbContextPool<JyCRMDBReadContext>(
        options => options.UseMySql(Configuration.GetConnectionString("MySqlRead"),
        mysqlOptions => mysqlOptions.MaxBatchSize(100)));
            //设置读取appsetting.json
            services.AddOptions();
            services.Configure<UrlConfigSetting>(Configuration.GetSection("UrlConfig"));//配置url
            services.Configure<SDBSettings>(Configuration.GetSection("SDBSettings"));

            //---------------缓存配置
            services.AddMemoryCache();
            //services.Configure<CacheProvider>(Configuration.GetSection("CacheConfig"));

            if ("TRUE".Equals(Configuration.GetSection("CacheConfig").GetValue<string>("UseRedis").ToUpper()))
            {
                //Use Redis
                services.AddSingleton(typeof(ICached), new RedisCacheRepository(new RedisCacheOptions
                {
                    Configuration = Configuration.GetSection("CacheConfig").GetValue<string>("Redis_ConnectionString"),
                    InstanceName = Configuration.GetSection("CacheConfig").GetValue<string>("Redis_InstanceName"),
                    expTime = new TimeSpan(0, Configuration.GetSection("CacheConfig").GetValue<int>("expTime"), 0)
                }, 0));
            }
            else if ("TRUE".Equals(Configuration.GetSection("CacheConfig").GetValue<string>("UseHttpCache").ToUpper()))
            {
                //Use MemoryCache
                services.AddSingleton<IMemoryCache>(factory =>
                {
                    var cache = new MemoryCache(new MemoryCacheOptions());
                    return cache;
                });
                services.AddSingleton<IHttpCached, MemoryCacheRepository>();

                services.AddSingleton(typeof(IHttpCached),
                    new MemoryCacheRepository(new MemoryCache(new MemoryCacheOptions()), new TimeSpan(0, Configuration.GetSection("CacheConfig").GetValue<int>("expTime"), 0)));
            }
            //---------------缓存配置 end

            //txtlog
            services.AddSingleton<ILog.ILogger, Logger>();

            //------------------------rabbitMQ
            //services.AddRawRabbit();
            //services.AddScoped<IQueueOperation, QueueOperationRawRabbit>();
            //services.AddScoped<IQueueOperationSubscriptionsManager, InMemorySubscriptionsManager>();
            services.AddRabbitMQServices(new RabbitMQOptions()
            {
                HostName = Configuration.GetSection("RabbitMQConfig").GetValue<string>("HostName"),
                UserName = Configuration.GetSection("RabbitMQConfig").GetValue<string>("UserName"),
                Password = Configuration.GetSection("RabbitMQConfig").GetValue<string>("Password"),
                Port = Configuration.GetSection("RabbitMQConfig").GetValue<int>("Port")
            });
            //------------------------rabbitMQ

            //------------------------kafka
            services.AddScoped<IBigQueueOperation, QueueOperationRdKafka>();
            //------------------------kafka

            services.AddScoped<PagedHelper>();

            services.AddScoped<ICacheService, Jy.CacheService.CacheService>();
            services.AddScoped<IQueueService, Jy.QueueSerivce.QueueSerivce>();
            services.AddScoped<IRepositoryContext, EntityFrameworkRepositoryContext>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserRepositoryRead, UserRepositoryRead>();
            services.AddScoped<IAddressRepository, AddressRepository>();
            services.AddScoped<IAddressRepositoryRead, AddressRepositoryRead>();
            services.AddScoped<ICommodityRepository, CommodityRepository>();
            services.AddScoped<ICommodityRepositoryRead, CommodityRepositoryRead>();
            services.AddScoped<IPaymentRepository, PaymentRepository>();
            services.AddScoped<IPaymentRepositoryRead, PaymentRepositoryRead>();
            services.AddScoped<ISecKillOrderRepository, SecKillOrderRepository>();
            services.AddScoped<ISecKillOrderRepositoryRead, SecKillOrderRepositoryRead>();

            services.AddScoped<IRepositoryFactory, RepositoryFactory>();
            services.AddScoped<IRepositoryReadFactory, RepositoryReadFactory>();

            //services.AddScoped<IUserIndexsIndex, UserIndexsIndex>();
            //services.AddScoped<IUserIndexsIndexRead, UserIndexsIndexRead>();
            //services.AddScoped<IIndexFactory, IndexFactory<Jy.IIndex.Entity>>();
            //services.AddScoped<IIndexReadFactory, IndexReadFactory<Jy.IIndex.Entity>>();
            services.AddScoped<ITokenAuthService, TokenAuthService>();

            //jwt  http://www.cnblogs.com/JacZhu/p/6837676.html
            services.AddAuthorization(auth =>
            {
                auth.AddPolicy("Bearer", new AuthorizationPolicyBuilder()
                    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser()
                    .AddRequirements(new Jy.MVCAuthorization.ValidJtiRequirement()) // 添加上面的验证要求
                    .Build());
            });
            var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey));
            var tokenValidationParameters = new TokenValidationParameters
            {
                // The signing key must match!
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey,

                // Validate the JWT Issuer (iss) claim
                ValidateIssuer = true,
                ValidIssuer = "JyIssuer",

                // Validate the JWT Audience (aud) claim
                ValidateAudience = true,
                ValidAudience = "JyAudience",

                // Validate the token expiry
                ValidateLifetime = true,

                ClockSkew = TimeSpan.Zero

            };
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = tokenValidationParameters;
                options.RequireHttpsMetadata = false;
            });

            // 注册验证要求的处理器，可通过这种方式对同一种要求添加多种验证
            services.AddSingleton<IAuthorizationHandler, Jy.MVCAuthorization.ValidJtiHandler>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // Add framework services.
            services.AddMvc(options =>
            {
                options.Filters.Add(typeof(HttpGlobalExceptionFilter));
            }).AddControllersAsServices();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            //-------------------------------------------------------serilog 配置
            MVCLogOptions mvcLogOptions = new MVCLogOptions()
            {
                LogPath = "D:\\LogFiles_CRM_API",//Configuration[nameof(AuthLogOptions.LogPath)],
                PathFormat = "{Date}.log"
            };
            var serilog = new LoggerConfiguration()
               .MinimumLevel.Debug()
               .Enrich.FromLogContext()
               .WriteTo.RollingFile(Path.Combine(mvcLogOptions.LogPath, mvcLogOptions.PathFormat),
                   outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {Message}{NewLine}{Exception}");
            MVCLogOptions.EnsurePreConditions(mvcLogOptions);

            loggerFactory.AddSerilog(serilog.CreateLogger());

            // Ensure any buffered events are sent at shutdown 日志的生命周期
            IApplicationLifetime appLifetime = (IApplicationLifetime)app.ApplicationServices.GetService(typeof(IApplicationLifetime));
            if (appLifetime != null)
            {
                appLifetime.ApplicationStopped.Register(Serilog.Log.CloseAndFlush);
            }
            app.UseAuthLog(mvcLogOptions);//这个中间件用作记录请求中的过程日志
            //---------------------------------------------------serilog 配置

            SeedData.Initialize(app.ApplicationServices); //EF初始化数据

            if (env.IsDevelopment())
            {
                //开发环境异常处理
                app.UseDeveloperExceptionPage();
            }
            else
            {
                //生产环境异常处理
                app.UseExceptionHandler("/Shared/Error");
            }
            //使用静态文件
            app.UseStaticFiles();
            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(Directory.GetCurrentDirectory())
            });

            ////Session 
            //app.UseSession(new SessionOptions() { IdleTimeout = TimeSpan.FromMinutes(30) });

            //Jwt认证
            //ConfigureJwtAuth(app);

            SeedData.Initialize(app.ApplicationServices); //EF初始化数据

            app.UseMvcWithDefaultRoute();

            app.UseSwagger()
             .UseSwaggerUI(c =>
             {
                 c.SwaggerEndpoint("/swagger/v1/swagger.json", "CRM API V1");
             });
        }

        private static readonly string secretKey = "123456Jy_12312321312dafdsfds";
        //private void ConfigureJwtAuth(IApplicationBuilder app)
        //{
        //    var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey));

        //    var tokenValidationParameters = new TokenValidationParameters
        //    {
        //        // The signing key must match!
        //        ValidateIssuerSigningKey = true,
        //        IssuerSigningKey = signingKey,

        //        // Validate the JWT Issuer (iss) claim
        //        ValidateIssuer = true,
        //        ValidIssuer = "JyIssuer",

        //        // Validate the JWT Audience (aud) claim
        //        ValidateAudience = true,
        //        ValidAudience = "JyAudience",

        //        // Validate the token expiry
        //        ValidateLifetime = true,

        //        ClockSkew = TimeSpan.Zero
                
        //    };

        //    app.UseJwtBearerAuthentication(new JwtBearerOptions
        //    {
        //        AutomaticAuthenticate = true,
        //        AutomaticChallenge = true,
        //        TokenValidationParameters = tokenValidationParameters
        //    });
        //}
    }
}
