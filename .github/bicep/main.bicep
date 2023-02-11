param site_name string
param server_farm_name string
param database_server_name string
param server_region string

resource postgres_flexible_server 'Microsoft.DBforPostgreSQL/flexibleServers@2022-12-01' = {
  name: database_server_name
  location: server_region
  sku: {
    name: 'Standard_B1ms'
    tier: 'Burstable'
  }
  properties: {
    authConfig: {
      activeDirectoryAuth: 'Disabled'
      passwordAuth: 'Enabled'
    }
    dataEncryption: {
      type: 'SystemManaged'
    }
    version: '14'
    availabilityZone: '1'
    storage: {
      storageSizeGB: 32
    }
    backup: {
      backupRetentionDays: 7
      geoRedundantBackup: 'Disabled'
    }
    network: {
    }
    highAvailability: {
      mode: 'Disabled'
    }
    maintenanceWindow: {
      customWindow: 'Disabled'
      dayOfWeek: 0
      startHour: 0
      startMinute: 0
    }
    replicationRole: 'Primary'
    replicaCapacity: 5
  }
}

resource server_farm 'Microsoft.Web/serverfarms@2022-03-01' = {
  name: server_farm_name
  location: server_region
  sku: {
    name: 'B1'
    tier: 'Basic'
    size: 'B1'
    family: 'B'
    capacity: 1
  }
  kind: 'linux'
  properties: {
    perSiteScaling: false
    elasticScaleEnabled: false
    maximumElasticWorkerCount: 1
    isSpot: false
    freeOfferExpirationTime: '2023-05-14T10:53:10.3633333'
    reserved: true
    isXenon: false
    hyperV: false
    targetWorkerCount: 0
    targetWorkerSizeId: 0
    zoneRedundant: false
  }
}


resource web_site 'Microsoft.Web/sites@2022-03-01' = {
  name: site_name
  location: server_region
  kind: 'app,linux'
  properties: {
    enabled: true
    hostNameSslStates: [
      {
        name: '${site_name}.azurewebsites.net'
        sslState: 'Disabled'
        hostType: 'Standard'
      }
      {
        name: '${site_name}.scm.azurewebsites.net'
        sslState: 'Disabled'
        hostType: 'Repository'
      }
    ]
    serverFarmId: server_farm.id
    reserved: true
    httpsOnly: true
    siteConfig: {
      numberOfWorkers: 1
      netFrameworkVersion: 'v7.0'
      linuxFxVersion: 'DOTNETCORE|7.0'
      alwaysOn: false
      http20Enabled: false
    } 
  }
}
