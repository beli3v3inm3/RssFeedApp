using System;
using System.Data.Entity;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Facebook;
using Microsoft.Owin.Security.Google;
using Microsoft.Owin.Security.OAuth;
using Owin;
using RssFeedApp.Models;
using RssFeedApp.Provider;

[assembly: OwinStartup(typeof(RssFeedApp.Startup))]
namespace RssFeedApp
{
    public class Startup
    {
        public static OAuthBearerAuthenticationOptions OAuthBearerOptions { get; private set; }
        public static GoogleOAuth2AuthenticationOptions GoogleOAuth2Authentication { get; private set; }
        public static FacebookAuthenticationOptions FacebookAuthenticationOptions { get; private set; }


        public void Configuration(IAppBuilder app)
        {
            var config = new HttpConfiguration();

            ConfigureOAuth(app);

            WebApiConfig.Register(config);
            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
            app.UseWebApi(config);
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<AuthContext, Migrations.Configuration>());
        }

        public void ConfigureOAuth(IAppBuilder app)
        {
            //use a cookie to temporarily store information about a user logging in with a third party login provider
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);
            OAuthBearerOptions = new OAuthBearerAuthenticationOptions();

            var oAuthServerOptions = new OAuthAuthorizationServerOptions()
            {
                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString("/token"),
                AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(30),
                Provider = new SimpleAuthorizationServerProvider(),
                RefreshTokenProvider = new SimpleRefreshTokenProvider()
            };

            // Token Generation
            app.UseOAuthAuthorizationServer(oAuthServerOptions);
            app.UseOAuthBearerAuthentication(OAuthBearerOptions);

            //Configure Google External Login
            GoogleOAuth2Authentication = new GoogleOAuth2AuthenticationOptions()
            {
                ClientId = "104335682354-0209fuh5sruk6nof9t1l62av59le18he.apps.googleusercontent.com",
                ClientSecret = "sN7ObbrmprU_OAK0jb5BKarv",
                Provider = new GoogleAuthProvider()
            };
            app.UseGoogleAuthentication(GoogleOAuth2Authentication);

            //Configure Facebook External Login
            FacebookAuthenticationOptions = new FacebookAuthenticationOptions()
            {
                AppId = "463597530517445",
                AppSecret = "a4d8a3f979e170851e8e3013baf97b20",
                Provider = new FacebookProvider()
            };
            app.UseFacebookAuthentication(FacebookAuthenticationOptions);
        }
    }
}