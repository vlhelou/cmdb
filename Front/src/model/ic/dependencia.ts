import { icIc } from "src/model/ic/ic";

export interface IcDependencia {
    id: number;
    idIcPrincipal: number;
    idIcDependente: number;
    observacao?: string;
    idAutor: number;
    dataAlteracao: Date;
    
    IcPrincipal?: icIc;
    IcDependente?: icIc;

}