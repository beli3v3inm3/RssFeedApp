using System;
using System.Data.Entity.Core.Metadata.Edm;
using System.Reflection;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Owin;
using RssFeedApp.Models;
using RssFeedApp.Provider;
using RssFeedApp.Repository;
using StructureMap;
using StructureMap.Graph;
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
            container.Configure(_ =>
            {
                _.Scan(scan =>
                {
                    scan.TheCallingAssembly();
                    scan.WithDefaultConventions();
                });
            });
            container.Configure(_ =>
            {
                _.For<IRequestRepository>().Use<RequestRepository>()
                .Singleton()
                .SetLifecycleTo(new UniquePerRequestLifecycle());
                _.For<IRssRepository>().Use<RssRepository>();
                _.For<IUserRepository>().Use<UserRepository>();
            });

            config.DependencyResolver = new StrctureMapConfig(container);

        }

        public void ConfigureOAuth(IAppBuilder app)
        {
            //use a cookie to temporarily store information about a user logging in with a third party login provider
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);
            OAuthBearerOptions = new OAuthBearerAuthenticationOptions();
            var repo = new RequestRepository();
            var userRepo = new UserRepository(repo);

            var oAuthServerOptions = new OAuthAuthorizationServerOptions()
            {
                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString("/token"),
                AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(30),
                Provider = new SimpleAuthorizationServerProvider(userRepo),
                RefreshTokenProvider = new SimpleRefreshTokenProvider(userRepo)
            };
            app.UseOAuthAuthorizationServer(oAuthServerOptions);
            app.UseOAuthBearerAuthentication(OAuthBearerOptions);
        }
    }
}