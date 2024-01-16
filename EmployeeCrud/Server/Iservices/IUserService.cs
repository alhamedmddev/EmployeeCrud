using EmployeeCrud.Shared.ViewModel;

namespace EmployeeCrud.Server.Iservices
{
    public interface IUserService
    {
        Task<(bool IsUserRegistered, string Message)> RegisterNewUserAsync(UserVM userVM);
        Task<(bool IsLoginSuccess, JWTTokenResponseVM TokeResponse)> LoginAsync(LoginVM loginPayload);
    }
}
