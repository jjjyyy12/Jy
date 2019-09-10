using Jy.Application.DepartmentApp;
using Jy.Application.MenuApp;
using Jy.Application.RoleApp;
using Jy.Application.UserApp;
using Jy.AuthAdmin.SolrIndex;
using Jy.AuthService;
using Jy.Cache;
using Jy.CacheService;
using Jy.CKafka;
using Jy.Dapper;
using Jy.Domain.IIndex;
using Jy.Domain.IRepositories;
using Jy.EntityFrameworkCore;
using Jy.EntityFrameworkCore.Repositories;
using Jy.HealthCheck;
using Jy.ICache;
using Jy.IIndex;
using Jy.ILog;
using Jy.IRepositories;
using Jy.QueueSerivce;
using Jy.RabbitMQ;
using Jy.Resilience.Http;
using Jy.ServicesKeep;
using Jy.TokenService;
using Jy.Utility;
using Jy.Utility.Const;
using Jy.Utility.Paged;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using SolrNet;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Jy.Component.Extensions
{
    public static class JyExtensions
    {
        public static void InitAllComponents(this IServiceCollection services, IConfigurationRoot Configuration, string nodeName)
        {
            services.AddOptions();
            services.AddSerializerServices();
            //txtlog
            services.AddSingleton<ILog.ILogger, ILog.Logger>();

            services.Configure<UrlConfigSetting>(Configuration.GetSection("UrlConfig"));//配置url

            InitSolrIndex(services, Configuration);
            InitCache(services, Configuration);
            InitDB(services, Configuration);
            InitMQ(services, Configuration);
            InitService(services, Configuration);
            InitTokenService(services, Configuration);

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            //ResilientHttpClient
            if (Configuration.GetValue<string>("UseResilientHttp") == bool.TrueString)
            {
                services.AddSingleton<IResilientHttpClientFactory, ResilientHttpClientFactory>();
                services.AddSingleton<IHttpClient, ResilientHttpClient>(sp => sp.GetService<IResilientHttpClientFactory>().CreateResilientHttpClient());
            }
            else if (Configuration.GetValue<string>("UseHttpClientFactory") == bool.TrueString)
            {
                services.AddHttpClient();
                var serviceProvider = services.BuildServiceProvider();
                JyHttpClientFactory.Init(serviceProvider);
                services.AddScoped<IHttpClient, JyHttpClient>();
            }
            else
            {
                services.AddSingleton<IHttpClient, StandardHttpClient>();
            }
            services.AddMvc(options =>
            {
                options.Filters.Add(typeof(HttpGlobalExceptionFilter));
            });//.AddControllersAsServices();

            InitSwagger(services, Configuration);
            InitRegisterAPIHostPort(nodeName, Configuration);

            services.AddScoped<PagedHelper>();
        }
        public static void InitAllUIComponents(this IServiceCollection services, IConfigurationRoot Configuration, string nodeName)
        {
            services.AddOptions();
            services.AddSerializerServices();
            //txtlog
            services.AddSingleton<ILog.ILogger, ILog.Logger>();

            services.Configure<UrlConfigSetting>(Configuration.GetSection("UrlConfig"));//配置url

            InitSolrIndex(services, Configuration);
            InitCache(services, Configuration);

            InitMQ(services, Configuration);
            InitService(services, Configuration);
            InitTokenService(services, Configuration);

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            //ResilientHttpClient
            if (Configuration.GetValue<string>("UseResilientHttp") == bool.TrueString)
            {
                services.AddSingleton<IResilientHttpClientFactory, ResilientHttpClientFactory>();
                services.AddSingleton<IHttpClient, ResilientHttpClient>(sp => sp.GetService<IResilientHttpClientFactory>().CreateResilientHttpClient());
            }
            else if (Configuration.GetValue<string>("UseHttpClientFactory") == bool.TrueString)
            {
                services.AddHttpClient();
                var serviceProvider = services.BuildServiceProvider();
                JyHttpClientFactory.Init(serviceProvider);
                services.AddScoped<IHttpClient, JyHttpClient>();
            }
            else
            {
                services.AddSingleton<IHttpClient, StandardHttpClient>();
            }
            services.AddMvc(options =>
            {
                options.Filters.Add(typeof(HttpGlobalExceptionFilter));
            });//.AddControllersAsServices();

            services.AddMvc();
            InitRegisterAPIHostPort(nodeName, Configuration);
            services.AddSession();
        }
        public static void InitSwagger(this IServiceCollection services,IConfigurationRoot Configuration)
        {
            //------------version control and api document swagger
            // format the version as "'v'major[.minor][-status]"
            services.AddMvcCore();
            services.AddVersionedApiExplorer(o => o.GroupNameFormat = "'v'VVV");
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
                            Title = $"AuthAdmin HTTP API {description.ApiVersion}",
                            Version = description.ApiVersion.ToString()
                        });
                }
                options.DescribeAllEnumsAsStrings();
                // 添加httpHeader参数
                options.OperationFilter<AuthorizationHeaderParameterOperationFilter>();
                // integrate xml comments
                options.IncludeXmlComments(XmlCommentsFilePath);
            });
            //------------end version control and api document swagger
        }
        static string XmlCommentsFilePath
        {
            get
            {
                var basePath = PlatformServices.Default.Application.ApplicationBasePath;
                var fileName = "swagger.xml";
                return Path.Combine(basePath, fileName);
            }
        }
        public static void InitService(this IServiceCollection services, IConfigurationRoot Configuration)
        {
            services.AddScoped<IRoleAppService, RoleAppService>();
            services.AddScoped<IUserAppService, UserAppService>();
            services.AddScoped<IMenuAppService, MenuAppService>();
            services.AddScoped<IDepartmentAppService, DepartmentAppService>();
        }
        public static void InitDB(this IServiceCollection services, IConfigurationRoot Configuration)
        {
            services.Configure<SDBSettings>(Configuration.GetSection("SDBSettings"));
            //----dapper
            services.Configure<DapperOptions>(options =>
            {
                options.ConnectionString = Configuration.GetConnectionString("DapperMySql");
            });
            services.AddSingleton<DapperHelper>();

            //添加数据上下文，已换成SDBSettings初始化DbContext，除了目前的主库中的userindex操作，以后可以换成存solr或elec
            //services.AddDbContext<JyDbContext>(options => options.UseNpgsql(Configuration.GetConnectionString("PostgreSQL"))); //PostgreSQL
            services.AddDbContext<JyDbContext>(options => options.UseMySql(Configuration.GetConnectionString("MySql")), ServiceLifetime.Scoped);//mysql
                                                                                                                                                //services.AddDbContext<JyDBReadContext>(options => options.UseMySql(Configuration.GetConnectionString("MySqlRead")), ServiceLifetime.Scoped);//mysqlread
            services.AddDbContextPool<JyDBReadContext>(
                options => options.UseMySql(Configuration.GetConnectionString("MySqlRead"),
                mysqlOptions => mysqlOptions.MaxBatchSize(100)));

            services.AddScoped<IRepositoryContext, AuthRepositoryContext>();
            services.AddScoped<IRepositoryReadContext, AuthRepositoryReadContext>();

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
        }
       
        public static void InitSolrIndex(this IServiceCollection services, IConfigurationRoot Configuration)
        {
            services.Configure<SIndexSettings>(Configuration.GetSection("SIndexSettings"));
            //default solr connect
            services.AddSolrNet(Configuration.GetSection("SIndexSettings").GetValue<string>("defaultConnectionString"));
            services.AddScoped<IUserIndexsIndex, UserIndexsIndex>();
            services.AddScoped<IUserIndexsIndexRead, UserIndexsIndexRead>();
        }
        
        public static void InitCache(this IServiceCollection services, IConfigurationRoot Configuration)
        {
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
                    expTime = new TimeSpan(0, Configuration.GetSection("CacheConfig").GetValue<int>("expTime"), 0),
                    ConnectTimeout = Configuration.GetSection("CacheConfig").GetValue<int>("expTime") * 60 * 1000
                }, 0));
            }
            if ("TRUE".Equals(Configuration.GetSection("CacheConfig").GetValue<string>("UseHttpCache").ToUpper()))
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

            services.AddScoped<ICacheService, Jy.CacheService.CacheService>();
        }

        public static void InitMQ(this IServiceCollection services, IConfigurationRoot Configuration)
        {
            //------------------------rabbitMQ
            //services.AddRawRabbit();
            //services.AddScoped<IQueueOperation, QueueOperationRawRabbit>();
            //services.AddScoped<IQueueOperationSubscriptionsManager, InMemorySubscriptionsManager>();
            services.AddRabbitMQServices(Configuration);
            //------------------------rabbitMQ

            //------------------------kafka
            //services.AddScoped<IBigQueueOperation, QueueOperationRdKafka>();
            services.AddCKafkaServices(Configuration);
            //------------------------kafka

            services.AddScoped<IQueueService, Jy.QueueSerivce.QueueSerivce>();
        }
        private static readonly string secretKey = "123456Jy_12312321312dafdsfds";
        public static void InitTokenService(this IServiceCollection services, IConfigurationRoot Configuration)
        {
            services.AddScoped<IVerifyTokenAppService, VerifyTokenAppService>();
            services.AddScoped<ITokenAuthService, TokenAuthService>();

            //----------------jwt  http://www.cnblogs.com/JacZhu/p/6837676.html
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
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = tokenValidationParameters;
                options.RequireHttpsMetadata = false;
            });
            // 注册验证要求的处理器，可通过这种方式对同一种要求添加多种验证
            services.AddSingleton<IAuthorizationHandler, Jy.MVCAuthorization.ValidJtiHandler>();
            //----------------end jwt
        }


        public static void ConfigureAllComponent(this IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory,
IApiVersionDescriptionProvider provider, IConfigurationRoot Configuration)
        {
            app.UseHealthCheck("/HealthCheck", null, new TimeSpan(0, 0, 10));

            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            //-------------------------------------------------------serilog 配置
            MVCLogOptions mvcLogOptions = new MVCLogOptions()
            {
                LogPath = "D:\\LogFiles_API",//Configuration[nameof(AuthLogOptions.LogPath)],
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
          
 
        }
        public static void ConfigureAllUIComponent(this IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory,
 IConfigurationRoot Configuration)
        {
            app.UseHealthCheck("/HealthCheck", null, new TimeSpan(0, 0, 10));

            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            //-------------------------------------------------------serilog 配置
            MVCLogOptions mvcLogOptions = new MVCLogOptions()
            {
                LogPath = "D:\\LogFiles_API",//Configuration[nameof(AuthLogOptions.LogPath)],
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

            //Session 
            app.UseSession(new SessionOptions() { IdleTimeout = TimeSpan.FromMinutes(30) });

            //使用Mvc，设置默认路由为系统登录
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Login}/{action=Index}/{id?}");
            });

            //cookie验证
            //app.UseCookieAuthentication(CookieAuthMiddleware.GetOptions());
            //app.UseOwin();
            //app.UseCors(a => { a.AllowAnyOrigin(); });
            // Listen for login and logout requests //cookie验证
            //app.Map("/loginverify", builder =>
            //{
            //    builder.Run(async context =>
            //    {
            //        var name = context.Request.Form["name"];
            //        var pwd = context.Request.Form["pwd"];
            //        if (name == "wushuang" && pwd == "wushuang")
            //        {

            //            var claims = new List<Claim>() { new Claim("name", name), new Claim("role", "admin") };
            //            var identity = new ClaimsIdentity(claims, "password");
            //            var principal = new ClaimsPrincipal(identity);
            //            await context.Authentication.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
            //            context.Response.Redirect("/Index/Home");
            //        }
            //        else
            //        {
            //            await context.Authentication.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            //            context.Response.Redirect("/Index/Home");
            //        }
            //    });
            //});

            //app.Map("/logout", builder =>
            //{
            //    builder.Run(async context =>
            //    {
            //        // Sign the user out / clear the auth cookie
            //        await context.Authentication.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            //        // Perform a simple redirect after logout
            //        context.Response.Redirect("/");
            //    });
            //});
        }
        //往zookeeper注册服务
        public static void InitRegisterAPIHostPort(string nodeName, IConfigurationRoot Configuration)
        {
            switch (nodeName)
            {
                case NodeName.AuthAdmin:
                    AuthAPIRegister.registerAuthAPIHostPort(Configuration.GetSection("UrlConfig").GetValue<string>("ZooKeeperList"));
                    break;
                case NodeName.TokenAuth:
                    TokenAuthRegister.registerTokenAuthHostPort(Configuration.GetSection("UrlConfig").GetValue<string>("ZooKeeperList"));
                    break;
            }
        }

    }
}
