import { segUsuario } from "../seg/usuario";
import { segOrganograma } from "./organograma";


export interface segEquipe {
    id: number;
    idUsuario: number;   
    idOrganograma: number;
    idAutor: number;
    dataAlteracao: Date;
    usuario: segUsuario;
    organograma: segOrganograma;
}