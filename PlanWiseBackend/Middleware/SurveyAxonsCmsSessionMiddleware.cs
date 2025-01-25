using Entities.ConfigurationModels;
using Microsoft.Extensions.Options;
using Shared.Exceptions;
using Shared.Utils;

namespace PlanWiseBackend.Middleware
{
    public class SurveyAxonsCmsSessionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly JwtConfiguration _jwtConfiguration;
        //private readonly ISerilogManager _logger;

        public SurveyAxonsCmsSessionMiddleware(
            RequestDelegate next,
            IOptions<JwtConfiguration> configurationJWT
            //ISerilogManagerFactory serilogManagerFactory
        )
        {
            _next = next;
            _jwtConfiguration = configurationJWT.Value;
            //_logger = serilogManagerFactory.CreateLogger<SurveyAxonsCmsSessionMiddleware>();
        }

        public async Task InvokeAsync(HttpContext context, IUserProvider userProvider)
        {
            try
            {
                //IAuthServiceManager _authServiceManager =
                //    context.RequestServices.GetRequiredService<IAuthServiceManager>();

                var ipAddress = context.Connection.RemoteIpAddress.ToString();
                var controller = context.GetRouteValue("controller").ToString().ToUpper();
                var methodName = context.GetRouteValue("action").ToString().ToUpper() ?? string.Empty;

                if (
                    controller != "AUTHENTICATION"
                    && methodName != "LOGINLOCAL"
                    && methodName != "REFRESHTOKEN"
                    && methodName != "VERIFYACCESSTOKEN"
                )
                {
                    string authorizationHeader = context.Request.Headers.Authorization;
                    if (
                        !string.IsNullOrEmpty(authorizationHeader)
                        && authorizationHeader.StartsWith("Bearer ")
                    )
                    {
                        string token = context
                            .Request.Headers.Authorization.ToString()
                            .Replace("Bearer ", "");

                        if (!string.IsNullOrEmpty(token))
                        {
                            throw new VerificationTokenNotFoundForCheckSessionException();
                        }

                            bool isVerifyJwtToken = JWTHelper.VerifyToken(token, _jwtConfiguration);

                        if (isVerifyJwtToken)
                        {
                            string sessionId = JWTHelper.GetAxonsCmsSessionIdFromToken(token);

                            int accountId = JWTHelper.GetAccoutIdFromToken(token);

                            Guid guidSessionId = Guid.Parse(sessionId);

                            //bool checkedSession =
                            //    await _authServiceManager.AxonscmsSessionService.ChackeAxonscmsSessionStatusAsync(
                            //        guidSessionId,
                            //        accountId,
                            //        ipAddress
                            //    );

                            //if (!checkedSession)
                            //{
                            //    throw new PlanWiseSessionFailUnauthorizedException();
                            //}

                            //userProvider =
                            //    await _authServiceManager.AuthenticationService.GetUserProvider(
                            //        accountId
                            //    );

                            context.Items["userProvider"] = userProvider;
                        }
                        else
                        {
                            throw new FailedAuthenticateSessionException();
                        }
                    }
                    else
                    {
                        throw new VerificationTokenNotFoundForCheckSessionException();
                    }
                }

                await _next(context);
            }
            catch (Exception ex)
            {
                //_logger.LogError(
                //    $"Something Went wrong while processing {context.Request.Path}, Details: {ex}"
                //);

                throw;
            }
        }
    }
}
