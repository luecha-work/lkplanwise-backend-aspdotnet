using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using IRepository;
using IService;
using Microsoft.AspNetCore.Http;
using Shared.Enum;
using Shared.Utils;

namespace Service
{
    public class BlockBruteForceService : IBlockBruteForceService
    {

        private readonly IRepositoryManager _repositoryManager;
        private BlockBruteForce? _bruteForce;

        public BlockBruteForceService(
            IRepositoryManager repositoryManager
        )
        {
            _repositoryManager = repositoryManager;
        }

        public BlockBruteForce? BlockBruteForceManagement(string email)
        {
            _bruteForce = _repositoryManager.BlockBruteForceRepository.FindByCondition(force => force.Email == email).FirstOrDefault();

            if (_bruteForce == null)
            {
                // CreateBlockBruteForce(email);
                BlockBruteForce newBruteForce = new BlockBruteForce()
                {
                    Count = 1,
                    Email = email,
                    Status = EnumHelper.GetEnumValue(BlockForceStatusEnum.UnLock),
                    LockedTime = null,
                    UnLockTime = null,
                    CreatedBy = "system",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = null,
                    UpdatedBy = null
                };

                _repositoryManager.BlockBruteForceRepository.Create(newBruteForce);
                _repositoryManager.Commit();
            }
            else if (_bruteForce.Count >= 4)
            {
                //TODO: Add UnLockTime in Block Bruteforce
                var dateNow = DateTime.UtcNow;
                var unLockTime = dateNow.AddMinutes(15);

                _bruteForce.Count += 1;
                _bruteForce.Status = EnumHelper.GetEnumValue(BlockForceStatusEnum.Locked);
                _bruteForce.LockedTime = dateNow;
                _bruteForce.UnLockTime = unLockTime;
                _bruteForce.UpdatedAt = DateTime.UtcNow;

                _repositoryManager.BlockBruteForceRepository.Update(_bruteForce);
                _repositoryManager.Commit();
            }
            else
            {
                _bruteForce.Count += 1;
                _repositoryManager.BlockBruteForceRepository.Update(_bruteForce);
                _repositoryManager.Commit();
            }

            return _bruteForce;
        }

        public bool CheckBlockForceStatus(string email)
        {
            var blockForce = _repositoryManager.BlockBruteForceRepository.FindByCondition(force => force.Email == email).FirstOrDefault();

            if (blockForce == null)
            {
                return true;
            }

            if (
                blockForce.Status == EnumHelper.GetEnumValue(BlockForceStatusEnum.UnLock)
                && blockForce.Count < 5
            )
            {
                return true;
            }

            if (
                blockForce.Status == EnumHelper.GetEnumValue(BlockForceStatusEnum.Locked)
                && DateTime.UtcNow > blockForce.UnLockTime
            )
            {
                UnLockBlockBruteForce(blockForce);

                // await _repositoryManager.AccountRepository.UnLockAccountAsync(email);

                return true;
            }

            return false;
        }

        public BlockBruteForce? CreateBlockBruteForce(string email)
        {
            BlockBruteForce newBruteForce = new BlockBruteForce()
            {
                Count = 1,
                Email = email,
                Status = EnumHelper.GetEnumValue(BlockForceStatusEnum.UnLock),
                LockedTime = null,
                UnLockTime = null,
                CreatedBy = "system",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = null,
                UpdatedBy = null
            };


            _repositoryManager.BlockBruteForceRepository.Create(newBruteForce);
            _repositoryManager.Commit();

            return _bruteForce;
        }

        public void DeleteBlockBruteForce(Guid blockForceId)
        {
            var checkBruteForce = _repositoryManager.BlockBruteForceRepository.FindOneById(blockForceId);

            if (checkBruteForce != null)
            {
                _repositoryManager.BlockBruteForceRepository.Delete(checkBruteForce);
                _repositoryManager.Commit();
            }
        }

        // public void UpdateBlockBruteForce(BlockBruteForce blockForce)
        // {
        //     throw new NotImplementedException();
        // }

        private void UnLockBlockBruteForce(BlockBruteForce blockForce)
        {
            if (blockForce.Status == EnumHelper.GetEnumValue(BlockForceStatusEnum.Locked))
            {
                blockForce.Status = EnumHelper.GetEnumValue(BlockForceStatusEnum.UnLock);
                blockForce.UpdatedAt = DateTime.UtcNow;
                blockForce.Count = 0;
                blockForce.LockedTime = null;
                blockForce.UnLockTime = null;

                _repositoryManager.BlockBruteForceRepository.Update(blockForce);
                _repositoryManager.Commit();
            }
        }
    }
}
