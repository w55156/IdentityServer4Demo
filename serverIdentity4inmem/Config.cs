// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityModel;
using IdentityServer4.Models;
using System.Collections.Generic;

namespace serverIdentity4inmem
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
            new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Address(),
                new IdentityResources.Phone(),
                new IdentityResources.Email(),
                new IdentityResource("roles","角色",new List<string>{JwtClaimTypes.Role}),
                new IdentityResource("locations","地点",new List<string>{"location"}),
            };

        public static IEnumerable<ApiScope> ApiScopes =>
            new ApiScope[]
            {
                new ApiScope("scope1","my scope1",new List<string>{"location"}),
                new ApiScope("scope2","my scope2"),
            };

        public static IEnumerable<Client> Clients =>
            new Client[]
            {
                // console client,client credentials flow client
                new Client{
                    ClientId="consoleClient",
                    ClientSecrets= {new Secret("secret".Sha256())},
                    AllowedGrantTypes=GrantTypes.ClientCredentials,
                    AllowedScopes={"scope1"}
                },

                // wfp client, password
                new Client
                {
                    ClientId="wpfClient",
                    ClientSecrets={new Secret("secret".Sha256())},
                    AllowedGrantTypes=GrantTypes.ResourceOwnerPassword,
                    AllowedScopes={
                        "scope1",
                        IdentityServer4.IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServer4.IdentityServerConstants.StandardScopes.Profile,
                        IdentityServer4.IdentityServerConstants.StandardScopes.Address,
                        IdentityServer4.IdentityServerConstants.StandardScopes.Phone,
                        IdentityServer4.IdentityServerConstants.StandardScopes.Email,
                    }
                },

                // mvc client,interactive client using code flow + pkce
                new Client
                {
                    ClientId="mvcClient",
                    ClientName="ASP NET CORE MVC Client",
                    ClientSecrets={new Secret("secret".Sha256())},
                    AllowedGrantTypes=GrantTypes.CodeAndClientCredentials,
                    AlwaysIncludeUserClaimsInIdToken=true,
                    RedirectUris={"http://localhost:5003/signin-oidc" },
                    FrontChannelLogoutUri="http://localhost:5003/signout-oidc",
                    PostLogoutRedirectUris={"http://localhost:5003/signout-callback-oidc" },
                    AllowOfflineAccess=true,
                    AccessTokenLifetime=30, //会话超时时间，单位是秒，默认为一个小时
                    AllowedScopes={
                        "scope1",
                        IdentityServer4.IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServer4.IdentityServerConstants.StandardScopes.Profile,
                        IdentityServer4.IdentityServerConstants.StandardScopes.Address,
                        IdentityServer4.IdentityServerConstants.StandardScopes.Phone,
                        IdentityServer4.IdentityServerConstants.StandardScopes.Email,
                    }
                },
                // javascript client
                new Client
                {
                    ClientId="javascriptClient",
                    ClientName="javascript Client",
                    ClientUri="http://localhost:5004",
                    RequireClientSecret=false,

                    AllowedGrantTypes=GrantTypes.Code,
                    //AllowAccessTokensViaBrowser=true,
                    //RequireConsent=true,
                    //AccessTokenLifetime=60*5, //会话超时时间，单位是秒，默认为一个小时

                    RedirectUris={
                        "http://localhost:5004/callback.html",
                    },
                    PostLogoutRedirectUris={"http://localhost:5004/index.html" },
                    AllowedCorsOrigins={"http://localhost:5004"},
                   
                    AllowedScopes={
                        "scope1",
                        IdentityServer4.IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServer4.IdentityServerConstants.StandardScopes.Profile,
                        IdentityServer4.IdentityServerConstants.StandardScopes.Address,
                        IdentityServer4.IdentityServerConstants.StandardScopes.Phone,
                        IdentityServer4.IdentityServerConstants.StandardScopes.Email,
                    }
                },
                // mvc,hybrid client
                new Client
                {
                    ClientId="hybridClient",
                    ClientName="asp.net core mvc hybrid Client",
                    ClientSecrets={new Secret("secret".Sha256())},
                    AllowedGrantTypes=GrantTypes.Hybrid,
                    //AccessTokenType=AccessTokenType.Reference,

                    // 如果客户端 response_type 包含 token 这里必须启用
                    AllowAccessTokensViaBrowser=true,

                    RedirectUris={
                        "http://localhost:5005/signin-oidc",
                    },
                    FrontChannelLogoutUri="http://localhost:5005/signout-oidc",
                    PostLogoutRedirectUris={"http://localhost:5005/signout-callback-oidc" },
                    AllowOfflineAccess=true,
                    AlwaysIncludeUserClaimsInIdToken=true,
                    RequireConsent=true,
                    // RequirePkce默认为true  会报错误： code challenge required
                    RequirePkce=false,
                    AllowedScopes={
                        "scope1",
                        IdentityServer4.IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServer4.IdentityServerConstants.StandardScopes.Profile,
                        IdentityServer4.IdentityServerConstants.StandardScopes.Address,
                        IdentityServer4.IdentityServerConstants.StandardScopes.Phone,
                        IdentityServer4.IdentityServerConstants.StandardScopes.Email,
                        "roles",
                        "locations",
                    }
                },
            };
    }
}