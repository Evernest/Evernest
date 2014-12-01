#Requêtes, réponses

La classe Process contient les méthodes statiques utilisables par le site Web et l'API.

 Par exemple :

	Answers.Push Process.Push(string user, string streamName, Event eventToPush	)
	Answers.PullRandom Process.PullRandom(string user, string streamName)
	Answers.Pull Process.Pull(string user, string streamName, int eventId)
	Answers.PullRange Process.PullRange(string user, string streamName, int eventIdFrom, int eventIdTo)
	Answers.CreateStream Process.CreateStream(string user, string streamName)

L'idée est que pour chaque requête d'un client, le site Web appelle la méthode concernée. 

La valeur de retour est un objet de type Answer. Une Answer contient une propriété booléenne Success, une propriété Error de type FrontError et des données. Si Success est vrai (la requête a abouti), Error vaut null et les données ne valent pas null. Si Success est faux, c'est l'inverse et on peut transmettre l'erreur au client.


----------------------------------------------------------------------------------------------------


#Gestion des droits des utilisateurs


Pour chaque paire utilisateur/stream, l'utilisateur a un des droits suivants sur la stream :

* NoRights (aucun droit, valeur par défaut)
* ReadOnly
* WriteOnly
* ReadWrite
* Admin (peut consulter et modifier les droits des autres utilisateurs sur la stream, lire, et écrire)

Ces droits sont définis dans l'enum public AccessRights.

Le frontend maintient une table des droits des utilisateurs sur les streams, les méthodes pour la consulter et la modifier 
sont aussi dans la classe Process. 

Exemples :
	
	Answers.SetRights Process.SetRights(string user, string streamName, string targetUser, AccessRights rights)
	Answers.AddUser Process.AddUser(string userToAdd)
	Answers.AddStream Process.AddStream(string userCreatingStream, string streamToCreate)
	Answers.RelatedStreams Process.RelatedStreams(string user)
	Answers.RelatedUsers Process.RelatedUsers(string userAsking, string stream)

Modifications à faire : créer un super-utilisateur système ? (une constante publique Process.RootUser existe mais le reste n'est pas implémenté)

#Gestion des droits des sources

Chaque source possède un droit de type AccessRights sur la stream qu'elle gère. Ce droit est indépendant de celui de son User propriétaire : à chaque requête, on vérifie qu'à la fois la source et son propriétaire possèdent les droits nécessaires.

Les méthodes de Process qui concernent la lecture/écriture/administration de streams sont surchargées : plutôt que de donner un ID d'user et de stream, on peut simplement donner la clé de la source.

Exemples :
	
	Answers.Push Process.Push(string sourceKey, Event eventToPush	)
	Answers.PullRandom Process.PullRandom(string sourceKey)
	Answers.Pull Process.Pull(string sourceKey, int eventId)
	Answers.PullRange Process.PullRange(string sourceKey, int eventIdFrom, int eventIdTo)
	Answers.CreateStream Process.CreateStream(string sourceKey)

------------------------------------------------------------------------------------------------------------------

#Authentification des utilisateurs

Le front-end maintient, pour chaque objet User, un hash du mot de passe de l'utilisateur concerné. La méthode Process.IdentifyUser(string userName, string password) permet de vérifier si l'utilisateur userName a password pour mot de passe. Les mots de passe peuvent être modifiés avec Process.SetPassword(Int64 userId, string newPassword)
