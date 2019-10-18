using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Diagnostics;
using Newtonsoft.Json;

namespace VkClient
{
    class Program
    {
        private const string AppId = "ololo";
        private const string MyToken = "ololo";
        private const string Token = "ololo";
        private const string MyId = "ololo";

        private static Dictionary<int, string> Attitudes = new Dictionary<int, string>
        {
            { 0, "не указано" },
            { 1, "резко негативное" },
            { 2, "негативное" },
            { 3, "компромиссное" },
            { 4, "нейтральное" },
            { 5, "положительное (:" },
        };

        public static async Task GetFriendsOnline(HttpClient httpClient)
        {
            var request = $"https://api.vk.com/method/friends.getOnline?user_id={MyId}&v=5.89&access_token={MyToken}";
            var response = await httpClient.GetAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var idListResponse = JsonConvert.DeserializeObject<IdListResponse>(content);

                foreach (var user in idListResponse.response)
                {
                    await GetUserInfoById(user, httpClient);
                }
            }
        }

        public static async Task GetUserInfoById(int id, HttpClient httpClient)
        {
            var request = $"https://api.vk.com/method/users.get?user_id={id}&v=5.89&access_token={Token}&fields=personal";
            var response = await httpClient.GetAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var userResponse = JsonConvert.DeserializeObject<UserResponse>(content);
                var alcoholAttitude = userResponse.response[0].personal?.alcohol ?? 0;
                Console.WriteLine($"{userResponse.response[0].first_name} {userResponse.response[0].last_name}, отношение к алкоголю: {Attitudes[alcoholAttitude]}");
            }
        }

        public static async Task Main(string[] args)
        {
            var httpClient = new HttpClient();
            await GetFriendsOnline(httpClient);
            await GetUserInfoById(1, httpClient);

            //var authString = $"https://oauth.vk.com/authorize?client_id={AppId}&display=page&redirect_uri=https://oauth.vk.com/blank.html&scope=friends&response_type=token&v=5.89";
            //authString = authString.Replace("&", "^&");
            //Process.Start(new ProcessStartInfo("cmd", $"/c start {authString}") { CreateNoWindow = true });
        }
    }
}