// import { SegOrganograma } from './Organograma';
// import { SegEquipe } from './Equipe';


export interface segUsuario {
    id: number;
    nome: string;
    email: string;
    senha: string;
    administrador: boolean;
    ativo: string;
    token: string;

    // Locacoes: SegEquipe[];
}
