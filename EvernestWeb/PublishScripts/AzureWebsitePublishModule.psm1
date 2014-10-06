#  AzureWebSitePublishModule.psm1 est un module de script Windows PowerShell. Ce module exporte des fonctions Windows PowerShell qui automatisent la gestion du cycle de vie pour les applications Web. Vous pouvez utiliser ces fonctions en l'état ou les personnaliser pour votre application et votre environnement de publication.

Set-StrictMode -Version 3

# Variable d'enregistrement de l'abonnement d'origine.
$Script:originalCurrentSubscription = $null

# Variable d'enregistrement du compte de stockage d'origine.
$Script:originalCurrentStorageAccount = $null

# Variable d'enregistrement du compte de stockage de l'abonnement spécifique à l'utilisateur.
$Script:originalStorageAccountOfUserSpecifiedSubscription = $null

# Variable d'enregistrement du nom de l'abonnement.
$Script:userSpecifiedSubscription = $null


<#
.SYNOPSIS
Indique la date et l'heure avant un message.

.DESCRIPTION
Indique la date et l'heure avant un message. Cette fonction est conçue pour les messages écrits dans les flux Error et Verbose.

.PARAMETER  Message
Spécifie les messages sans la date.

.INPUTS
System.String

.OUTPUTS
System.String

.EXAMPLE
PS C:\> Format-DevTestMessageWithTime -Message "Ajout du fichier $filename à l'annuaire"
2/5/2014 1:03:08 PM - Ajout du fichier $filename à l'annuaire

.LINK
Write-VerboseWithTime

.LINK
Write-ErrorWithTime
#>
function Format-DevTestMessageWithTime
{
    [CmdletBinding()]
    param
    (
        [Parameter(Position=0, Mandatory = $true, ValueFromPipeline = $true)]
        [String]
        $Message
    )

    return ((Get-Date -Format G)  + ' - ' + $Message)
}


<#

.SYNOPSIS
Écrit un message d'erreur précédé de l'heure actuelle.

.DESCRIPTION
Écrit un message d'erreur précédé de l'heure actuelle. Cette fonction appelle la fonction Format-DevTestMessageWithTime pour ajouter l'heure au début du message avant de l'écrire dans le flux Error.

.PARAMETER  Message
Spécifie le message dans l'appel du message d'erreur. Vous pouvez utiliser le pipe de la chaîne de message pour la fonction.

.INPUTS
System.String

.OUTPUTS
Aucune. La fonction écrit dans le flux Error.

.EXAMPLE
PS C:> Write-ErrorWithTime -Message "Failed. Cannot find the file."

Write-Error: 2/6/2014 8:37:29 AM - Failed. Cannot find the file.
 + CategoryInfo     : NotSpecified: (:) [Write-Error], WriteErrorException
 + FullyQualifiedErrorId : Microsoft.PowerShell.Commands.WriteErrorException

.LINK
Write-Error

#>
function Write-ErrorWithTime
{
    [CmdletBinding()]
    param
    (
        [Parameter(Position=0, Mandatory = $true, ValueFromPipeline = $true)]
        [String]
        $Message
    )

    $Message | Format-DevTestMessageWithTime | Write-Error
}


<#
.SYNOPSIS
Écrit un message détaillé précédé de l'heure actuelle.

.DESCRIPTION
Écrit un message détaillé précédé de l'heure actuelle. Dans la mesure où il appelle Write-Verbose, le message ne s'affiche que lorsque le script s'exécute avec le paramètre Verbose ou que la préférence VerbosePreference a la valeur Continue.

.PARAMETER  Message
Spécifie le message dans l'appel du message détaillé. Vous pouvez utiliser le pipe de la chaîne de message pour la fonction.

.INPUTS
System.String

.OUTPUTS
Aucune. La fonction écrit dans le flux Verbose.

.EXAMPLE
PS C:> Write-VerboseWithTime -Message "The operation succeeded."
PS C:>
PS C:\> Write-VerboseWithTime -Message "The operation succeeded." -Verbose
VERBOSE: 1/27/2014 11:02:37 AM - The operation succeeded.

.EXAMPLE
PS C:\ps-test> "The operation succeeded." | Write-VerboseWithTime -Verbose
VERBOSE: 1/27/2014 11:01:38 AM - The operation succeeded.

.LINK
Write-Verbose
#>
function Write-VerboseWithTime
{
    [CmdletBinding()]
    param
    (
        [Parameter(Position=0, Mandatory = $true, ValueFromPipeline = $true)]
        [String]
        $Message
    )

    $Message | Format-DevTestMessageWithTime | Write-Verbose
}


<#
.SYNOPSIS
Écrit un message d'hôte précédé de l'heure actuelle.

.DESCRIPTION
Cette fonction écrit un message au programme hôte (Write-Host) en le faisant précéder de l'heure actuelle. L'effet de l'écriture au programme hôte varie. La plupart des programmes qui hébergent Windows PowerShell écrivent ces messages vers la sortie standard.

.PARAMETER  Message
Spécifie le message de base sans la date. Vous pouvez utiliser le pipe de la chaîne de message pour la fonction.

.INPUTS
System.String

.OUTPUTS
Aucune. La fonction écrit le message au programme hôte.

.EXAMPLE
PS C:> Write-HostWithTime -Message "L'opération a réussi."
1/27/2014 11:02:37 AM - L'opération a réussi.

.LINK
Write-Host
#>
function Write-HostWithTime
{
    [CmdletBinding()]
    param
    (
        [Parameter(Position=0, Mandatory = $true, ValueFromPipeline = $true)]
        [String]
        $Message
    )
    
    if ((Get-Variable SendHostMessagesToOutput -Scope Global -ErrorAction SilentlyContinue) -and $Global:SendHostMessagesToOutput)
    {
        if (!(Get-Variable -Scope Global AzureWebAppPublishOutput -ErrorAction SilentlyContinue) -or !$Global:AzureWebAppPublishOutput)
        {
            New-Variable -Name AzureWebAppPublishOutput -Value @() -Scope Global -Force
        }

        $Global:AzureWebAppPublishOutput += $Message | Format-DevTestMessageWithTime
    }
    else 
    {
        $Message | Format-DevTestMessageWithTime | Write-Host
    }
}


<#
.SYNOPSIS
Retourne $true si une propriété ou une méthode est membre de l'objet. Sinon, $false.

.DESCRIPTION
Retourne $true si la propriété ou la méthode est membre de l'objet. Cette fonction retourne $false pour les méthodes statiques de la classe et pour les vues, par exemple PSBase et PSObject.

.PARAMETER  Object
Spécifie l'objet dans le test. Entrez une variable qui contient un objet ou une expression qui retourne un objet. Vous ne pouvez pas spécifier de types, par exemple [DateTime], ou utiliser le pipe d'objets pour cette fonction.

.PARAMETER  Member
Spécifie le nom de la propriété ou de la méthode dans le test. Lors de la spécification d'une méthode, omettez les parenthèses placées à la suite du nom de la méthode.

.INPUTS
Aucune. Cette fonction n'accepte aucune entrée du pipeline.

.OUTPUTS
System.Boolean

.EXAMPLE
PS C:\> Test-Member -Object (Get-Date) -Member DayOfWeek
True

.EXAMPLE
PS C:\> $date = Get-Date
PS C:\> Test-Member -Object $date -Member AddDays
True

.EXAMPLE
PS C:\> [DateTime]::IsLeapYear((Get-Date).Year)
True
PS C:\> Test-Member -Object (Get-Date) -Member IsLeapYear
False

.LINK
Get-Member
#>
function Test-Member
{
    [CmdletBinding()]
    param
    (
        [Parameter(Mandatory = $true)]
        [Object]
        $Object,

        [Parameter(Mandatory = $true)]
        [String]
        $Member
    )

    return $null -ne ($Object | Get-Member -Name $Member)
}


<#
.SYNOPSIS
Retourne $true si le module Windows Azure correspond à la version 0.7.4 ou une version ultérieure. Sinon, $false.

.DESCRIPTION
Test-AzureModuleVersion retourne $true si le module Azure correspond à la version 0.7.4 ou une version ultérieure. Elle retourne $false si le module n'est pas installé ou s'il correspond à une version antérieure. Cette fonction n'a aucun paramètre.

.INPUTS
Aucun

.OUTPUTS
System.Boolean

.EXAMPLE
PS C:\> Get-Module Azure -ListAvailable
PS C:\> #No module
PS C:\> Test-AzureModuleVersion
False

.EXAMPLE
PS C:\> (Get-Module Azure -ListAvailable).Version

Major  Minor  Build  Revision
-----  -----  -----  --------
0      7      4      -1

PS C:\> Test-AzureModuleVersion
True

.LINK
Get-Module

.LINK
PSModuleInfo object (http://msdn.microsoft.com/en-us/library/system.management.automation.psmoduleinfo(v=vs.85).aspx)
#>
function Test-AzureModuleVersion
{
    [CmdletBinding()]
    param
    (
        [Parameter(Mandatory = $true)]
        [ValidateNotNull()]
        [System.Version]
        $Version
    )

    return ($Version.Major -gt 0) -or ($Version.Minor -gt 7) -or ($Version.Minor -eq 7 -and $Version.Build -ge 4)
}


<#
.SYNOPSIS
Retourne $true si le module Windows Azure installé correspond à la version 0.7.4 ou une version ultérieure.

.DESCRIPTION
Test-AzureModule retourne $true si le module Windows Azure installé correspond à la version 0.7.4 ou une version ultérieure. Retourne $false si le module n'est pas installé ou s'il correspond à une version antérieure. Cette fonction n'a aucun paramètre.

.INPUTS
Aucun

.OUTPUTS
System.Boolean

.EXAMPLE
PS C:\> Get-Module Azure -ListAvailable
PS C:\> #No module
PS C:\> Test-AzureModule
False

.EXAMPLE
PS C:\> (Get-Module Azure -ListAvailable).Version

Major  Minor  Build  Revision
-----  -----  -----  --------
    0      7      4      -1

PS C:\> Test-AzureModule
True

.LINK
Get-Module

.LINK
PSModuleInfo object (http://msdn.microsoft.com/en-us/library/system.management.automation.psmoduleinfo(v=vs.85).aspx)
#>
function Test-AzureModule
{
    [CmdletBinding()]

    $module = Get-Module -Name Azure

    if (!$module)
    {
        $module = Get-Module -Name Azure -ListAvailable

        if (!$module -or !(Test-AzureModuleVersion $module.Version))
        {
            return $false;
        }
        else
        {
            $ErrorActionPreference = 'Continue'
            Import-Module -Name Azure -Global -Verbose:$false
            $ErrorActionPreference = 'Stop'

            return $true
        }
    }
    else
    {
        return (Test-AzureModuleVersion $module.Version)
    }
}


<#
.SYNOPSIS
Enregistre l'abonnement Microsoft Azure actif dans la variable $Script:originalSubscription du script.

.DESCRIPTION
La fonction Backup-Subscription enregistre l'abonnement Microsoft Azure actif (Get-AzureSubscription -Current) et son compte de stockage, ainsi que l'abonnement modifié par ce script ($UserSpecifiedSubscription) et son compte de stockage, dans la portée du script. En enregistrant les valeurs, vous pouvez utiliser une fonction telle que Restore-Subscription pour restaurer l'abonnement actif d'origine et son compte de stockage à l'état actif, si ce dernier a changé.

.PARAMETER UserSpecifiedSubscription
Spécifie le nom de l'abonnement dans lequel les ressources doivent être créées et publiées. La fonction enregistre les noms de l'abonnement et de ses comptes de stockage dans la portée du script. Ce paramètre est obligatoire.

.INPUTS
Aucun

.OUTPUTS
Aucun

.EXAMPLE
PS C:\> Backup-Subscription -UserSpecifiedSubscription Contoso
PS C:\>

.EXAMPLE
PS C:\> Backup-Subscription -UserSpecifiedSubscription Contoso -Verbose
VERBOSE: Backup-Subscription: Start
VERBOSE: Backup-Subscription: Original subscription is Microsoft Azure MSDN - Visual Studio Ultimate
VERBOSE: Backup-Subscription: End
#>
function Backup-Subscription
{
    [CmdletBinding()]
    param
    (
        [Parameter(Mandatory = $true)]
        [AllowEmptyString()]
        [string]
        $UserSpecifiedSubscription
    )

    Write-VerboseWithTime 'Backup-Subscription : début'

    $Script:originalCurrentSubscription = Get-AzureSubscription -Current -ErrorAction SilentlyContinue
    if ($Script:originalCurrentSubscription)
    {
        Write-VerboseWithTime ('Backup-Subscription : l'abonnement d'origine est ' + $Script:originalCurrentSubscription.SubscriptionName)
        $Script:originalCurrentStorageAccount = $Script:originalCurrentSubscription.CurrentStorageAccountName
    }
    
    $Script:userSpecifiedSubscription = $UserSpecifiedSubscription
    if ($Script:userSpecifiedSubscription)
    {        
        $userSubscription = Get-AzureSubscription -SubscriptionName $Script:userSpecifiedSubscription -ErrorAction SilentlyContinue
        if ($userSubscription)
        {
            $Script:originalStorageAccountOfUserSpecifiedSubscription = $userSubscription.CurrentStorageAccountName
        }        
    }

    Write-VerboseWithTime 'Backup-Subscription : fin'
}


<#
.SYNOPSIS
Restaure à l'état "actif" l'abonnement Microsoft Azure enregistré dans la variable $Script:originalSubscription du script.

.DESCRIPTION
La fonction Restore-Subscription rend (de nouveau) actif l'abonnement enregistré dans la variable $Script:originalSubscription. Si l'abonnement d'origine a un compte de stockage, cette fonction rend ce compte de stockage actif pour l'abonnement actif. La fonction restaure l'abonnement uniquement s'il existe une variable $SubscriptionName non Null dans l'environnement. Sinon, son exécution s'arrête. Si $SubscriptionName est rempli, mais que $Script:originalSubscription a la valeur $null, Restore-Subscription utilise la cmdlet Select-AzureSubscription pour effacer les paramètres actuels et par défaut des abonnements dans Microsoft Azure PowerShell. Cette fonction n'a aucun paramètre, elle n'accepte aucune entrée et ne retourne rien (void). Vous pouvez utiliser -Verbose pour écrire des messages dans le flux Verbose.

.INPUTS
Aucun

.OUTPUTS
Aucun

.EXAMPLE
PS C:\> Restore-Subscription
PS C:\>

.EXAMPLE
PS C:\> Restore-Subscription -Verbose
VERBOSE: Restore-Subscription: Start
VERBOSE: Restore-Subscription: End
#>
function Restore-Subscription
{
    [CmdletBinding()]
    param()

    Write-VerboseWithTime 'Restore-Subscription : début'

    if ($Script:originalCurrentSubscription)
    {
        if ($Script:originalCurrentStorageAccount)
        {
            Set-AzureSubscription `
                -SubscriptionName $Script:originalCurrentSubscription.SubscriptionName `
                -CurrentStorageAccountName $Script:originalCurrentStorageAccount
        }

        Select-AzureSubscription -SubscriptionName $Script:originalCurrentSubscription.SubscriptionName
    }
    else 
    {
        Select-AzureSubscription -NoCurrent
        Select-AzureSubscription -NoDefault
    }
    
    if ($Script:userSpecifiedSubscription -and $Script:originalStorageAccountOfUserSpecifiedSubscription)
    {
        Set-AzureSubscription `
            -SubscriptionName $Script:userSpecifiedSubscription `
            -CurrentStorageAccountName $Script:originalStorageAccountOfUserSpecifiedSubscription
    }

    Write-VerboseWithTime 'Restore-Subscription : fin'
}


<#
.SYNOPSIS
Valide le fichier de configuration et retourne une table de hachage des valeurs du fichier de configuration.

.DESCRIPTION
La fonction Read-ConfigFile valide le fichier de configuration JSON et retourne une table de hachage des valeurs sélectionnées.
-- Il convertit d'abord le fichier JSON en un PSCustomObject. La table de hachage du site Web contient les clés suivantes :
-- Location: Emplacement du site Web
-- Databases: Bases de données SQL du site Web

.PARAMETER  ConfigurationFile
Spécifie le chemin d'accès et le nom du fichier de configuration JSON de votre projet Web. Visual Studio génère le fichier JSON automatiquement lorsque vous créez un projet Web et que vous le stockez dans le dossier PublishScripts de votre solution.

.PARAMETER HasWebDeployPackage
Indique la présence d'un fichier ZIP de package Web Deploy pour l'application Web. Pour spécifier une valeur de $true, utilisez -HasWebDeployPackage ou HasWebDeployPackage:$true. Pour spécifier une valeur de false, utilisez HasWebDeployPackage:$false. Ce paramètre est obligatoire.

.INPUTS
Aucune. Vous ne pouvez pas utiliser le pipe d'entrée pour cette fonction.

.OUTPUTS
System.Collections.Hashtable

.EXAMPLE
PS C:\> Read-ConfigFile -ConfigurationFile <path> -HasWebDeployPackage


Name                           Value                                                                                                                                                                     
----                           -----                                                                                                                                                                     
databases                      {@{connectionStringName=; databaseName=; serverName=; user=; password=}}                                                                                                  
website                        @{name="mysite"; location="West US";}                                                      
#>
function Read-ConfigFile
{
    [CmdletBinding()]
    param
    (
        [Parameter(Mandatory = $true)]
        [ValidateScript({Test-Path $_ -PathType Leaf})]
        [String]
        $ConfigurationFile
    )

    Write-VerboseWithTime 'Read-ConfigFile : début'

    # Obtenir le contenu du fichier JSON (-raw ignore les sauts de ligne) et le convertir en PSCustomObject
    $config = Get-Content $ConfigurationFile -Raw | ConvertFrom-Json

    if (!$config)
    {
        throw ('Échec de Read-ConfigFile : ConvertFrom-Json : ' + $error[0])
    }

    # Déterminer si l'objet environmentSettings a les propriétés 'webSite' (quelle que soit la valeur de la propriété)
    $hasWebsiteProperty =  Test-Member -Object $config.environmentSettings -Member 'webSite'

    if (!$hasWebsiteProperty)
    {
        throw 'Read-ConfigFile : le fichier de configuration ne contient pas de propriété webSite.'
    }

    # Créer une table de hachage à partir des valeurs de PSCustomObject
    $returnObject = New-Object -TypeName Hashtable

    $returnObject.Add('name', $config.environmentSettings.webSite.name)
    $returnObject.Add('location', $config.environmentSettings.webSite.location)

    if (Test-Member -Object $config.environmentSettings -Member 'databases')
    {
        $returnObject.Add('databases', $config.environmentSettings.databases)
    }

    Write-VerboseWithTime 'Read-ConfigFile : fin'

    return $returnObject
}


<#
.SYNOPSIS
Crée un site Web Microsoft Azure.

.DESCRIPTION
Crée un site Web Microsoft Azure avec un nom et un emplacement spécifiques. Cette fonction appelle l'applet de commande New-AzureWebsite dans le module Azure. Si l'abonnement n'a pas encore de site Web avec le nom spécifié, cette fonction crée le site Web et retourne un objet de site Web. Si ce n'est pas le cas, elle retourne le site Web existant.

.PARAMETER  Name
Spécifie un nom pour le nouveau site Web. Le nom doit être unique dans Microsoft Azure. Ce paramètre est obligatoire.

.PARAMETER  Location
Spécifie l'emplacement du site Web. Les valeurs valides correspondent aux emplacements de Microsoft Azure, par exemple "Ouest des États-Unis". Ce paramètre est obligatoire.

.INPUTS
AUCUNE.

.OUTPUTS
Microsoft.WindowsAzure.Commands.Utilities.Websites.Services.WebEntities.Site

.EXAMPLE
Add-AzureWebsite -Name TestSite -Location "West US"

Name       : contoso
State      : Running
Host Names : contoso.azurewebsites.net

.LINK
New-AzureWebsite
#>
function Add-AzureWebsite
{
    [CmdletBinding()]
    param
    (
        [Parameter(Mandatory = $true)]
        [String]
        $Name,

        [Parameter(Mandatory = $true)]
        [String]
        $Location
    )

    Write-VerboseWithTime 'Add-AzureWebsite : début'
    $website = Get-AzureWebsite -Name $Name -ErrorAction SilentlyContinue

    if ($website)
    {
        Write-HostWithTime ('Add-AzureWebsite : site Web existant ' +
        $website.Name + ' trouvé')
    }
    else
    {
        if (Test-AzureName -Website -Name $Name)
        {
            Write-ErrorWithTime ('Le site Web {0} existe déjà' -f $Name)
        }
        else
        {
            $website = New-AzureWebsite -Name $Name -Location $Location
        }
    }

    $website | Out-String | Write-VerboseWithTime
    Write-VerboseWithTime 'Add-AzureWebsite : fin'

    return $website
}

<#
.SYNOPSIS
Retourne $True lorsque l'URL est absolue et que son modèle est https.

.DESCRIPTION
La fonction Test-HttpsUrl convertit l'URL d'entrée en objet System.Uri. Retourne $True lorsque l'URL est absolue (non relative) et que son modèle est https. Si la valeur est false dans les deux cas, ou si la chaîne d'entrée ne peut pas être convertie en URL, la fonction retourne $false.

.PARAMETER Url
Spécifie l'URL à tester. Entrez une chaîne d'URL,

.INPUTS
AUCUNE.

.OUTPUTS
System.Boolean

.EXAMPLE
PS C:\>$profile.publishUrl
waws-prod-bay-001.publish.azurewebsites.windows.net:443

PS C:\>Test-HttpsUrl -Url 'waws-prod-bay-001.publish.azurewebsites.windows.net:443'
False
#>
function Test-HttpsUrl
{

    param
    (
        [Parameter(Mandatory = $true)]
        [String]
        $Url
    )

    # Si $uri ne peut pas être converti en objet System.Uri, Test-HttpsUrl retourne $false
    $uri = $Url -as [System.Uri]

    return $uri.IsAbsoluteUri -and $uri.Scheme -eq 'https'
}


<#
.SYNOPSIS
Crée une chaîne qui vous permet de vous connecter à une base de données SQL Microsoft Azure.

.DESCRIPTION
La fonction Get-AzureSQLDatabaseConnectionString assemble une chaîne de connexion pour se connecter à une base de données SQL Microsoft Azure.

.PARAMETER  DatabaseServerName
Spécifie le nom d'un serveur de base de données existant dans l'abonnement Microsoft Azure. Toutes les bases de données SQL Microsoft Azure doivent être associées à un serveur de base de données SQL. Pour obtenir le nom du serveur, utilisez la cmdlet Get-AzureSqlDatabaseServer (module Microsoft Azure). Ce paramètre est obligatoire.

.PARAMETER  DatabaseName
Spécifie le nom de la base de données SQL. Il peut s'agir d'une base de données SQL existante ou d'un nom utilisé pour une nouvelle base de données SQL. Ce paramètre est obligatoire.

.PARAMETER  Username
Spécifie le nom de l'administrateur de base de données SQL. Le nom d'utilisateur est $Username@DatabaseServerName. Ce paramètre est obligatoire.

.PARAMETER  Password
Spécifie le mot de passe de l'administrateur de base de données SQL. Entrez un mot de passe en texte clair. Les chaînes sécurisées ne sont pas autorisées. Ce paramètre est obligatoire.

.INPUTS
Aucune.

.OUTPUTS
System.String

.EXAMPLE
PS C:\> $ServerName = (Get-AzureSqlDatabaseServer).ServerName[0]
PS C:\> Get-AzureSQLDatabaseConnectionString -DatabaseServerName $ServerName `
        -DatabaseName 'testdb' -UserName 'admin'  -Password 'password'

Server=tcp:testserver.database.windows.net,1433;Database=testdb;User ID=admin@testserver;Password=password;Trusted_Connection=False;Encrypt=True;Connection Timeout=20;
#>
function Get-AzureSQLDatabaseConnectionString
{
    [CmdletBinding()]
    param
    (
        [Parameter(Mandatory = $true)]
        [String]
        $DatabaseServerName,

        [Parameter(Mandatory = $true)]
        [String]
        $DatabaseName,

        [Parameter(Mandatory = $true)]
        [String]
        $UserName,

        [Parameter(Mandatory = $true)]
        [String]
        $Password
    )

    return ('Server=tcp:{0}.database.windows.net,1433;Database={1};' +
           'User ID={2}@{0};' +
           'Password={3};' +
           'Trusted_Connection=False;' +
           'Encrypt=True;' +
           'Connection Timeout=20;') `
           -f $DatabaseServerName, $DatabaseName, $UserName, $Password
}


<#
.SYNOPSIS
Crée des bases de données SQL Microsoft Azure à partir des valeurs du fichier de configuration JSON généré par Visual Studio.

.DESCRIPTION
La fonction Add-AzureSQLDatabases accepte les informations de la section databases du fichier JSON. Cette fonction, Add-AzureSQLDatabases (pluriel), appelle la fonction Add-AzureSQLDatabase (singulier) pour chaque base de données SQL du fichier JSON. Add-AzureSQLDatabase (singulier) appelle la cmdlet New-AzureSqlDatabase (module Windows Azure), qui crée les bases de données SQL. Cette fonction ne retourne pas d'objet de base de données. Elle retourne une table de hachage des valeurs utilisées pour créer les bases de données.

.PARAMETER DatabaseConfig
 Accepte un tableau de PSCustomObjects qui proviennent du fichier JSON retourné par la fonction Read-ConfigFile lorsque le fichier JSON possède une propriété de site Web. Cela inclut les propriétés de environmentSettings.databases. Vous pouvez utiliser le pipe de la liste pour cette fonction.
PS C:\> $config = Read-ConfigFile <name>.json
PS C:\> $DatabaseConfig = $config.databases| where {$_.connectionStringName}
PS C:\> $DatabaseConfig
connectionStringName: Default Connection
databasename : TestDB1
edition   :
size     : 1
collation  : SQL_Latin1_General_CP1_CI_AS
servertype  : New SQL Database Server
servername  : r040tvt2gx
user     : dbuser
password   : Test.123
location   : West US

.PARAMETER  DatabaseServerPassword
Spécifie le mot de passe pour l'administrateur du serveur de base de données SQL. Entrez une table de hachage avec les clés Nom et Mot de passe. La valeur de Nom correspond au nom du serveur de base de données SQL. La valeur de Mot de passe correspond au mot de passe de l'administrateur. Par exemple : @Name = "TestDB1"; Password = "password" Ce paramètre est facultatif. Si vous l'omettez ou si le nom du serveur de base de données SQL ne correspond pas à la valeur de la propriété serverName de l'objet $DatabaseConfig la fonction utilise la propriété Mot de passe de l'objet $DatabaseConfig pour la base de données SQL dans la chaîne de connexion.

.PARAMETER CreateDatabase
Vérifie que vous souhaitez créer une base de données. Ce paramètre est facultatif.

.INPUTS
System.Collections.Hashtable[]

.OUTPUTS
System.Collections.Hashtable

.EXAMPLE
PS C:\> $config = Read-ConfigFile <name>.json
PS C:\> $DatabaseConfig = $config.databases| where {$_.connectionStringName}
PS C:\> $DatabaseConfig | Add-AzureSQLDatabases

Name                           Value
----                           -----
ConnectionString               Server=tcp:testdb1.database.windows.net,1433;Database=testdb;User ID=admin@testdb1;Password=password;Trusted_Connection=False;Encrypt=True;Connection Timeout=20;
Name                           Default Connection
Type                           SQLAzure

.LINK
Get-AzureSQLDatabaseConnectionString

.LINK
Create-AzureSQLDatabase
#>
function Add-AzureSQLDatabases
{
    [CmdletBinding()]
    param
    (
        [Parameter(Mandatory = $true, ValueFromPipeline = $true)]
        [PSCustomObject]
        $DatabaseConfig,

        [Parameter(Mandatory = $false)]
        [AllowNull()]
        [Hashtable[]]
        $DatabaseServerPassword,

        [Parameter(Mandatory = $false)]
        [Switch]
        $CreateDatabase = $false
    )

    begin
    {
        Write-VerboseWithTime 'Add-AzureSQLDatabases : début'
    }
    process
    {
        Write-VerboseWithTime ('Add-AzureSQLDatabases : création ' + $DatabaseConfig.databaseName)

        if ($CreateDatabase)
        {
            # Crée une base de données SQL avec les valeurs DatabaseConfig (à moins qu'elle n'existe déjà)
            # La sortie de la commande est supprimée.
            Add-AzureSQLDatabase -DatabaseConfig $DatabaseConfig | Out-Null
        }

        $serverPassword = $null
        if ($DatabaseServerPassword)
        {
            foreach ($credential in $DatabaseServerPassword)
            {
               if ($credential.Name -eq $DatabaseConfig.serverName)
               {
                   $serverPassword = $credential.password             
                   break
               }
            }               
        }

        if (!$serverPassword)
        {
            $serverPassword = $DatabaseConfig.password
        }

        return @{
            Name = $DatabaseConfig.connectionStringName;
            Type = 'SQLAzure';
            ConnectionString = Get-AzureSQLDatabaseConnectionString `
                -DatabaseServerName $DatabaseConfig.serverName `
                -DatabaseName $DatabaseConfig.databaseName `
                -UserName $DatabaseConfig.user `
                -Password $serverPassword }
    }
    end
    {
        Write-VerboseWithTime 'Add-AzureSQLDatabases : fin'
    }
}


<#
.SYNOPSIS
Crée une base de données SQL Microsoft Azure.

.DESCRIPTION
La fonction Add-AzureSQLDatabase crée une base de données SQL Microsoft Azure à partir des données du fichier de configuration JSON généré par Visual Studio, puis retourne la nouvelle base de données. Si l'abonnement a déjà une base de données SQL avec le nom de base de données spécifié sur le serveur de base de données SQL désigné, la fonction retourne la base de données existante. Cette fonction appelle la cmdlet New-AzureSqlDatabase (module Microsoft Azure), qui crée en fait la base de données SQL.

.PARAMETER DatabaseConfig
Accepte un PSCustomObject qui provient du fichier de configuration JSON retourné par la fonction Read-ConfigFile lorsque le fichier JSON possède une propriété de site Web. Cela inclut les propriétés de environmentSettings.databases. Vous ne pouvez pas utiliser le pipe de l'objet pour cette fonction. Visual Studio génère un fichier de configuration JSON pour tous les projets Web et le stocke dans le dossier PublishScripts de votre solution.

.INPUTS
Aucune. Cette fonction n'accepte aucune entrée du pipeline

.OUTPUTS
Microsoft.WindowsAzure.Commands.SqlDatabase.Services.Server.Database

.EXAMPLE
PS C:\> $config = Read-ConfigFile <name>.json
PS C:\> $DatabaseConfig = $config.databases | where connectionStringName
PS C:\> $DatabaseConfig

connectionStringName    : Default Connection
databasename : TestDB1
edition      :
size         : 1
collation    : SQL_Latin1_General_CP1_CI_AS
servertype   : New SQL Database Server
servername   : r040tvt2gx
user         : dbuser
password     : Test.123
location     : West US

PS C:\> Add-AzureSQLDatabase -DatabaseConfig $DatabaseConfig

.LINK
Add-AzureSQLDatabases

.LINK
New-AzureSQLDatabase
#>
function Add-AzureSQLDatabase
{
    [CmdletBinding()]
    param
    (
        [Parameter(Mandatory = $true)]
        [ValidateNotNull()]
        [Object]
        $DatabaseConfig
    )

    Write-VerboseWithTime 'Add-AzureSQLDatabase : début'

    # Échec, si la valeur de paramètre n'a pas la propriété serverName, ou si la valeur de la propriété serverName n'est pas indiquée.
    if (-not (Test-Member $DatabaseConfig 'serverName') -or -not $DatabaseConfig.serverName)
    {
        throw 'Add-AzureSQLDatabase : le nom du serveur de base de données (obligatoire) est absent de la valeur DatabaseConfig.'
    }

    # Échec, si la valeur de paramètre n'a pas la propriété databasename, ou si la valeur de propriété databasename n'est pas indiquée.
    if (-not (Test-Member $DatabaseConfig 'databaseName') -or -not $DatabaseConfig.databaseName)
    {
        throw 'Add-AzureSQLDatabase : le nom de la base de données (obligatoire) est absent de la valeur DatabaseConfig.'
    }

    $DbServer = $null

    if (Test-HttpsUrl $DatabaseConfig.serverName)
    {
        $absoluteDbServer = $DatabaseConfig.serverName -as [System.Uri]
        $subscription = Get-AzureSubscription -Current -ErrorAction SilentlyContinue

        if ($subscription -and $subscription.ServiceEndpoint -and $subscription.SubscriptionId)
        {
            $absoluteDbServerRegex = 'https:\/\/{0}\/{1}\/services\/sqlservers\/servers\/(.+)\.database\.windows\.net\/databases' -f `
                                     $subscription.serviceEndpoint.Host, $subscription.SubscriptionId

            if ($absoluteDbServer -match $absoluteDbServerRegex -and $Matches.Count -eq 2)
            {
                 $DbServer = $Matches[1]
            }
        }
    }

    if (!$DbServer)
    {
        $DbServer = $DatabaseConfig.serverName
    }

    $db = Get-AzureSqlDatabase -ServerName $DbServer -DatabaseName $DatabaseConfig.databaseName -ErrorAction SilentlyContinue

    if ($db)
    {
        Write-HostWithTime ('Create-AzureSQLDatabase : utilisation de la base de données existante ' + $db.Name)
        $db | Out-String | Write-VerboseWithTime
    }
    else
    {
        $param = New-Object -TypeName Hashtable
        $param.Add('serverName', $DbServer)
        $param.Add('databaseName', $DatabaseConfig.databaseName)

        if ((Test-Member $DatabaseConfig 'size') -and $DatabaseConfig.size)
        {
            $param.Add('MaxSizeGB', $DatabaseConfig.size)
        }
        else
        {
            $param.Add('MaxSizeGB', 1)
        }

        # Si l'objet $DatabaseConfig a une propriété collation dont la valeur n'est pas Null ou vide
        if ((Test-Member $DatabaseConfig 'collation') -and $DatabaseConfig.collation)
        {
            $param.Add('Collation', $DatabaseConfig.collation)
        }

        # Si l'objet $DatabaseConfig a une propriété edition dont la valeur n'est pas Null ou vide
        if ((Test-Member $DatabaseConfig 'edition') -and $DatabaseConfig.edition)
        {
            $param.Add('Edition', $DatabaseConfig.edition)
        }

        # Écrire la table de hachage dans le flux des commentaires
        $param | Out-String | Write-VerboseWithTime
        # Appeler New-AzureSqlDatabase à l'aide de la projection (supprimer la sortie)
        $db = New-AzureSqlDatabase @param
    }

    Write-VerboseWithTime 'Add-AzureSQLDatabase : fin'
    return $db
}
