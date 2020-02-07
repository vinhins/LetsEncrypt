﻿using LetsEncrypt.Client.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LetsEncrypt.Client
{
    public partial class AcmeClient
    {
        // Public Methods

        public async Task<Account> CreateNewAccountAsync(string contactEmail)
        {
            return await NewAccountAsync(new List<string> { $"mailto:{contactEmail}" });
        }

        // Private Methods

        //private async Task<Account> GetAccountAsync(Uri accountLocation)
        //{
        //    var nonce = await GetNonceAsync();

        //    var signedData = _jws.Sign(null, accountLocation, accountLocation, nonce);
        //    var account = await PostAsync<Account>(accountLocation, signedData);
        //    account.Location = accountLocation;
        //    return account;
        //}

        private async Task<Account> DeactivateAccountAsync(Account account)
        {
            var nonce = await GetNonceAsync();

            var data = new Account { Status = AccountStatus.Deactivated };
            var signedData = account.Signer.Sign(data, account.Location, account.Location, nonce);
            return await PostAsync<Account>(account.Location, signedData);
        }

        private async Task<Account> UpdateAccountAsync(Account account)
        {
            var nonce = await GetNonceAsync();

            var signedData = account.Signer.Sign(account, account.Location, account.Location, nonce);
            return await PostAsync<Account>(account.Location, signedData);
        }

        private async Task<Account> NewAccountAsync(List<string> contactEmails)
        {
            var account = Account.Create(contactEmails);

            var directory = await GetDirectoryAsync();
            var nonce = await GetNonceAsync();

            var signedData = account.Signer.Sign(account, url: directory.NewAccount, nonce: nonce);
            return await PostAsync<Account>(directory.NewAccount, signedData);
        }
    }
}