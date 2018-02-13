using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Jy.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Jy.TokenService;
using Jy.Application.DepartmentApp;
using Jy.Domain.IRepositories;
using Jy.Application.MenuApp;
using Jy.Application.UserApp;
using Jy.Application.RoleApp;
using Jy.EntityFrameworkCore.Repositories;
using Jy.Utility.Paged;
using Jy.IMessageQueue;
using Jy.RabbitMQ;
using Jy.Kafka;
using RawRabbit.vNext;
using Jy.Cache;
using Microsoft.Extensions.Caching.Memory;
using Jy.ICache;
using Microsoft.Extensions.FileProviders;
using System.IO;
using Jy.Dapper;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Jy.AuthAdmin.API.Middleware;
using Serilog;
using Jy.CacheService;
using Jy.QueueSerivce;
using Microsoft.AspNetCore.Http;
using Jy.Domain;
using Jy.AuthAdmin.API.Swagger;
using Jy.IRepositories;
using Jy.Utility;
using Jy.AuthAdmin.API.Filter;
using Jy.ServicesKeep;
using Jy.HealthCheck;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Swashbuckle.AspNetCore.Swagger;
using Microsoft.Extensions.PlatformAbstractions;
using System.Reflection;
using Pivotal.Discovery.Client;
using SolrNetCore;
using SolrNet;
using Jy.AuthAdmin.SolrIndex;
using Jy.Domain.IIndex;
using Jy.IIndex;
using Jy.AuthService;
using Jy.EntityFramewordCoreBase.Repositories;

namespace Jy.AuthAdmin.API
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
            //TokenService.JyMapper.Initialize();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //default solr connect
            services.AddSolrNet(Configuration.GetSection("SIndexSettings").GetValue<string>("defaultConnectionString"));
            //添加数据上下文，已换成SDBSettings初始化DbContext，除了目前的主库中的userindex操作，以后可以换成存solr或elec
            //services.AddDbContext<JyDbContext>(options => options.UseNpgsql(Configuration.GetConnectionString("PostgreSQL"))); //PostgreSQL
            services.AddDbContext<JyDbContext>(options => options.UseMySql(Configuration.GetConnectionString("MySql")), ServiceLifetime.Scoped);//mysql
                                                                                                                                                //services.AddDbContext<JyDBReadContext>(options => options.UseMySql(Configuration.GetConnectionString("MySqlRead")), ServiceLifetime.Scoped);//mysqlread
            services.AddDbContextPool<JyDBReadContext>(
        options => options.UseMySql(Configuration.GetConnectionString("MySqlRead"),
        mysqlOptions => mysqlOptions.MaxBatchSize(100)));

            //----dapper
            services.Configure<DapperOptions>(options =>
            {
                options.ConnectionString = Configuration.GetConnectionString("DapperMySql");
            });
            services.AddSingleton<DapperHelper>();

            //设置读取appsetting.json
            services.AddOptions();
            services.Configure<UrlConfigSetting>(Configuration.GetSection("UrlConfig"));//配置url
            services.Configure<SDBSettings>(Configuration.GetSection("SDBSettings"));
            services.Configure<SIndexSettings>(Configuration.GetSection("SIndexSettings"));
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

            //依赖注入
            services.AddScoped<PagedHelper>();

            services.AddScoped<ICacheService, Jy.CacheService.CacheService>();
            services.AddScoped<IQueueService, Jy.QueueSerivce.QueueSerivce>();

            services.AddScoped<IRepositoryContext, EntityFrameworkRepositoryContext>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IMenuRepository, MenuRepository>();
            services.AddScoped<IDepartmentRepository, DepartmentRepository>();

            services.AddScoped<IUserRepositoryRead, UserRepositoryRead>();
            services.AddScoped<IRoleRepositoryRead, RoleRepositoryRead>();
            services.AddScoped<IMenuRepositoryRead, MenuRepositoryRead>();
            services.AddScoped<IDepartmentRepositoryRead, DepartmentRepositoryRead>();

            services.AddScoped<IRepositoryFactory, RepositoryFactory>();
            services.AddScoped<IRepositoryReadFactory, RepositoryReadFactory>();

            services.AddScoped<IRoleAppService, RoleAppService>();
            services.AddScoped<IUserAppService, UserAppService>();
            services.AddScoped<IMenuAppService, MenuAppService>();
            services.AddScoped<IDepartmentAppService, DepartmentAppService>();

            services.AddScoped<IUserIndexsIndex, UserIndexsIndex>();
            services.AddScoped<IUserIndexsIndexRead, UserIndexsIndexRead>();
 
            services.AddScoped<IVerifyTokenAppService, VerifyTokenAppService>();
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
            services.AddMvc(options =>
            {
                options.Filters.Add(typeof(HttpGlobalExceptionFilter));
            });//.AddControllersAsServices();


            //------------version control and api document swagger
            // format the version as "'v'major[.minor][-status]"
            services.AddMvcCore().AddVersionedApiExplorer(o => o.GroupNameFormat = "'v'VVV");
            //http://www.hanselman.com/blog/ASPNETCoreRESTfulWebAPIVersioningMadeEasy.aspx
            //http://blog.csdn.net/jjhaochang/article/details/76573752
            services.AddApiVersioning(options => {
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ApiVersionReader = ApiVersionReader.Combine(new QueryStringApiVersionReader("v"));
            });//版本控制
            services.AddSwaggerGen(options =>
            {
                var provider = services.BuildServiceProvider()
                                          .GetRequiredService<IApiVersionDescriptionProvider>();
                foreach (var description in provider.ApiVersionDescriptions)
                {
                    options.SwaggerDoc(
                        description.GroupName,
                        new Info()
                        {
                            Title = $"Sample API {description.ApiVersion}",
                            Version = description.ApiVersion.ToString()
                        });
                }
                options.DescribeAllEnumsAsStrings();
                // 添加httpHeader参数
                options.OperationFilter<AuthorizationHeaderParameterOperationFilter>(); 
                // integrate xml comments
                options.IncludeXmlComments(XmlCommentsFilePath);
            });
            //------------version control and api document swagger

            //注册到eureka，springcloud服务发现的注册
            services.AddDiscoveryClient(Configuration);        
        }
        static string XmlCommentsFilePath
        {
            get
            {
                var basePath = PlatformServices.Default.Application.ApplicationBasePath;
                var fileName = typeof(Startup).GetTypeInfo().Assembly.GetName().Name + ".xml";
                return Path.Combine(basePath, fileName);
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory,
    IApiVersionDescriptionProvider provider)
        {
            app.UseHealthCheck("/HealthCheck", null,new TimeSpan(0,0,10));

            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            //-------------------------------------------------------serilog 配置
            MVCLogOptions mvcLogOptions = new MVCLogOptions()
            {
                LogPath = "D:\\LogFiles_AuthAdmin_API",//Configuration[nameof(AuthLogOptions.LogPath)],
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

            //app.UsePetapoco();//use  Petapocomiddleware
            app.UseDapper(); //----dapper

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
             .UseSwaggerUI(options =>
             {
                 // build a swagger endpoint for each discovered API version
                 foreach (var description in provider.ApiVersionDescriptions)
                 {
                     options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
                 }
             });
            //往zookeeper注册服务
            AuthAPIRegister.registerAuthAPIHostPort(Configuration.GetSection("UrlConfig").GetValue<string>("ZooKeeperList"));

            //注册到eureka，springcloud服务发现的注册
            //app.UseDiscoveryClient();
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
