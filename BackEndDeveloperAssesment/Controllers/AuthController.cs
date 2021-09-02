using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authorization;


namespace BackEndDeveloperAssesment.Controllers
{
    /// <summary>
    /// core banking api Authentication
    /// </summary>
    [Route("BankingApi")]
    public class AuthController : Controller
    {
        private const string client_id = "test_user";
        private const string client_secret = "12345";

        /// <summary>
        /// client using client id and client secret to authenticate and recieve access_token
        /// Add { Bearer <access_token> } in header request of each subsequent request
        /// </summary>
        /// <param name="clientID"></param>
        /// <param name="clientSecret"></param>
        /// <returns></returns>
        [HttpGet("RequestToken")]
        public async Task<IActionResult> RequestToken(string clientID = client_id, string clientSecret = client_secret)
        {
            try
            {
                var disco = await DiscoveryClient.GetAsync(Globals.oauth_server);
                var tokenClient = new TokenClient(
                    address: disco.TokenEndpoint,
                    clientId: clientID,
                    clientSecret: clientSecret);
                var tokenResponse = await tokenClient.RequestClientCredentialsAsync("api");
                if (tokenResponse.IsError)
                {
                    return Json(tokenResponse.Exception);
                }
                else
                {
                    return Json(tokenResponse.Json);
                }
            }
            catch (Exception ex)
            {
                return Ok(ex);
            }
        }

    }
}
