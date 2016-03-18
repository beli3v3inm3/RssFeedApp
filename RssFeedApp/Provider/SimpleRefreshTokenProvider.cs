using System;
using System.Threading.Tasks;
using Microsoft.Owin.Security.Infrastructure;
using RssFeedApp.Entities;

namespace RssFeedApp.Provider
{
    public class SimpleRefreshTokenProvider : IAuthenticationTokenProvider
    {
        private readonly UserProvider _repository = UserProvider.GetInstance;

        public async Task CreateAsync(AuthenticationTokenCreateContext context)
        {
            var clientid = context.Ticket.Properties.Dictionary["as:client_id"];

            if (string.IsNullOrEmpty(clientid))
            {
                return;
            }

            var refreshTokenId = Guid.NewGuid().ToString("n");

            var refreshTokenLifeTime = context.OwinContext.Get<string>("as:clientRefreshTokenLifeTime");

            var token = new RefreshToken()
            {
                Id = Helper.GetHash(refreshTokenId),
                ClientId = clientid,
                Subject = context.Ticket.Identity.Name,
                IssuedUtc = DateTime.UtcNow,
                ExpiresUtc = DateTime.UtcNow.AddMinutes(Convert.ToDouble(refreshTokenLifeTime))
            };

            context.Ticket.Properties.IssuedUtc = token.IssuedUtc;
            context.Ticket.Properties.ExpiresUtc = token.ExpiresUtc;

            token.ProtectedTicket = context.SerializeTicket();

            var result = await _repository.AddRefreshToken(token);

            if (result != null)
            {
                context.SetToken(refreshTokenId);
            }
        }

        public async Task ReceiveAsync(AuthenticationTokenReceiveContext context)
        {

            var allowedOrigin = context.OwinContext.Get<string>("as:clientAllowedOrigin");
            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { allowedOrigin });

            var hashedTokenId = Helper.GetHash(context.Token);

            var refreshToken = _repository.FindRefreshToken(hashedTokenId);

            if (refreshToken != null)
            {
                //Get protectedTicket from refreshToken class
                context.DeserializeTicket(refreshToken.ProtectedTicket);
                await _repository.RemoveRefreshToken(hashedTokenId);
            }
        }

        public void Create(AuthenticationTokenCreateContext context)
        {
            throw new NotImplementedException();
        }


        public void Receive(AuthenticationTokenReceiveContext context)
        {
            throw new NotImplementedException();
        }

    }
}