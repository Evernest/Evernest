#Requires -Version 3.0

<#
.SYNOPSIS
Crée et déploie un site Web Microsoft Azure pour un projet Web Visual Studio.
Pour plus de détails visitez le site à l'adresse suivante : http://go.microsoft.com/fwlink/?LinkID=394471 

.EXAMPLE
PS C:\> .\Publish-WebApplicationWebSite.ps1 `
-Configuration .\Configurations\WebApplication1-WAWS-dev.json `
-WebDeployPackage ..\WebApplication1\WebApplication1.zip `
-Verbose

#>
[CmdletBinding(HelpUri = 'http://go.microsoft.com/fwlink/?LinkID=391696')]
param
(
    [Parameter(Mandatory = $true)]
    [ValidateScript({Test-Path $_ -PathType Leaf})]
    [String]
    $Configuration,

    [Parameter(Mandatory = $false)]
    [String]
    $SubscriptionName,

    [Parameter(Mandatory = $false)]
    [ValidateScript({Test-Path $_ -PathType Leaf})]
    [String]
    $WebDeployPackage,

    [Parameter(Mandatory = $false)]
    [ValidateScript({ !($_ | Where-Object { !$_.Contains('Name') -or !$_.Contains('Password')}) })]
    [Hashtable[]]
    $DatabaseServerPassword,

    [Parameter(Mandatory = $false)]
    [Switch]
    $SendHostMessagesToOutput = $false
)


function New-WebDeployPackage
{
    #Écrire une fonction pour générer et empaqueter votre application Web

    #Pour générer votre application Web, utilisez MsBuild.exe. Pour obtenir de l'aide, consultez les informations de référence relatives à la syntaxe de ligne de commande de MSBuild à l'adresse suivante : http://go.microsoft.com/fwlink/?LinkId=391339
}

function Test-WebApplication
{
    #Modifier cette fonction pour exécuter des tests unitaires sur votre application Web

    #Pour écrire une fonction permettant d'exécuter des tests unitaires sur votre application Web, utilisez VSTest.Console.exe. Pour obtenir de l'aide, consultez les informations de référence relatives à la syntaxe de ligne de commande de VSTest.Console à l'adresse suivante : http://go.microsoft.com/fwlink/?LinkId=391340
}

function New-AzureWebApplicationWebsiteEnvironment
{
    [CmdletBinding()]
    param
    (
        [Parameter(Mandatory = $true)]
        [Object]
        $Configuration,

        [Parameter (Mandatory = $false)]
        [AllowNull()]
        [Hashtable[]]
        $DatabaseServerPassword
    )
       
    Add-AzureWebsite -Name $Config.name -Location $Config.location | Out-String | Write-HostWithTime
    # Créez les bases de données SQL. La chaîne de connexion est utilisée pour le déploiement.
    $connectionString = New-Object -TypeName Hashtable
    
    if ($Config.Contains('databases'))
    {
        @($Config.databases) |
            Where-Object {$_.connectionStringName -ne ''} |
            Add-AzureSQLDatabases -DatabaseServerPassword $DatabaseServerPassword -CreateDatabase |
            ForEach-Object { $connectionString.Add($_.Name, $_.ConnectionString) }           
    }
    
    return @{ConnectionString = $connectionString}   
}

function Publish-AzureWebApplicationToWebsite
{
    [CmdletBinding()]
    param
    (
        [Parameter(Mandatory = $true)]
        [Object]
        $Configuration,

        [Parameter(Mandatory = $false)]
        [AllowNull()]
        [Hashtable]
        $ConnectionString,

        [Parameter(Mandatory = $true)]
        [ValidateScript({Test-Path $_ -PathType Leaf})]
        [String]
        $WebDeployPackage
    )

    if ($ConnectionString -and $ConnectionString.Count -gt 0)
    {
        Publish-AzureWebsiteProject `
            -Name $Config.name `
            -Package $WebDeployPackage `
            -ConnectionString $ConnectionString
    }
    else
    {
        Publish-AzureWebsiteProject `
            -Name $Config.name `
            -Package $WebDeployPackage
    }
}


# Routine principale du script
Set-StrictMode -Version 3

Remove-Module AzureWebSitePublishModule -ErrorAction SilentlyContinue
$scriptDirectory = Split-Path -Parent $PSCmdlet.MyInvocation.MyCommand.Definition
Import-Module ($scriptDirectory + '\AzureWebSitePublishModule.psm1') -Scope Local -Verbose:$false

New-Variable -Name VMWebDeployWaitTime -Value 30 -Option Constant -Scope Script 
New-Variable -Name AzureWebAppPublishOutput -Value @() -Scope Global -Force
New-Variable -Name SendHostMessagesToOutput -Value $SendHostMessagesToOutput -Scope Global -Force

try
{
    $originalErrorActionPreference = $Global:ErrorActionPreference
    $originalVerbosePreference = $Global:VerbosePreference
    
    if ($PSBoundParameters['Verbose'])
    {
        $Global:VerbosePreference = 'Continue'
    }
    
    $scriptName = $MyInvocation.MyCommand.Name + ':'
    
    Write-VerboseWithTime ($scriptName + ' Démarrer')
    
    $Global:ErrorActionPreference = 'Stop'
    Write-VerboseWithTime ('{0} $ErrorActionPreference a la valeur {1}' -f $scriptName, $ErrorActionPreference)
    
    Write-Debug ('{0} : $PSCmdlet.ParameterSetName = {1}' -f $scriptName, $PSCmdlet.ParameterSetName)

    # Enregistrez l'abonnement actif. Il sera restauré à l'état Actif plus tard dans le script
    Backup-Subscription -UserSpecifiedSubscription $SubscriptionName
    
    # Vérifiez que vous disposez du module Windows Azure, version 0.7.4 ou ultérieure.
    if (-not (Test-AzureModule))
    {
         throw 'Vous avez une version obsolète de Microsoft Azure PowerShell. Pour installer la dernière version, accédez à http://go.microsoft.com/fwlink/?LinkID=320552.'
    }
    
    if ($SubscriptionName)
    {

        # Si vous avez fourni un nom d'abonnement, vérifiez que l'abonnement existe dans votre compte.
        if (!(Get-AzureSubscription -SubscriptionName $SubscriptionName))
        {
            throw ("{0} : impossible de trouver le nom d'abonnement $SubscriptionName" -f $scriptName)

        }

        # Définissez l'abonnement spécifié à l'état actif.
        Select-AzureSubscription -SubscriptionName $SubscriptionName | Out-Null

        Write-VerboseWithTime ('{0} : l'abonnement a la valeur {1}' -f $scriptName, $SubscriptionName)
    }

    $Config = Read-ConfigFile $Configuration 

    #Générer et empaqueter votre application Web
    New-WebDeployPackage

    #Exécuter des tests unitaires sur votre application Web
    Test-WebApplication

    #Créer l'environnement Windows Azure décrit dans le fichier de configuration JSON
    $newEnvironmentResult = New-AzureWebApplicationWebsiteEnvironment -Configuration $Config -DatabaseServerPassword $DatabaseServerPassword

    #Déployer le package d'applications Web si $WebDeployPackage est spécifié par l'utilisateur 
    if($WebDeployPackage)
    {
        Publish-AzureWebApplicationToWebsite `
            -Configuration $Config `
            -ConnectionString $newEnvironmentResult.ConnectionString `
            -WebDeployPackage $WebDeployPackage
    }
}
finally
{
    $Global:ErrorActionPreference = $originalErrorActionPreference
    $Global:VerbosePreference = $originalVerbosePreference

    # Restaurer l'abonnement actif d'origine à l'état Actif
    Restore-Subscription

    Write-Output $Global:AzureWebAppPublishOutput    
    $Global:AzureWebAppPublishOutput = @()
}
