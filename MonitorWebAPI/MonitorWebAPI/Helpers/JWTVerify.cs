using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace MonitorWebAPI.Helpers
{
    public class JWTVerify
    {
        public static async Task<HttpResponseMessage> VerifyJWT(string JWT)
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", JWT);
            return await client.GetAsync("https://si-2021.167.99.244.168.nip.io:3333/login");
        }

        public static string GetToken(string Authorization)
        {
            var temp = Authorization.Split(' ');
            if(temp.Length==2)
            {
                return temp[1];
            }
            else
            {
                return null;
            }
        }
    }
}
