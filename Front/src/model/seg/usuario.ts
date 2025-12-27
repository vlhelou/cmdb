// import { SegOrganograma } from './Organograma';
import { segEquipe } from 'src/model/seg/equipe';


export interface segUsuario {
    id: number;
    identificacao: string;
    administrador: boolean;
    ativo: boolean;
    local: boolean;
    email: string;

    locacoes: segEquipe[] | null;
}
