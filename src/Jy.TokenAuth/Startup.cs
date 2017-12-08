using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Jy.TokenAuth.Middleware;
using System.Text;
using Microsoft.Extensions.Options;
using Jy.Domain.IRepositories;
using Jy.EntityFrameworkCore.Repositories;
using Jy.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Jy.Cache;
using Microsoft.Extensions.Caching.Memory;
using Serilog;
using System.IO;
using Jy.Utility.Paged;
using Jy.ICache;
using Jy.CacheService;
using Jy.TokenService;
using Jy.IRepositories;
using Jy.ServicesKeep;
using Jy.IIndex;
using Jy.AuthAdmin.SolrIndex;
using Jy.Domain.IIndex;

namespace Jy.TokenAuth
{
    /// <summary>
    /// 授权服务器
    /// </summary>
    public class Startup
    {
        // The secret key every token will be signed with.
         // In production, you should store this securely in environment variables
         // or a key management tool. Don't hardcode this into your application!
         private static readonly string secretKey = "123456Jy_12312321312dafdsfds";

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsEnvironment("Development"))
            {
                // This will push telemetry data through Application Insights pipeline faster, allowing you to view results immediately.
                builder.AddApplicationInsightsSettings(developerMode: true);
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
            //初始化映射关系
            JyMapper.Initialize();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container
        public void ConfigureServices(IServiceCollection services)
        {
            //添加数据上下文
            //services.AddDbContext<JyDbContext>(options => options.UseNpgsql(sqlConnectionString)); //PostgreSQL
            services.AddDbContext<JyDbContext>(options => options.UseMySql(Configuration.GetConnectionString("MySql")), ServiceLifetime.Scoped);//mysql
                                                                                                                                                    //services.AddDbContext<JyDBReadContext>(options => options.UseMySql(Configuration.GetConnectionString("MySqlRead")), ServiceLifetime.Scoped);//mysqlread
            services.AddDbContextPool<JyDBReadContext>(
        options => options.UseMySql(Configuration.GetConnectionString("MySqlRead"),
        mysqlOptions => mysqlOptions.MaxBatchSize(100)));

            services.Configure<SDBSettings>(Configuration.GetSection("SDBSettings"));
            services.Configure<SIndexSettings>(Configuration.GetSection("SIndexSettings"));
            //---------------缓存配置
            services.AddMemoryCache();
            //services.Configure<CacheProvider>(Configuration.GetSection("CacheConfig")); 

            string isUseRedis = Configuration.GetSection("CacheConfig").GetValue<string>("UseRedis");
            if (isUseRedis.ToUpper() == "TRUE")
            {
                //Use Redis
                services.AddSingleton(typeof(ICached), new RedisCacheRepository(new RedisCacheOptions
                {
                    Configuration = Configuration.GetSection("CacheConfig").GetValue<string>("Redis_ConnectionString"),//_cacheProvider._connectionString,
                    InstanceName = Configuration.GetSection("CacheConfig").GetValue<string>("Redis_InstanceName"),//_cacheProvider._instanceName
                    expTime = new TimeSpan(0, Configuration.GetSection("CacheConfig").GetValue<int>("expTime"), 0)
                }, 0));
            }
            else
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

            //依赖注入
            services.AddScoped<PagedHelper>();
            services.AddScoped<ICacheService, Jy.CacheService.CacheService>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IUserRepositoryRead, UserRepositoryRead>();
            services.AddScoped<IRoleRepositoryRead, RoleRepositoryRead>();
            services.AddScoped<IRepositoryFactory, RepositoryFactory>();
            services.AddScoped<IRepositoryReadFactory, RepositoryReadFactory>();

            services.AddScoped<IUserIndexsIndex, UserIndexsIndex>();
            services.AddScoped<IUserIndexsIndexRead, UserIndexsIndexRead>();

            services.AddScoped<IVerifyTokenAppService, VerifyTokenAppService>();
            //txtlog
            services.AddSingleton<ILog.ILogger, Jy.TokenAuth.Logger>();

            // Add framework services.
            services.AddApplicationInsightsTelemetry(Configuration);

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, Microsoft.Extensions.Logging.ILoggerFactory loggerFactory)
        {
            //-------------------------------------------------------serilog 配置
            AuthLogOptions authLogOptions = new AuthLogOptions()
            {
                LogPath = "D:\\LogFiles_TokenAuth",//Configuration[nameof(AuthLogOptions.LogPath)],
                PathFormat = "Auth_{Date}.log"
            };
            var serilog = new LoggerConfiguration()
               .MinimumLevel.Debug()
               .Enrich.FromLogContext()
               .WriteTo.RollingFile(Path.Combine(authLogOptions.LogPath, authLogOptions.PathFormat),
                   outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {Message}{NewLine}{Exception}");
            AuthLogOptions.EnsurePreConditions(authLogOptions);

            loggerFactory.AddSerilog(serilog.CreateLogger());

            // Ensure any buffered events are sent at shutdown 日志的生命周期
            IApplicationLifetime appLifetime = (IApplicationLifetime)app.ApplicationServices.GetService(typeof(IApplicationLifetime));
            if (appLifetime != null)
            {
                appLifetime.ApplicationStopped.Register(Log.CloseAndFlush);
            }
            app.UseAuthLog(authLogOptions);//这个中间件用作记录请求中的过程日志
            //---------------------------------------------------serilog 配置

            //loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            //loggerFactory.AddDebug();

            app.UseMvc();

            app.UseStaticFiles();

            // Add JWT generation endpoint:
            var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey));
            var options = new TokenProviderOptions
            {
                Audience = "JyAudience",
                Issuer = "JyIssuer",
                Expiration = new TimeSpan(0,30,0),
                SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256),
            };

            app.UseMiddleware<TokenProviderMiddleware>(Options.Create(options));

            app.UseMvc();
            //往zookeeper注册服务
            TokenAuthRegister.registerTokenAuthHostPort(Configuration.GetSection("UrlConfig").GetValue<string>("ZooKeeperList"));
        }
        /*
         * http://www.cnblogs.com/tdws/p/6536864.html
         * 
         　　　　1.这个人是谁？

　　　　2.这个人可以用此token访问什么样的内容？（scope）

　　　　3.token的过期时间 (expire)

　　　　4.谁发行的token。

　　　　5.其他任何你希望加入的声明（Claims）

　那我们为什么要使用token呢？使用session或者用redis来实现stateServer不好吗？

　　　　1.token是低（无）状态的，Statelessness

　　　　2.token可以与移动端应用紧密结合

　　　　3.支持多平台服务器和分布式微服务


        需要验证token的应用端添加：
        
         private static readonly string secretKey = "123456Jy_12312321312dafdsfds";
        private void ConfigureJwtAuth(IApplicationBuilder app)
        {            
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
 
            app.UseJwtBearerAuthentication(new JwtBearerOptions
            {
                AutomaticAuthenticate = true,
                AutomaticChallenge = true,
                TokenValidationParameters = tokenValidationParameters,
            });                        
         }

         */
    }
}
