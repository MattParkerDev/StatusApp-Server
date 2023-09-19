function RetrieveSecretFromKeyVault {
    param(
        [Parameter(Mandatory = $true)]
        [string] $secretName
    )
    $secretValue = ""
        try {
            $secretValue = az keyvault secret show --vault-name "prod-mattparkerdev-vault" --name $secretName --query "value" --output tsv
        }
        catch {
            write-host "ERROR - $secretName not found"
            throw $_
        }
    return $secretValue
}