using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Common
{
    class RegexMatch
    {
        /// <summary>
        ///    获取服务器配置信息
        /// </summary>
        /// <param name="connectionString"></param>
        public static ServerConfig GetServerConfig(string connectionString)
        {
            //connectionString = "host=127.0.0.1:5000;username=guest;password=guest;"
            if (string.IsNullOrEmpty(connectionString)) return null;
            ServerConfig server = new ServerConfig();
            var hostNamePattern = @"host=([^;]+)";
            var hostMatch = Regex.Match(connectionString, hostNamePattern);
            if (hostMatch.Success)
            {
                var hostConfig = hostMatch.Groups[1].Value.Split(':');
                server.HostName = hostConfig[0];
                server.Port = int.Parse(hostConfig[1]);
            }

            var usernamePattern = @"username=([^;]+)";
            var usernameMatch = Regex.Match(connectionString, usernamePattern);
            if (usernameMatch.Success)
            {
                server.UserName = usernameMatch.Groups[1].Value;
            }

            var passwordPattern = @"password=([^;]+)";
            var passwordMatch = Regex.Match(connectionString, passwordPattern);
            if (passwordMatch.Success)
            {
                server.Password = passwordMatch.Groups[1].Value;
            }

            return server;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="placeHolderDic"></param>
        /// <returns></returns>
        public static string GetFormatString(string source, Dictionary<string, string> placeHolderDic)
        {
            var pattern = "{{([a-zA-Z]+)}}";

            var result = Regex.Replace(source, pattern, match =>
            {
                var paramName = match.Groups[1].ToString();
                if (placeHolderDic.Keys.Any(e => e == paramName))
                    return placeHolderDic[paramName];

                throw new ArgumentException("占位符匹配字典中不存在此占位符");
            });

            return result;
        }

    }

    public class ServerConfig
    {

        /// <summary>
        /// Server地址
        /// </summary>
        public string HostName { get; set; }

        /// <summary>
        /// Server端口
        /// </summary>
        public int Port { get; set; } = 5672;

        /// <summary>
        /// Server用户名
        /// </summary>
        public string UserName { get; set; } = "guest";

        /// <summary>
        /// Server密码
        /// </summary>
        public string Password { get; set; } = "guest";
    }
}
