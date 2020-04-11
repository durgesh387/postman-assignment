using System.Threading.Tasks;
using PostmanAssignment.Entities;

namespace PostmanAssignment.Services
{
    public interface IUserService
    {
        Task<User> AuthenticateUserAsync(string email, string password);
        Task<int> RegisterUserAsync(User user);
    }
}