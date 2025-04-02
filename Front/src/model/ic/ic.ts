import {icPropriedades} from 'src/model/ic/propriedades';
import {corpTipo} from 'src/model/corp/tipo';
import {segOrganograma} from 'src/model/seg/organograma';

export interface icIc{
    id: number;
    idPai: number;
    nome: string;
    ativo: boolean;
    ativoFinal: boolean;
    Propriedades: icPropriedades[];
    idTipo: number;
    idOrganograma: number;
    momeCompleto: string;
    listaAncestrais: string;
    nivel: number;
    lstAncestrais: number[];
 
    Pai: icIc;
    Filhos: icIc[];
    Ancestrais: icIc[];
    Tipo: corpTipo;
    Responsavel: segOrganograma;
    // Conhecimentos: ICConhecimento[];
    // Segredos: ICSegredo[];
    // Dependentes: ICDependencia[];
    // Dependencias: ICDependencia[];

}