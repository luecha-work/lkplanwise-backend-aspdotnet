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
using Shared.Enum;
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
        private readonly IPlanWiseSessionService _planWiseSessionService;
        private readonly IBlockBruteForceService _blockForceService;
        private readonly IConfiguration _configuration;
        private readonly JwtConfiguration _jwtConfiguration;
        private readonly IdentityProviderConfigure _configurationIdentityProvider;
        private readonly IMapper _mapper;

        private Account? _user;
        private PlanWiseSession? _session;

        public AuthenticationService(
            IRepositoryManager repositoryManager,
            Lazy<IPlanWiseSessionService> planWiseSessionService,
            Lazy<IBlockBruteForceService> blockforceService,
            IConfiguration configuration,
            IOptions<JwtConfiguration> configurationJwt,
            IOptions<IdentityProviderConfigure> configurationIdentityConfigure,
            IMapper mapper
        )
        {
            _repositoryManager = repositoryManager;
            _planWiseSessionService = planWiseSessionService.Value;
            _blockForceService = blockforceService.Value;
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

        //         public async Task<AuthResponseDto> LoginAzureADForMultiTenantAsync(
        //     AuthenticationAzureDto loginADDto
        // )
        //         {
        //             var azureADTokenService = new AzureAdTokenService(_configuration);
        //             var azureADToken = azureADTokenService.GetAccessToken(loginADDto.AzureCode);
        //             var email = string.Empty;
        //             var azureDecryptToken = azureADTokenService.GetDataFromToken(azureADToken);

        //             if (azureDecryptToken != null)
        //             {
        //                 email = azureDecryptToken.email;
        //                 if (string.IsNullOrEmpty(email))
        //                     email = azureDecryptToken.unique_name;
        //             }

        //             bool checkeForce = _blockforceService.CheckeBlockforceStatus(email);

        //             if (!checkeForce)
        //             {
        //                 return null;
        //             }

        //             _user = await _repositoryManager.AccountRepository.FindAccountByEmailAsync(email);

        //             if (_user == null)
        //             {
        //                 await _blockforceService.BlockBruteforceManagmentAsync(email);

        //                 throw new LoginBadRequestException(
        //                     "This user was not found in the system. Please contact the system administrator."
        //                 );
        //             }

        //             // if (_user.Active == false)
        //             // {
        //             //     return null;
        //             // }

        //             var clientDetail = new BaseAuthenticationDto()
        //             {
        //                 Os = loginADDto.Os,
        //                 Browser = loginADDto.Browser,
        //                 PlatForm = loginADDto.PlatForm
        //             };

        //             _cmsSession = await _axonscmsSessionService.CreateAxonscmsSessionAsync(
        //                 _user,
        //                 clientDetail
        //             );

        //             var token = await GenerateToken();

        //             _cmsSession.Token = token;
        //             _cmsSession.UpdatedAt = DateTime.UtcNow;

        //             _axonscmsSessionService.UpdatAxonscmsSession(_cmsSession);

        //             return new AuthResponseDto
        //             {
        //                 AccessToken = token,
        //                 RefreshToken = await CreateRefreshTokenAsync()
        //             };
        //         }

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
                _blockForceService.BlockBruteForceManagement(loginLocalDto.Email);

                throw new LoginBadRequestException("Please check your username or password incorrect.");
            }

            var clientDetail = new BaseAuthenticationDto()
            {
                Os = loginLocalDto.Os,
                Browser = loginLocalDto.Browser,
                PlatForm = loginLocalDto.PlatForm
            };
            // TODO: Implement session management
            _session = _planWiseSessionService.CreatePlanWiseSession(
                _user,
                clientDetail
            );

            var token = await GenerateToken();

            _session.Token = token;
            _session.UpdatedAt = DateTime.UtcNow;

            //TDOD: Update session
            _repositoryManager.PlanWiseSessionRepository.Update(_session);
            _repositoryManager.Commit();

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

        public async Task<AuthResponseDto?> VerifyRefreshTokenAsync(AuthResponseDto request)
        {
            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var tokenContent = jwtSecurityTokenHandler.ReadJwtToken(request.AccessToken);
            var username = tokenContent
               .Claims.ToList()
               .Find(q => q.Type == JwtRegisteredClaimNames.Sub)
               ?.Value;

            var sessionId = tokenContent.Claims.FirstOrDefault(q => q.Type == "session_id")?.Value;

            if (sessionId == null)
            {
                throw new ArgumentNullException(nameof(sessionId), "Session ID cannot be null.");
            }
            Guid guidSessionId = Guid.Parse(sessionId);

            var requestUserId = Guid.Parse(
               tokenContent.Claims.FirstOrDefault(q => q.Type == "account_id")?.Value ?? throw new ArgumentNullException("account_id")
            );

            if (username == null)
            {
                throw new ArgumentNullException(nameof(username), "Username cannot be null.");
            }

            _user = await _repositoryManager.AccountRepository.FindAccountByUsernameAsync(username);

            _session = _repositoryManager.PlanWiseSessionRepository.FindOneById(guidSessionId);

            if (_user == null || _user.Id != requestUserId || _session == null)
                return null;

            //TODO: Validate Last Time Refresh Token
            var refreshTokenTimeAgain =
               _session.RefreshTokenAt != null
                   ? _session.RefreshTokenAt.Value.AddMinutes(5)
                   : (DateTime?)null;

            if (
               _session.RefreshTokenAt != null
               && _session.RefreshTokenAt < refreshTokenTimeAgain
            )
                throw new RefreshTokensTooOftenException();

            if (_session.SessionStatus == EnumHelper.GetEnumValue(SessionStatusEnum.Expired))
                throw new RefreshTokenExpirationTimeException();

            if (_session.SessionStatus == EnumHelper.GetEnumValue(SessionStatusEnum.Blocked))
                throw new BlockedRefreshTokenExpirationException();

            var isValidRefreshToken = await _repositoryManager.AuthenticationManager.VerifyUserTokenAsync(
               _user,
               _configurationIdentityProvider.LoginProvider,
               _configurationIdentityProvider.RefreshTokenProvider,
               request.RefreshToken
            );

            if (isValidRefreshToken)
            {
                var token = await GenerateToken();

                _session.Token = token;
                _session.UpdatedAt = DateTime.UtcNow;
                _session.RefreshTokenAt = DateTime.UtcNow;

                _repositoryManager.PlanWiseSessionRepository.Update(_session);


                return new AuthResponseDto
                {
                    AccessToken = token,
                    RefreshToken = await CreateRefreshTokenAsync()
                };
            }

            await _repositoryManager.AuthenticationManager.UpdateSecurityStampAsync(_user);

            _planWiseSessionService.BlockPlanWiseSession(_session);

            _repositoryManager.Commit();

            throw new InvalidRefreshTokenException();
        }

        private async Task<string> GenerateToken()
        {
            Console.WriteLine("Generate Token 111");
            var securityKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_jwtConfiguration.SecretKey) //TODO: Get Key from JwtSettings in  applications.json
            );
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            if (_user == null)
            {
                throw new InvalidOperationException("User cannot be null when retrieving roles.");
            }
            var roles = await _repositoryManager.AccountRepository.GetRolesForAccountAsync(_user);
            var roleClaims = roles.Select(x => new Claim(ClaimTypes.Role, x)).ToList();
            // var userClaims = await _repositoryManager.AccountRepository.GetClaimsForAccount(_user);

            if (_user == null)
            {
                throw new InvalidOperationException("User cannot be null when generating token.");
            }

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, _user.UserName ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, _user.Email ?? string.Empty),
                new Claim("session_id", _session?.SessionId.ToString() ?? string.Empty),
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

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
