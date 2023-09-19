1. Create a management Resource Group, such as `prod-mattparkerdev-config`
2. Create a management Key Vault, such as `prod-mattparkerdev-vault`
   * Add secret `statusapp-database-admin-username`
   * Add secret `statusapp-database-admin-password`
3. Create the application Resource Group, such as `prod-statusapp`
4. Create GitHub repo for the application, such as `StatusApp`
5. Generate a service principal for GitHub Actions pipeline
   * `az ad sp create-for-rbac --name "prod-statusapp-cicd" --role contributor --scopes /subscriptions/{{YOUR SUBSCRIPTION ID}}/resourceGroups/prod-statusapp --sdk-auth`
   * Add the output to GitHub Secrets as `AZURE_CREDENTIALS`
   * Shape is:  
     ```
     {
         "clientId": "{{CLIENT ID}}",
         "clientSecret": "{{CLIENT SECRET}}",
         "tenantId": "{{TENANT ID}}",
     }
     ```
6. Give the service principal access to the Key Vault
   * `az keyvault set-policy -n "prod-mattparkerdev-vault" --secret-permissions get list --spn {{YOUR SERVICE PRINCIPAL ID (Get from previous step)}}`
