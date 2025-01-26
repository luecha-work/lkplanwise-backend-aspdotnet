using AutoMapper;
using Entities.ConfigurationModels;
using IRepository;
using IService;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class ServiceManager: IServiceManager
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly Lazy<IAuthenticationService> _authenticationService;

        public ServiceManager(
          IRepositoryFactory repositoryFactory,
          IMapper mapper,
          IHttpContextAccessor httpContextAccessor,
          IOptions<AwsS3Configuration> configurationS3,
          IConfiguration configuration,
          IOptions<JwtConfiguration> configurationJwt,
          IOptions<IdentityProviderConfigure> configurationIdentityConfigure
      )
        {

            _httpContextAccessor = httpContextAccessor;
            // var _repositoryDPManager = repositoryFactory.Create(RepoType.Dapper);
            var _repositoryEFManager = repositoryFactory.Create(RepoType.EntityFramework);

            _authenticationService = new Lazy<IAuthenticationService>(
                () => new AuthenticationService(_repositoryEFManager, configuration, configurationJwt, configurationIdentityConfigure, mapper)
            );
        }

        public IAuthenticationService AuthenticationService => _authenticationService.Value;

        private IUserProvider GetUserProvider()
        {
            var context = _httpContextAccessor.HttpContext;
            if (context != null && context.Items.ContainsKey("userProvider"))
            {
                return context.Items["userProvider"] as IUserProvider;
            }

            return null;
        }
    }
}
