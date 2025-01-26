using Entities;
using IRepository;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Repository.EntityFramework
{
    public class AccountRepository: GenericRepository<Account>, IAccountsRepository
    {
        private readonly PlanWiseDbContext _context;
        private readonly UserManager<Account> _userManager;
        private readonly RoleManager<Roles> _roleManager;

        public AccountRepository(
            PlanWiseDbContext context,
            UserManager<Account> userManager,
            RoleManager<Roles> roleManager
            ): base(context)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<bool> CheckPasswordAsync(Account account, string password)
        {
            var result = await _userManager.CheckPasswordAsync(account, password);
            return result;
        }

        public async Task<IEnumerable<IdentityError>> CreateAccountAsync(Account account, string password)
        {
            var result = await _userManager.CreateAsync(account, password);

            return result.Errors;
        }

        public async Task<Account?> FindAccountByAccountIdAsync(Guid AccountId)
        {
            var account = await _userManager.FindByIdAsync(AccountId.ToString());
            return account;
        }

        public Task<Account?> FindAccountByEmailAsync(string email)
        {
            var account = _userManager.FindByEmailAsync(email);
            return account;
        }

        public Task<Account?> FindAccountByUsernameAsync(string username)
        {
            var account = _userManager.FindByNameAsync(username);
            return account;
        }

        public Task<IList<Claim>> GetClaimsForAccountAsync(Account account)
        {
            var claims = _userManager.GetClaimsAsync(account);
            return claims;
        }

        public Task<IList<string>> GetRolesForAccountAsync(Account account)
        {
            var roles = _userManager.GetRolesAsync(account);
            return roles;
        }
    }
}
