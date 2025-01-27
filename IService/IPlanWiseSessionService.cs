using Entities;
using Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IService
{
    public interface IPlanWiseSessionService
    {
        PlanWiseSession? GetPlanWiseSessionById(Guid sessionId);
        PlanWiseSession CreatePlanWiseSession(
            Account account,
            BaseAuthenticationDto clientDetail
        );
        void UpdatePlanWiseSession(PlanWiseSession planWiseSession);
        void DeletePlanWiseSession(Guid sessionId);
        bool CheckPlanWiseSessionStatus(
            Guid sessionId,
            Guid accountId,
            string reqIpAddress
        );
        void BlockPlanWiseSession(PlanWiseSession planWiseSession);
        void PlanWiseSessionExpired(PlanWiseSession planWiseSession);
        void ClearSession();
    }
}
