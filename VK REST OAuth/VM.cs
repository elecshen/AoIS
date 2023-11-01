using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using System.Windows;

namespace VK_REST_OAuth
{
    class VM
    {
        public CefSharp.Wpf.ChromiumWebBrowser? WebBrowser { get; set; }
        static readonly HttpClient httpClient = new();

        public VM()
        {
            var config = new ConfigurationBuilder().AddUserSecrets<App>().Build();
            foreach (var child in config.GetChildren())
            {
                Environment.SetEnvironmentVariable(child.Key, child.Value);
            }
        }

        private Command? authCommand;
        public Command AuthCommand
        {
            get => authCommand ??= new Command(obj =>
            {
                if (WebBrowser is null)
                    return;
                WebBrowser.AddressChanged += WebBrowser_AddressChanged;
                
                string appId = Environment.GetEnvironmentVariable("AppId")!;
                var uriStr = @"https://oauth.vk.com/authorize?client_id=" + appId + @"&scope=friends&redirect_uri=https://oauth.vk.com/blank.html&display=page&v=5.6&response_type=code";
                WebBrowser.Load(uriStr);
            });
        }

        private void WebBrowser_AddressChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var uri = new Uri((string)e.NewValue);
            if (uri.AbsoluteUri.Contains(@"oauth.vk.com/blank.html#code")) {
                string? code = HttpUtility.ParseQueryString(uri.Fragment.Trim('#')).Get("code");
                if (code is null)
                {
                    MessageBox.Show(uri.AbsoluteUri, "Bad uri");
                }
                string appId = Environment.GetEnvironmentVariable("AppId")!;
                string clientSecret = Environment.GetEnvironmentVariable("ClientSecret")!;
                var uriStr = @"https://oauth.vk.com/access_token?client_id=" + appId + "&client_secret=" + clientSecret + "&redirect_uri=https://oauth.vk.com/blank.html&code=" + code;
                WorkWithAPI(uriStr);
            }
        }

        private static async void WorkWithAPI(string accessResponseUri)
        {
            var response = await GET(accessResponseUri);
            if (response.TryGetProperty("access_token", out _))
            {
                string access_token = response.GetProperty("access_token").ToString();
                string user_id = response.GetProperty("user_id").ToString();

                string methodUriT = @"https://api.vk.com/method/{0}?{1}&access_token="+access_token+"&v=5.154";

                response = await GET(string.Format(methodUriT, "friends.getOnline", "user_id="+user_id+"&online_mobile=1"));
                MessageBox.Show(response.GetProperty("response").ToString(), "friends.getOnline");

                response = await GET(string.Format(methodUriT, "database.getMetroStations", "city_id=1&count=10"));
                MessageBox.Show(response.GetProperty("response").ToString(), "database.getMetroStations");
            }
        }

        private static async Task<JsonElement> GET(string url)
        {
            var json = await httpClient.GetStringAsync(url);
            return JsonDocument.Parse(json).RootElement;
        }

        public static class VKAppInfo
        {
            public static string AppId { get; set; }
            public static string ClientSecret { get; set; }
        }
    }
}
