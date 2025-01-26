using AutoMapper;
using Azure.Core;
using Entities;
using Entities.ConfigurationModels;
using IRepository;
using IService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Shared.DTOs;
using Shared.Exceptions;
using Shared.Utils;
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

        public async Task<string> CreateRefreshTokenAsync()
        {
            await _repositoryManager.AuthenticationManager.RemoveAuthenticationTokenAsync(
                _user,
                _configurationIdentityProvider.LoginProvider,
                _configurationIdentityProvider.RefreshTokenProvider
            );

            var newRefreshToken = await _repositoryManager.AuthenticationManager.GenerateUserTokenAsync(
                _user,
                _configurationIdentityProvider.LoginProvider,
                _configurationIdentityProvider.RefreshTokenProvider
            );

            await _repositoryManager.AuthenticationManager.SetAuthenticationTokenAsync(
                _user,
                _configurationIdentityProvider.LoginProvider,
                _configurationIdentityProvider.RefreshTokenProvider,
                newRefreshToken
            );

            return newRefreshToken;
        }

        public async Task<UserProvider> GetUserProvider(string accountId)
        {
            _user = await _repositoryManager.AccountRepository.FindAccountByAccountIdAsync(Guid.Parse(accountId));


            List<string> roles = (
                await _repositoryManager.AccountRepository.GetRolesForAccountAsync(_user)
            ).ToList();

            return new UserProvider()
            {
                UserInfo = _user,
                RoleInfo = roles
            };
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

            //_Session.Token = token;
            //_Session.UpdatedAt = DateTime.UtcNow;

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
            if (string.IsNullOrEmpty(accessToken))
            {
                return false;
            }

            return JWTHelper.VerifyToken(accessToken, _jwtConfiguration);
        }

        public async Task<AuthResponseDto> VerifyRefreshTokenAsync(AuthResponseDto request)
        {
            //var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            //var tokenContent = jwtSecurityTokenHandler.ReadJwtToken(request.AccessToken);
            //var username = tokenContent
            //    .Claims.ToList()
            //    .Find(q => q.Type == JwtRegisteredClaimNames.Sub)
            //    ?.Value;

            //var sessionId = tokenContent.Claims.FirstOrDefault(q => q.Type == "session_id")?.Value;
            //Guid guidSessionId = Guid.Parse(sessionId);

            //var requestUserId = int.Parse(
            //    tokenContent.Claims.FirstOrDefault(q => q.Type == "account_id")?.Value
            //);

            //_user = await _repositoryManager.AccountRepository.FindAccountByUsernameAsync(username);

            ////_Session =
            ////    await _repositoryManager.axonscmsSessionRepository.FindSessionByIdAsync(
            ////        guidSessionId
            ////    );

            //if (_user == null || _user.Id != requestUserId || _cmsSession == null)
            //    return null;

            ////TODO: Validate Last Time Refresh Token
            //var refreshTokenTimeAgain =
            //    _cmsSession.RefreshTokenAt != null
            //        ? _cmsSession.RefreshTokenAt.Value.AddMinutes(5)
            //        : (DateTime?)null;

            //if (
            //    _cmsSession.RefreshTokenAt != null
            //    && _cmsSession.RefreshTokenAt < refreshTokenTimeAgain
            //)
            //    throw new RefreshTokensTooOftenException();

            //if (_cmsSession.SessionStatusEnum == EnumHelper.GetEnumValue(SessionStatusEnum.Expired))
            //    throw new RefreshTokenExpirationTimeException();

            //if (_cmsSession.SessionStatusEnum == EnumHelper.GetEnumValue(SessionStatusEnum.Blocked))
            //    throw new BlockedRefreshTokenExpirationException();

            //var isValidRefreshToken = await _authRepositoryManager.authManager.VerifyUserToken(
            //    _user,
            //    _configurationIdentityProvider.LoginProvider,
            //    _configurationIdentityProvider.RefreshTokenProvider,
            //    request.RefreshToken
            //);

            //if (isValidRefreshToken)
            //{
            //    var token = await GenerateToken();

            //    _cmsSession.Token = token;
            //    _cmsSession.UpdatedAt = DateTime.UtcNow;
            //    _cmsSession.RefreshTokenAt = DateTime.UtcNow;

            //    _axonscmsSessionService.UpdatAxonscmsSession(_cmsSession);

            //    return new AuthResponseDto
            //    {
            //        AccessToken = token,
            //        RefreshToken = await CreateRefreshTokenAsync()
            //    };
            //}

            //await _authRepositoryManager.authManager.UpdateSecurityStamp(_user);

            //_axonscmsSessionService.BlockAxonscmsSession(_cmsSession);

            //_authRepositoryManager.Commit();

            //throw new InvalidRefreshTokenException();

            throw new NotImplementedException();
        }


        private async Task<string> GenerateToken()
        {
            Console.WriteLine("Generate Token 111");
            var securityKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_jwtConfiguration.SecretKey) //TODO: Get Key from JwtSettings in  applications.json
            );

            Console.WriteLine("Security Key: " + securityKey);

            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            Console.WriteLine("_user is null: " + _user == null);

            var roles = await _repositoryManager.AccountRepository.GetRolesForAccountAsync(_user);
            var roleClaims = roles.Select(x => new Claim(ClaimTypes.Role, x)).ToList();
            // var userClaims = await _authRepositoryManager.authManager.GetClaimsForAccount(_user);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, _user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, _user.Email),
                //new Claim("session_id", _Session.SessionId.ToString()),
                new Claim("account_id", _user.Id.ToString()),
            }
            // .Union(userClaims)
            .Union(roleClaims);

            Console.WriteLine("Claims: " + claims);

            var token = new JwtSecurityToken(
                issuer: _jwtConfiguration.ValidIssuer,
                audience: _jwtConfiguration.ValidAudience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(
                    Convert.ToInt32(_jwtConfiguration.DurationInMinutes)
                ),
                signingCredentials: credentials
            );

            Console.WriteLine("Token: " + token);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
