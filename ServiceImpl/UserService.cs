using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using postman_assignment.Exceptions;
using PostmanAssignment.Entities;
using PostmanAssignment.Exceptions;
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

        private void ValidateAuthParams(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new InvalidArgumentException(nameof(email), email, "valid email");
            }
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new InvalidArgumentException(nameof(password), password, "valid password");
            }
        }
        public async Task<User> AuthenticateUserAsync(string email, string password)
        {
            ValidateAuthParams(email, password);
            var user = await _repository.FindUserAsync(email);
            if (user == null)
            {
                throw new EntityNotFoundException(nameof(user.Email), user.Email);
            }
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
                    throw new UnauthorizedAccessException("Password is incorrect");
            user.Password = null;
            return user;
        }

        private void ValidateUser(User user)
        {
            if (user == null)
            {
                throw new InvalidArgumentException(nameof(user), null, "non null user");
            }
            if (string.IsNullOrWhiteSpace(user.Email))
            {
                throw new InvalidArgumentException(nameof(user.Email), user.Email, "valid email");
            }
            if (string.IsNullOrWhiteSpace(user.Password))
            {
                throw new InvalidArgumentException(nameof(user.Password), user.Password, "valid password");
            }
            if (string.IsNullOrWhiteSpace(user.FirstName))
            {
                throw new InvalidArgumentException(nameof(user.FirstName), null, "non null first name");
            }
        }

        public async Task<int> RegisterUserAsync(User user)
        {
            ValidateUser(user);
            var storedUser = await _repository.FindUserAsync(user.Email);
            if (storedUser != null)
            {
                throw new ConflictingEntityException(nameof(user.Email), user.Email);
            }
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