using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PostmanAssignment.Entities;
using PostmanAssignment.Repositories;
using PostmanAssignment.Services;

namespace PostmanAssignment.ServiceImpl
{
    public class UserService : IUserService
    {
        private readonly ILogger<UserService> _logger;
        private readonly IUserRepository _repository;


        public UserService(ILogger<UserService> logger, IUserRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }
        public async Task<User> AuthenticateUserAsync(string email, string password)
        {
            var user = await _repository.FindUserAsync(email);
            /* Fetch the stored value */
            string savedPasswordHash = user.Password;
            /* Extract the bytes */
            byte[] hashBytes = Convert.FromBase64String(savedPasswordHash);
            /* Get the salt */
            byte[] salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, 16);
            /* Compute the hash on the password the user entered */
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
            byte[] hash = pbkdf2.GetBytes(20);
            /* Compare the results */
            for (int i = 0; i < 20; i++)
                if (hashBytes[i + 16] != hash[i])
                    return null;
            //throw new UnauthorizedAccessException();

            // return null if user not found
            // if (user == null)
            //     return null;
            user.Password = null;
            return user;
        }

        public async Task<int> RegisterUserAsync(User user)
        {
            var storedUser = await _repository.FindUserAsync(user.Email);
            // if (storedUser != null)
            // {
            //     return null;
            // }
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);

            //Create the Rfc2898DeriveBytes and get the hash value:
            var pbkdf2 = new Rfc2898DeriveBytes(user.Password, salt, 10000);
            byte[] hash = pbkdf2.GetBytes(20);

            //Combine the salt and password bytes for later use:
            byte[] hashBytes = new byte[36];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);

            //Turn the combined salt+hash into a string for storage
            string passwordHashToSave = Convert.ToBase64String(hashBytes);
            user.Password = passwordHashToSave;
            var createdUserId = await _repository.CreateUserAsync(user);
            return createdUserId;
        }
    }
}