using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace IRepository
{
    public interface IPlanWiseSessionRepository: IGenericRepositoryEntityFramework<PlanWiseSession>
    {
        //PlanWiseSession FindSessionById(Guid sessionId);
        //void CreateSession(PlanWiseSession sessionEntity);
        //void UpdateSession(PlanWiseSession sessionEntity);
        //void DeleteSession(PlanWiseSession sessionEntity);
    }
}
