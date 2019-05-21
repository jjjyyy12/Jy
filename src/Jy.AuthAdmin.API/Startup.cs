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
using Jy.CKafka;
using Jy.Component.Extensions;
using Jy.Utility.Const;

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
            services.InitAllComponents(Configuration, NodeName.AuthAdmin);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory,
    IApiVersionDescriptionProvider provider)
        {
            app.ConfigureAllComponent(env, loggerFactory, provider,Configuration);
        }
    
       
    }
}
