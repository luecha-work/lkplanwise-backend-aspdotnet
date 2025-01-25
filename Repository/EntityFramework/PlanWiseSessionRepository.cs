using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Entities;
using IRepository;

namespace Repository.EntityFramework
{
    public class PlanWiseSessionRepository: GenericRepository<PlanWiseSession>, IPlanWiseSessionRepository
    {
        private readonly PlanWiseDbContext _context;

        public PlanWiseSessionRepository(PlanWiseDbContext context) : base(context)
        {
            _context = context;
        }

        //public PlanWiseSession FindSessionById(Guid sessionId)
        //{
        //    var sessionEntity =  _context.PlanWiseSession.Find(sessionId);

        //    var axonscmsSessionResult = _mapper.Map<PlanWiseSession>(sessionEntity);

        //    return axonscmsSessionResult;
        //}

        //public void UpdateSession(PlanWiseSession sessionEntity) =>
        //    _context.PlanWiseSession.Update(sessionEntity);

        //public void CreateSession(PlanWiseSession sessionEntity) =>
        //    _context.PlanWiseSession.Add(sessionEntity);

        //public void DeleteSession(PlanWiseSession sessionEntity) =>
        //    _context.PlanWiseSession.Remove(sessionEntity);
    }
}
