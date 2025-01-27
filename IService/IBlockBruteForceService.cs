using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;

namespace IService
{
    public interface IBlockBruteForceService
    {
        BlockBruteForce? CreateBlockBruteForce(string email);
        // void UpdateBlockBruteForce(BlockBruteForce blockForce);
        void DeleteBlockBruteForce(Guid blockForceId);
        BlockBruteForce? BlockBruteForceManagement(string email);
        bool CheckBlockForceStatus(string email);
    }
}
