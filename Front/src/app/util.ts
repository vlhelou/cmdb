import { AbstractControl, ValidationErrors, ValidatorFn } from "@angular/forms";

export const ptBr = {
    startsWith: 'Começa com',
    contains: 'Contém',
    notContains: 'Não contém',
    endsWith: 'Termina com',
    equals: 'Igual',
    notEquals: 'Diferente',
    noFilter: 'Sem filtro',
    lt: 'Menor',
    lte: 'Menor ou igual',
    gt: 'Maior',
    gte: 'Maior ou igual',
    is: 'Is',
    isNot: 'Is not',
    before: 'Before',
    after: 'After',
    clear: 'Clear',
    apply: 'Apply',
    matchAll: 'Match All',
    matchAny: 'Match Any',
    addRule: 'Add Rule',
    removeRule: 'Remove Rule',
    accept: 'Yes',
    // reject 'No',
    choose: 'Choose',
    upload: 'Upload',
    cancel: 'Cancel',
    dayNames: ['Domingo', 'Segunda', 'Terça', 'Quarta', 'Quinta', 'Sexta', 'Sábado'],
    dayNamesShort: ['Dom', 'Seg', 'Ter', 'Qua', 'Qui', 'Sex', 'Sab'],
    dayNamesMin: ['Dom', 'Seg', 'Ter', 'Qua', 'Qui', 'Sex', 'Sab'],
    monthNames: [
        'Janeiro', 'Fevereiro', 'Março', 'Abril', 'Maio', 'Junho', 'Julho', 'Agosto', 'Setembro', 'Outubro', 'Novembro', 'Dezembro'],
    monthNamesShort: ['Jan', 'Fev', 'Mar', 'Abr', 'Mai', 'Jun', 'Jul', 'Ago', 'Set', 'Out', 'Nov', 'Dez'],
    today: 'Today',
    weekHeader: 'Wk',
    weak: 'Weak',
    medium: 'Medium',
    strong: 'Strong',
    passwordPrompt: 'Enter a password',
    emptyMessage: 'No results found',
    emptyFilterMessage: 'No results found'
};


export function spinnerOn() {
    const blocker = document.getElementById("blocker");
    if (blocker) blocker.style.display = "block";
}

export function spinnerOff() {
    const blocker = document.getElementById("blocker");
    if (blocker) blocker.style.display = "none";
}

export function passwordsMatchValidator(
    passwordField: string,
    confirmField: string
): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
        const password = control.get(passwordField);
        const confirm = control.get(confirmField);

        if (!password || !confirm) return null;
        if (confirm.errors && !confirm.errors['passwordMismatch']) return null;

        return password.value === confirm.value ? null : { passwordMismatch: true };
    };
}
