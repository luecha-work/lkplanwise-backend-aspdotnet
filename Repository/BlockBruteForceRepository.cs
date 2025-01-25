using Entities;
using IRepository;
using Microsoft.EntityFrameworkCore;
using Repository.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class BlockBruteForceRepository: GenericRepository<BlockBruteForce>,
            IBlockBruteForceRepository
    {
        private readonly PlanWiseDbContext _context;

        public BlockBruteForceRepository(PlanWiseDbContext context)
           : base(context)
        {
            _context = context;
        }
    }
}
