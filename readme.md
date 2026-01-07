# Objetivo



Software para controlar os IC´s de forma hierarquica, com buscas variadas, por árvore, textual e embedding, além de oferecer mais algumas funcionalidades, como armazenamentos de dados sensíveis por grupo de trabalho, associação de conhecimento, e relacionamento de dependência



# Dependências



é necessário um servidor de banco de dados postgres, e como opcional um servidor de embedding, no caso é o **Ollama**, como o modelo utilizado sendo **mxbai-embed-large:latest**





## Criando o banco de dados
Observação: quando o bando de dados é criado, tem o usuário **admin**, com com a senha **123456**
é recomendado que no primeiro acesso já coloque uma senha forte.

1. Criar uma base de dados, normalmente utilizo o nome **cmdb**;
2. Criar um usuário com o nome **usrapp**;
   2.1. **Coloque uma senha forte para o usuário;**
3. Executar o script **./Banco/01\_criacao.sql**;
4. Executar o script **./Banco/02\_carga.sql**;



## Instação via docker

É necessário configurar uma variável de ambiente cmdb\_db contendo a string de conexão, pode substituir o caractere "=" por ":"

Executando via container exemplo

`docker run -d --name ct-cmdb -p 4500:8080 --restar always -e "cmdb\\\_db=server:localhost; Port:5432. UserID=usrapp; password:senhadousuario; database:cmdb" vlhelou/cmdb:latest`



## Instalação do banco separado

##### Atenção para o banco de dados é necessário ter a extensão [pgVector](https://github.com/pgvector/pgvector)

Usar o arquivo (Banco/docker/scripts/inicio.sql)

e executar o comando

    psql -f inicio.sql

atenção para as partes abaixo desse script:

     CREATE database cmdb; /\*escolher o nome da base desejada\*/
     
     create user usrapp with password 'usrapp'; /\\\*escolher o usuário e a senha desejados, lembrar de ajustar esses valores no docker run, ou no docker compose */

## Instalação via docker compose
no diretório `Compose\cmdb` executar:

    git clone https://github.com/vlhelou/cmdb 
    cd .\cmdb\Compose\cmdb\
    docker compose up -d




