export interface segOrganograma {
    id: number;
    idPai: number|null;
    nome: string;
    ativo: boolean;
    ativoFinal: boolean;
    nomeCompleto: string;
    listaAncestrais: string;
    nivel: number;
    lstAncestrais: number[];
    pai: segOrganograma|null;
    filhos: segOrganograma[]|null;
    ancestrais: segOrganograma[]|null;
}
