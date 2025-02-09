﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IService
{
    public interface IServiceManager
    {
        IAuthenticationService AuthenticationService { get; }
        IBlockBruteForceService BlockBruteForceService { get; }
        IPlanWiseSessionService PlanWiseSessionService { get; }
    }
}
