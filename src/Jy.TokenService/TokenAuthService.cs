using Jy.ILog;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;


namespace Jy.TokenService
{
    /// <summary>
    /// 调用tokenauth服务相关，针对token在cache中统一的存取
    /// </summary>
    public class TokenAuthService : ITokenAuthService
    {
        private ILogger _logger;
        public TokenAuthService(ILogger logger)
        {
            _logger = logger;
        }
        public async Task<string> GetToken(string username, string password, string role, string tokenServerURL)
        {
            _logger.LogInformation("TokenAuthService.GetToken: username:{0},role:{1}", username, role);
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
        public async Task<string> VerifyToken(string token, string role, string tokenServerURL)
        {
            _logger.LogInformation("TokenAuthService.VerifyToken: token:{0},role:{1}", token, role);
            HttpClient _httpClient = new HttpClient();
            var parameters = new Dictionary<string, string>();
            parameters.Add("token", token);
            parameters.Add("role", role);
            HttpResponseMessage message_token = await _httpClient.PostAsync(tokenServerURL, new FormUrlEncodedContent(parameters));
            string res = await message_token.Content.ReadAsStringAsync();
            if (message_token.StatusCode != HttpStatusCode.OK)
                return res;

            var json = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(res);
            return json.Where(t => t.Key == "Result").FirstOrDefault().Value.ToString();
        }
        public async Task<string> BlackToken(string oldToken, string role, string tokenServerURL)
        {
            _logger.LogInformation("TokenAuthService.BlackToken: token:{0},role:{1}", oldToken, role);
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
       

    }
}
