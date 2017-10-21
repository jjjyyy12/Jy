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
    public class RoleService : IRoleService
    {
        private readonly string _remoteServiceBaseUrl;
        private readonly IHttpClient _apiClient;
        private readonly ILogger _logger;
        private readonly IOptionsSnapshot<UrlConfigSetting> _urlConfig;
        private IHttpContextAccessor _httpContextAccesor;
        public RoleService(IHttpClient apiClient, ILogger logger, IOptionsSnapshot<UrlConfigSetting> urlConfig, IHttpContextAccessor httpContextAccesor)
        {
            _urlConfig = urlConfig;
            _httpContextAccesor = httpContextAccesor;
            _apiClient = apiClient;
            _logger = logger;
            _remoteServiceBaseUrl = $"{KeepCallServer.getAuthAPIHostPort(_urlConfig.Value.ZooKeeperList)}{_urlConfig.Value.AuthUrl}/Role/";
        }

        public async Task<ReturnObj> Delete(Guid id)
        {
            var token = await GetUserTokenAsync();
            var response = await _apiClient.DeleteAsync(API.Auth.Delete(_remoteServiceBaseUrl, id), token);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ReturnObj>(result);
        }

        public async Task<ReturnObj> DeleteMuti(string ids)
        {
            var token = await GetUserTokenAsync();
            var response = await _apiClient.DeleteAsync(API.Auth.DeleteMuti(_remoteServiceBaseUrl, ids), token);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ReturnObj>(result);
        }
        public async Task<ReturnObj> Create(Role role)
        {
            var token = await GetUserTokenAsync();
            var response = await _apiClient.PutAsync(API.Auth.Edit(_remoteServiceBaseUrl), role, token);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ReturnObj>(result);
        }
        public async Task<ReturnObj> Edit(Role role)
        {
            var token = await GetUserTokenAsync();
            var response = await _apiClient.PutAsync(API.Auth.Edit(_remoteServiceBaseUrl), role, token);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ReturnObj>(result);
        }

        public async Task<Role> Get(Guid id)
        {
            var ctoken = await GetUserTokenAsync();
            var response = await _apiClient.GetStringAsync(API.Auth.Get(_remoteServiceBaseUrl, id), ctoken);
            return JsonConvert.DeserializeObject<Role>(response);
        }

        public async Task<Paged<Role>> GetListPaged(int startPage, int pageSize)
        {
            var ctoken = await GetUserTokenAsync();
            var response = await _apiClient.GetStringAsync(API.Auth.GetListPaged(_remoteServiceBaseUrl, startPage, pageSize), ctoken);
            return JsonConvert.DeserializeObject<Paged<Role>>(response);
        }

        public async Task<List<TreeCheckBoxModel>> GetMenuTreeData(Guid id)
        {
            var ctoken = await GetUserTokenAsync();
            var response = await _apiClient.GetStringAsync(API.Auth.GetMenuTreeData(_remoteServiceBaseUrl, id), ctoken);
            return JsonConvert.DeserializeObject<List<TreeCheckBoxModel>>(response);
        }

        public async Task<ReturnObj> RoleMenu(RoleMenuModel rpm)
        {
            var token = await GetUserTokenAsync();
            var response = await _apiClient.PutAsync(API.Auth.RoleMenu(_remoteServiceBaseUrl), rpm, token);
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
