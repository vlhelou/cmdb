import { icIc } from 'src/model/ic/ic'
import { segOrganograma } from 'src/model/seg/organograma'
import { segUsuario } from 'src/model/seg/usuario'

export interface icSegredo {
    id: number;
    idIC: number;
    idUsuarioDono: number;
    idOrganogramaDono: number;
    ic: icIc;
    organogramaDono: segOrganograma;
    usuarioDono: segUsuario;
}