#Requêtes, réponses

La classe Process (à renommer ? le nom n'est peut-être pas très clair) contient, pour chaque type de requête devant 
passer par le back-end (tout sauf ce qui concerne les droits des utilisateurs), une méthode statique appropriée.

 Par exemple :

	Process.Push(string user, string streamName, Event eventToPush	)
	Process.PullRandom(string user, string streamName)
	Process.PullRange(string user, string streamName, string eventIdFrom, string eventIdTo)
	Process.CreateStream(string user, string streamName)

L'idée est donc que pour chaque requête d'un client, le site Web appelle la méthode concernée. 

La valeur de retour est un objet héritant de la classe abstraite Answer (définie dans le namespace EvernestFront.Answers)
et du type approprié : Process.Push retourne un objet de type Answers.Push, etc. 

Un objet de type Answer contient une propriété booléenne Success qui indique si la requête a abouti, 
une propriété contenant les informations obtenues si la requête aboutit, et une propriété Exception pouvant 
contenir une exception définie dans le namespace EvernestFront.Exceptions. 

Si Success vaut true, l'exception vaut null, et si Success vaut false, les données valent null et Exception contient alors 
nécessairement une exception qui explique pourquoi la requête a échoué. (Si ça vous semble peu pratique, on peut aussi enlever la propriété
Exception et lever directement l'exception au lieu de retourner lorsque la requête échoue.)


Pour augmenter les performances, Martin propose d'utiliser le paradigme fire and forget. 
On pourrait faire en sorte que les méthodes de Process prennent en argument un delegate, 
qui prend lui-même en argument l'objet de type Answer qu'on retourne actuellement : à discuter...

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
sont dans la classe Users. Ces méthodes ont pour type de retour *void*, et lèvent des exceptions (spécifiées dans leurs commentaires)
en cas d'échec.

Exemples :
	
	Users.SetRights(string user, string streamName, string targetUser, AccessRights rights)
	Users.AddUser(string userToAdd)
	Users.StreamsOfUser(string user)
	Users.UsersOfStream(string user, string stream)

Modifications à faire : garantir certaines choses comme l'existence d'au moins un Admin par stream (pour l'instant, 
le créateur est un Admin, mais rien ne l'empêche de se destituer), peut-être créer un super-utilisateur système ?


