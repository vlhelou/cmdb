// import { SegOrganograma } from './Organograma';
// import { SegEquipe } from './Equipe';


export interface segUsuario {
    id: number;
    identificacao: string;
    administrador: boolean;
    ativo: boolean;
    local: boolean;
    email: string;

    // Locacoes: SegEquipe[];
}
