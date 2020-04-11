using System;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;
using PostmanAssignment.Entities;
using PostmanAssignment.Repositories;
using PostmanAssignment.Utilities;

namespace PostmanAssignment.RepositoryImpl
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationSettings _appSettings;
        public UserRepository(IOptions<ApplicationSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }
        public async Task<int> CreateUserAsync(User user)
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            using (MySqlConnection connection = new MySqlConnection(_appSettings.ConnectionString))
            {
                await connection.OpenAsync();
                using (MySqlCommand command = new MySqlCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Connection = connection;
                    command.CommandText = Routines.CreateUser;
                    AddUserParams(command, user);
                    var userId = await command.ExecuteScalarAsync();
                    return Convert.ToInt32(userId);
                }
            }
        }

        private void AddUserParams(MySqlCommand command, User user)
        {
            command.Parameters.AddWithValue("vFirstName", user.FirstName);
            command.Parameters.AddWithValue("vLastName", user.LastName);
            command.Parameters.AddWithValue("vEmail", user.Email);
            command.Parameters.AddWithValue("vPassword", user.Password);
        }

        public async Task<User> FindUserAsync(string email)
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            using (MySqlConnection connection = new MySqlConnection(_appSettings.ConnectionString))
            {
                await connection.OpenAsync();
                using (MySqlCommand command = new MySqlCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Connection = connection;
                    command.CommandText = Routines.FindUser;
                    command.Parameters.AddWithValue("vEmail", email);
                    using (DbDataReader dr = await command.ExecuteReaderAsync())
                    {
                        return await ReadUserAsync(dr);
                    }
                }
            }
        }

        private async Task<User> ReadUserAsync(DbDataReader dr)
        {
            User user = null;
            if (await dr.ReadAsync())
            {
                user = new User();
                user.Id = Convert.ToInt32(dr["id"]);
                user.FirstName = Convert.ToString(dr["first_name"]);
                user.LastName = Convert.ToString(dr["last_name"]);
                user.Email = Convert.ToString(dr["email"]);
                user.Password = Convert.ToString(dr["password"]);
            }
            return user;
        }

        public Task<User> GetUserAsync(int userId)
        {
            throw new System.NotImplementedException();
        }
    }
}