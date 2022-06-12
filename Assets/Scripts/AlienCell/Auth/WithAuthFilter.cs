using System;
using System.Threading.Tasks;
using Grpc.Core;
using MagicOnion;
using MagicOnion.Client;
using UnityEngine;

using AlienCell.Shared.Services;


namespace AlienCell.Auth
{
    class WithAuthFilter : IClientFilter
    {
        private readonly AuthManager _authManager;
        private readonly GrpcChannelx _channel;

        public WithAuthFilter(AuthManager authManager, GrpcChannelx channel)
        {
            _authManager = authManager ?? throw new ArgumentNullException(nameof(authManager));
            _channel = channel ?? throw new ArgumentNullException(nameof(channel));
        }

        public async ValueTask<ResponseContext> SendAsync(RequestContext context, Func<RequestContext, ValueTask<ResponseContext>> next)
        {
            if (AuthTokenStorage.Current.IsExpired)
            {
                Debug.Log($@"[WithAuthFilter/IAuthService.SignInAsync] Try signing in as '{_authManager.UserId.ToString()}'... ({(AuthTokenStorage.Current.Token == null ? "FirstTime" : "RefreshToken")})");

                var client = MagicOnionClient.Create<IAuthService>(_channel);
                var authResult = await _authManager.SignInAsync();
                if (!authResult.Success)
                {
                    throw new Exception("Failed to sign-in on the server.");
                }
                Debug.Log($@"[WithAuthFilter/IAuthService.SignInAsync] User authenticated as (UserId:{_authManager.UserId.ToString()})");

                AuthTokenStorage.Current.Update(authResult.Token, authResult.Expiration); // NOTE: You can also read the token expiration date from JWT.

                context.CallOptions.Headers.Remove(new Metadata.Entry("Authorization", string.Empty));
            }

            if (!context.CallOptions.Headers.Contains(new Metadata.Entry("Authorization", string.Empty)))
            {
                context.CallOptions.Headers.Add("Authorization", "Bearer " + AuthTokenStorage.Current.Token);
            }

            return await next(context);
        }
    }
}
