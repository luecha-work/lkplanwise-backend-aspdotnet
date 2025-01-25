using Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.ConfigurationModels;

namespace Repository.EntityFramework
{
    public class RepositoryManager
    {
        private readonly PlanWiseDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        //private readonly Lazy<IAccountRepository> _accountRepository;

        public RepositoryManager(
            PlanWiseDbContext context,
            UserManager<Account> userManager,
            RoleManager<Roles> roleManager,
            IHttpContextAccessor httpContextAccessor
        )
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;


        }

        public void Commit() => _context.SaveChanges();

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
