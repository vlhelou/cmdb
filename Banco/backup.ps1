docker exec -u postgres cmdbdb bash -c "pg_dump cmdb > /var/lib/postgresql/cmdb.bak"

docker cp cmdbdb:/var/lib/postgresql/cmdb.bak .

Get-Content -Path .\header.txt, .\cmdb.bak | Set-Content -Path .\01-inicial.sql