﻿using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRepository
{
    public interface IRepositoryManager
    {
        IAccountsRepository AccountRepository { get; }
        IRoleRepository RoleRepository { get; }
        IAuthenticationManager AuthenticationManager { get; }
        IBlockBruteForceRepository BlockBruteForceRepository { get; }
        IPlanWiseSessionRepository PlanWiseSessionRepository { get; }

        void Commit();
    }
}
