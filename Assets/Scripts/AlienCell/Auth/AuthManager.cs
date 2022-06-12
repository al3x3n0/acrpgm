using System;
using System.IO;
using System.Threading.Tasks;
using MagicOnion;
using MagicOnion.Client;
using Newtonsoft.Json;
using UnityEngine;

using AlienCell.Shared.Services;
using AlienCell.Shared.Protocol;


namespace AlienCell.Auth
{
    public class AuthManager
    {
        private const string ACCOUNT_DATA_FILENAME = "AccountData.json";
        
        private string GetAccountDataPath()
        {
            return Path.Combine(Application.persistentDataPath, ACCOUNT_DATA_FILENAME);
        }    

        private AccountPersistentData _accountData;
        private Ulid _userId;
        public Ulid UserId { get => _userId; }

        public void SaveAccountData()
        {
            var accountDataJson = JsonConvert.SerializeObject(_accountData, Formatting.Indented);
            File.WriteAllText(GetAccountDataPath(), accountDataJson);
        }

        public AccountPersistentData LoadAccountData()
        {
            var accountDataJson = File.ReadAllText(GetAccountDataPath());
            _accountData = JsonConvert.DeserializeObject<AccountPersistentData>(accountDataJson);
            return _accountData;
        }
        public async void Start()
        {
            var accountDataPath = GetAccountDataPath();
            Debug.Log($"Loading account data from: {accountDataPath}");
            if (!File.Exists(accountDataPath))
            {
                Debug.Log("Starting auth...");
                await this.CreateNewAccountAsync();
            }
            else
            {
                LoadAccountData();
            }
        }

        public async Task<AccountPersistentData> CreateNewAccountAsync()
        {
            Debug.Log("CreateNewAccountAsync()");
            var client = MagicOnionClient.Create<IAccountService>(GameManager.Instance.NewChannel());
            var regAccReq = new RegisterAccountRequest
            {
                DeviceUId = SystemInfo.deviceUniqueIdentifier
            };
            Debug.Log($"Sending request... {regAccReq.DeviceUId}");
            var regAccResp = await client.RegisterAccount(regAccReq);
            Debug.Log("After req...");
            if (!regAccResp.Success)
            {
                Debug.Log("SignUp Failure");
                Application.Quit();
            }
            Debug.Log($"SignedUp as {regAccResp.Account.Id.ToString()}; UserId={regAccResp.UserId.ToString()}");

            _accountData = new AccountPersistentData
            {
                AccountId = regAccResp.Account.Id
            };

            SaveAccountData();

            return _accountData;
        }

        public async Task<ValidateChallengeResponse> SignInAsync()
        {
            var client = MagicOnionClient.Create<IAuthService>(GameManager.Instance.NewChannel());
            var getChallengeReq = new GetChallengeRequest
            {
                UserId = _userId
            };
            var challenge = await client.GetChallengeAsync(getChallengeReq); // Ulid
            Debug.Log(challenge.Challenge);
            var validateChallengeReq = new ValidateChallengeRequest
            {

            };
            var signInResult = await client.ValidateAsync(validateChallengeReq);
            return signInResult;
        }
    }
}
