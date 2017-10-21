using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Serilog;
using Jy.MVC.Middleware;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Jy.MVC.Services;
using Jy.MVC.Infrastructure;
using Jy.Resilience.Http;
using Jy.Domain;
using Jy.Utility;

namespace Jy.MVC
{
    public class Startup
    {
        public IConfigurationRoot Configuration { get; }
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);
            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
            //初始化映射关系
            DomainMapper.Initialize();
           // TokenService.JyMapper.Initialize();
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        { 
            //添加数据上下文
            //services.AddDbContext<JyDbContext>(options => options.UseNpgsql(sqlConnectionString)); //PostgreSQL
            //services.AddDbContext<JyDbContext>(options => options.UseMySql(Configuration.GetConnectionString("MySql")),ServiceLifetime.Scoped);//mysql
            //services.AddDbContext<JyDBReadContext>(options => options.UseMySql(Configuration.GetConnectionString("MySqlRead")), ServiceLifetime.Scoped);//mysqlread
            //services.Configure<SDBSettings>(Configuration.GetSection("SDBSettings"));
            ////----dapper
            //services.Configure<DapperOptions>(options =>
            //{
            //    options.ConnectionString = Configuration.GetConnectionString("DapperMySql");
            //});
            //services.AddSingleton<DapperHelper>();
            //----PetaPoco
            //services.Configure<PetapocoOptions>(options =>
            //{
            //    options.ConnectionString = Configuration.GetConnectionString("PetaPocoMySql");
            //});
            //services.AddSingleton<PetaPocoHelper>();

            //设置读取appsetting.json
            services.AddOptions();
            services.Configure<UrlConfigSetting>(Configuration.GetSection("UrlConfig"));//配置url
        
            //txtlog
            services.AddSingleton<ILog.ILogger, Logger>();

            
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            //ResilientHttpClient
            if (Configuration.GetValue<string>("UseResilientHttp") == bool.TrueString)
            {
                services.AddSingleton<IResilientHttpClientFactory, ResilientHttpClientFactory>();
                services.AddSingleton<IHttpClient, ResilientHttpClient>(sp => sp.GetService<IResilientHttpClientFactory>().CreateResilientHttpClient());
            }
            else
            {
                services.AddSingleton<IHttpClient, StandardHttpClient>();
            }
            services.AddScoped<ILoginService, LoginService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IDepartmentService, DepartmentService>();
            services.AddScoped<IMenuService, MenuService>();
            services.AddScoped<IAuthorityService, AuthorityService>();
            
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

            services.AddMvc();
            //Session服务
            services.AddSession();
            //anttoken [ValidateAntiForgeryToken] csrf攻击验证
            //var anttoken = $("input[name='csrfField']").val();     //,"CUSTOMER-CSRF-HEADER": anttoken
            //services.AddAntiforgery(option => { option.CookieName = "CUSTOMER-CSRF-COOKIE"; option.FormFieldName = "csrfField"; option.HeaderName = "CUSTOMER-CSRF-HEADER"; });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            //-------------------------------------------------------serilog 配置
            MVCLogOptions mvcLogOptions = new MVCLogOptions()
            {
                LogPath = "D:\\LogFiles_WEB",//Configuration[nameof(AuthLogOptions.LogPath)],
                PathFormat = "WEB_{Date}.log"
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

            //loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            //loggerFactory.AddDebug();
            //loggerFactory.AddConsole();

            //app.UsePetapoco();//use  Petapocomiddleware
            //app.UseDapper(); //----dapper

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

            //Jwt认证
            //ConfigureJwtAuth(app);

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
        // }
    }
}
