using Entities;
using IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.EntityFramework
{
    public class BlockBruteForceRepository : GenericRepository<BlockBruteForce>,
            IBlockBruteForceRepository
    {
        private readonly LKPlanWiseDbContext _context;

        public BlockBruteForceRepository(LKPlanWiseDbContext context)
           : base(context)
        {
            _context = context;
        }
    }
}
