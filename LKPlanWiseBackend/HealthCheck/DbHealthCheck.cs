﻿using Microsoft.Extensions.Diagnostics.HealthChecks;
using Npgsql;

namespace PlanWiseBackend.HealthCheck
{
    public class DbHealthCheck : IHealthCheck
    {
        private readonly IConfiguration _configuration;

        public DbHealthCheck(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default
        )
        {
            try
            {
                string connectionString = _configuration.GetConnectionString("DefaultConnection");

                using (NpgsqlConnection sqlConnection = new NpgsqlConnection(connectionString))
                {
                    if (sqlConnection.State != System.Data.ConnectionState.Open)
                        sqlConnection.Open();

                    if (sqlConnection.State == System.Data.ConnectionState.Open)
                    {
                        sqlConnection.Close();
                        return Task.FromResult(
                            HealthCheckResult.Healthy("The database is up and running.")
                        );
                    }
                }

                return Task.FromResult(
                    new HealthCheckResult(
                        context.Registration.FailureStatus,
                        "The database is down."
                    )
                );
            }
            catch (Exception)
            {
                return Task.FromResult(
                    new HealthCheckResult(
                        context.Registration.FailureStatus,
                        "The database is down."
                    )
                );
            }
        }
    }
}
