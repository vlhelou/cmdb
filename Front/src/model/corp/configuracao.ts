export interface corpConfiguracao {
    id: number;
    idPai?: number;
    nome: string;
    tipoValor: string;
    valorNumerico?: number;
    valorTexto?: string;
    valorData?: Date;
    valorComplexo?: object;
    valorSensivel: string;
    ajuda?: number;
    nomeCompleto: string;
    listaAncestrais: string;
    nivel: number;
    filhos?: corpConfiguracao[];
}