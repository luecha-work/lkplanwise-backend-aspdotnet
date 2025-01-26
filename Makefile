DB_URL=mssql://sa:secret@localhost:1433/principalsdb
OUTPUT_FOLDER=Models
PROJECT_PATH=PlanWiseBackend/PlanWiseBackend.csproj

SCAFFOLD_CMD = dotnet ef dbcontext scaffold "Server=localhost;Database=principalsdb;User ID=sa;Password=P@ssw0rd123;TrustServerCertificate=True" Microsoft.EntityFrameworkCore.SqlServer -o Models


network:
	docker network create bank-network

postgres:
	docker run --name postgres12 -p 5432:5432 -e POSTGRES_USER=root -e POSTGRES_PASSWORD=P@ssw0rd123 -d postgres

mssql:
	docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=P@ssw0rd123" -p 1433:1433 --name planwise-container --hostname planwise-container -d mcr.microsoft.com/mssql/server:2022-latest

exec-container:
	docker exec -it planwise-container "bash"

createdb:
# 	docker exec -it planwise-container /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P P@ssw0rd123 -Q "CREATE DATABASE principalsdb"
	docker exec -u root -it planwise-container bash -c "apt-get update && apt-get install -y mssql-tools unixodbc-dev && /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P P@ssw0rd123 -Q \"CREATE DATABASE principalsdb\""
creaet_schema:
	docker exec -u root -it planwise-container bash -c "sleep 15 && apt-get update && apt-get install -y mssql-tools unixodbc-dev && /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P P@ssw0rd123 -Q \"CREATE DATABASE principalsdb; USE principalsdb; CREATE SCHEMA public\""

dropdb:
# docker exec -it planwise-container /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P P@ssw0rd123 -Q "DROP DATABASE principalsdb"
	dropdb:
	docker exec -u root -it planwise-container bash -c "apt-get update && apt-get install -y mssql-tools unixodbc-dev && /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P P@ssw0rd123 -Q \"ALTER DATABASE principalsdb SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE principalsdb;\" -N -C"

scaffold:
	cd Entities && \
	$(SCAFFOLD_CMD)

# make new_migrations migrationsName=AddBaseTableForIdentity

new_migrations:
	cd PlanWiseBackend && \
	dotnet ef --project ../Entities migrations add $(migrationsName) --context PlanWiseDbContext

migrations_update:
	cd PlanWiseBackend && \
	dotnet ef database update --context PlanWiseDbContext

server:
	dotnet watch run --project $(PROJECT_PATH)


.PHONY: network postgres new_migration migrations_update mssql createdb dropdb scaffold server