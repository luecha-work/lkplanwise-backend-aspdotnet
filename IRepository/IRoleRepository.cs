using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRepository
{
    public interface IRoleRepository: IGenericRepositoryEntityFramework<Roles>
    {
        Task<Roles?> FindByRoleNameAsync(string roleName);
    }
}
