using EmployeeCrud.Client.Pages;
using EmployeeCrud.Server.Data;
using EmployeeCrud.Server.Iservices;
using EmployeeCrud.Server.Settings;
using EmployeeCrud.Shared.Model;
using EmployeeCrud.Shared.ViewModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace EmployeeCrud.Server.Services
{
    public class UserService : IUserService
    {
        private readonly DatabaseContext _dbContext;
        private readonly TokenSettings _tokenSettings;

        public UserService(DatabaseContext databaseContext, IOptions<TokenSettings> tokenSettings)
        {
            _dbContext = databaseContext;
            _tokenSettings = tokenSettings.Value;
        }
        private User FromUserRegisterModelToUserModel(UserVM userVM)
        {
            return new User
            {
                Email = userVM.Email,
                FirstName = userVM.FirstName,
                LastName = userVM.LastName,
                Password = userVM.Password,
            };
        }
        private string HashPassword(string plainpassword)
        {
            


            byte[] salt = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            var rfcPassord = new Rfc2898DeriveBytes(plainpassword, salt, 1000, HashAlgorithmName.SHA1);
            byte[] rfcPasswordHash = rfcPassord.GetBytes(20);

            byte[] passwordHash = new byte[36];
            Array.Copy(salt, 0, passwordHash, 0, 16);
            Array.Copy(rfcPasswordHash, 0, passwordHash, 16, 20);

            return Convert.ToBase64String(passwordHash);
        }
        private string GenerateJwtToken(User user)
        {
            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenSettings.SecretKey));

            var credentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var cliams = new List<Claim>();
            cliams.Add(new Claim("Sub", user.Id.ToString()));
            cliams.Add(new Claim("FirstName", user.FirstName));
            cliams.Add(new Claim("LastName", user.LastName));
            cliams.Add(new Claim("Email", user.Email));

            var securityToken = new JwtSecurityToken(
                issuer: _tokenSettings.Issuer,
                audience: _tokenSettings.Audience,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: credentials,
                claims: cliams);

            return new JwtSecurityTokenHandler().WriteToken(securityToken);
        }

        private bool PasswordVerification(string plainPassword, string dbPassword)
        {
            byte[] dbPasswordHash = Convert.FromBase64String(dbPassword);

            byte[] salt = new byte[16];
            Array.Copy(dbPasswordHash, 0, salt, 0, 16);

            var rfcPassord = new Rfc2898DeriveBytes(plainPassword, salt, 1000, HashAlgorithmName.SHA1);
            byte[] rfcPasswordHash = rfcPassord.GetBytes(20);

            for (int i = 0; i < rfcPasswordHash.Length; i++)
            {
                if (dbPasswordHash[i + 16] != rfcPasswordHash[i])
                {
                    return false;
                }
            }
            return true;
        }

        public async Task<(bool IsLoginSuccess, JWTTokenResponseVM TokeResponse)> LoginAsync(LoginVM loginPayload)
        {
            if (string.IsNullOrEmpty(loginPayload.Email) || string.IsNullOrEmpty(loginPayload.Password))
            {
                return (false, null);
            }

            var user = await _dbContext.User.Where(_ => _.Email.ToLower() == loginPayload.Email.ToLower()).FirstOrDefaultAsync();

            if (user == null) { return (false, null); }

            bool validPassword = PasswordVerification(loginPayload.Password, user.Password);
            if (!validPassword) { return (false, null); }

            var jwtAccessToken = GenerateJwtToken(user);

            var result = new JWTTokenResponseVM
            {
                AccessToken = jwtAccessToken,
            };
            return (true, result);
        }

        public async Task<(bool IsUserRegistered, string Message)> RegisterNewUserAsync(UserVM userVM)
        {
            var isuserExist = _dbContext.User.Any(u => u.Email.ToLower() == userVM.Email.ToLower());
            if (isuserExist)
            {
                return (false, "Email Address Already Registered");
            }
            var NewUser = FromUserRegisterModelToUserModel(userVM);
            NewUser.Password = HashPassword(NewUser.Password);

            _dbContext.User.Add(NewUser);
            await _dbContext.SaveChangesAsync();

            return (true, "Success");
        }
    }
}
