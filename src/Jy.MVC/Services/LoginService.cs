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
    public class LoginService : ILoginService
    {
        private readonly string _remoteServiceBaseUrl;
        private readonly IHttpClient _apiClient;
        private readonly ILogger _logger;
        private readonly IOptionsSnapshot<UrlConfigSetting> _urlConfig;
        private IHttpContextAccessor _httpContextAccesor;
        public LoginService(IHttpClient apiClient, ILogger logger, IOptionsSnapshot<UrlConfigSetting> urlConfig, IHttpContextAccessor httpContextAccesor)
        {
            _urlConfig = urlConfig;
            _httpContextAccesor = httpContextAccesor;
            _apiClient = apiClient;
            _logger = logger;
            _remoteServiceBaseUrl = $"{KeepCallServer.getAuthAPIHostPort(_urlConfig.Value.ZooKeeperList)}{_urlConfig.Value.AuthUrl}/Login/";
        }

        async Task<string> GetUserTokenAsync()
        {
            //var context = _httpContextAccesor.HttpContext;
            //return await context.Authentication.GetTokenAsync("access_token");
            return await _httpContextAccesor.HttpContext.GetTokenAsync("access_token");
        }
        public async Task<ReturnObj> Index()
        {
            var token = await GetUserTokenAsync();
            var response = await _apiClient.PostAsync(API.Auth.Index(_remoteServiceBaseUrl),"", token);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ReturnObj>(result);
        }
        public async Task<TokenRes> GetToken(LoginModel model)
        {
            var token = await GetUserTokenAsync();
            var response = await _apiClient.PostAsync(API.Auth.GetToken(_remoteServiceBaseUrl), model, token);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<TokenRes>(result);
        }
        public async Task<TokenRes> VerifyToken(string token)
        {
            var ctoken = await GetUserTokenAsync();
            var response = await _apiClient.PostAsync(API.Auth.VerifyToken(_remoteServiceBaseUrl), token, ctoken);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<TokenRes>(result);
        }
        public async Task<TokenRes> LogOutToken(string token)
        {
            var ctoken = await GetUserTokenAsync();
            var response = await _apiClient.PostAsync(API.Auth.LogOutToken(_remoteServiceBaseUrl), token, ctoken);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<TokenRes>(result);
        }
        public async Task<LeftMenu> GetMenuForLeftMenu(string token)
        {
            var ctoken = await GetUserTokenAsync();
            var response = await _apiClient.GetStringAsync(API.Auth.GetMenuForLeftMenu(_remoteServiceBaseUrl,token), ctoken);
            return JsonConvert.DeserializeObject<LeftMenu>(response);
        }
      
    }
}
