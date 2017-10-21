using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jy.MVC.Models;
using Jy.ILog;
using Jy.Resilience.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Jy.MVC.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using Jy.Utility;
using Jy.ServicesKeep;

namespace Jy.MVC.Services
{
    public class AuthorityService : IAuthorityService
    {
        private readonly string _remoteServiceBaseUrl;
        private readonly IHttpClient _apiClient;
        private readonly ILogger _logger;
        private readonly IOptionsSnapshot<UrlConfigSetting> _urlConfig;
        private IHttpContextAccessor _httpContextAccesor;
        public AuthorityService(IHttpClient apiClient, ILogger logger, IOptionsSnapshot<UrlConfigSetting> urlConfig, IHttpContextAccessor httpContextAccesor)
        {
            _urlConfig = urlConfig;
            _httpContextAccesor = httpContextAccesor;
            _apiClient = apiClient;
            _logger = logger;
            _remoteServiceBaseUrl = $"{KeepCallServer.getAuthAPIHostPort(_urlConfig.Value.ZooKeeperList)}{_urlConfig.Value.AuthUrl}/Authority/";
        }

        public async Task<ReturnObj> BatchUserRole(BatchUserRoleModel rpm)
        {
            var token = await GetUserTokenAsync();
            var response = await _apiClient.PutAsync(API.Auth.BatchUserRole(_remoteServiceBaseUrl), rpm, token);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ReturnObj>(result);
        }

        public async Task<List<TreeCheckBoxModel>> GetBatchRoleTreeData()
        {
            var ctoken = await GetUserTokenAsync();
            var response = await _apiClient.GetStringAsync(API.Auth.GetBatchRoleTreeData(_remoteServiceBaseUrl), ctoken);
            return JsonConvert.DeserializeObject<List<TreeCheckBoxModel>>(response);
        }
         
        public async Task<List<TreeCheckBoxModel>> GetRoleTreeData(Guid id)
        {
            var ctoken = await GetUserTokenAsync();
            var response = await _apiClient.GetStringAsync(API.Auth.GetRoleTreeData(_remoteServiceBaseUrl,id), ctoken);
            return JsonConvert.DeserializeObject<List<TreeCheckBoxModel>>(response);
        } 
        public async Task<ReturnObj> UserRole(UserRoleModel rpm)
        {
            var token = await GetUserTokenAsync();
            var response = await _apiClient.PutAsync(API.Auth.UserRole(_remoteServiceBaseUrl), rpm, token);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ReturnObj>(result);
        }

        async Task<string> GetUserTokenAsync()
        {
            //var context = _httpContextAccesor.HttpContext;
            //return await context.Authentication.GetTokenAsync("access_token");
            return await _httpContextAccesor.HttpContext.GetTokenAsync("access_token");
        }
 
    }
}
