using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using System.Reflection.Metadata.Ecma335;
using System.Text.Json.Serialization;

namespace Backend
{
    public class Account
    {
        public int Id { get; private set; }
        public string Username { get; set; }
        public int Balance { get; private set; }
        [JsonIgnore] public string PasswordHash { get; private set; }


        private Account() { } // Empty constructor for EF Core

        public Account(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username)) throw new ArgumentException("Username is required.", nameof(username));
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Password is required.", nameof(username));

            this.Username = username;
            this.Balance = 5000;

            SetPassword(password);
        }

        public bool TrySpend(int amount)
        {
            if (amount <= 0)
                throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be positive.");
            if (Balance < amount) 
                return false;

            Balance -= amount;
            return true;
        }
        
        public void AddFunds(int amount)
        {
            if (amount <= 0) 
                throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be positive.");
            
            Balance += amount;
        }
        public bool VerifyPassword(string password)
        {
            var hasher = new PasswordHasher<Account>();
            var result = hasher.VerifyHashedPassword(this, PasswordHash, password);
            return result == PasswordVerificationResult.Success;
        }

        public int ChangePassword(string oldPassword, string newPassword)
        {
            if (string.IsNullOrWhiteSpace(newPassword)) return 2; // Invalid new password ret code

            if (VerifyPassword(oldPassword))
            {
                SetPassword(newPassword);
                return 0; // success ret code
            }
            return 1; // invalid old password ret code
        }

        private void SetPassword(string password)
        {
            var hasher = new PasswordHasher<Account>();
            PasswordHash = hasher.HashPassword(this, password);
        }

        public override string ToString() => $"{Username}: {Balance} tokens";

    }
}