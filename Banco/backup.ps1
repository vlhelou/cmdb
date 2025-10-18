docker exec -u postgres ct-cmdb-db bash -c "pg_dump cmdb > /var/lib/postgresql/cmdb.bak"
docker exec -u postgres ct-cmdb-db bash -c "pg_dump -s cmdb > /var/lib/postgresql/01_criacao.sql"

docker cp ct-cmdb-db:/var/lib/postgresql/cmdb.bak .
docker cp ct-cmdb-db:/var/lib/postgresql/01_criacao.sql .

Get-Content -Path .\header.txt, .\cmdb.bak | Set-Content -Path .\01-inicial.sql