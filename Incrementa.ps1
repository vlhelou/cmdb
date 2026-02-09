param(
    [string]$tipo,
    [string]$comentario
) 

$tipo = $tipo.ToLower()

# $tipo='patch'
# $comentario = "Incrementando versão $tipo"

$versaonOriginal = (git describe --tags --abbrev=0).Substring(1)

Write-Host  $versaonOriginal
# Validar se está no formato de semantic version (MAJOR.MINOR.PATCH)
$semanticVersionPattern = '^\d+\.\d+\.\d+(-[a-zA-Z0-9]+)*(\+[a-zA-Z0-9]+)*$'

if ($versaonOriginal -match $semanticVersionPattern) {
    Write-Host "Versão válida: $versaonOriginal"
    
} else {
    Write-Error "Versão inválida: $versaonOriginal (esperado formato: MAJOR.MINOR.PATCH)"
    exit 1
}


    # Dividir a versão em major, minor e patch
    $versionParts = $versaonOriginal -split '\.'
    $major = [int]$versionParts[0]
    $minor = [int]$versionParts[1]
    $patch = [int]($versionParts[2] -replace '[^0-9].*', '')
    
    Write-Host "Major: $major"
    Write-Host "Minor: $minor"
    Write-Host "Patch: $patch"

switch ($tipo) {
    "major" { 
        $major++
        $minor = 0
        $patch = 0
     }
    "minor" { 
        $minor++
        $patch = 0
     }
    "patch" { 
        $patch++
     }
    default {
        Write-Error "Tipo de incremento inválido: $tipo (esperado: major, minor ou patch)"
        exit 1
    }
}

$versao = "v$major.$minor.$patch"

git tag $versao -m "$comentario"
git push origin $versao

Write-Host "Nova versão: $versao com comentário: $comentario"