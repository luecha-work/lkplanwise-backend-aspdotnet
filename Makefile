DB_URL=mssql://sa:secret@localhost:1433/planwisedb
OUTPUT_FOLDER=Models
PROJECT_PATH=LKPlanWiseBackend/LKPlanWiseBackend.csproj

MSSQL_SCAFFOLD_CMD = dotnet ef dbcontext scaffold "Server=localhost;Database=planwisedb;User ID=sa;Password=P@ssw0rd123;TrustServerCertificate=True" Microsoft.EntityFrameworkCore.SqlServer -o $(OUTPUT_FOLDER)
POSTGRES_SCAFFOLD_CMD = dotnet ef dbcontext scaffold "Host=localhost;Database=planwisedb;Username=planwiseroot;Password=P@ssw0rd123" Npgsql.EntityFrameworkCore.PostgreSQL -o $(OUTPUT_FOLDER)

network:
	docker network create bank-network

postgres:
	docker run --name planwise-pg-container -p 5432:5432 -e POSTGRES_USER=planwiseroot -e POSTGRES_PASSWORD=P@ssw0rd123 -d postgres

mssql:
	docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=P@ssw0rd123" -p 1433:1433 --name planwise-ms-container --hostname planwise-ms-container -d mcr.microsoft.com/mssql/server:2022-latest

postgres_createdb:
	docker exec -it planwise-pg-container createdb --username=planwiseroot --owner=planwiseroot planwisedb

mssql_exec_container:
	docker exec -it planwise-container "bash"

mssql_createdb:
	docker exec -u root -it planwise-ms-container bash -c "apt-get update && apt-get install -y mssql-tools unixodbc-dev && /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P P@ssw0rd123 -Q \"CREATE DATABASE planwisedb\""

mssql_create_schema:
	docker exec -u root -it planwise-ms-container bash -c "sleep 15 && apt-get update && apt-get install -y mssql-tools unixodbc-dev && /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P P@ssw0rd123 -Q \"CREATE DATABASE planwisedb; USE planwisedb; CREATE SCHEMA public\""

postgres_scaffold:
	cd Entities && \
	$(POSTGRES_SCAFFOLD_CMD)

mssql_scaffold:
	cd Entities && \
	$(MSSQL_SCAFFOLD_CMD)

new_migrations:
	cd LKPlanWiseBackend && \
	dotnet ef --project ../Entities migrations add $(migrationsName) --context LKPlanWiseDbContext

migrations_update:
	cd LKPlanWiseBackend && \
	dotnet ef database update --context LKPlanWiseDbContext

server:
	dotnet watch run --project $(PROJECT_PATH)

.PHONY: network postgres mssql postgres_createdb mssql_exec_container mssql_createdb mssql_create_schema postgres_scaffold mssql_scaffold new_migration migrations_update server