REFRESH MATERIALIZED view ic.vw_ic;

REFRESH MATERIALIZED view seg.vw_organograma;


/*			Tipo 			*/
INSERT INTO corp.tipo (id,nome,grupo,ativo) VALUES
	 (1,'Outros','Tipo',true);

SELECT setval('corp.sqtipo', 1 );


/*			Organograma 			*/
INSERT INTO seg.organograma (id,idpai,nome,ativo,gd) VALUES
	 (1,NULL,'Corp',true,'fe12a60b-fb0d-4967-bd0b-b736d0edbc3e'::uuid);

SELECT setval('seg.sqorganograma', 1 );


/*			IC 			*/
INSERT INTO ic.ic (id,idpai,nome,ativo,propriedades,idtipo,idorganograma) VALUES
	 (1,NULL,'Root',true,null,1,1);

SELECT setval('ic.sqic', 1 );


/*			IC 			*/
INSERT INTO seg.usuario (id,identificacao,gd,senha,administrador,ativo,"local",email,chavetrocasenha,chavevalidade) VALUES
	 (1,'admin','5c355ca3-8e6d-49ed-97a1-3f6eeff85d27'::uuid,'d2160e670a6261bcd5164a5b2164a89e98a34b54ccd59944ccc68fcc725f12ce3746367919d55fa43101e0d2e1f2ff1c1c824499622e0643024f9d77bfebcbd2',true,true,true,'teste@gmail.com',NULL,NULL);

SELECT setval('seg.squsuario', 1 );



/*			Configuracao 			*/
INSERT INTO corp.configuracao (id,idpai,nome,ativo,tipovalor,valornumerico,valortexto,valordata,valorcomplexo,valorsensivel,ajuda,valorboleano) VALUES
	 (1,NULL,'CMDB',true,'grupo',NULL,NULL,NULL,NULL,false,NULL,NULL),
	 (4,3,'AD',true,'grupo',NULL,NULL,NULL,NULL,false,NULL,NULL),
	 (9,4,'Searchbase',true,'texto',NULL,'dc=cmdb,dc=com',NULL,NULL,false,NULL,NULL),
	 (3,1,'Conexão',true,'grupo',NULL,NULL,NULL,NULL,false,NULL,NULL),
	 (12,4,'Pesquisa nome usuário',true,'texto',NULL,'(&(objectClass=person)(uid={0}))',NULL,NULL,false,NULL,NULL),
	 (10,4,'Propriedades',true,'complexo',NULL,NULL,NULL,'{
    "Email":"mail",
    "Descricao":"description",
    "Nome":"Name",
    "SammAccount":"SamAccountName"
}',false,NULL,NULL),
	 (14,1,'Segurança',true,'grupo',NULL,NULL,NULL,NULL,false,NULL,NULL),
	 (15,14,'JWT',true,'grupo',NULL,NULL,NULL,NULL,false,NULL,NULL),
	 (2,14,'chave',true,'texto',NULL,'bcf4a772-a7b9-4ed5-9a99-f30f96cbe452',NULL,NULL,false,NULL,NULL),
	 (8,4,'Senha',true,'texto',NULL,'veXdgbn14GuHsjSJl14gdg==',NULL,NULL,true,NULL,NULL);
INSERT INTO corp.configuracao (id,idpai,nome,ativo,tipovalor,valornumerico,valortexto,valordata,valorcomplexo,valorsensivel,ajuda,valorboleano) VALUES
	 (6,4,'Servidor',true,'texto',NULL,'192.168.0.100',NULL,NULL,false,NULL,NULL),
	 (16,15,'Chave JWT',true,'texto',NULL,'1XYXJgvc9NBI+bVySCl3HLAKl5U4gjPG2saPjwL5bGdm5D0omDVt5geMxMfIGEttl8WTGvw7f73I+2sMYjDoDMZU5+M9WLrEW1EV2nFfFI3PE2AOyBoaobkMgLl7jzfcYqyb5oC6GP1JEyyNa70twA==',NULL,NULL,false,NULL,NULL),
	 (11,4,'Usuário DN',true,'texto',NULL,'uid=john,ou=People,dc=cmdb,dc=com',NULL,NULL,false,NULL,NULL),
	 (18,3,'SMTP',true,'grupo',NULL,NULL,NULL,NULL,false,NULL,NULL),
	 (19,18,'Servidor',true,'texto',NULL,'localhost',NULL,NULL,false,NULL,NULL),
	 (21,18,'Autenticado',true,'boleano',NULL,'22',NULL,NULL,false,NULL,false),
	 (22,18,'Usuario',true,'texto',NULL,NULL,NULL,NULL,false,NULL,NULL),
	 (24,18,'Senha',true,'texto',NULL,NULL,NULL,NULL,true,NULL,NULL),
	 (20,18,'Porta',true,'numerico',25.0000,'22',NULL,NULL,false,NULL,NULL),
	 (23,18,'SSH',true,'boleano',NULL,NULL,NULL,NULL,false,NULL,false);
INSERT INTO corp.configuracao (id,idpai,nome,ativo,tipovalor,valornumerico,valortexto,valordata,valorcomplexo,valorsensivel,ajuda,valorboleano) VALUES
	 (7,4,'Porta',true,'numerico',389.0000,NULL,NULL,NULL,false,NULL,NULL),
	 (17,15,'Duração Horas',true,'numerico',240.0000,NULL,NULL,NULL,false,NULL,NULL),
	 (25,1,'Embedding',true,'grupo',NULL,NULL,NULL,NULL,false,NULL,NULL),
	 (27,25,'Modelo',true,'texto',NULL,'mxbai-embed-large',NULL,NULL,false,NULL,NULL),
	 (26,25,'URL',true,'texto',NULL,'http://localhost:11434',NULL,NULL,false,NULL,NULL),
	 (28,25,'Ativo',true,'boleano',NULL,NULL,NULL,NULL,false,NULL,true);

SELECT setval('corp.configuracao_id_seq', 1 );
