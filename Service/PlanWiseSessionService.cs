using Entities;
using IRepository;
using IService;
using Microsoft.AspNetCore.Http;
using Shared.DTOs;
using Shared.Enum;
using Shared.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class PlanWiseSessionService : IPlanWiseSessionService
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PlanWiseSessionService(
            IRepositoryManager repositoryManager,
            IHttpContextAccessor httpContextAccessor
        )
        {
            _repositoryManager = repositoryManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public void PlanWiseSessionExpired(PlanWiseSession planWiseSession)
        {
            planWiseSession.SessionStatus = EnumHelper.GetEnumValue(SessionStatusEnum.Expired);
            planWiseSession.UpdatedAt = DateTime.UtcNow;

            _repositoryManager.PlanWiseSessionRepository.Update(planWiseSession);
            _repositoryManager.Commit();
        }

        public void BlockPlanWiseSession(PlanWiseSession planWiseSession)
        {
            planWiseSession.SessionStatus = EnumHelper.GetEnumValue(SessionStatusEnum.Blocked);
            planWiseSession.UpdatedAt = DateTime.UtcNow;

            _repositoryManager.PlanWiseSessionRepository.Update(planWiseSession);
            _repositoryManager.Commit();
        }

        public bool CheckPlanWiseSessionStatus(Guid sessionId, Guid accountId, string reqIpAddress)
        {
            var session =
                 _repositoryManager.PlanWiseSessionRepository.FindOneById(sessionId);

            if (session == null) return false;

            if (session.AccountId != accountId || session.LoginIp != reqIpAddress)
            {
                //TODO: Block Axons Cms Session
                BlockPlanWiseSession(session);

                return false;
            }

            if (session.SessionStatus != EnumHelper.GetEnumValue(SessionStatusEnum.Active)) return false;

            if (DateTime.UtcNow > session.ExpirationTime)
            {
                //TODO: Axons Cms Session Expired
                PlanWiseSessionExpired(session);

                return false;
            }

            return true;
        }

        public void ClearSession()
        {
            string authorizationHeader = GetAuthorizationHeader();

            if (string.IsNullOrEmpty(authorizationHeader))
            {
                throw new Exception("Clear session notfound authorization token.");
            }

            string token = authorizationHeader.Substring("Bearer ".Length).Trim();
            string sessionId = JWTHelper.GetSessionIdFromToken(token);

            var session =
                 _repositoryManager.PlanWiseSessionRepository.FindOneById(Guid.Parse(sessionId));

            if (session != null)
            {
                _repositoryManager.PlanWiseSessionRepository.Delete(session);
                _repositoryManager.Commit();
            }
        }

        public PlanWiseSession CreatePlanWiseSession(Account account, BaseAuthenticationDto clientDetail)
        {
            var ipAddr = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? string.Empty;
            var dateNow = DateTime.UtcNow;
            var dateExpiration = dateNow.AddHours(24);

            var oldPlanWiseSession = _repositoryManager.PlanWiseSessionRepository.FindByCondition(session =>
                    session.AccountId == account.Id && session.LoginIp == ipAddr).FirstOrDefault();

            if (oldPlanWiseSession != null)
            {
                DeletePlanWiseSession(oldPlanWiseSession.SessionId);
            }

            var sessionForCreate = new PlanWiseSession()
            {
                AccountId = account.Id,
                Browser = clientDetail.Browser,
                Os = clientDetail.Os,
                Platform = clientDetail.PlatForm,
                LoginIp = ipAddr ?? "",
                SessionStatus = EnumHelper.GetEnumValue(SessionStatusEnum.Active),
                IssuedTime = dateNow,
                ExpirationTime = dateExpiration,
                LoginAt = dateNow,
                CreatedAt = dateNow,
                UpdatedAt = dateNow,
            };

            _repositoryManager.PlanWiseSessionRepository.Create(sessionForCreate);
            _repositoryManager.Commit();

            return sessionForCreate;
        }

        public void DeletePlanWiseSession(Guid sessionId)
        {
            var checkSession = _repositoryManager.PlanWiseSessionRepository.FindOneById(sessionId);

            if (checkSession != null)
            {
                _repositoryManager.PlanWiseSessionRepository.Delete(checkSession);
                _repositoryManager.Commit();
            }
        }

        public PlanWiseSession? GetPlanWiseSessionById(Guid sessionId)
        {
            var session = _repositoryManager.PlanWiseSessionRepository.FindOneById(sessionId);
            if (session == null)
            {
                throw new Exception("Session not found.");
            }

            return session;
        }

        // public void UpdatePlanWiseSession(PlanWiseSession planWiseSession)
        // {
        //     _repositoryManager.PlanWiseSessionRepository.Update(planWiseSession);
        //     _repositoryManager.Commit();
        // }

        private string GetAuthorizationHeader()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext != null && httpContext.Request.Headers.Authorization.Count > 0)
            {
                return httpContext.Request.Headers.Authorization.ToString();
            }
            return string.Empty;
        }
    }
}
