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
    public class DepartmentService : IDepartmentService
    {
        private readonly string _remoteServiceBaseUrl;
        private readonly IHttpClient _apiClient;
        private readonly ILogger _logger;
        private readonly IOptionsSnapshot<UrlConfigSetting> _urlConfig;
        private IHttpContextAccessor _httpContextAccesor;
        public DepartmentService(IHttpClient apiClient, ILogger logger, IOptionsSnapshot<UrlConfigSetting> urlConfig, IHttpContextAccessor httpContextAccesor)
        {
            _urlConfig = urlConfig;
            _httpContextAccesor = httpContextAccesor;
            _apiClient = apiClient;
            _logger = logger;
            _remoteServiceBaseUrl = $"{KeepCallServer.getAuthAPIHostPort(_urlConfig.Value.ZooKeeperList)}{_urlConfig.Value.AuthUrl}/Department/";
        }

        public async Task<ReturnObj> Delete(Guid id)
        {
            var token = await GetUserTokenAsync();
            var response = await _apiClient.DeleteAsync(API.Auth.Delete(_remoteServiceBaseUrl,id),  token);
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

        public async Task<ReturnObj> Create(Department dto)
        {
            var token = await GetUserTokenAsync();
            var response = await _apiClient.PostAsync(API.Auth.Edit(_remoteServiceBaseUrl), dto, token);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ReturnObj>(result);
        }
        public async Task<ReturnObj> Edit(Department dto)
        {
            var token = await GetUserTokenAsync();
            var response = await _apiClient.PutAsync(API.Auth.Edit(_remoteServiceBaseUrl), dto, token);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ReturnObj>(result);
        }
        public async Task<Department> Get(Guid id)
        {
            var ctoken = await GetUserTokenAsync();
            var response = await _apiClient.GetStringAsync(API.Auth.Get(_remoteServiceBaseUrl, id), ctoken);
            return JsonConvert.DeserializeObject<Department>(response);
        }

        public async Task<Paged<Department>> GetChildrenByParent(Guid parentId, int startPage, int pageSize)
        {
            var ctoken = await GetUserTokenAsync();
            var response = await _apiClient.GetStringAsync(API.Auth.GetChildrenByParent(_remoteServiceBaseUrl, parentId, startPage, pageSize), ctoken);
            return JsonConvert.DeserializeObject<Paged<Department>>(response);
        }

        public async Task<List<TreeModel>> GetTreeData()
        {
            var ctoken = await GetUserTokenAsync();
            var response = await _apiClient.GetStringAsync(API.Auth.GetTreeData(_remoteServiceBaseUrl), ctoken);
            return JsonConvert.DeserializeObject<List<TreeModel>>(response);
        }
 

        async Task<string> GetUserTokenAsync()
        {
            //var context = _httpContextAccesor.HttpContext;
            //return await context.Authentication.GetTokenAsync("access_token");
            return await _httpContextAccesor.HttpContext.GetTokenAsync("access_token");
        }
 
    }
}
