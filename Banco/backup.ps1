docker exec -u postgres post-cmdb bash -c "pg_dump cmdb > /var/lib/postgresql/cmdb.bak"

docker cp post-cmdb:/var/lib/postgresql/cmdb.bak .