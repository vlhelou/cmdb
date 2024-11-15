// import { SegOrganograma } from './Organograma';
// import { SegEquipe } from './Equipe';


export interface segUsuario {
    Id: number;
    Nome: string;
    Email: string;
    Senha: string;
    Administrador: string;
    Ativo: string;
    token: string;

    // Locacoes: SegEquipe[];
}
