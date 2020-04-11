using System;
using System.Threading.Tasks;
using PostmanAssignment.Entities;

namespace PostmanAssignment.Repositories
{
    public interface IUserRepository
    {
        Task<User> FindUserAsync(string email);
        Task<int> CreateUserAsync(User user);
        Task<User> GetUserAsync(int userId);
    }
}