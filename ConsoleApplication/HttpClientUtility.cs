using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Qunau.NetFrameWork.Common.Extension;

namespace Notify.Code.Utility
{
    /// <summary>
    /// HttpClientUtility
    /// </summary>
    public class HttpClientUtility
    {
        /// <summary>
        /// 将对象属性转换为key-value对
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns>字典</returns>
        private static Dictionary<string, string> ToMap<T>(T obj)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            Type t = typeof(T);
            foreach (PropertyInfo property in t.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                string name = property.Name;
                DynamicExpressionAttribute dynamicExpressionAttribute = CustomAttributeExtension<DynamicExpressionAttribute>.GetCustomAttributeValue(t, property);
                string key = dynamicExpressionAttribute.Name ?? name;
                string value = property.GetValue(obj).ToString();
                result.Add(key, value);
            }
            if (result.Count <= 0)
            {
                var str = obj.SerializeObject();
                string pattern = "(\"(?<key>[^,^\"]+)\":\"(?<value>[^:^\"]+)\")|(\"(?<key>[^,^\"]+)\":(?<value>[\\d\\.]+))";
                Regex regex = new Regex(pattern);
                var match = regex.Matches(str);
                foreach (Match item in match)
                {
                    var key = item.Groups["key"].ToString();
                    var value = item.Groups["value"].ToString();
                    result.Add(key, value);
                }
            }
            return result;
        }

        /// <summary>
        /// POST请求(FormUrlEncodedContent)
        /// </summary>
        /// <typeparam name="T">请求参数类型</typeparam>
        /// <typeparam name="V">返回结果类型</typeparam>
        /// <param name="url">请求Url</param>
        /// <param name="t">请求参数</param>
        /// <returns>返回结果</returns>
        public static V PostFormUrl<T, V>(string url, T t)
        {
            var rel = PostFormUrl(url, t);
            return rel.DeserializeObject<V>();
        }

        /// <summary>
        /// POST请求(FormUrlEncodedContent)
        /// </summary>
        /// <typeparam name="T">请求参数类型</typeparam>
        /// <param name="url">请求Url</param>
        /// <param name="t">请求参数</param>
        /// <returns>返回结果</returns>
        public static string PostFormUrl<T>(string url, T t)
        {
            var map = ToMap(t);
            Task<string> result = Post(url, map, null);
            return result.Result;
        }

        /// <summary>
        /// POST请求(ByteArrayContent)
        /// </summary>
        /// <typeparam name="T">请求参数类型</typeparam>
        /// <typeparam name="V">返回结果类型</typeparam>
        /// <param name="url">请求Url</param>
        /// <param name="t">请求参数</param>
        /// <returns>返回结果</returns>
        public static V PostByte<T, V>(string url, T t)
        {
            var rel = PostByte(url, t);
            return rel.DeserializeObject<V>();
        }

        /// <summary>
        /// POST请求(ByteArrayContent)
        /// </summary>
        /// <typeparam name="T">请求参数类型</typeparam>
        /// <param name="url">请求Url</param>
        /// <param name="t">请求参数</param>
        /// <returns>返回结果</returns>
        public static string PostByte<T>(string url, T t)
        {
            var map = t.SerializeObject();
            var data = Encoding.UTF8.GetBytes(map);
            Task<string> result = Post(url, null, data);
            return result.Result;
        }

        /// <summary>
        /// POST请求
        /// </summary>
        /// <param name="url">url</param>
        /// <param name="parameters">请求参数</param>
        /// <param name="datas">请求参数</param>
        /// <returns>返回结果</returns>
        public static async Task<string> Post(string url, Dictionary<string, string> parameters, byte[] datas)
        {
            Uri uri;
            Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out uri);
            using (var http = new HttpClient())
            {
                http.DefaultRequestHeaders.Add("Accept", "application/json");
                var content = parameters != null ? GetHttpContent(parameters) : GetHttpContent(datas);
                var response = await http.PostAsync(uri, content);
                var rel = await response.Content.ReadAsStringAsync();
                response.EnsureSuccessStatusCode();
                return rel;
            }
        }

        /// <summary>
        /// 获取参数
        /// </summary>
        /// <param name="parameters">parameters</param>
        /// <returns>HttpContent</returns>
        private static HttpContent GetHttpContent(Dictionary<string, string> parameters)
        {
            return new FormUrlEncodedContent(parameters);
        }

        /// <summary>
        /// 获取参数
        /// </summary>
        /// <param name="datas">datas</param>
        /// <returns>HttpContent</returns>
        private static HttpContent GetHttpContent(byte[] datas)
        {
            return new ByteArrayContent(datas);
        }

        /// <summary>
        /// Get请求
        /// </summary>
        /// <typeparam name="T">请求参数类型</typeparam>
        /// <param name="url">请求Url</param>
        /// <param name="t">请求参数</param>
        /// <returns>返回结果</returns>
        public static string GetString<T>(string url, T t)
        {
            var map = ToMap(t);
            Task<string> result = Get(url, map);
            return result.Result;
        }

        /// <summary>
        /// Get请求
        /// </summary>
        /// <typeparam name="T">请求参数类型</typeparam>
        /// <typeparam name="V">返回结果类型</typeparam>
        /// <param name="url">请求Url</param>
        /// <param name="t">请求参数</param>
        /// <returns>返回结果</returns>
        public static V GetModel<T, V>(string url, T t)
        {
            var map = ToMap(t);
            var rel = GetString(url, map);
            return rel.DeserializeObject<V>();
        }

        /// <summary>
        /// GET请求
        /// </summary>
        /// <param name="url">url</param>
        /// <param name="parameters">请求参数</param>
        /// <returns>结果</returns>
        public static async Task<string> Get(string url, Dictionary<string, string> parameters)
        {
            url = url + GteParameters(parameters);
            Uri uri;
            Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out uri);
            using (var http = new HttpClient())
            {
                var response = await http.GetAsync(uri);
                var rel = await response.Content.ReadAsStringAsync();
                response.EnsureSuccessStatusCode();
                return rel;
            }
        }

        /// <summary>
        /// GteParameters
        /// </summary>
        /// <param name="parameters">parameters</param>
        /// <returns>string</returns>
        private static string GteParameters(Dictionary<string, string> parameters)
        {
            if (parameters == null || !parameters.Any())
            {
                return string.Empty;
            }
            var param = parameters.Join("&", p => p.Key + "=" + p.Value);
            return "?" + param.Substring(1);
        }
    }

    /// <summary>
    /// 动态表达式特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class, AllowMultiple = true)]
    public class DynamicExpressionAttribute : Attribute
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 运行符号
        /// </summary>
        public string Operator { get; set; }

        /// <summary>
        /// 是否参与表达式计算
        /// </summary>
        public bool IsDynamicExpression { get; set; } = true;
    }

    /// <summary>
    /// 获取实体对象的自定义特性
    /// </summary>
    /// <typeparam name="TAttribute">自定义特性类型</typeparam>
    public class CustomAttributeExtension<TAttribute>
        where TAttribute : Attribute
    {
        /// <summary>
        /// Cache Data
        /// </summary>
        private static readonly Dictionary<string, TAttribute> Cache = new Dictionary<string, TAttribute>();

        /// <summary>
        /// 获取CustomAttribute Value
        /// </summary>
        /// <param name="type">实体对象类型</param>
        /// <param name="propertyInfo">实体对象属性信息</param>
        /// <returns>返回Attribute的值，没有则返回null</returns>
        public static TAttribute GetCustomAttributeValue(Type type, PropertyInfo propertyInfo)
        {
            var key = BuildKey(type, propertyInfo);
            if (!Cache.ContainsKey(key))
            {
                CacheAttributeValue(type, propertyInfo);
            }
            return Cache[key];
        }

        /// <summary>
        /// 获取CustomAttribute Value
        /// </summary>
        /// <param name="sourceType">实体对象数据类型</param>
        /// <returns>返回Attribute的值，没有则返回null</returns>
        public static TAttribute GetCustomAttributeValue(Type sourceType)
        {
            var key = BuildKey(sourceType, null);
            if (!Cache.ContainsKey(key))
            {
                CacheAttributeValue(sourceType, null);
            }
            return Cache[key];
        }

        /// <summary>
        /// 获取实体类上的特性
        /// </summary>
        /// <param name="type">实体对象类型</param>
        /// <returns>返回Attribute的值，没有则返回null</returns>
        private static TAttribute GetClassAttributes(Type type)
        {
            var attribute = type.GetCustomAttributes(typeof(TAttribute), false).FirstOrDefault();
            return (TAttribute)attribute;
        }

        /// <summary>
        /// 获取实体属性上的特性
        /// </summary>
        /// <param name="propertyInfo">属性信息</param>
        /// <returns>返回Attribute的值，没有则返回null</returns>
        private static TAttribute GetPropertyAttributes(PropertyInfo propertyInfo)
        {
            var attribute = propertyInfo?.GetCustomAttributes(typeof(TAttribute), false).FirstOrDefault();
            return (TAttribute)attribute;
        }

        /// <summary>
        /// 缓存Attribute Value
        /// <param name="type">实体对象类型</param>
        /// <param name="propertyInfo">实体对象属性信息</param>
        /// </summary>
        private static void CacheAttributeValue(Type type, PropertyInfo propertyInfo)
        {
            var key = BuildKey(type, propertyInfo);
            var value = propertyInfo == null ? GetClassAttributes(type) : GetPropertyAttributes(propertyInfo);
            lock (key + "_attributeValueLockKey")
            {
                if (!Cache.ContainsKey(key))
                {
                    Cache[key] = value;
                }
            }
        }

        /// <summary>
        /// 缓存Collection Name Key
        /// <param name="type">type</param>
        /// <param name="propertyInfo">propertyInfo</param>
        /// </summary>
        private static string BuildKey(Type type, PropertyInfo propertyInfo)
        {
            if (string.IsNullOrEmpty(propertyInfo?.Name))
            {
                return type.FullName;
            }
            return type.FullName + "." + propertyInfo.Name;
        }
    }
}