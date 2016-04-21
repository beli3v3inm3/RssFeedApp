using System;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Infrastructure;
using Microsoft.Owin.Security.OAuth;
using Owin;
using RssFeedApp.Models;
using RssFeedApp.Provider;
using RssFeedApp.Repository;
using StructureMap;
using StructureMap.Attributes;
using StructureMap.Pipeline;

[assembly: OwinStartup(typeof(RssFeedApp.Startup))]
namespace RssFeedApp
{
    public class Startup
    {
        public static OAuthBearerAuthenticationOptions OAuthBearerOptions { get; private set; }

        public void Configuration(IAppBuilder app)
        {
            var config = new HttpConfiguration();

            ConfigureOAuth(app);

            WebApiConfig.Register(config);
            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
            app.UseWebApi(config);
            

            var container = new Container();
            var uniqueRequest = new UniquePerRequestLifecycle();

            container.Configure(_ =>
            {
                _.Policies.FillAllPropertiesOfType<IUserRepository>().Use<UserRepository>();

                _.For<IRequestRepository>().Use<RequestRepository>()
                .SetLifecycleTo(uniqueRequest);

                _.For<IRssRepository>().Use<RssRepository>();
                _.For<IUserRepository>().Use<UserRepository>();
            });

            config.DependencyResolver = new StrctureMapConfig(container);
        }

        public void ConfigureOAuth(IAppBuilder app)
        {

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
            app.UseOAuthAuthorizationServer(oAuthServerOptions);
            app.UseOAuthBearerAuthentication(OAuthBearerOptions);
        }
    }
}