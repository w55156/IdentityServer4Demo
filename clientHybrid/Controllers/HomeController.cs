using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using clientHybrid.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using System.Net.Http;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Globalization;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using IdentityModel.Client;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

namespace clientHybrid.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        
        public IActionResult Index()
        {
            var result = User.IsInRole("管理员");
            return View();
        }

        [Authorize(Roles ="管理员")]
        public async Task<IActionResult> Privacy()
        {
            var accessToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);
            var idToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.IdToken);
            var refreshToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.RefreshToken);
            //var code = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.Code);
            ViewData["AccessToken"] = accessToken;
            ViewData["IdToken"] = idToken;
            ViewData["RefreshToken"] = refreshToken;
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Authorize(Roles = "普通用户")]
        public async Task<IActionResult> CallApi()
        {
            var client = new HttpClient();
            var accessToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);
            client.SetBearerToken(accessToken);
            var response = await client.GetAsync("http://localhost:5002/identity");
            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    await RenewTokenAsync();
                    return RedirectToAction();
                }
                throw new Exception(response.ReasonPhrase);
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();
                ViewBag.Json = JArray.Parse(content).ToString();
                return View();
            }
        }
        public async Task<string> RenewTokenAsync()
        {
            var refreshToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.RefreshToken);
            var httpClient = new HttpClient();
            var disco = await httpClient.GetDiscoveryDocumentAsync("http://localhost:5001");
            if (disco.IsError)
            {
                throw new Exception(disco.Error);
            }
            var tokenResponse = await httpClient.RequestRefreshTokenAsync(new RefreshTokenRequest
            {
                Address = disco.TokenEndpoint,
                ClientId = "mvcClient",
                ClientSecret = "secret",
                Scope = "scope1 openid profile email phone address",
                GrantType = OpenIdConnectGrantTypes.RefreshToken,
                RefreshToken = refreshToken,
            });
            if (tokenResponse.IsError)
            {
                throw new Exception(tokenResponse.Error);
            }
            else
            {
                var expiresAt = DateTime.UtcNow + TimeSpan.FromSeconds(tokenResponse.ExpiresIn);
                var tokens = new[] {
                    new AuthenticationToken{
                        Name=OpenIdConnectParameterNames.IdToken,
                        Value=tokenResponse.IdentityToken,
                    },
                    new AuthenticationToken{
                        Name=OpenIdConnectParameterNames.AccessToken,
                        Value=tokenResponse.AccessToken,
                    },
                    new AuthenticationToken{
                        Name=OpenIdConnectParameterNames.RefreshToken,
                        Value=tokenResponse.RefreshToken,
                    },
                    new AuthenticationToken{
                        Name="expires_at",
                        Value=expiresAt.ToString("o",CultureInfo.InvariantCulture),
                    },
                };
                // 获取身份验证的结果，包含当前的Principal和Properties
                var currentAuthenticateResult = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                // 把新的Tokens存起来
                currentAuthenticateResult.Properties.StoreTokens(tokens);
                // 登录
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    currentAuthenticateResult.Principal,
                    currentAuthenticateResult.Properties
                    );
                // 返回AccessToken
                return tokenResponse.AccessToken;
            }
        }
        public IActionResult Logout()
        {
            return SignOut(CookieAuthenticationDefaults.AuthenticationScheme, OpenIdConnectDefaults.AuthenticationScheme);
            //await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            //await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);
        }
        [Authorize(Policy = "SmithInSomeWhere")]
        public IActionResult PolicyBasedAuthorize()
        {// 基于Policy授权测试
            return View();
        }
    }
}
