import { icIc } from "src/model/ic/ic";

export interface IcConhecimento {
    id: number;
    idIc: number;
    problema:string;
    solucao:string;
    idAutor: number;
    dataAlteracao: Date;
    
    ic?: icIc;
    
}