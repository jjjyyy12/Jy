using Jy.Domain.Dtos;
using Jy.Domain.Entities;
using Jy.ILog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Jy.CacheService;
using Jy.Domain.IRepositories;
using AutoMapper;
using Jy.Utility.Const;
using Jy.IRepositories;
using Jy.IIndex;
using Jy.Domain.IIndex;
using Microsoft.Extensions.Options;
using Jy.AuthAdmin.SolrIndex;

namespace Jy.TokenService
{
    /// <summary>
    /// 用户登录、验证用户权限
    /// </summary>
    public class VerifyTokenAppService : IVerifyTokenAppService
    {
        private ILogger _logger;
        private ICacheService _cacheService;
        private readonly IOptionsSnapshot<SIndexSettings> _SIndexSettings;
        //缓存 
        public ICacheService CacheService { set { _cacheService = value; } get { return _cacheService; } }

        private readonly IUserRepositoryRead _userrepositoryread;
        private readonly IRepositoryFactory _repositoryFactory;
        private readonly IRepositoryReadFactory _repositoryReadFactory;
        private readonly IRoleRepositoryRead _rolerepositoryread;
        private readonly IIndexFactory _indexFactory;
        private readonly IIndexReadFactory _indexReadFactory;
        public VerifyTokenAppService(ICacheService cacheService, ILogger logger
            , IUserRepositoryRead userrepositoryread, IRoleRepositoryRead rolerepository
            , IRepositoryFactory repositoryFactory, IRepositoryReadFactory repositoryReadFactory
            , IOptionsSnapshot<SIndexSettings> SIndexSettings)
        {
            _cacheService = cacheService;
            _userrepositoryread = userrepositoryread;
            _repositoryFactory = repositoryFactory;
            _repositoryReadFactory = repositoryReadFactory;
            _rolerepositoryread = rolerepository;
            _SIndexSettings = SIndexSettings;
            _indexReadFactory = new IndexReadFactory<UserIndexs>(_SIndexSettings);
            _indexFactory = new IndexFactory<UserIndexs>(_SIndexSettings);
            _logger = logger;
        }
        public async Task<string> GetToken(string username, string password, string role, string tokenServerURL)
        {
            HttpClient _httpClient = new HttpClient();
            var parameters = new Dictionary<string, string>();
            parameters.Add("username", username);
            parameters.Add("password", password);
            parameters.Add("role", role);
            HttpResponseMessage message_token = await _httpClient.PostAsync(tokenServerURL, new FormUrlEncodedContent(parameters));
            string res = await message_token.Content.ReadAsStringAsync();
            if (message_token.StatusCode != HttpStatusCode.OK)
                return res;

            var json = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(res);
            return json.Where(t => t.Key == "access_token").FirstOrDefault().Value.ToString();
        }
        public async Task<string> BlackToken(string oldToken, string role, string tokenServerURL)
        {
            HttpClient _httpClient = new HttpClient();
            var parameters = new Dictionary<string, string>();
            parameters.Add("oldtoken", oldToken);
            parameters.Add("role", role);
            HttpResponseMessage message_token = await _httpClient.PostAsync(tokenServerURL, new FormUrlEncodedContent(parameters));
            string res = await message_token.Content.ReadAsStringAsync();
            if (message_token.StatusCode != HttpStatusCode.OK)
                return res;

            var json = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(res);
            return json.Where(t => t.Key == "Result").FirstOrDefault().Value.ToString();
        }
        public void SaveToken(UserDto dto, string token, string jti, TimeSpan expires)
        {
            Dictionary<string,string> uinfo = new Dictionary<string, string>(2);
            uinfo.Add("jti", jti);
            uinfo.Add("userId",dto.Id.ToString());

            _cacheService.Cached.Add(token, uinfo, expires);
            _cacheService.Cached.Add(dto.Id.ToString(), new UserToken() { UserId = dto.Id
                ,Token = token
                ,LoginTime = DateTime.Now
                ,RoleIds = dto.RoleIds
                ,DepartmentId = dto.DepartmentId 
                ,DepartmentName=dto.DepartmentName
                ,UserName = dto.UserName
                ,Name = dto.Name
                ,Email = dto.Email
                ,Mobile = dto.MobileNumber
            }, expires);
        }
        public void BlackToken(string token, TimeSpan expires)
        {
            var uinfo = _cacheService.Cached.Get<Dictionary<string, string>>(token);
            var jti = uinfo?["jti"];
            if (!string.IsNullOrWhiteSpace(jti))
            {
                _cacheService.Cached.Add(jti, expires.TotalSeconds.ToString(), expires);
                _cacheService.Cached.Remove(token);
            }
        }
        public bool VerifyToken(string token)
        {
            var objtoken = _cacheService.Cached.Get<Dictionary<string, string>>(token);
            return objtoken != null;
        }
        public bool VerifyTokenRole(string userId, string roleIds)
        {
            var objtoken = _cacheService.Cached.Get<UserToken>(userId);
            if (objtoken == null) return false;
            return string.Join(",", objtoken.RoleIds).Equals(roleIds);
        }
        //验证当前请求权限时，父权限下需要的子权限action，T1存controllername,如Authority，T2存子请求的url的集合：如/User/Get,代表Authority依赖User
        //应避免页面的controller交叉
        private static readonly Dictionary<string, HashSet<string>> controllerSubActions = new Dictionary<string, HashSet<string>>() { { "Authority", new HashSet<string>() { "User" } } };
        private void LoadControllerSubActions(List<string> urls)
        {
            foreach (KeyValuePair<string, HashSet<string>> item in controllerSubActions)
            {
                foreach(string val in item.Value)
                {
                    string str = $"/{val}/Index";
                    if (urls.Exists(x => x.IndexOf(item.Key) >= 0)
                        && !urls.Exists(x => x.IndexOf(str) >= 0))
                    {
                        urls.Add(str);
                    }
                }
            }
        }
        public bool VerifyCurrActionRole(string userId, string currController)
        {
            var objtoken = _cacheService.Cached.Get<UserToken>(userId);
            if (objtoken == null) return false;

            List<string> urls = GetUserRowMenusUrls(objtoken.RoleIds);
            LoadControllerSubActions(urls);
            if (urls.IndexOf($"/{ currController}/Index") > -1)
            {
                return true;
            }
            return false;
        }
        public bool VerifyBlackRecordsToken(string jti)
        {
            var jtiBlackRecords = _cacheService.Cached.Get(jti);
            return jtiBlackRecords != null;
        }

        public List<RoleMenuDto> GetRowMenuForLeftMenu(string token)
        {
            var uinfo = _cacheService.Cached.Get<Dictionary<string, string>>(token);
            if (uinfo == null) return null;
            var userId = uinfo?["userId"];
            var userToken = _cacheService.Cached.Get<UserToken>(userId);
            if (userToken == null) return null;
            return GetRowMenuForLeftMenu(userToken.RoleIds);
        }
        public UserToken GetCurrentUserStatus(string token)
        {
            var uinfo = _cacheService.Cached.Get<Dictionary<string, string>>(token);
            if (uinfo == null) return null;
            var userId = uinfo?["userId"];
            return _cacheService.Cached.Get<UserToken>(userId);
        } 

        public UserDto CheckUser(string userName, string password)
        {
            _logger.LogInformation("CheckUser: username:{0}", userName);
            if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(password)) return null;

            //get from indexs
            var readindex = _indexReadFactory.CreateIndex<UserIndexs, IUserIndexsIndexRead>(Guid.NewGuid().ToString(), "authcore1");
            var userindexs = readindex.FirstOrDefault(new List<KeyValuePair<string, string>>(2) { new KeyValuePair<string, string>("name", userName), new KeyValuePair<string, string>("keywords", password) });
            UserIndex userindex = null;
            if (userindexs == null)
            {
                userindex = _userrepositoryread.CheckUserIndex(userName, password);
                if(userindex != null)//write to indexs
                {
                    var writeuserindexs = Mapper.Map<UserIndexs>(userindex);
                    var writeindex = _indexFactory.CreateIndex<UserIndexs, IUserIndexsIndex>(writeuserindexs.Id.ToString(), "authcore1");
                    writeindex.Insert(writeuserindexs);
                }
            }
            else
                userindex = Mapper.Map<UserIndex>(userindexs);
            
            var user = _repositoryReadFactory.CreateRepository<User,IUserRepositoryRead>(userindex.UserId.ToString()).CheckUser(userName, password);//
            return Mapper.Map<UserDto>(user);
        }
        public Task<bool> Login(Guid id)
        {
            return Task.Run(()=> {
                var cuser = _repositoryReadFactory.CreateRepository<User,IUserRepositoryRead>(id.ToString()).Get(id);
                if (cuser == null) return false;

                cuser.LastLoginTime = DateTime.Now;
                cuser.LoginTimes++;

                var user = _repositoryFactory.CreateRepository<User,IUserRepository>(id.ToString()).InsertOrUpdate(cuser);
                if (user == null)
                    return false;
                else
                    return true;
            });

        }
        private List<RoleMenuDto> GetAllRowMenus()
        {
            return _cacheService.Cached.Get<List<RoleMenuDto>>(() => { return Mapper.Map<List<RoleMenuDto>>(_rolerepositoryread.GetAllRoleMenus()); }, CacheKeyName.RoleMenuKey, default(TimeSpan));
        }

        private List<string> GetUserRowMenusUrls(List<Guid> roleIds)
        {
            List<RoleMenuDto> rlist = GetUserRowMenus(roleIds);
            List<string> slist = Mapper.Map<List<string>>(rlist);
            slist.RemoveAll(x => string.IsNullOrWhiteSpace(x));
            return slist;
        }
        private List<RoleMenuDto> GetRowMenuForLeftMenu(List<Guid> roleIds)
        {
            List<RoleMenuDto> rlist = GetUserRowMenus(roleIds);
            rlist.RemoveAll(x => string.IsNullOrWhiteSpace(x.MenuName));
            return rlist.Distinct(new RoleMenuDtoComparer()).ToList();
        }
        private List<RoleMenuDto> GetUserRowMenus(List<Guid> roleIds)
        {
            List<RoleMenuDto> rlist = GetAllRowMenus().Where(t => roleIds.Contains(t.RoleId)).ToList();
            return rlist;
        }

    }
}
