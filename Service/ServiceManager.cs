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
    public class ServiceManager : IServiceManager
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly Lazy<IAuthenticationService> _authenticationService;
        private readonly Lazy<IBlockBruteForceService> _blockBruteForceService;
        private readonly Lazy<IPlanWiseSessionService> _planWiseSessionService;

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

            _blockBruteForceService = new Lazy<IBlockBruteForceService>(
                () => new BlockBruteForceService(_repositoryEFManager)
            );
            _planWiseSessionService = new Lazy<IPlanWiseSessionService>(
                () => new PlanWiseSessionService(_repositoryEFManager, _httpContextAccessor)
            );
            _authenticationService = new Lazy<IAuthenticationService>(
                () => new AuthenticationService(_repositoryEFManager, _planWiseSessionService, _blockBruteForceService, configuration, configurationJwt, configurationIdentityConfigure, mapper)
            );
        }

        public IAuthenticationService AuthenticationService => _authenticationService.Value;
        public IBlockBruteForceService BlockBruteForceService => _blockBruteForceService.Value;
        public IPlanWiseSessionService PlanWiseSessionService => _planWiseSessionService.Value;

        private IUserProvider? GetUserProvider()
        {
            var context = _httpContextAccessor.HttpContext;
            if (context != null && context.Items.ContainsKey("userProvider"))
            {
                return context?.Items["userProvider"] as IUserProvider ?? null;
            }

            return null;
        }
    }
}
