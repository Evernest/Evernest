Version provisoire de la future interface. Ce n'est pas une véritable doc, l'idée étant surtout d'avoir du feedback, 
notamment sur l'organisation et sur des fonctionalités qui pourraient manquer. On est en train de refaire une bonne partie du
code dans une branche, donc les changements sont actuellement faciles à faire.



La valeur de retour des méthodes est un objet de type Answer. Une Answer contient une propriété booléenne Success, une 
propriété Error de type FrontError et des données. Si Success est vrai (la requête a abouti), Error vaut null et les données
ne valent pas null. Si Success est faux, c'est l'inverse et on peut transmettre l'erreur au client.


---------------------------------------------------------------------------------------------------------------------------
#Classe User

	static Answers.AddUser User.AddUser(string userName, string password)
en cas de succès, la réponse contient le nom, l'id, le mot de passe et la clé de l'utilisateur. On ne peut pas se contenter 
de renvoyer un objet User car celui-ci ne contient pas le mot de passe (nécessaire pour la surcharge suivante). Si vous 
préférez on peut mettre un objet User et le mot de passe à côté, mais de toute façon les autres propriétés d'un User ne sont 
pas intéressantes au moment de la création a priori.

	static Answers.AddUser User.AddUser(string userName)
un mot de passe est généré

	static Answers.IdentifyUser User.IdentifyUser(string userName, string password)
en cas de succès, la réponse contient pour l'instant juste l'id, on peut aussi mettre un objet User si vous préférez

	static Answers.GetUser User.GetUser(long userId)
en cas de succès, la réponse contient un objet User



 	long Id
	string Name
	List<Source> Sources
	List<KeyValuePair<long, AccessRights>> RelatedStreams

	string Key
Cette propriété ne devrait peut-être pas être publique, mais il faut un moyen de récupérer une clé perdue.

	Answers.SetPassword(string newPassword)

	Answers.Push Push(long streamId, string Message)
	Answers.PullRandom Process.PullRandom(long streamId)
	Answers.Pull Pull(long streamId, int eventId)
	Answers.PullRange PullRange(long streamId, int eventIdFrom, int eventIdTo)

	Answers.SetUserRights SetRights(long streamId, long targetUserId, AccessRights rights)

	Answers.CreateSource CreateSource(string sourceName, long streamId, AccessRights rights)
	Answers.SetSourceRights SetSourceRights(string sourceKey, AccessRights rights)


---------------------------------------------------------------------------------------------------------------------------
#Classe Stream

	static Answers.CreateStream Stream.CreateStream(long creatorId, string streamName)
	static Answers.GetStream Stream.GetStream(long streamId)
	
	long Id
	string Name
	List<KeyValuePair<long, AccessRights>> RelatedUsers


---------------------------------------------------------------------------------------------------------------------------
#Classe Source

	static Answers.GetSource GetSource(string sourceKey)

	Answers.Push Push(string Message)
	Answers.PullRandom Process.PullRandom()
	Answers.Pull Pull(int eventId)
	Answers.PullRange PullRange(int eventIdFrom, int eventIdTo)

	Answers.SetUserRights SetRights(long targetUserId, AccessRights rights)

Les sources sont identifiées directement par leur clé. Il n'y a pas d'id de stream à donner car chaque source est associée à 
une seule stream et donc agit toujours sur cette stream.
