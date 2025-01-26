using AutoMapper;
using Entities;
using Entities.ConfigurationModels;
using IRepository;
using IService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Shared.DTOs;
using Shared.Exceptions;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly IConfiguration _configuration;
        private readonly JwtConfiguration _jwtConfiguration;
        private readonly IdentityProviderConfigure _configurationIdentityProvider;
        private readonly IMapper _mapper;

        private Account? _user;
        private PlanWiseSession? _Session;

        public AuthenticationService(
            IRepositoryManager repositoryManager,
            IConfiguration configuration,
            IOptions<JwtConfiguration> configurationJwt,
            IOptions<IdentityProviderConfigure> configurationIdentityConfigure,
            IMapper mapper
        )
        {
            _repositoryManager = repositoryManager;
            _configuration = configuration;
            _jwtConfiguration = configurationJwt.Value;
            _configurationIdentityProvider = configurationIdentityConfigure.Value;
            _mapper = mapper;
        }

        public Task<string> CreateRefreshTokenAsync()
        {
            throw new NotImplementedException();
        }

        public Task<UserProvider> GetUserProvider(int accountId)
        {
            throw new NotImplementedException();
        }

        public string LoginAzureADForMultiTenant(string authorizeCode)
        {
            throw new NotImplementedException();
        }

        public async Task<AuthResponseDto> LoginLocalAsync(AuthenticationLocalDto loginLocalDto)
        {
            _user = await _repositoryManager.AccountRepository.FindAccountByEmailAsync(
                loginLocalDto.Email
            );

            bool isValidUser = await _repositoryManager.AccountRepository.CheckPasswordAsync(
                _user,
                loginLocalDto.Password
            );

            if (_user == null || !isValidUser)
            {
                // TODO: Implement bruteforce management
                // await _blockforceService.BlockBruteforceManagmentAsync(loginLocalDto.Email);

                throw new LoginBadRequestException("Please check your username or password incorrect.");
            }

            var clientDetail = new BaseAuthenticationDto()
            {
                Os = loginLocalDto.Os,
                Browser = loginLocalDto.Browser,
                PlatForm = loginLocalDto.PlatForm
            };
            // TODO: Implement session management
            //_Session = await _axonscmsSessionService.CreateAxonscmsSessionAsync(
            //    _user,
            //    clientDetail
            //);

            var token = await GenerateToken();

            _Session.Token = token;
            _Session.UpdatedAt = DateTime.UtcNow;

            //TDOD: Update session
            //_axonscmsSessionService.UpdatAxonscmsSession(_cmsSession);

            return new AuthResponseDto
            {
                AccessToken = token,
                RefreshToken = await CreateRefreshTokenAsync()
            };
        }

        public async Task<AccountDto> SingUpAsync(SingUpDto singUpDto)
        {
            //TODO:Valisdate User to create
            var isAdEmailDuplicate =
                await _repositoryManager.AccountRepository.FindAccountByEmailAsync(singUpDto.Email);

            var isAdUsernameDuplicate =
                await _repositoryManager.AccountRepository.FindAccountByUsernameAsync(
                    singUpDto.UserName
                );

            if (isAdEmailDuplicate != null)
                throw new SingUpEmailDuplicateBadRequestException(singUpDto.Email);

            if (isAdUsernameDuplicate != null)
                throw new SingUpWithUsernameDuplicateBadRequestException(singUpDto.Email);

            var AccountForCreate = _mapper.Map<Account>(singUpDto);
            AccountForCreate.CreatedBy = "system";
            AccountForCreate.CreatedAt = DateTime.UtcNow;

            var errors = await _repositoryManager.AccountRepository.CreateAccountAsync(
                AccountForCreate,
                singUpDto.Password
            );

            if (!errors.Any())
            {
              

                var role = await _repositoryManager.RoleRepository.FindByRoleNameAsync("User");

                var AccountsRole = new AccountRoles()
                {
                    UserId = AccountForCreate.Id,
                    RoleId = role.Id,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "system",
                };

                await _repositoryManager.AccountRolesRepository.CreateAsync(AccountsRole);
            }

            var accountResult = _mapper.Map<AccountDto>(AccountForCreate);

            return accountResult;
        }

        public bool VerifyAccessToken(string accessToken)
        {
            throw new NotImplementedException();
        }

        public Task<AuthResponseDto> VerifyRefreshTokenAsync(AuthResponseDto request)
        {
            throw new NotImplementedException();
        }


        private async Task<string> GenerateToken()
        {
            var securityKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_jwtConfiguration.SecretKey) //TODO: Get Key from JwtSettings in  applications.json
            );

            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var roles = await _repositoryManager.AccountRepository.GetRolesForAccountAsync(_user);
            var roleClaims = roles.Select(x => new Claim(ClaimTypes.Role, x)).ToList();
            // var userClaims = await _authRepositoryManager.authManager.GetClaimsForAccount(_user);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, _user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, _user.Email),
                new Claim("session_id", _Session.SessionId.ToString()),
                new Claim("account_id", _user.Id.ToString()),
            }
            // .Union(userClaims)
            .Union(roleClaims);

            var token = new JwtSecurityToken(
                issuer: _jwtConfiguration.ValidIssuer,
                audience: _jwtConfiguration.ValidAudience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(
                    Convert.ToInt32(_jwtConfiguration.DurationInMinutes)
                ),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
