DB_URL=mssql://sa:secret@localhost:1433/principalsdb
OUTPUT_FOLDER=Models
PROJECT_PATH=PlanWiseBackend/PlanWiseBackend.csproj

MSSQL_SCAFFOLD_CMD = dotnet ef dbcontext scaffold "Server=localhost;Database=principalsdb;User ID=sa;Password=P@ssw0rd123;TrustServerCertificate=True" Microsoft.EntityFrameworkCore.SqlServer -o Models
POSTGRES_SCAFFOLD_CMD = dotnet ef dbcontext scaffold "Host=localhost;Database=$(DB_NAME);Username=$(DB_USERNAME);Password=$(DB_PASSWORD)" Npgsql.EntityFrameworkCore.PostgreSQL -o $(OUTPUT_FOLDER)

network:
	docker network create bank-network

postgres:
	docker run --name planwise-postgres-container -p 5432:5432 -e POSTGRES_USER=root -e POSTGRES_PASSWORD=P@ssw0rd123 -d postgres

mssql:
	docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=P@ssw0rd123" -p 1433:1433 --name planwise-mssql-container --hostname planwise-mssql-container -d mcr.microsoft.com/mssql/server:2022-latest

postgres_createdb:
	docker exec -it planwise-postgres-container createdb --username=planwiseroot --owner=planwiseroot planwisedb

postgres_dropdb:
	docker exec -it planwise-postgres-container dropdb planwisedb

mssql_exec_container:
	docker exec -it planwise-container "bash"

mssql_createdb:
# 	docker exec -it planwise-container /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P P@ssw0rd123 -Q "CREATE DATABASE principalsdb"
	docker exec -u root -it planwise-container bash -c "apt-get update && apt-get install -y mssql-tools unixodbc-dev && /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P P@ssw0rd123 -Q \"CREATE DATABASE principalsdb\""
mssql_create_schema:
	docker exec -u root -it planwise-container bash -c "sleep 15 && apt-get update && apt-get install -y mssql-tools unixodbc-dev && /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P P@ssw0rd123 -Q \"CREATE DATABASE principalsdb; USE principalsdb; CREATE SCHEMA public\""

mssql_scaffold:
	cd Entities && \
	$(MSSQL_SCAFFOLD_CMD)

new_migrations:
	cd PlanWiseBackend && \
	dotnet ef --project ../Entities migrations add $(migrationsName) --context PlanWiseDbContext

migrations_update:
	cd PlanWiseBackend && \
	dotnet ef database update --context PlanWiseDbContext

server:
	dotnet watch run --project $(PROJECT_PATH)


.PHONY: network postgres mssql postgres_createdb postgres_dropdb mssql_exec_container mssql_createdb mssql_create_schema mssql_scaffold new_migration migrations_update server