using Microsoft.AspNetCore.Identity;
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

            this.Username = username;
            this.Balance = 5000;

            var hasher = new PasswordHasher<Account>();
            PasswordHash = hasher.HashPassword(this, password);
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

        public override string ToString() => $"{Username}: {Balance} tokens";

    }
}