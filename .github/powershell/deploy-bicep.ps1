. ($PSScriptRoot + "\functions.ps1") -Verbose # Import helper functions

az account show

$kv_db_username = RetrieveSecretFromKeyVault("statusapp-database-admin-username")
$kv_db_password = RetrieveSecretFromKeyVault("statusapp-database-admin-password")

az deployment group create --resource-group "Statusapp" `
                           --template-file "../bicep/main.bicep" `
                           --parameters "../bicep/main.parameters.json" "database_admin_username=$kv_db_username" "database_admin_password=$kv_db_password" `
                           --mode Incremental
