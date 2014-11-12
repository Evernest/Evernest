#Requêtes, réponses

La classe Process (à renommer ? le nom n'est peut-être pas très clair) contient, pour chaque type de requête devant 
passer par le back-end (tout sauf ce qui concerne les droits des utilisateurs), une méthode statique appropriée.

 Par exemple :

	int Process.Push(string user, string streamName, Event eventToPush	)
	Event Process.PullRandom(string user, string streamName)
	Event Process.Pull(string user, string streamName, int eventId)
	List<Event> Process.PullRange(string user, string streamName, int eventIdFrom, int eventIdTo)
	Int64 Process.CreateStream(string user, string streamName)

L'idée est donc que pour chaque requête d'un client, le site Web appelle la méthode concernée. 

Chacune de ces fonctions peut lever des exceptions héritant de FrontException. Consulter leurs commentaires pour plus de détails !

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
sont aussi dans la classe Process. Ces méthodes lèvent des exceptions (spécifiées dans leurs commentaires)
en cas d'échec.

Exemples :
	
	void Process.SetRights(string user, string streamName, string targetUser, AccessRights rights)
	Int64 Process.AddUser(string userToAdd)
	void Process.AddStream(string userCreatingStream, string streamToCreate)
	List<KeyValuePair<string, AccessRights>> Process.RelatedStreams(string user)
	List<KeyValuePair<string, AccessRights>> Process.RelatedUsers(string userAsking, string stream)

Modifications à faire : créer un super-utilisateur système ? (une constante publique Process.RootUser existe mais le reste n'est pas implémenté)

#Gestion des droits des sources

à faire
