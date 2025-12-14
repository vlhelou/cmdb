import {icPropriedades} from 'src/model/ic/propriedades';
import {corpTipo} from 'src/model/corp/tipo';
import {segOrganograma} from 'src/model/seg/organograma';

export interface icIc{
    id: number;
    idPai: number;
    nome: string;
    ativo: boolean;
    ativoFinal: boolean;
    ativoPai: boolean;
    propriedades: icPropriedades[];
    idTipo: number;
    idOrganograma: number;
    nomeCompleto: string;
    listaAncestrais: string;
    nivel: number;
    observacao: string;
    lstAncestrais: number[];
 
    pai: icIc;
    filhos: icIc[];
    ancestrais: icIc[];
    tipo: corpTipo;
    responsavel: segOrganograma;
    // Conhecimentos: ICConhecimento[];
    // Segredos: ICSegredo[];
    // Dependentes: ICDependencia[];
    // Dependencias: ICDependencia[];

}