FROM mcr.microsoft.com/mssql/server:2022-latest

USER root

# Copy SQL scripts to the container
COPY ./init-db.sql /scripts/init-db.sql

# Grant execute permissions for the scripts
RUN chmod +x /scripts/*

USER mssql

# Run the SQL script after SQL Server starts
CMD /bin/bash -c "/opt/mssql/bin/sqlservr & sleep 30 && /opt/mssql-tools18/bin/sqlcmd -S 127.0.0.1 -U sa -P Strong@Passw0rd -d master -i /scripts/init-db.sql -C && wait"
HEALTHCHECK --interval=10s --timeout=5s --start-period=20s --retries=3 CMD /opt/mssql-tools18/bin/sqlcmd -S 127.0.0.1 -U sa -P Strong@Passw0rd -d master -Q "SELECT 1" -C