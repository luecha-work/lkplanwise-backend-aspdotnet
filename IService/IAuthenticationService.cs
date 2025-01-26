using Entities.ConfigurationModels;
using Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IService
{
    public interface IAuthenticationService
    {
        Task<AuthResponseDto> LoginLocalAsync(AuthenticationLocalDto loginLocalDto);
        //Task<AuthResponseDto?> LoginAzureADForMultiTenantAsync(AuthenticationAzureDto loginADDto);
        string LoginAzureADForMultiTenant(string authorizeCode);
        Task<string> CreateRefreshTokenAsync();
        Task<AuthResponseDto> VerifyRefreshTokenAsync(AuthResponseDto request);
        Task<UserProvider> GetUserProvider(int accountId);
        bool VerifyAccessToken(string accessToken);
        Task<AccountDto> SingUpAsync(SingUpDto singUpDto);
    }
}
