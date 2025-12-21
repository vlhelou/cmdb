# Objetivo

Software para controlar os IC´s de forma hierarquica, com buscas variadas, por árvore, textual e embedding, além de oferecer mais algumas funcionalidades, como armazenamentos de dados sensíveis por grupo de trabalho, associação de conhecimento, e relacionamento de dependência

# Dependências

é necessário um servidor de banco de dados postgres, e como opcional um servidor de embedding, no caso é o **Ollama**, como o modelo utilizado sendo **mxbai-embed-large:latest**



## Criando o banco de dados

1. Criar uma base de dados, normalmente utilizo o nome **cmdb**;
2. Criar um usuário com o nome **usrapp**;
3. Executar o script **./Banco/01\_criacao.sql**;
4. Executar o script **./Banco/02\_carga.sql**;



## configurando a aplicação

É necessário configurar uma variável de ambiente cmdb\_db contendo a string de conexão, pode substituir o caractere "=" por ":"

Executando via container exemplo

`docker run -d --name ct-cmdb -p 4500:8080 --restar always -e "cmdb_db=server:localhost; Port:5432. UserID=usrapp; password:senhadousuario; database:cmdb"  vlhelou/cmdb:latest`

