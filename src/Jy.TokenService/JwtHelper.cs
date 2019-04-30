using JWT;
using System;
using System.Collections.Generic;
using System.Text;
using Jy.Utility;
using JWT.Algorithms;
using JWT.Serializers;
using System.Security.Cryptography;
using System.Linq;

namespace Jy.TokenService
{
    public class JwtHelper
    {
        private const int JwtSurvivalMinutes = 20;
        public const int JwtEarlyWarningMinutes = -5;

        private static string _baseAccessPrivateKey;
        private static string _baseJwtPrivateKey;
        private static string _baseAccessKey;
        private static string _baseJwtKey;

        public static void Init(string baseJwtPrivateKey)
        {
            _baseAccessPrivateKey =
                "1123322132133423413fdsffdfdfskjj12df";
            _baseJwtPrivateKey = baseJwtPrivateKey;

            _baseAccessKey = _baseAccessPrivateKey.FromBase64String();
            _baseJwtKey = _baseJwtPrivateKey.FromBase64String();
        }

        public static Ret<JyJwt> ApplyJwt(string bizName, long time, string applyCode)
        {
            var codeCreated = CreateApplyCode(bizName, time, new RealKey(bizName, _baseAccessKey));
            if (codeCreated != applyCode)
                return new Ret<JyJwt>(
                    string.Format($"applyCode错误.请求参数:bizName={bizName},time={time},applyCode={applyCode}."), null,
                    null);

            return CreateJwt(bizName);
        }

        public static Ret<JyJwt> CreateJwt(string bizName)
        {
            IJwtAlgorithm algorithm = new HMACSHA256Algorithm();
            IJsonSerializer serializer = new JsonNetSerializer();
            IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
            IJwtEncoder encoder = new JwtEncoder(algorithm, serializer, urlEncoder);

            var payload = CreatePayload(bizName, out DateTime expireTime);
            var token = "bear " + encoder.Encode(payload, new RealKey(bizName, _baseJwtKey).ToString());
            return new Ret<JyJwt>(true, "", "", new JyJwt { ExpireTime = expireTime, Jwt = token });
        }

        public static Ret<bool> CheckJwt(string jwt)
        {
            var jwtMain = jwt.Replace("bear ", "", StringComparison.OrdinalIgnoreCase);

            IJsonSerializer serializer = new JsonNetSerializer();
            IDateTimeProvider provider = new UtcDateTimeProvider();
            IJwtValidator validator = new JwtValidator(serializer, provider);
            IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
            IJwtDecoder decoder = new JwtDecoder(serializer, validator, urlEncoder);

            var payloadRet = GetPayload(jwtMain);
            if (!payloadRet.IsNormal)
                return new Ret<bool>(string.Format($"token={jwt},{payloadRet.ExceptionMsg}"), null, false);

            var payload = payloadRet.Data;
            var bizName = payload.Iss;

            var reqestInfo = string.Format($"BizName={bizName}");
            try
            {
                decoder.DecodeToObject<JyPayload>(jwtMain, new RealKey(bizName, _baseJwtKey).ToString(), verify: true);

                long expTime;
                if (!long.TryParse(payload.Exp, out expTime))
                    return new Ret<bool>(string.Format($"Exp={payload.Exp}的值无效!"), null, false);

                var nowTime = GetTimeStamp(DateTime.UtcNow);
                if (expTime < nowTime)
                    return new Ret<bool>(
                        string.Format(
                            $"Exp={payload.Exp}小于当前时间{nowTime},已过期!"),
                        null, false);

                //验证通过
                return new Ret<bool>(true, "", "", true);
            }
            catch (TokenExpiredException tee)
            {
                return new Ret<bool>(string.Format($"{reqestInfo}. token={jwt}"), tee, false);
            }
            catch (SignatureVerificationException sve)
            {
                return new Ret<bool>(string.Format($"{reqestInfo}. token={jwt}"), sve, false);
            }
            catch (Exception ex)
            {
                return new Ret<bool>(string.Format($"{reqestInfo}. token={jwt}"), ex, false);
            }
        }

        private static Ret<JyPayload> GetPayload(string tokenMain)
        {
            try
            {
                var splits = tokenMain.Split('.');
                if (splits.Length == 3)
                {
                    var payloadEncoded = splits[1];
                    var bytes = new JwtBase64UrlEncoder().Decode(payloadEncoded);
                    var payload = Encoding.Default.GetString(bytes);
                    var JyPayload = Newtonsoft.Json.JsonConvert.DeserializeObject<JyPayload>(payload);
                    var iss = JyPayload.Iss;
                    return new Ret<JyPayload>(true, "", "", JyPayload);
                }
            }
            catch (Exception ex)
            {
                return new Ret<JyPayload>(ex.Message, ex, null);
            }

            return new Ret<JyPayload>("Token格式错误,或Decode时失败.", null, null);
        }

        private static JyPayload CreatePayload(string bizName, out DateTime expireTime)
        {
            var now = DateTime.UtcNow;
            expireTime = now.AddMinutes(JwtSurvivalMinutes);

            var iat = GetTimeStamp(now).ToString();
            var exp = GetTimeStamp(expireTime).ToString();
            return new JyPayload { Iss = bizName, Iat = iat, Exp = exp };
        }

        private static long GetTimeStamp(DateTime utcTime)
        {
            var ts = utcTime - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return (long)(ts.TotalSeconds);
        }

        private static string CreateApplyCode(string bizName, long time, RealKey realKey)
        {
            var queryStr = string.Format($"biz={bizName}&time={time}&key={realKey}");
            return GetMd5(queryStr);
        }

        private static string GetMd5(string str)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            var bytValue = Encoding.UTF8.GetBytes(str);
            var bytHash = md5.ComputeHash(bytValue);
            md5.Clear();
            string sTemp = bytHash.Aggregate("", (current, t) => current + t.ToString("X").PadLeft(2, '0'));
            return sTemp.ToLower();
        }
    }

    class RealKey
    {
        public string BaseKey { get; set; }
        public string BizName { get; set; }

        public RealKey(string bizName, string baseKey)
        {
            BaseKey = baseKey;
            BizName = bizName;
        }

        public override string ToString()
        {
            return BizName + "__" + BaseKey;
        }
    }
    public class JyJwt
    {
        public DateTime ExpireTime { get; set; }
        public string Jwt { get; set; }
    }
    public class JyPayload
    {
        /// <summary>
        /// jwt签发者
        /// </summary>
        public string Iss { get; set; }

        /// <summary>
        /// jwt的签发时间
        /// </summary>
        public string Iat { get; set; }

        /// <summary>
        /// jwt的过期时间,必须要大于签发时间
        /// </summary>
        public string Exp { get; set; }
    }
    public class Ret<T> 
    {
        public T Data { get; set; }
        public bool IsNormal { get; set; }
        public string ExceptionMsg { get; set; }
        public string ExceptionDetail { get; set; }

        public Ret()
        {

        }

        public Ret(bool isNormal, string msg, string detail, T data) 
        {
            Data = data;
            IsNormal = isNormal;
            ExceptionMsg = msg;
            ExceptionDetail = detail;
        }

        public Ret(string msg, Exception ex, T data)
        {
            Data = data;

            IsNormal = false;

            if (string.IsNullOrWhiteSpace(msg))
                ExceptionMsg = string.Format($"{msg}.");

            var tempEx = ex;
            while (tempEx != null)
            {
                ExceptionMsg += string.Format($"{ex.Message}.");
                ExceptionDetail += string.Format($"{ex.StackTrace}.");

                tempEx = tempEx.InnerException;
            }
        }
    }
}
