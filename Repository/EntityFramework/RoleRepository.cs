using Entities;
using IRepository;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.EntityFramework
{
    public class RoleRepository : GenericRepository<Roles>, IRoleRepository
    {
        private readonly RoleManager<Roles> _roleManager;

        public RoleRepository(PlanWiseDbContext context, RoleManager<Roles> roleManager)
            : base(context)
        {
            _roleManager = roleManager;
        }

        public async Task<Roles?> FindByRoleNameAsync(string roleName)
        {
           var role = await  _roleManager.FindByNameAsync(roleName);
            return role;
        }
    }
}
