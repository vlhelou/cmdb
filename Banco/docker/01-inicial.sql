CREATE database cmdb;

create user usrapp with password 'usrapp';

\connect cmdb;
--
-- PostgreSQL database dump
--

-- Dumped from database version 17.0 (Debian 17.0-1.pgdg120+1)
-- Dumped by pg_dump version 17.0 (Debian 17.0-1.pgdg120+1)

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET transaction_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

--
-- Name: chamado; Type: SCHEMA; Schema: -; Owner: postgres
--

CREATE SCHEMA chamado;


ALTER SCHEMA chamado OWNER TO postgres;

--
-- Name: corp; Type: SCHEMA; Schema: -; Owner: postgres
--

CREATE SCHEMA corp;


ALTER SCHEMA corp OWNER TO postgres;

--
-- Name: ic; Type: SCHEMA; Schema: -; Owner: postgres
--

CREATE SCHEMA ic;


ALTER SCHEMA ic OWNER TO postgres;

--
-- Name: mudanca; Type: SCHEMA; Schema: -; Owner: postgres
--

CREATE SCHEMA mudanca;


ALTER SCHEMA mudanca OWNER TO postgres;

--
-- Name: seg; Type: SCHEMA; Schema: -; Owner: postgres
--

CREATE SCHEMA seg;


ALTER SCHEMA seg OWNER TO postgres;

--
-- Name: servico; Type: SCHEMA; Schema: -; Owner: postgres
--

CREATE SCHEMA servico;


ALTER SCHEMA servico OWNER TO postgres;

--
-- Name: citext; Type: EXTENSION; Schema: -; Owner: -
--

CREATE EXTENSION IF NOT EXISTS citext WITH SCHEMA public;


--
-- Name: EXTENSION citext; Type: COMMENT; Schema: -; Owner: 
--

COMMENT ON EXTENSION citext IS 'data type for case-insensitive character strings';


--
-- Name: sla_calcula(timestamp without time zone, interval); Type: FUNCTION; Schema: corp; Owner: postgres
--

CREATE FUNCTION corp.sla_calcula(inicio timestamp without time zone, sla interval) RETURNS timestamp without time zone
    LANGUAGE plpgsql
    AS $$
DECLARE dia DATE;
DECLARE hrref TIME;
DECLARE saldo INTERVAL;
DECLARE expediente corp.expediente%ROWTYPE;
BEGIN
    dia = inicio::DATE;
    hrref = inicio::TIME;
    saldo=sla;
    RAISE NOTICE 'dia %, href %, saldo %, dia semana %',dia,hrref, saldo, EXTRACT(dow from dia);

    select into expediente
        id, data, turno1hrinicio, turno1hrtermino, turno2hrinicio, turno2hrtermino from corp.expediente where data=dia;    

    if expediente is NULL THEN
        select into expediente
            id, data, turno1hrinicio, turno1hrtermino, turno2hrinicio, turno2hrtermino 
        from corp.expediente 
            where 
                EXTRACT(dow from data)=EXTRACT(dow from dia)
                and EXTRACT(year from data)=1900;    
    end if;
    -- RAISE NOTICE 'id do expediente %', expediente.id;
    if (expediente.turnohr1termino is not null AND expediente.turnohr1termino>hrref) then
        raise NOTICE 'tem no primeiro turno';
    end if;

    if (expediente.turnohr2termino is not null AND expediente.turnohr2termino>hrref) then
        raise NOTICE 'tem no primeiro turno';
    end if;

    RETURN dia;

END;
$$;


ALTER FUNCTION corp.sla_calcula(inicio timestamp without time zone, sla interval) OWNER TO postgres;

--
-- Name: sla_subtraisaldo(interval, time without time zone, time without time zone, time without time zone); Type: PROCEDURE; Schema: corp; Owner: postgres
--

CREATE PROCEDURE corp.sla_subtraisaldo(INOUT saldo interval, INOUT ponto time without time zone, IN turnoinicio time without time zone, IN turnotermino time without time zone)
    LANGUAGE plpgsql
    AS $$
DECLARE hrinicio TIME;
DECLARE utilizado INTERVAL;
BEGIN
    hrinicio= GREATEST(ponto, turnoinicio);
    if (hrinicio+saldo)>turnotermino THEN
        utilizado = turnotermino-hrinicio;
    else 
        utilizado = saldo;
    end if;
    saldo=saldo-utilizado;
    ponto=hrinicio+utilizado;
end;
$$;


ALTER PROCEDURE corp.sla_subtraisaldo(INOUT saldo interval, INOUT ponto time without time zone, IN turnoinicio time without time zone, IN turnotermino time without time zone) OWNER TO postgres;

--
-- Name: fn_vwicupdate(); Type: FUNCTION; Schema: ic; Owner: postgres
--

CREATE FUNCTION ic.fn_vwicupdate() RETURNS trigger
    LANGUAGE plpgsql SECURITY DEFINER
    AS $$

begin

	REFRESH MATERIALIZED view CONCURRENTLY  ic.vw_ic;

	return null;

end;

$$;


ALTER FUNCTION ic.fn_vwicupdate() OWNER TO postgres;

--
-- Name: fn_vworganogramaupdate(); Type: FUNCTION; Schema: seg; Owner: postgres
--

CREATE FUNCTION seg.fn_vworganogramaupdate() RETURNS trigger
    LANGUAGE plpgsql SECURITY DEFINER
    AS $$

begin

	REFRESH MATERIALIZED view CONCURRENTLY  seg.vw_organograma;

	return null;

end;

$$;


ALTER FUNCTION seg.fn_vworganogramaupdate() OWNER TO postgres;

SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- Name: chamado; Type: TABLE; Schema: chamado; Owner: postgres
--

CREATE TABLE chamado.chamado (
    id integer NOT NULL,
    idautor integer NOT NULL,
    idsolicitante integer NOT NULL,
    datacriacao timestamp without time zone NOT NULL,
    slaprevisto timestamp without time zone NOT NULL,
    idservicoandamento integer,
    idsituacao integer NOT NULL,
    idorganograma integer NOT NULL
);


ALTER TABLE chamado.chamado OWNER TO postgres;

--
-- Name: chamado_id_seq; Type: SEQUENCE; Schema: chamado; Owner: postgres
--

CREATE SEQUENCE chamado.chamado_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE chamado.chamado_id_seq OWNER TO postgres;

--
-- Name: chamado_id_seq; Type: SEQUENCE OWNED BY; Schema: chamado; Owner: postgres
--

ALTER SEQUENCE chamado.chamado_id_seq OWNED BY chamado.chamado.id;


--
-- Name: servico; Type: TABLE; Schema: chamado; Owner: postgres
--

CREATE TABLE chamado.servico (
    id integer NOT NULL,
    datacriacao timestamp without time zone NOT NULL,
    idchamado integer NOT NULL,
    idservico integer NOT NULL,
    idatendente integer,
    slaprevisto timestamp without time zone NOT NULL,
    respostaformulario json,
    idsituacao integer NOT NULL,
    slasaldo time without time zone NOT NULL,
    slaoriginal time without time zone NOT NULL
);


ALTER TABLE chamado.servico OWNER TO postgres;

--
-- Name: chamadoservico_id_seq; Type: SEQUENCE; Schema: chamado; Owner: postgres
--

CREATE SEQUENCE chamado.chamadoservico_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE chamado.chamadoservico_id_seq OWNER TO postgres;

--
-- Name: chamadoservico_id_seq; Type: SEQUENCE OWNED BY; Schema: chamado; Owner: postgres
--

ALTER SEQUENCE chamado.chamadoservico_id_seq OWNED BY chamado.servico.id;


--
-- Name: servicoic; Type: TABLE; Schema: chamado; Owner: postgres
--

CREATE TABLE chamado.servicoic (
    id integer NOT NULL,
    idservico integer NOT NULL,
    idic integer NOT NULL
);


ALTER TABLE chamado.servicoic OWNER TO postgres;

--
-- Name: servicoic_id_seq; Type: SEQUENCE; Schema: chamado; Owner: postgres
--

CREATE SEQUENCE chamado.servicoic_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE chamado.servicoic_id_seq OWNER TO postgres;

--
-- Name: servicoic_id_seq; Type: SEQUENCE OWNED BY; Schema: chamado; Owner: postgres
--

ALTER SEQUENCE chamado.servicoic_id_seq OWNED BY chamado.servicoic.id;


--
-- Name: servicoinformacao; Type: TABLE; Schema: chamado; Owner: postgres
--

CREATE TABLE chamado.servicoinformacao (
    id integer NOT NULL,
    idservico integer NOT NULL,
    data timestamp without time zone NOT NULL,
    idautor integer NOT NULL,
    informacao character varying(500),
    idarquivo uuid
);


ALTER TABLE chamado.servicoinformacao OWNER TO postgres;

--
-- Name: servicoinformacao_id_seq; Type: SEQUENCE; Schema: chamado; Owner: postgres
--

CREATE SEQUENCE chamado.servicoinformacao_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE chamado.servicoinformacao_id_seq OWNER TO postgres;

--
-- Name: servicoinformacao_id_seq; Type: SEQUENCE OWNED BY; Schema: chamado; Owner: postgres
--

ALTER SEQUENCE chamado.servicoinformacao_id_seq OWNED BY chamado.servicoinformacao.id;


--
-- Name: situacao; Type: TABLE; Schema: chamado; Owner: postgres
--

CREATE TABLE chamado.situacao (
    id integer NOT NULL,
    nome character varying(50) NOT NULL,
    contasla boolean NOT NULL
);


ALTER TABLE chamado.situacao OWNER TO postgres;

--
-- Name: sqchamado; Type: SEQUENCE; Schema: chamado; Owner: postgres
--

CREATE SEQUENCE chamado.sqchamado
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE chamado.sqchamado OWNER TO postgres;

--
-- Name: arquivo; Type: TABLE; Schema: corp; Owner: postgres
--

CREATE TABLE corp.arquivo (
    id uuid NOT NULL,
    nome character varying(200) NOT NULL,
    tipo character varying(300) NOT NULL,
    conteudo bytea NOT NULL
);


ALTER TABLE corp.arquivo OWNER TO postgres;

--
-- Name: expediente; Type: TABLE; Schema: corp; Owner: postgres
--

CREATE TABLE corp.expediente (
    id integer NOT NULL,
    data date NOT NULL,
    turno1hrinicio time without time zone,
    turno1hrtermino time without time zone,
    turno2hrinicio time without time zone,
    turno2hrtermino time without time zone,
    numerodiasemana integer GENERATED ALWAYS AS (date_part('dow'::text, data)) STORED NOT NULL
);


ALTER TABLE corp.expediente OWNER TO postgres;

--
-- Name: expediente_id_seq; Type: SEQUENCE; Schema: corp; Owner: postgres
--

CREATE SEQUENCE corp.expediente_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE corp.expediente_id_seq OWNER TO postgres;

--
-- Name: expediente_id_seq; Type: SEQUENCE OWNED BY; Schema: corp; Owner: postgres
--

ALTER SEQUENCE corp.expediente_id_seq OWNED BY corp.expediente.id;


--
-- Name: parametro; Type: TABLE; Schema: corp; Owner: postgres
--

CREATE TABLE corp.parametro (
    id integer NOT NULL,
    nome character varying(100),
    valor character varying(1000)
);


ALTER TABLE corp.parametro OWNER TO postgres;

--
-- Name: parametro_id_seq; Type: SEQUENCE; Schema: corp; Owner: postgres
--

CREATE SEQUENCE corp.parametro_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE corp.parametro_id_seq OWNER TO postgres;

--
-- Name: parametro_id_seq; Type: SEQUENCE OWNED BY; Schema: corp; Owner: postgres
--

ALTER SEQUENCE corp.parametro_id_seq OWNED BY corp.parametro.id;


--
-- Name: sqtipo; Type: SEQUENCE; Schema: corp; Owner: postgres
--

CREATE SEQUENCE corp.sqtipo
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE corp.sqtipo OWNER TO postgres;

--
-- Name: tipo; Type: TABLE; Schema: corp; Owner: postgres
--

CREATE TABLE corp.tipo (
    id integer DEFAULT nextval('corp.sqtipo'::regclass) NOT NULL,
    nome character varying(100) NOT NULL,
    grupo public.citext NOT NULL,
    ativo boolean DEFAULT true NOT NULL
);


ALTER TABLE corp.tipo OWNER TO postgres;

--
-- Name: vw_arquivo; Type: VIEW; Schema: corp; Owner: postgres
--

CREATE VIEW corp.vw_arquivo AS
 SELECT id,
    nome,
    tipo
   FROM corp.arquivo;


ALTER VIEW corp.vw_arquivo OWNER TO postgres;

--
-- Name: sqconhecimento; Type: SEQUENCE; Schema: ic; Owner: postgres
--

CREATE SEQUENCE ic.sqconhecimento
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE ic.sqconhecimento OWNER TO postgres;

--
-- Name: conhecimento; Type: TABLE; Schema: ic; Owner: postgres
--

CREATE TABLE ic.conhecimento (
    id integer DEFAULT nextval('ic.sqconhecimento'::regclass) NOT NULL,
    idic integer NOT NULL,
    problema character varying(400) NOT NULL,
    solucao character varying(4000),
    idautor integer NOT NULL,
    dataalteracao timestamp without time zone DEFAULT now() NOT NULL
);


ALTER TABLE ic.conhecimento OWNER TO postgres;

--
-- Name: sqdependencia; Type: SEQUENCE; Schema: ic; Owner: postgres
--

CREATE SEQUENCE ic.sqdependencia
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE ic.sqdependencia OWNER TO postgres;

--
-- Name: dependencia; Type: TABLE; Schema: ic; Owner: postgres
--

CREATE TABLE ic.dependencia (
    id integer DEFAULT nextval('ic.sqdependencia'::regclass) NOT NULL,
    idicprincipal integer NOT NULL,
    idicdependente integer NOT NULL,
    idautor integer NOT NULL,
    dataalteracao timestamp with time zone NOT NULL
);


ALTER TABLE ic.dependencia OWNER TO postgres;

--
-- Name: sqic; Type: SEQUENCE; Schema: ic; Owner: postgres
--

CREATE SEQUENCE ic.sqic
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE ic.sqic OWNER TO postgres;

--
-- Name: ic; Type: TABLE; Schema: ic; Owner: postgres
--

CREATE TABLE ic.ic (
    id integer DEFAULT nextval('ic.sqic'::regclass) NOT NULL,
    idpai integer,
    nome character varying(100) NOT NULL,
    ativo boolean NOT NULL,
    propriedades json,
    idtipo integer NOT NULL,
    idorganograma integer
);


ALTER TABLE ic.ic OWNER TO postgres;

--
-- Name: sqincidente; Type: SEQUENCE; Schema: ic; Owner: postgres
--

CREATE SEQUENCE ic.sqincidente
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE ic.sqincidente OWNER TO postgres;

--
-- Name: incidente; Type: TABLE; Schema: ic; Owner: postgres
--

CREATE TABLE ic.incidente (
    id integer DEFAULT nextval('ic.sqincidente'::regclass) NOT NULL,
    inicio timestamp without time zone NOT NULL,
    termino timestamp without time zone,
    causa character varying(1000),
    solucao character varying(1000),
    sintoma character varying(1000) NOT NULL
);


ALTER TABLE ic.incidente OWNER TO postgres;

--
-- Name: sqincidenteic; Type: SEQUENCE; Schema: ic; Owner: postgres
--

CREATE SEQUENCE ic.sqincidenteic
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE ic.sqincidenteic OWNER TO postgres;

--
-- Name: incidenteic; Type: TABLE; Schema: ic; Owner: postgres
--

CREATE TABLE ic.incidenteic (
    id integer DEFAULT nextval('ic.sqincidenteic'::regclass) NOT NULL,
    idincidente integer NOT NULL,
    idic integer NOT NULL
);


ALTER TABLE ic.incidenteic OWNER TO postgres;

--
-- Name: incidentemensagem; Type: TABLE; Schema: ic; Owner: postgres
--

CREATE TABLE ic.incidentemensagem (
    id integer NOT NULL,
    idincidente integer NOT NULL,
    idautor integer NOT NULL,
    data timestamp without time zone NOT NULL,
    mensagem character varying(200) NOT NULL
);


ALTER TABLE ic.incidentemensagem OWNER TO postgres;

--
-- Name: incidentemensagem_id_seq; Type: SEQUENCE; Schema: ic; Owner: postgres
--

CREATE SEQUENCE ic.incidentemensagem_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE ic.incidentemensagem_id_seq OWNER TO postgres;

--
-- Name: incidentemensagem_id_seq; Type: SEQUENCE OWNED BY; Schema: ic; Owner: postgres
--

ALTER SEQUENCE ic.incidentemensagem_id_seq OWNED BY ic.incidentemensagem.id;


--
-- Name: incidenteorganograma_id_seq; Type: SEQUENCE; Schema: ic; Owner: postgres
--

CREATE SEQUENCE ic.incidenteorganograma_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE ic.incidenteorganograma_id_seq OWNER TO postgres;

--
-- Name: incidenteorganograma; Type: TABLE; Schema: ic; Owner: postgres
--

CREATE TABLE ic.incidenteorganograma (
    id integer DEFAULT nextval('ic.incidenteorganograma_id_seq'::regclass) NOT NULL,
    idincidente integer NOT NULL,
    idorganograma integer NOT NULL
);


ALTER TABLE ic.incidenteorganograma OWNER TO postgres;

--
-- Name: segredo; Type: TABLE; Schema: ic; Owner: postgres
--

CREATE TABLE ic.segredo (
    id integer NOT NULL,
    idic integer NOT NULL,
    conteudo character varying(1000),
    idusuariodono integer,
    idorganogramadono integer,
    algoritmo character varying(100) NOT NULL
);


ALTER TABLE ic.segredo OWNER TO postgres;

--
-- Name: segredo_id_seq; Type: SEQUENCE; Schema: ic; Owner: postgres
--

CREATE SEQUENCE ic.segredo_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE ic.segredo_id_seq OWNER TO postgres;

--
-- Name: segredo_id_seq; Type: SEQUENCE OWNED BY; Schema: ic; Owner: postgres
--

ALTER SEQUENCE ic.segredo_id_seq OWNED BY ic.segredo.id;


--
-- Name: sqincidenteacao; Type: SEQUENCE; Schema: ic; Owner: postgres
--

CREATE SEQUENCE ic.sqincidenteacao
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE ic.sqincidenteacao OWNER TO postgres;

--
-- Name: sqsegredo; Type: SEQUENCE; Schema: ic; Owner: postgres
--

CREATE SEQUENCE ic.sqsegredo
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE ic.sqsegredo OWNER TO postgres;

--
-- Name: vw_ic; Type: MATERIALIZED VIEW; Schema: ic; Owner: postgres
--

CREATE MATERIALIZED VIEW ic.vw_ic AS
 WITH RECURSIVE herdeiros AS (
         SELECT ic.id,
            ic.idpai,
            ic.nome,
            ic.ativo,
            ic.ativo AS ativofinal,
            ic.propriedades,
            ic.idtipo,
            ic.idorganograma,
            concat('.', ic.nome) AS nomecompleto,
            ''::text AS listaancestrais,
            to_tsvector('portuguese'::regconfig, concat('.', ic.nome)) AS pesquisats,
            0 AS nivel
           FROM ic.ic
          WHERE (ic.idpai IS NULL)
        UNION ALL
         SELECT filho.id,
            filho.idpai,
            filho.nome,
            filho.ativo,
            (filho.ativo AND a.ativo) AS ativofinal,
            filho.propriedades,
            filho.idtipo,
            COALESCE(filho.idorganograma, a.idorganograma) AS idorganograma,
            concat(a.nomecompleto, '.', filho.nome) AS nomecompleto,
            concat(a.listaancestrais, ',', a.id) AS listaancestrais,
            to_tsvector('portuguese'::regconfig, concat(a.nomecompleto, '.', filho.nome)) AS pesquisats,
            (a.nivel + 1)
           FROM ic.ic filho,
            herdeiros a
          WHERE ((filho.idpai = a.id) AND (filho.id IS NOT NULL))
        )
 SELECT id,
    idpai,
    nome,
    ativo,
    ativofinal,
    propriedades,
    idtipo,
    idorganograma,
    substr(nomecompleto, 2) AS nomecompleto,
    listaancestrais,
    pesquisats,
    nivel
   FROM herdeiros
  WITH NO DATA;


ALTER MATERIALIZED VIEW ic.vw_ic OWNER TO postgres;

--
-- Name: etapaarquivo; Type: TABLE; Schema: mudanca; Owner: postgres
--

CREATE TABLE mudanca.etapaarquivo (
    id integer NOT NULL
);


ALTER TABLE mudanca.etapaarquivo OWNER TO postgres;

--
-- Name: etapaarquivo_id_seq; Type: SEQUENCE; Schema: mudanca; Owner: postgres
--

CREATE SEQUENCE mudanca.etapaarquivo_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE mudanca.etapaarquivo_id_seq OWNER TO postgres;

--
-- Name: etapaarquivo_id_seq; Type: SEQUENCE OWNED BY; Schema: mudanca; Owner: postgres
--

ALTER SEQUENCE mudanca.etapaarquivo_id_seq OWNED BY mudanca.etapaarquivo.id;


--
-- Name: mudanca; Type: TABLE; Schema: mudanca; Owner: postgres
--

CREATE TABLE mudanca.mudanca (
    id integer NOT NULL,
    datacriacao time without time zone NOT NULL,
    inicioprevisto time without time zone,
    idsituacao integer NOT NULL
);


ALTER TABLE mudanca.mudanca OWNER TO postgres;

--
-- Name: mudanca_id_seq; Type: SEQUENCE; Schema: mudanca; Owner: postgres
--

CREATE SEQUENCE mudanca.mudanca_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE mudanca.mudanca_id_seq OWNER TO postgres;

--
-- Name: mudanca_id_seq; Type: SEQUENCE OWNED BY; Schema: mudanca; Owner: postgres
--

ALTER SEQUENCE mudanca.mudanca_id_seq OWNED BY mudanca.mudanca.id;


--
-- Name: sqequipe; Type: SEQUENCE; Schema: seg; Owner: postgres
--

CREATE SEQUENCE seg.sqequipe
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE seg.sqequipe OWNER TO postgres;

--
-- Name: equipe; Type: TABLE; Schema: seg; Owner: postgres
--

CREATE TABLE seg.equipe (
    id integer DEFAULT nextval('seg.sqequipe'::regclass) NOT NULL,
    idusuario integer NOT NULL,
    idorganograma integer NOT NULL,
    idautor integer NOT NULL,
    data timestamp without time zone DEFAULT now()
);


ALTER TABLE seg.equipe OWNER TO postgres;

--
-- Name: sqorganograma; Type: SEQUENCE; Schema: seg; Owner: postgres
--

CREATE SEQUENCE seg.sqorganograma
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE seg.sqorganograma OWNER TO postgres;

--
-- Name: organograma; Type: TABLE; Schema: seg; Owner: postgres
--

CREATE TABLE seg.organograma (
    id integer DEFAULT nextval('seg.sqorganograma'::regclass) NOT NULL,
    idpai integer,
    nome character varying(100) NOT NULL,
    ativo boolean DEFAULT true NOT NULL,
    gd uuid
);


ALTER TABLE seg.organograma OWNER TO postgres;

--
-- Name: squsuario; Type: SEQUENCE; Schema: seg; Owner: postgres
--

CREATE SEQUENCE seg.squsuario
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE seg.squsuario OWNER TO postgres;

--
-- Name: usuario; Type: TABLE; Schema: seg; Owner: postgres
--

CREATE TABLE seg.usuario (
    id integer DEFAULT nextval('seg.squsuario'::regclass) NOT NULL,
    nome character varying(100) NOT NULL,
    senha character varying(128) NOT NULL,
    email character varying(200) NOT NULL,
    administrador boolean DEFAULT false NOT NULL,
    ativo boolean DEFAULT true NOT NULL,
    gd uuid
);


ALTER TABLE seg.usuario OWNER TO postgres;

--
-- Name: vw_organograma; Type: MATERIALIZED VIEW; Schema: seg; Owner: postgres
--

CREATE MATERIALIZED VIEW seg.vw_organograma AS
 WITH RECURSIVE herdeiros AS (
         SELECT organograma.id,
            organograma.idpai,
            organograma.nome,
            organograma.ativo,
            organograma.ativo AS ativofinal,
            concat('|', organograma.nome) AS nomecompleto,
            ''::text AS listaancestrais,
            0 AS nivel
           FROM seg.organograma
          WHERE (organograma.idpai IS NULL)
        UNION ALL
         SELECT org.id,
            org.idpai,
            org.nome,
            org.ativo,
            (org.ativo AND a.ativo) AS ativofinal,
            concat(a.nomecompleto, '|', org.nome) AS nomecompleto,
            concat(a.listaancestrais, ',', a.id) AS listaancestrais,
            (a.nivel + 1)
           FROM seg.organograma org,
            herdeiros a
          WHERE ((org.idpai = a.id) AND (org.id IS NOT NULL))
        )
 SELECT id,
    idpai,
    nome,
    ativo,
    ativofinal,
    nomecompleto,
    listaancestrais,
    nivel
   FROM herdeiros
  WITH NO DATA;


ALTER MATERIALIZED VIEW seg.vw_organograma OWNER TO postgres;

--
-- Name: sqservico; Type: SEQUENCE; Schema: servico; Owner: postgres
--

CREATE SEQUENCE servico.sqservico
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE servico.sqservico OWNER TO postgres;

--
-- Name: servico; Type: TABLE; Schema: servico; Owner: postgres
--

CREATE TABLE servico.servico (
    id integer DEFAULT nextval('servico.sqservico'::regclass) NOT NULL,
    idorganograma integer NOT NULL,
    nome character varying(100) NOT NULL,
    descricao character varying(1000),
    formulario json,
    ativo boolean NOT NULL,
    sla interval NOT NULL,
    orientacao character varying(1000),
    tempoestimadoexecucao interval NOT NULL
);


ALTER TABLE servico.servico OWNER TO postgres;

--
-- Name: sqservicoarquivos; Type: SEQUENCE; Schema: servico; Owner: postgres
--

CREATE SEQUENCE servico.sqservicoarquivos
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE servico.sqservicoarquivos OWNER TO postgres;

--
-- Name: servicoarquivos; Type: TABLE; Schema: servico; Owner: postgres
--

CREATE TABLE servico.servicoarquivos (
    id integer DEFAULT nextval('servico.sqservicoarquivos'::regclass) NOT NULL,
    idservico integer NOT NULL,
    idarquivo uuid NOT NULL,
    descricao character varying(1000)
);


ALTER TABLE servico.servicoarquivos OWNER TO postgres;

--
-- Name: sqservicoativos; Type: SEQUENCE; Schema: servico; Owner: postgres
--

CREATE SEQUENCE servico.sqservicoativos
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE servico.sqservicoativos OWNER TO postgres;

--
-- Name: servicoativos; Type: TABLE; Schema: servico; Owner: postgres
--

CREATE TABLE servico.servicoativos (
    id integer DEFAULT nextval('servico.sqservicoativos'::regclass) NOT NULL,
    idservico integer NOT NULL,
    idic integer NOT NULL
);


ALTER TABLE servico.servicoativos OWNER TO postgres;

--
-- Name: sqchamado; Type: SEQUENCE; Schema: servico; Owner: postgres
--

CREATE SEQUENCE servico.sqchamado
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE servico.sqchamado OWNER TO postgres;

--
-- Name: chamado id; Type: DEFAULT; Schema: chamado; Owner: postgres
--

ALTER TABLE ONLY chamado.chamado ALTER COLUMN id SET DEFAULT nextval('chamado.chamado_id_seq'::regclass);


--
-- Name: servico id; Type: DEFAULT; Schema: chamado; Owner: postgres
--

ALTER TABLE ONLY chamado.servico ALTER COLUMN id SET DEFAULT nextval('chamado.chamadoservico_id_seq'::regclass);


--
-- Name: servicoic id; Type: DEFAULT; Schema: chamado; Owner: postgres
--

ALTER TABLE ONLY chamado.servicoic ALTER COLUMN id SET DEFAULT nextval('chamado.servicoic_id_seq'::regclass);


--
-- Name: servicoinformacao id; Type: DEFAULT; Schema: chamado; Owner: postgres
--

ALTER TABLE ONLY chamado.servicoinformacao ALTER COLUMN id SET DEFAULT nextval('chamado.servicoinformacao_id_seq'::regclass);


--
-- Name: expediente id; Type: DEFAULT; Schema: corp; Owner: postgres
--

ALTER TABLE ONLY corp.expediente ALTER COLUMN id SET DEFAULT nextval('corp.expediente_id_seq'::regclass);


--
-- Name: parametro id; Type: DEFAULT; Schema: corp; Owner: postgres
--

ALTER TABLE ONLY corp.parametro ALTER COLUMN id SET DEFAULT nextval('corp.parametro_id_seq'::regclass);


--
-- Name: incidentemensagem id; Type: DEFAULT; Schema: ic; Owner: postgres
--

ALTER TABLE ONLY ic.incidentemensagem ALTER COLUMN id SET DEFAULT nextval('ic.incidentemensagem_id_seq'::regclass);


--
-- Name: segredo id; Type: DEFAULT; Schema: ic; Owner: postgres
--

ALTER TABLE ONLY ic.segredo ALTER COLUMN id SET DEFAULT nextval('ic.segredo_id_seq'::regclass);


--
-- Name: etapaarquivo id; Type: DEFAULT; Schema: mudanca; Owner: postgres
--

ALTER TABLE ONLY mudanca.etapaarquivo ALTER COLUMN id SET DEFAULT nextval('mudanca.etapaarquivo_id_seq'::regclass);


--
-- Name: mudanca id; Type: DEFAULT; Schema: mudanca; Owner: postgres
--

ALTER TABLE ONLY mudanca.mudanca ALTER COLUMN id SET DEFAULT nextval('mudanca.mudanca_id_seq'::regclass);


--
-- Data for Name: chamado; Type: TABLE DATA; Schema: chamado; Owner: postgres
--

COPY chamado.chamado (id, idautor, idsolicitante, datacriacao, slaprevisto, idservicoandamento, idsituacao, idorganograma) FROM stdin;
\.


--
-- Data for Name: servico; Type: TABLE DATA; Schema: chamado; Owner: postgres
--

COPY chamado.servico (id, datacriacao, idchamado, idservico, idatendente, slaprevisto, respostaformulario, idsituacao, slasaldo, slaoriginal) FROM stdin;
\.


--
-- Data for Name: servicoic; Type: TABLE DATA; Schema: chamado; Owner: postgres
--

COPY chamado.servicoic (id, idservico, idic) FROM stdin;
\.


--
-- Data for Name: servicoinformacao; Type: TABLE DATA; Schema: chamado; Owner: postgres
--

COPY chamado.servicoinformacao (id, idservico, data, idautor, informacao, idarquivo) FROM stdin;
\.


--
-- Data for Name: situacao; Type: TABLE DATA; Schema: chamado; Owner: postgres
--

COPY chamado.situacao (id, nome, contasla) FROM stdin;
\.


--
-- Data for Name: arquivo; Type: TABLE DATA; Schema: corp; Owner: postgres
--

COPY corp.arquivo (id, nome, tipo, conteudo) FROM stdin;
\.


--
-- Data for Name: expediente; Type: TABLE DATA; Schema: corp; Owner: postgres
--

COPY corp.expediente (id, data, turno1hrinicio, turno1hrtermino, turno2hrinicio, turno2hrtermino) FROM stdin;
\.


--
-- Data for Name: parametro; Type: TABLE DATA; Schema: corp; Owner: postgres
--

COPY corp.parametro (id, nome, valor) FROM stdin;
\.


--
-- Data for Name: tipo; Type: TABLE DATA; Schema: corp; Owner: postgres
--

COPY corp.tipo (id, nome, grupo, ativo) FROM stdin;
1	Outros	Tipo	t
\.


--
-- Data for Name: conhecimento; Type: TABLE DATA; Schema: ic; Owner: postgres
--

COPY ic.conhecimento (id, idic, problema, solucao, idautor, dataalteracao) FROM stdin;
\.


--
-- Data for Name: dependencia; Type: TABLE DATA; Schema: ic; Owner: postgres
--

COPY ic.dependencia (id, idicprincipal, idicdependente, idautor, dataalteracao) FROM stdin;
\.


--
-- Data for Name: ic; Type: TABLE DATA; Schema: ic; Owner: postgres
--

COPY ic.ic (id, idpai, nome, ativo, propriedades, idtipo, idorganograma) FROM stdin;
1	\N	Root	t	\N	1	1
2	1	nome teste	t	\N	1	1
\.


--
-- Data for Name: incidente; Type: TABLE DATA; Schema: ic; Owner: postgres
--

COPY ic.incidente (id, inicio, termino, causa, solucao, sintoma) FROM stdin;
\.


--
-- Data for Name: incidenteic; Type: TABLE DATA; Schema: ic; Owner: postgres
--

COPY ic.incidenteic (id, idincidente, idic) FROM stdin;
\.


--
-- Data for Name: incidentemensagem; Type: TABLE DATA; Schema: ic; Owner: postgres
--

COPY ic.incidentemensagem (id, idincidente, idautor, data, mensagem) FROM stdin;
\.


--
-- Data for Name: incidenteorganograma; Type: TABLE DATA; Schema: ic; Owner: postgres
--

COPY ic.incidenteorganograma (id, idincidente, idorganograma) FROM stdin;
\.


--
-- Data for Name: segredo; Type: TABLE DATA; Schema: ic; Owner: postgres
--

COPY ic.segredo (id, idic, conteudo, idusuariodono, idorganogramadono, algoritmo) FROM stdin;
\.


--
-- Data for Name: etapaarquivo; Type: TABLE DATA; Schema: mudanca; Owner: postgres
--

COPY mudanca.etapaarquivo (id) FROM stdin;
\.


--
-- Data for Name: mudanca; Type: TABLE DATA; Schema: mudanca; Owner: postgres
--

COPY mudanca.mudanca (id, datacriacao, inicioprevisto, idsituacao) FROM stdin;
\.


--
-- Data for Name: equipe; Type: TABLE DATA; Schema: seg; Owner: postgres
--

COPY seg.equipe (id, idusuario, idorganograma, idautor, data) FROM stdin;
\.


--
-- Data for Name: organograma; Type: TABLE DATA; Schema: seg; Owner: postgres
--

COPY seg.organograma (id, idpai, nome, ativo, gd) FROM stdin;
1	\N	Corp	t	fe12a60b-fb0d-4967-bd0b-b736d0edbc3e
\.


--
-- Data for Name: usuario; Type: TABLE DATA; Schema: seg; Owner: postgres
--

COPY seg.usuario (id, nome, senha, email, administrador, ativo, gd) FROM stdin;
1	admin	d2160e670a6261bcd5164a5b2164a89e98a34b54ccd59944ccc68fcc725f12ce3746367919d55fa43101e0d2e1f2ff1c1c824499622e0643024f9d77bfebcbd2	adm@cmdb.com.br	t	t	96615102-1e2e-4b71-b831-391286f33637
7	usr	9c1d9dc025d54d999fbf8d3ff28a0ddc336bebb7c3b8b4ee42510064deb36d0ccc598d3ac38d96d06cf201725c0725fd51c154ff7759812e4b23989b0eae2a38	user1@cmdb.com.br	f	t	31a29536-2b41-4eb4-aded-eb758ba01db0
\.


--
-- Data for Name: servico; Type: TABLE DATA; Schema: servico; Owner: postgres
--

COPY servico.servico (id, idorganograma, nome, descricao, formulario, ativo, sla, orientacao, tempoestimadoexecucao) FROM stdin;
\.


--
-- Data for Name: servicoarquivos; Type: TABLE DATA; Schema: servico; Owner: postgres
--

COPY servico.servicoarquivos (id, idservico, idarquivo, descricao) FROM stdin;
\.


--
-- Data for Name: servicoativos; Type: TABLE DATA; Schema: servico; Owner: postgres
--

COPY servico.servicoativos (id, idservico, idic) FROM stdin;
\.


--
-- Name: chamado_id_seq; Type: SEQUENCE SET; Schema: chamado; Owner: postgres
--

SELECT pg_catalog.setval('chamado.chamado_id_seq', 1, false);


--
-- Name: chamadoservico_id_seq; Type: SEQUENCE SET; Schema: chamado; Owner: postgres
--

SELECT pg_catalog.setval('chamado.chamadoservico_id_seq', 1, false);


--
-- Name: servicoic_id_seq; Type: SEQUENCE SET; Schema: chamado; Owner: postgres
--

SELECT pg_catalog.setval('chamado.servicoic_id_seq', 1, false);


--
-- Name: servicoinformacao_id_seq; Type: SEQUENCE SET; Schema: chamado; Owner: postgres
--

SELECT pg_catalog.setval('chamado.servicoinformacao_id_seq', 1, false);


--
-- Name: sqchamado; Type: SEQUENCE SET; Schema: chamado; Owner: postgres
--

SELECT pg_catalog.setval('chamado.sqchamado', 1, false);


--
-- Name: expediente_id_seq; Type: SEQUENCE SET; Schema: corp; Owner: postgres
--

SELECT pg_catalog.setval('corp.expediente_id_seq', 1, false);


--
-- Name: parametro_id_seq; Type: SEQUENCE SET; Schema: corp; Owner: postgres
--

SELECT pg_catalog.setval('corp.parametro_id_seq', 1, false);


--
-- Name: sqtipo; Type: SEQUENCE SET; Schema: corp; Owner: postgres
--

SELECT pg_catalog.setval('corp.sqtipo', 1, true);


--
-- Name: incidentemensagem_id_seq; Type: SEQUENCE SET; Schema: ic; Owner: postgres
--

SELECT pg_catalog.setval('ic.incidentemensagem_id_seq', 1, false);


--
-- Name: incidenteorganograma_id_seq; Type: SEQUENCE SET; Schema: ic; Owner: postgres
--

SELECT pg_catalog.setval('ic.incidenteorganograma_id_seq', 1, false);


--
-- Name: segredo_id_seq; Type: SEQUENCE SET; Schema: ic; Owner: postgres
--

SELECT pg_catalog.setval('ic.segredo_id_seq', 1, false);


--
-- Name: sqconhecimento; Type: SEQUENCE SET; Schema: ic; Owner: postgres
--

SELECT pg_catalog.setval('ic.sqconhecimento', 1, false);


--
-- Name: sqdependencia; Type: SEQUENCE SET; Schema: ic; Owner: postgres
--

SELECT pg_catalog.setval('ic.sqdependencia', 1, false);


--
-- Name: sqic; Type: SEQUENCE SET; Schema: ic; Owner: postgres
--

SELECT pg_catalog.setval('ic.sqic', 2, true);


--
-- Name: sqincidente; Type: SEQUENCE SET; Schema: ic; Owner: postgres
--

SELECT pg_catalog.setval('ic.sqincidente', 1, false);


--
-- Name: sqincidenteacao; Type: SEQUENCE SET; Schema: ic; Owner: postgres
--

SELECT pg_catalog.setval('ic.sqincidenteacao', 1, false);


--
-- Name: sqincidenteic; Type: SEQUENCE SET; Schema: ic; Owner: postgres
--

SELECT pg_catalog.setval('ic.sqincidenteic', 1, false);


--
-- Name: sqsegredo; Type: SEQUENCE SET; Schema: ic; Owner: postgres
--

SELECT pg_catalog.setval('ic.sqsegredo', 1, false);


--
-- Name: etapaarquivo_id_seq; Type: SEQUENCE SET; Schema: mudanca; Owner: postgres
--

SELECT pg_catalog.setval('mudanca.etapaarquivo_id_seq', 1, false);


--
-- Name: mudanca_id_seq; Type: SEQUENCE SET; Schema: mudanca; Owner: postgres
--

SELECT pg_catalog.setval('mudanca.mudanca_id_seq', 1, false);


--
-- Name: sqequipe; Type: SEQUENCE SET; Schema: seg; Owner: postgres
--

SELECT pg_catalog.setval('seg.sqequipe', 1, false);


--
-- Name: sqorganograma; Type: SEQUENCE SET; Schema: seg; Owner: postgres
--

SELECT pg_catalog.setval('seg.sqorganograma', 1, true);


--
-- Name: squsuario; Type: SEQUENCE SET; Schema: seg; Owner: postgres
--

SELECT pg_catalog.setval('seg.squsuario', 7, true);


--
-- Name: sqchamado; Type: SEQUENCE SET; Schema: servico; Owner: postgres
--

SELECT pg_catalog.setval('servico.sqchamado', 1, false);


--
-- Name: sqservico; Type: SEQUENCE SET; Schema: servico; Owner: postgres
--

SELECT pg_catalog.setval('servico.sqservico', 1, false);


--
-- Name: sqservicoarquivos; Type: SEQUENCE SET; Schema: servico; Owner: postgres
--

SELECT pg_catalog.setval('servico.sqservicoarquivos', 1, false);


--
-- Name: sqservicoativos; Type: SEQUENCE SET; Schema: servico; Owner: postgres
--

SELECT pg_catalog.setval('servico.sqservicoativos', 1, false);


--
-- Name: servico chamadoservico_pkey; Type: CONSTRAINT; Schema: chamado; Owner: postgres
--

ALTER TABLE ONLY chamado.servico
    ADD CONSTRAINT chamadoservico_pkey PRIMARY KEY (id);


--
-- Name: chamado pk_chamado_id; Type: CONSTRAINT; Schema: chamado; Owner: postgres
--

ALTER TABLE ONLY chamado.chamado
    ADD CONSTRAINT pk_chamado_id PRIMARY KEY (id);


--
-- Name: servicoic servicoic_pkey; Type: CONSTRAINT; Schema: chamado; Owner: postgres
--

ALTER TABLE ONLY chamado.servicoic
    ADD CONSTRAINT servicoic_pkey PRIMARY KEY (id);


--
-- Name: servicoinformacao servicoinformacao_pkey; Type: CONSTRAINT; Schema: chamado; Owner: postgres
--

ALTER TABLE ONLY chamado.servicoinformacao
    ADD CONSTRAINT servicoinformacao_pkey PRIMARY KEY (id);


--
-- Name: situacao situacao_pkey; Type: CONSTRAINT; Schema: chamado; Owner: postgres
--

ALTER TABLE ONLY chamado.situacao
    ADD CONSTRAINT situacao_pkey PRIMARY KEY (id);


--
-- Name: arquivo arquivo_pkey; Type: CONSTRAINT; Schema: corp; Owner: postgres
--

ALTER TABLE ONLY corp.arquivo
    ADD CONSTRAINT arquivo_pkey PRIMARY KEY (id);


--
-- Name: expediente expediente_pkey; Type: CONSTRAINT; Schema: corp; Owner: postgres
--

ALTER TABLE ONLY corp.expediente
    ADD CONSTRAINT expediente_pkey PRIMARY KEY (id);


--
-- Name: parametro parametro_pkey; Type: CONSTRAINT; Schema: corp; Owner: postgres
--

ALTER TABLE ONLY corp.parametro
    ADD CONSTRAINT parametro_pkey PRIMARY KEY (id);


--
-- Name: tipo tipo_pkey; Type: CONSTRAINT; Schema: corp; Owner: postgres
--

ALTER TABLE ONLY corp.tipo
    ADD CONSTRAINT tipo_pkey PRIMARY KEY (id);


--
-- Name: conhecimento conhecimento_pkey; Type: CONSTRAINT; Schema: ic; Owner: postgres
--

ALTER TABLE ONLY ic.conhecimento
    ADD CONSTRAINT conhecimento_pkey PRIMARY KEY (id);


--
-- Name: dependencia dependencia_pkey; Type: CONSTRAINT; Schema: ic; Owner: postgres
--

ALTER TABLE ONLY ic.dependencia
    ADD CONSTRAINT dependencia_pkey PRIMARY KEY (id);


--
-- Name: ic ic_pkey; Type: CONSTRAINT; Schema: ic; Owner: postgres
--

ALTER TABLE ONLY ic.ic
    ADD CONSTRAINT ic_pkey PRIMARY KEY (id);


--
-- Name: incidente incidente_pkey; Type: CONSTRAINT; Schema: ic; Owner: postgres
--

ALTER TABLE ONLY ic.incidente
    ADD CONSTRAINT incidente_pkey PRIMARY KEY (id);


--
-- Name: incidenteic incidenteic_pkey; Type: CONSTRAINT; Schema: ic; Owner: postgres
--

ALTER TABLE ONLY ic.incidenteic
    ADD CONSTRAINT incidenteic_pkey PRIMARY KEY (id);


--
-- Name: incidentemensagem incidentemensagem_pkey; Type: CONSTRAINT; Schema: ic; Owner: postgres
--

ALTER TABLE ONLY ic.incidentemensagem
    ADD CONSTRAINT incidentemensagem_pkey PRIMARY KEY (id);


--
-- Name: incidenteorganograma incidenteorganograma_pkey; Type: CONSTRAINT; Schema: ic; Owner: postgres
--

ALTER TABLE ONLY ic.incidenteorganograma
    ADD CONSTRAINT incidenteorganograma_pkey PRIMARY KEY (id);


--
-- Name: segredo segredo_pkey; Type: CONSTRAINT; Schema: ic; Owner: postgres
--

ALTER TABLE ONLY ic.segredo
    ADD CONSTRAINT segredo_pkey PRIMARY KEY (id);


--
-- Name: etapaarquivo etapaarquivo_pkey; Type: CONSTRAINT; Schema: mudanca; Owner: postgres
--

ALTER TABLE ONLY mudanca.etapaarquivo
    ADD CONSTRAINT etapaarquivo_pkey PRIMARY KEY (id);


--
-- Name: mudanca mudanca_pkey; Type: CONSTRAINT; Schema: mudanca; Owner: postgres
--

ALTER TABLE ONLY mudanca.mudanca
    ADD CONSTRAINT mudanca_pkey PRIMARY KEY (id);


--
-- Name: equipe equipe_pkey; Type: CONSTRAINT; Schema: seg; Owner: postgres
--

ALTER TABLE ONLY seg.equipe
    ADD CONSTRAINT equipe_pkey PRIMARY KEY (id);


--
-- Name: organograma organograma_pkey; Type: CONSTRAINT; Schema: seg; Owner: postgres
--

ALTER TABLE ONLY seg.organograma
    ADD CONSTRAINT organograma_pkey PRIMARY KEY (id);


--
-- Name: usuario usuario_pkey; Type: CONSTRAINT; Schema: seg; Owner: postgres
--

ALTER TABLE ONLY seg.usuario
    ADD CONSTRAINT usuario_pkey PRIMARY KEY (id);


--
-- Name: servico servico_pkey; Type: CONSTRAINT; Schema: servico; Owner: postgres
--

ALTER TABLE ONLY servico.servico
    ADD CONSTRAINT servico_pkey PRIMARY KEY (id);


--
-- Name: servicoarquivos servicoarquivos_pkey; Type: CONSTRAINT; Schema: servico; Owner: postgres
--

ALTER TABLE ONLY servico.servicoarquivos
    ADD CONSTRAINT servicoarquivos_pkey PRIMARY KEY (id);


--
-- Name: servicoativos servicoativos_pkey; Type: CONSTRAINT; Schema: servico; Owner: postgres
--

ALTER TABLE ONLY servico.servicoativos
    ADD CONSTRAINT servicoativos_pkey PRIMARY KEY (id);


--
-- Name: ix_chamadoservicoic_servico; Type: INDEX; Schema: chamado; Owner: postgres
--

CREATE INDEX ix_chamadoservicoic_servico ON chamado.servicoic USING btree (idservico);


--
-- Name: ix_servicoic_ic; Type: INDEX; Schema: chamado; Owner: postgres
--

CREATE INDEX ix_servicoic_ic ON chamado.servicoic USING btree (idic);


--
-- Name: ix_corpparametro_nome; Type: INDEX; Schema: corp; Owner: postgres
--

CREATE UNIQUE INDEX ix_corpparametro_nome ON corp.parametro USING btree (nome);


--
-- Name: ix_corptiponomegrupo; Type: INDEX; Schema: corp; Owner: postgres
--

CREATE UNIQUE INDEX ix_corptiponomegrupo ON corp.tipo USING btree (grupo, nome);


--
-- Name: ix_conhecimenot_ic; Type: INDEX; Schema: ic; Owner: postgres
--

CREATE INDEX ix_conhecimenot_ic ON ic.conhecimento USING btree (idic);


--
-- Name: ix_dependencia_icprincipal; Type: INDEX; Schema: ic; Owner: postgres
--

CREATE INDEX ix_dependencia_icprincipal ON ic.dependencia USING btree (idicprincipal);


--
-- Name: ix_dependencia_idicdependente; Type: INDEX; Schema: ic; Owner: postgres
--

CREATE INDEX ix_dependencia_idicdependente ON ic.dependencia USING btree (idicdependente);


--
-- Name: ix_icconhecimento_idic; Type: INDEX; Schema: ic; Owner: postgres
--

CREATE INDEX ix_icconhecimento_idic ON ic.conhecimento USING btree (idic);


--
-- Name: ix_icpainome; Type: INDEX; Schema: ic; Owner: postgres
--

CREATE UNIQUE INDEX ix_icpainome ON ic.ic USING btree (idpai, nome);


--
-- Name: ix_icsegredo_idic; Type: INDEX; Schema: ic; Owner: postgres
--

CREATE INDEX ix_icsegredo_idic ON ic.segredo USING btree (idic);


--
-- Name: ix_incidenteic; Type: INDEX; Schema: ic; Owner: postgres
--

CREATE INDEX ix_incidenteic ON ic.incidenteic USING btree (idic);


--
-- Name: ix_incidenteic_ic; Type: INDEX; Schema: ic; Owner: postgres
--

CREATE INDEX ix_incidenteic_ic ON ic.incidenteic USING btree (idic);


--
-- Name: ix_incidenteicincidente; Type: INDEX; Schema: ic; Owner: postgres
--

CREATE UNIQUE INDEX ix_incidenteicincidente ON ic.incidenteic USING btree (idincidente, idic);


--
-- Name: ix_incidentemensagem_incidente; Type: INDEX; Schema: ic; Owner: postgres
--

CREATE INDEX ix_incidentemensagem_incidente ON ic.incidentemensagem USING btree (idincidente);


--
-- Name: ix_incidenteorganograma_incidente; Type: INDEX; Schema: ic; Owner: postgres
--

CREATE INDEX ix_incidenteorganograma_incidente ON ic.incidenteorganograma USING btree (idincidente);


--
-- Name: ix_segredo_ic; Type: INDEX; Schema: ic; Owner: postgres
--

CREATE INDEX ix_segredo_ic ON ic.segredo USING btree (idic);


--
-- Name: ix_vw_ic_ts; Type: INDEX; Schema: ic; Owner: postgres
--

CREATE INDEX ix_vw_ic_ts ON ic.vw_ic USING gin (pesquisats);


--
-- Name: ix_vwicid; Type: INDEX; Schema: ic; Owner: postgres
--

CREATE UNIQUE INDEX ix_vwicid ON ic.vw_ic USING btree (id);


--
-- Name: ix_vwicnome; Type: INDEX; Schema: ic; Owner: postgres
--

CREATE INDEX ix_vwicnome ON ic.vw_ic USING btree (lower((nome)::text));


--
-- Name: ixdependenciadependencia; Type: INDEX; Schema: ic; Owner: postgres
--

CREATE INDEX ixdependenciadependencia ON ic.dependencia USING btree (idicdependente);


--
-- Name: ixdependenciaprincipaldependencia; Type: INDEX; Schema: ic; Owner: postgres
--

CREATE UNIQUE INDEX ixdependenciaprincipaldependencia ON ic.dependencia USING btree (idicprincipal, idicdependente);


--
-- Name: ix_equipeusuarioorganograma; Type: INDEX; Schema: seg; Owner: postgres
--

CREATE UNIQUE INDEX ix_equipeusuarioorganograma ON seg.equipe USING btree (idusuario, idorganograma);


--
-- Name: ix_segequipe_usuario; Type: INDEX; Schema: seg; Owner: postgres
--

CREATE INDEX ix_segequipe_usuario ON seg.equipe USING btree (idusuario);


--
-- Name: ix_vworganogramaid; Type: INDEX; Schema: seg; Owner: postgres
--

CREATE UNIQUE INDEX ix_vworganogramaid ON seg.vw_organograma USING btree (id);


--
-- Name: ix_vworganogramanome; Type: INDEX; Schema: seg; Owner: postgres
--

CREATE INDEX ix_vworganogramanome ON seg.vw_organograma USING btree (nome);


--
-- Name: ix_vworganogramanomecompleto; Type: INDEX; Schema: seg; Owner: postgres
--

CREATE INDEX ix_vworganogramanomecompleto ON seg.vw_organograma USING btree (nomecompleto);


--
-- Name: ixsegorganogramaidpainome; Type: INDEX; Schema: seg; Owner: postgres
--

CREATE UNIQUE INDEX ixsegorganogramaidpainome ON seg.organograma USING btree (idpai, nome);


--
-- Name: usuario_email_idx; Type: INDEX; Schema: seg; Owner: postgres
--

CREATE UNIQUE INDEX usuario_email_idx ON seg.usuario USING btree (lower((email)::text));


--
-- Name: ix_servicoativos_ic; Type: INDEX; Schema: servico; Owner: postgres
--

CREATE INDEX ix_servicoativos_ic ON servico.servicoativos USING btree (idic);


--
-- Name: ixservicoarquivoservico; Type: INDEX; Schema: servico; Owner: postgres
--

CREATE INDEX ixservicoarquivoservico ON servico.servicoarquivos USING btree (idservico);


--
-- Name: ixservicoorganograma; Type: INDEX; Schema: servico; Owner: postgres
--

CREATE INDEX ixservicoorganograma ON servico.servico USING btree (idorganograma);


--
-- Name: ic trgicicupdate; Type: TRIGGER; Schema: ic; Owner: postgres
--

CREATE TRIGGER trgicicupdate AFTER INSERT OR DELETE OR UPDATE OR TRUNCATE ON ic.ic FOR EACH STATEMENT EXECUTE FUNCTION ic.fn_vwicupdate();


--
-- Name: organograma trgsegorganogramaupdate; Type: TRIGGER; Schema: seg; Owner: postgres
--

CREATE TRIGGER trgsegorganogramaupdate AFTER INSERT OR DELETE OR UPDATE OR TRUNCATE ON seg.organograma FOR EACH STATEMENT EXECUTE FUNCTION seg.fn_vworganogramaupdate();


--
-- Name: chamado fk_chamado_servicoanndamento; Type: FK CONSTRAINT; Schema: chamado; Owner: postgres
--

ALTER TABLE ONLY chamado.chamado
    ADD CONSTRAINT fk_chamado_servicoanndamento FOREIGN KEY (idservicoandamento) REFERENCES chamado.servico(id);


--
-- Name: chamado fk_chamado_situacao; Type: FK CONSTRAINT; Schema: chamado; Owner: postgres
--

ALTER TABLE ONLY chamado.chamado
    ADD CONSTRAINT fk_chamado_situacao FOREIGN KEY (idsituacao) REFERENCES chamado.situacao(id);


--
-- Name: chamado fk_chamadochamado_organograma; Type: FK CONSTRAINT; Schema: chamado; Owner: postgres
--

ALTER TABLE ONLY chamado.chamado
    ADD CONSTRAINT fk_chamadochamado_organograma FOREIGN KEY (idorganograma) REFERENCES seg.organograma(id);


--
-- Name: servico fk_chamadoservico_chamado; Type: FK CONSTRAINT; Schema: chamado; Owner: postgres
--

ALTER TABLE ONLY chamado.servico
    ADD CONSTRAINT fk_chamadoservico_chamado FOREIGN KEY (idchamado) REFERENCES chamado.chamado(id);


--
-- Name: servico fk_chamadoservico_servico; Type: FK CONSTRAINT; Schema: chamado; Owner: postgres
--

ALTER TABLE ONLY chamado.servico
    ADD CONSTRAINT fk_chamadoservico_servico FOREIGN KEY (idservico) REFERENCES servico.servico(id);


--
-- Name: servico fk_chamadoservico_situacao; Type: FK CONSTRAINT; Schema: chamado; Owner: postgres
--

ALTER TABLE ONLY chamado.servico
    ADD CONSTRAINT fk_chamadoservico_situacao FOREIGN KEY (idsituacao) REFERENCES chamado.situacao(id);


--
-- Name: servicoic fk_chamadoservicoic_ic; Type: FK CONSTRAINT; Schema: chamado; Owner: postgres
--

ALTER TABLE ONLY chamado.servicoic
    ADD CONSTRAINT fk_chamadoservicoic_ic FOREIGN KEY (idic) REFERENCES ic.ic(id);


--
-- Name: servicoic fk_chamadoservicoic_servico; Type: FK CONSTRAINT; Schema: chamado; Owner: postgres
--

ALTER TABLE ONLY chamado.servicoic
    ADD CONSTRAINT fk_chamadoservicoic_servico FOREIGN KEY (idservico) REFERENCES chamado.servico(id);


--
-- Name: servicoinformacao fk_servicoinformacao_arquivo; Type: FK CONSTRAINT; Schema: chamado; Owner: postgres
--

ALTER TABLE ONLY chamado.servicoinformacao
    ADD CONSTRAINT fk_servicoinformacao_arquivo FOREIGN KEY (idarquivo) REFERENCES corp.arquivo(id);


--
-- Name: servicoinformacao fk_servicoinformacao_servico; Type: FK CONSTRAINT; Schema: chamado; Owner: postgres
--

ALTER TABLE ONLY chamado.servicoinformacao
    ADD CONSTRAINT fk_servicoinformacao_servico FOREIGN KEY (idservico) REFERENCES chamado.servico(id);


--
-- Name: conhecimento conhecimento_idic_fkey; Type: FK CONSTRAINT; Schema: ic; Owner: postgres
--

ALTER TABLE ONLY ic.conhecimento
    ADD CONSTRAINT conhecimento_idic_fkey FOREIGN KEY (idic) REFERENCES ic.ic(id);


--
-- Name: dependencia dependencia_idicdependente_fkey; Type: FK CONSTRAINT; Schema: ic; Owner: postgres
--

ALTER TABLE ONLY ic.dependencia
    ADD CONSTRAINT dependencia_idicdependente_fkey FOREIGN KEY (idicdependente) REFERENCES ic.ic(id);


--
-- Name: dependencia dependencia_idicprincipal_fkey; Type: FK CONSTRAINT; Schema: ic; Owner: postgres
--

ALTER TABLE ONLY ic.dependencia
    ADD CONSTRAINT dependencia_idicprincipal_fkey FOREIGN KEY (idicprincipal) REFERENCES ic.ic(id);


--
-- Name: ic fk_icpaifilho; Type: FK CONSTRAINT; Schema: ic; Owner: postgres
--

ALTER TABLE ONLY ic.ic
    ADD CONSTRAINT fk_icpaifilho FOREIGN KEY (idpai) REFERENCES ic.ic(id);


--
-- Name: ic fk_ictipo; Type: FK CONSTRAINT; Schema: ic; Owner: postgres
--

ALTER TABLE ONLY ic.ic
    ADD CONSTRAINT fk_ictipo FOREIGN KEY (idtipo) REFERENCES corp.tipo(id);


--
-- Name: ic ic_idorganograma_fkey; Type: FK CONSTRAINT; Schema: ic; Owner: postgres
--

ALTER TABLE ONLY ic.ic
    ADD CONSTRAINT ic_idorganograma_fkey FOREIGN KEY (idorganograma) REFERENCES seg.organograma(id);


--
-- Name: incidenteic incidenteic_idic_fkey; Type: FK CONSTRAINT; Schema: ic; Owner: postgres
--

ALTER TABLE ONLY ic.incidenteic
    ADD CONSTRAINT incidenteic_idic_fkey FOREIGN KEY (idic) REFERENCES ic.ic(id);


--
-- Name: incidenteic incidenteic_idincidente_fkey; Type: FK CONSTRAINT; Schema: ic; Owner: postgres
--

ALTER TABLE ONLY ic.incidenteic
    ADD CONSTRAINT incidenteic_idincidente_fkey FOREIGN KEY (idincidente) REFERENCES ic.incidente(id);


--
-- Name: incidentemensagem incidentemensagem_idincidente_fkey; Type: FK CONSTRAINT; Schema: ic; Owner: postgres
--

ALTER TABLE ONLY ic.incidentemensagem
    ADD CONSTRAINT incidentemensagem_idincidente_fkey FOREIGN KEY (idincidente) REFERENCES ic.incidente(id);


--
-- Name: incidenteorganograma incidenteorganograma_idincidente_fkey; Type: FK CONSTRAINT; Schema: ic; Owner: postgres
--

ALTER TABLE ONLY ic.incidenteorganograma
    ADD CONSTRAINT incidenteorganograma_idincidente_fkey FOREIGN KEY (idincidente) REFERENCES ic.incidente(id);


--
-- Name: incidenteorganograma incidenteorganograma_idorganograma_fkey; Type: FK CONSTRAINT; Schema: ic; Owner: postgres
--

ALTER TABLE ONLY ic.incidenteorganograma
    ADD CONSTRAINT incidenteorganograma_idorganograma_fkey FOREIGN KEY (idorganograma) REFERENCES seg.organograma(id);


--
-- Name: segredo segredo_idic_fkey; Type: FK CONSTRAINT; Schema: ic; Owner: postgres
--

ALTER TABLE ONLY ic.segredo
    ADD CONSTRAINT segredo_idic_fkey FOREIGN KEY (idic) REFERENCES ic.ic(id);


--
-- Name: segredo segredo_idorganogramadono_fkey; Type: FK CONSTRAINT; Schema: ic; Owner: postgres
--

ALTER TABLE ONLY ic.segredo
    ADD CONSTRAINT segredo_idorganogramadono_fkey FOREIGN KEY (idorganogramadono) REFERENCES seg.organograma(id);


--
-- Name: mudanca fk_mudanca_situracao; Type: FK CONSTRAINT; Schema: mudanca; Owner: postgres
--

ALTER TABLE ONLY mudanca.mudanca
    ADD CONSTRAINT fk_mudanca_situracao FOREIGN KEY (idsituacao) REFERENCES corp.tipo(id) NOT VALID;


--
-- Name: equipe equipe_idorganograma_fkey; Type: FK CONSTRAINT; Schema: seg; Owner: postgres
--

ALTER TABLE ONLY seg.equipe
    ADD CONSTRAINT equipe_idorganograma_fkey FOREIGN KEY (idorganograma) REFERENCES seg.organograma(id);


--
-- Name: organograma fk_organogramapaifilho; Type: FK CONSTRAINT; Schema: seg; Owner: postgres
--

ALTER TABLE ONLY seg.organograma
    ADD CONSTRAINT fk_organogramapaifilho FOREIGN KEY (idpai) REFERENCES seg.organograma(id);


--
-- Name: servico servico_idorganograma_fkey; Type: FK CONSTRAINT; Schema: servico; Owner: postgres
--

ALTER TABLE ONLY servico.servico
    ADD CONSTRAINT servico_idorganograma_fkey FOREIGN KEY (idorganograma) REFERENCES seg.organograma(id);


--
-- Name: servicoarquivos servicoarquivos_idarquivo_fkey; Type: FK CONSTRAINT; Schema: servico; Owner: postgres
--

ALTER TABLE ONLY servico.servicoarquivos
    ADD CONSTRAINT servicoarquivos_idarquivo_fkey FOREIGN KEY (idarquivo) REFERENCES corp.arquivo(id);


--
-- Name: servicoarquivos servicoarquivos_idservico_fkey; Type: FK CONSTRAINT; Schema: servico; Owner: postgres
--

ALTER TABLE ONLY servico.servicoarquivos
    ADD CONSTRAINT servicoarquivos_idservico_fkey FOREIGN KEY (idservico) REFERENCES servico.servico(id);


--
-- Name: servicoativos servicoativos_idic_fkey; Type: FK CONSTRAINT; Schema: servico; Owner: postgres
--

ALTER TABLE ONLY servico.servicoativos
    ADD CONSTRAINT servicoativos_idic_fkey FOREIGN KEY (idic) REFERENCES ic.ic(id);


--
-- Name: servicoativos servicoativos_idservico_fkey; Type: FK CONSTRAINT; Schema: servico; Owner: postgres
--

ALTER TABLE ONLY servico.servicoativos
    ADD CONSTRAINT servicoativos_idservico_fkey FOREIGN KEY (idservico) REFERENCES servico.servico(id);


--
-- Name: SCHEMA chamado; Type: ACL; Schema: -; Owner: postgres
--

GRANT USAGE ON SCHEMA chamado TO usrapp;


--
-- Name: SCHEMA corp; Type: ACL; Schema: -; Owner: postgres
--

GRANT USAGE ON SCHEMA corp TO usrapp;


--
-- Name: SCHEMA ic; Type: ACL; Schema: -; Owner: postgres
--

GRANT USAGE ON SCHEMA ic TO usrapp;


--
-- Name: SCHEMA seg; Type: ACL; Schema: -; Owner: postgres
--

GRANT USAGE ON SCHEMA seg TO usrapp;


--
-- Name: SCHEMA servico; Type: ACL; Schema: -; Owner: postgres
--

GRANT USAGE ON SCHEMA servico TO usrapp;


--
-- Name: FUNCTION fn_vwicupdate(); Type: ACL; Schema: ic; Owner: postgres
--

GRANT ALL ON FUNCTION ic.fn_vwicupdate() TO usrapp;


--
-- Name: FUNCTION fn_vworganogramaupdate(); Type: ACL; Schema: seg; Owner: postgres
--

GRANT ALL ON FUNCTION seg.fn_vworganogramaupdate() TO usrapp;


--
-- Name: TABLE chamado; Type: ACL; Schema: chamado; Owner: postgres
--

GRANT SELECT,INSERT,DELETE,UPDATE ON TABLE chamado.chamado TO usrapp;


--
-- Name: SEQUENCE chamado_id_seq; Type: ACL; Schema: chamado; Owner: postgres
--

GRANT SELECT,USAGE ON SEQUENCE chamado.chamado_id_seq TO usrapp;


--
-- Name: TABLE servico; Type: ACL; Schema: chamado; Owner: postgres
--

GRANT SELECT,INSERT,DELETE,UPDATE ON TABLE chamado.servico TO usrapp;


--
-- Name: SEQUENCE chamadoservico_id_seq; Type: ACL; Schema: chamado; Owner: postgres
--

GRANT SELECT,USAGE ON SEQUENCE chamado.chamadoservico_id_seq TO usrapp;


--
-- Name: TABLE servicoic; Type: ACL; Schema: chamado; Owner: postgres
--

GRANT SELECT,INSERT,DELETE,UPDATE ON TABLE chamado.servicoic TO usrapp;


--
-- Name: SEQUENCE servicoic_id_seq; Type: ACL; Schema: chamado; Owner: postgres
--

GRANT SELECT,USAGE ON SEQUENCE chamado.servicoic_id_seq TO usrapp;


--
-- Name: TABLE servicoinformacao; Type: ACL; Schema: chamado; Owner: postgres
--

GRANT SELECT,INSERT,DELETE,UPDATE ON TABLE chamado.servicoinformacao TO usrapp;


--
-- Name: SEQUENCE servicoinformacao_id_seq; Type: ACL; Schema: chamado; Owner: postgres
--

GRANT SELECT,USAGE ON SEQUENCE chamado.servicoinformacao_id_seq TO usrapp;


--
-- Name: TABLE situacao; Type: ACL; Schema: chamado; Owner: postgres
--

GRANT SELECT,INSERT,DELETE,UPDATE ON TABLE chamado.situacao TO usrapp;


--
-- Name: SEQUENCE sqchamado; Type: ACL; Schema: chamado; Owner: postgres
--

GRANT SELECT,USAGE ON SEQUENCE chamado.sqchamado TO usrapp;


--
-- Name: TABLE arquivo; Type: ACL; Schema: corp; Owner: postgres
--

GRANT SELECT,INSERT,DELETE,UPDATE ON TABLE corp.arquivo TO usrapp;


--
-- Name: TABLE expediente; Type: ACL; Schema: corp; Owner: postgres
--

GRANT SELECT,INSERT,DELETE,UPDATE ON TABLE corp.expediente TO usrapp;


--
-- Name: SEQUENCE expediente_id_seq; Type: ACL; Schema: corp; Owner: postgres
--

GRANT SELECT,USAGE ON SEQUENCE corp.expediente_id_seq TO usrapp;


--
-- Name: TABLE parametro; Type: ACL; Schema: corp; Owner: postgres
--

GRANT SELECT,INSERT,DELETE,UPDATE ON TABLE corp.parametro TO usrapp;


--
-- Name: SEQUENCE parametro_id_seq; Type: ACL; Schema: corp; Owner: postgres
--

GRANT SELECT,USAGE ON SEQUENCE corp.parametro_id_seq TO usrapp;


--
-- Name: SEQUENCE sqtipo; Type: ACL; Schema: corp; Owner: postgres
--

GRANT SELECT,USAGE ON SEQUENCE corp.sqtipo TO usrapp;


--
-- Name: TABLE tipo; Type: ACL; Schema: corp; Owner: postgres
--

GRANT SELECT,INSERT,DELETE,UPDATE ON TABLE corp.tipo TO usrapp;


--
-- Name: TABLE vw_arquivo; Type: ACL; Schema: corp; Owner: postgres
--

GRANT SELECT,INSERT,DELETE,UPDATE ON TABLE corp.vw_arquivo TO usrapp;


--
-- Name: SEQUENCE sqconhecimento; Type: ACL; Schema: ic; Owner: postgres
--

GRANT SELECT,USAGE ON SEQUENCE ic.sqconhecimento TO usrapp;


--
-- Name: TABLE conhecimento; Type: ACL; Schema: ic; Owner: postgres
--

GRANT SELECT,INSERT,DELETE,UPDATE ON TABLE ic.conhecimento TO usrapp;


--
-- Name: SEQUENCE sqdependencia; Type: ACL; Schema: ic; Owner: postgres
--

GRANT SELECT,USAGE ON SEQUENCE ic.sqdependencia TO usrapp;


--
-- Name: TABLE dependencia; Type: ACL; Schema: ic; Owner: postgres
--

GRANT SELECT,INSERT,DELETE,UPDATE ON TABLE ic.dependencia TO usrapp;


--
-- Name: SEQUENCE sqic; Type: ACL; Schema: ic; Owner: postgres
--

GRANT SELECT,USAGE ON SEQUENCE ic.sqic TO usrapp;


--
-- Name: TABLE ic; Type: ACL; Schema: ic; Owner: postgres
--

GRANT SELECT,INSERT,DELETE,UPDATE ON TABLE ic.ic TO usrapp;


--
-- Name: SEQUENCE sqincidente; Type: ACL; Schema: ic; Owner: postgres
--

GRANT SELECT,USAGE ON SEQUENCE ic.sqincidente TO usrapp;


--
-- Name: TABLE incidente; Type: ACL; Schema: ic; Owner: postgres
--

GRANT SELECT,INSERT,DELETE,UPDATE ON TABLE ic.incidente TO usrapp;


--
-- Name: SEQUENCE sqincidenteic; Type: ACL; Schema: ic; Owner: postgres
--

GRANT SELECT,USAGE ON SEQUENCE ic.sqincidenteic TO usrapp;


--
-- Name: TABLE incidenteic; Type: ACL; Schema: ic; Owner: postgres
--

GRANT SELECT,INSERT,DELETE,UPDATE ON TABLE ic.incidenteic TO usrapp;


--
-- Name: TABLE incidentemensagem; Type: ACL; Schema: ic; Owner: postgres
--

GRANT SELECT,INSERT,DELETE,UPDATE ON TABLE ic.incidentemensagem TO usrapp;


--
-- Name: SEQUENCE incidentemensagem_id_seq; Type: ACL; Schema: ic; Owner: postgres
--

GRANT SELECT,USAGE ON SEQUENCE ic.incidentemensagem_id_seq TO usrapp;


--
-- Name: SEQUENCE incidenteorganograma_id_seq; Type: ACL; Schema: ic; Owner: postgres
--

GRANT SELECT,USAGE ON SEQUENCE ic.incidenteorganograma_id_seq TO usrapp;


--
-- Name: TABLE incidenteorganograma; Type: ACL; Schema: ic; Owner: postgres
--

GRANT SELECT,INSERT,DELETE,UPDATE ON TABLE ic.incidenteorganograma TO usrapp;


--
-- Name: TABLE segredo; Type: ACL; Schema: ic; Owner: postgres
--

GRANT SELECT,INSERT,DELETE,UPDATE ON TABLE ic.segredo TO usrapp;


--
-- Name: SEQUENCE segredo_id_seq; Type: ACL; Schema: ic; Owner: postgres
--

GRANT SELECT,USAGE ON SEQUENCE ic.segredo_id_seq TO usrapp;


--
-- Name: SEQUENCE sqincidenteacao; Type: ACL; Schema: ic; Owner: postgres
--

GRANT SELECT,USAGE ON SEQUENCE ic.sqincidenteacao TO usrapp;


--
-- Name: SEQUENCE sqsegredo; Type: ACL; Schema: ic; Owner: postgres
--

GRANT SELECT,USAGE ON SEQUENCE ic.sqsegredo TO usrapp;


--
-- Name: TABLE vw_ic; Type: ACL; Schema: ic; Owner: postgres
--

GRANT SELECT ON TABLE ic.vw_ic TO usrapp;


--
-- Name: SEQUENCE sqequipe; Type: ACL; Schema: seg; Owner: postgres
--

GRANT SELECT,USAGE ON SEQUENCE seg.sqequipe TO usrapp;


--
-- Name: TABLE equipe; Type: ACL; Schema: seg; Owner: postgres
--

GRANT SELECT,INSERT,DELETE,UPDATE ON TABLE seg.equipe TO usrapp;


--
-- Name: SEQUENCE sqorganograma; Type: ACL; Schema: seg; Owner: postgres
--

GRANT SELECT,USAGE ON SEQUENCE seg.sqorganograma TO usrapp;


--
-- Name: TABLE organograma; Type: ACL; Schema: seg; Owner: postgres
--

GRANT SELECT,INSERT,DELETE,UPDATE ON TABLE seg.organograma TO usrapp;


--
-- Name: SEQUENCE squsuario; Type: ACL; Schema: seg; Owner: postgres
--

GRANT SELECT,USAGE ON SEQUENCE seg.squsuario TO usrapp;


--
-- Name: TABLE usuario; Type: ACL; Schema: seg; Owner: postgres
--

GRANT SELECT,INSERT,DELETE,UPDATE ON TABLE seg.usuario TO usrapp;


--
-- Name: TABLE vw_organograma; Type: ACL; Schema: seg; Owner: postgres
--

GRANT SELECT,INSERT,DELETE,UPDATE ON TABLE seg.vw_organograma TO usrapp;


--
-- Name: SEQUENCE sqservico; Type: ACL; Schema: servico; Owner: postgres
--

GRANT SELECT,USAGE ON SEQUENCE servico.sqservico TO usrapp;


--
-- Name: TABLE servico; Type: ACL; Schema: servico; Owner: postgres
--

GRANT SELECT,INSERT,DELETE,UPDATE ON TABLE servico.servico TO usrapp;


--
-- Name: SEQUENCE sqservicoarquivos; Type: ACL; Schema: servico; Owner: postgres
--

GRANT SELECT,USAGE ON SEQUENCE servico.sqservicoarquivos TO usrapp;


--
-- Name: TABLE servicoarquivos; Type: ACL; Schema: servico; Owner: postgres
--

GRANT SELECT,INSERT,DELETE,UPDATE ON TABLE servico.servicoarquivos TO usrapp;


--
-- Name: SEQUENCE sqservicoativos; Type: ACL; Schema: servico; Owner: postgres
--

GRANT SELECT,USAGE ON SEQUENCE servico.sqservicoativos TO usrapp;


--
-- Name: TABLE servicoativos; Type: ACL; Schema: servico; Owner: postgres
--

GRANT SELECT,INSERT,DELETE,UPDATE ON TABLE servico.servicoativos TO usrapp;


--
-- Name: SEQUENCE sqchamado; Type: ACL; Schema: servico; Owner: postgres
--

GRANT SELECT,USAGE ON SEQUENCE servico.sqchamado TO usrapp;


--
-- Name: vw_ic; Type: MATERIALIZED VIEW DATA; Schema: ic; Owner: postgres
--

REFRESH MATERIALIZED VIEW ic.vw_ic;


--
-- Name: vw_organograma; Type: MATERIALIZED VIEW DATA; Schema: seg; Owner: postgres
--

REFRESH MATERIALIZED VIEW seg.vw_organograma;


--
-- PostgreSQL database dump complete
--

