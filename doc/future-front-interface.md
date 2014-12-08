#Généralités

La valeur de retour des méthodes est un objet de type Answer. Une Answer contient une propriété booléenne Success, une 
propriété Error de type FrontError et des données. Si Success est vrai (la requête a abouti), Error vaut null et les données
ne valent pas null. Si Success est faux, c'est l'inverse et on peut transmettre l'erreur au client.

Il manque plusieurs méthodes de suppression.


---------------------------------------------------------------------------------------------------------------------------
#Classe User

Méthodes statiques

	static Answers.AddUser User.AddUser(string userName, string password)
	static Answers.AddUser User.AddUser(string userName)
Deuxième surcharge : un mot de passe est généré.

En cas de succès, la réponse contient l'id et le mot de passe. C'est la seule apparition en clair du mot de passe. Elle est nécessaire à cause du cas où on génère le mot de passe.



	static Answers.IdentifyUser User.IdentifyUser(string userName, string password)
	static Answers.IdentifyUser User.IdentifyUser(string userKey)
En cas de succès, la réponse contient à la fois un objet User et l'id de l'User.



/!\ Les objets User ne doivent pas être conservés : ils ne sont jamais mis à jour. Si on veut faire plusieurs actions, il faut stocker l'id et appeler GetUser pour chaque action. Par exemple, si vous créez une stream avec un objet User, l'objet lui-même n'a pas les droits sur la stream créée.

	static Answers.GetUser User.GetUser(long userId)
En cas de succès, la réponse contient un objet User.

----------------
Propriétés
 	long Id
	string Name
	List<Source> Sources
	List<KeyValuePair<long, AccessRights>> RelatedStreams


----------------
Méthodes

	Answers.SetPassword(string formerPassword, string newPassword)
	
	Answers.CreateUserKey CreateUserKey(string keyName)
	Answers.CreateUserKey CreateUserKey()
Dans le second cas, un nom est généré pour la clé.
/!\ Pour l'instant la deuxième surcharge génère toujours le même nom donc vous aurez une erreur (UserKeyNameTaken) si vous l'utilisez deux fois.

À ajouter : récupération de la liste des paires nom de clé / clé, et suppression de clé, qui demanderont le mot de passe pour qu'une clé ne puisse pas agir sur les autres.

	Answers.CreateStream CreateStream(string streamName)
	Answers.Push Push(long streamId, string message)
	Answers.PullRandom PullRandom(long streamId)
	Answers.Pull Pull(long streamId, int eventId)
	Answers.PullRange PullRange(long streamId, int eventIdFrom, int eventIdTo)
	Answers.SetUserRights SetRights(long streamId, long targetUserId, AccessRights rights)

	Answers.CreateSource CreateSource(string sourceName, long streamId, AccessRights rights)
	Answers.DeleteSource DeleteSource(string sourceName)


---------------------------------------------------------------------------------------------------------------------------
#Classe Stream

	static Answers.GetStream Stream.GetStream(long streamId)
	
	long Id
	string Name
	int Count
	List<KeyValuePair<long, AccessRights>> RelatedUsers


---------------------------------------------------------------------------------------------------------------------------
#Classe Source

	static Answers.GetSource GetSource(string sourceKey)
en cas de succès, la réponse contient un objet Source.
	
	string Name
	long UserId
	long StreamId
	string Key
	AccessRights Rights

	Answers.Push Push(string message)
	Answers.PullRandom Process.PullRandom()
	Answers.Pull Pull(int eventId)
	Answers.PullRange PullRange(int eventIdFrom, int eventIdTo)

	Answers.SetUserRights SetRights(long targetUserId, AccessRights rights)
	
	Answers.DeleteSource Delete()

Les sources sont identifiées directement par leur clé. Il n'y a pas d'id de stream à donner car chaque source est associée à
une seule stream et donc agit toujours sur cette stream.


---------------------------------------------------------------------------------------------------------------------------
#Classe Event

à documenter
