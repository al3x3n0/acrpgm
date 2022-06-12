using System;
using MagicOnion.Client;


namespace AlienCell.Auth   
{
    class AuthTokenStorage
    {
        public static AuthTokenStorage Current { get; } = new AuthTokenStorage();

        private readonly object _syncObject = new object();

        public string Token { get; private set; }
        public DateTimeOffset Expiration { get; private set; }

        public bool IsExpired => Token == null || Expiration < DateTimeOffset.Now;

        public void Update(string token, DateTimeOffset expiration)
        {
            lock (_syncObject)
            {
                Token = token ?? throw new ArgumentNullException(nameof(token));
                Expiration = expiration;
            }
        }
    }
}