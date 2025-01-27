﻿using Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.ConfigurationModels;
using IRepository;

namespace Repository.EntityFramework
{
    public class RepositoryManager : IRepositoryManager
    {
        private readonly PlanWiseDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly Lazy<IAccountsRepository> _accountRepository;
        private readonly Lazy<IRoleRepository> _roleRepository;
        private readonly Lazy<IAccountRolesRepository> _accountRolesRepository;
        private readonly Lazy<IAuthenticationManager> _authenticationManager;
        private readonly Lazy<IPlanWiseSessionRepository> _planWiseSessionRepository;
        private readonly Lazy<IBlockBruteForceRepository> _blockBruteForceRepository;

        public RepositoryManager(
            PlanWiseDbContext context,
            UserManager<Account> userManager,
            RoleManager<Roles> roleManager,
            IHttpContextAccessor httpContextAccessor
        )
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;

            _accountRepository = new Lazy<IAccountsRepository>(
                () => new AccountRepository(_context, userManager, roleManager)
            );
            _roleRepository = new Lazy<IRoleRepository>(
                () => new RoleRepository(_context, roleManager)
            );
            _accountRolesRepository = new Lazy<IAccountRolesRepository>(
                () => new AccountRolesRepository(_context)
            );
            _authenticationManager = new Lazy<IAuthenticationManager>(
                () => new AuthenticationManager(userManager)
            );
            _planWiseSessionRepository = new Lazy<IPlanWiseSessionRepository>(
                () => new PlanWiseSessionRepository(_context)
            );
            _blockBruteForceRepository = new Lazy<IBlockBruteForceRepository>(
                () => new BlockBruteForceRepository(_context)
            );
        }

        public IAccountsRepository AccountRepository => _accountRepository.Value;
        public IAccountRolesRepository AccountRolesRepository => _accountRolesRepository.Value;
        public IRoleRepository RoleRepository => _roleRepository.Value;
        public IAuthenticationManager AuthenticationManager => _authenticationManager.Value;
        public IPlanWiseSessionRepository PlanWiseSessionRepository => _planWiseSessionRepository.Value;
        public IBlockBruteForceRepository BlockBruteForceRepository => _blockBruteForceRepository.Value;

        public void Commit() => _context.SaveChanges();

        private IUserProvider? GetUserProviderAsync()
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
