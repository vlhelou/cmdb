docker exec -u postgres ct-cmdb-db bash -c "pg_dump cmdb > /etc/postgresql/cmdb.bak"
docker exec -u postgres ct-cmdb-db bash -c "pg_dump -s cmdb > /etc/postgresql/01_criacao.sql"

docker cp ct-cmdb-db:/var/etc/postgresql/cmdb.bak .
docker cp ct-cmdb-db:/var/etc/postgresql/01_criacao.sql .

Get-Content -Path .\header.txt, .\01_criacao.sql, .\02_carga.sql | Set-Content -Path .\docker\01-inicial.sql


