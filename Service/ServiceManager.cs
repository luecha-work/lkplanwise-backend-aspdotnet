using AutoMapper;
using Entities.ConfigurationModels;
using IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class ServiceManager
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ServiceManager(
          IRepositoryFactory repositoryFactory,
          IMapper mapper,
          IHttpContextAccessor httpContextAccessor,
          IOptions<AwsS3Configuration> configurationS3
      )
        {

            _httpContextAccessor = httpContextAccessor;
            // var _repositoryDPManager = repositoryFactory.Create(RepoType.Dapper);
            var _repositoryEFManager = repositoryFactory.Create(RepoType.EntityFramework);
        }
    }
}
