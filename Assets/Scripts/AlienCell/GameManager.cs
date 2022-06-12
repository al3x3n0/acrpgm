using Grpc.Core;
using MagicOnion;
using UnityEngine;

using AlienCell.Auth;
using AlienCell.Shared.Services;


namespace AlienCell
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;
        
        [Header("Server Settings")]
        public string serverUrl = "http://127.0.0.1:5001";
        
        private GrpcChannelx _channel;
        private AuthManager _authMgr = new AuthManager();

        public GrpcChannelx Channel { get => _channel; }

        void Awake()
        {
            _channel = GrpcChannelx.ForAddress(serverUrl);
            
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
            }

            _authMgr.Start();
        }
    }
}
