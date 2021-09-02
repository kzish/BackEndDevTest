using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;
using Auth.Models;
using Microsoft.EntityFrameworkCore;

namespace Auth.Models
{
    public class Config
    {

        /// <summary>
        /// fetch all the reources from the db
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<ApiResource> GetApiResources()
        {
            var apiResources = new List<ApiResource>();
            var res = new ApiResource();
            res.Name = "api";
            res.DisplayName = "api";
            res.Enabled = true;
            res.Scopes = new Scope[] {
                new Scope("api"),
            };
            apiResources.Add(res);
            return apiResources;
        }

        /// <summary>
        /// fetch all the clients from the db
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Client> GetApiClients()
        {
            var apiClients = new List<Client>();
            var nc = new Client
            {
                ClientId = "test_user",
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                ClientSecrets ={
                                new Secret("12345".Sha256())
                        },
                AllowedScopes = { "api" },
            };

            apiClients.Add(nc);
            return apiClients;
        }



    }
}
