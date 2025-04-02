export interface segOrganograma {
    id: number;
    idPai: number;
    nome: string;
    ativo: boolean;
    ativoFinal: boolean;
    nomeCompleto: string;
    listaAncestrais: string;
    nivel: number;
    lstAncestrais: number[];
    pai: segOrganograma;
    filhos: segOrganograma[];
    ancestrais: segOrganograma[];
}
