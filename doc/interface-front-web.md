#Requ�tes, r�ponses

La classe Process (� renommer ? le nom n'est peut-�tre pas tr�s clair) contient, pour chaque type de requ�te devant 
passer par le back-end (tout sauf ce qui concerne les droits des utilisateurs), une m�thode statique appropri�e.

 Par exemple :

	Process.Push(string user, string streamName, Event eventToPush	)
	Process.PullRandom(string user, string streamName)
	Process.PullRange(string user, string streamName, string eventIdFrom, string eventIdTo)
	Process.CreateStream(string user, string streamName)

L'id�e est donc que pour chaque requ�te d'un client, le site Web appelle la m�thode concern�e. 

La valeur de retour est un objet h�ritant de la classe abstraite Answer (d�finie dans le namespace EvernestFront.Answers)
et du type appropri� : Process.Push retourne un objet de type Answers.Push, etc. 

Un objet de type Answer contient une propri�t� bool�enne Success qui indique si la requ�te a abouti, 
une propri�t� contenant les informations obtenues si la requ�te aboutit, et une propri�t� Exception pouvant 
contenir une exception d�finie dans le namespace EvernestFront.Exceptions. 

Si Success vaut true, l'exception vaut null, et si Success vaut false, les donn�es valent null et Exception contient alors 
n�cessairement une exception qui explique pourquoi la requ�te a �chou�. (Si �a vous semble peu pratique, on peut aussi enlever la propri�t�
Exception et lever directement l'exception au lieu de retourner lorsque la requ�te �choue.)


Pour augmenter les performances, Martin propose d'utiliser le paradigme fire and forget. 
On pourrait faire en sorte que les m�thodes de Process prennent en argument un delegate, 
qui prend lui-m�me en argument l'objet de type Answer qu'on retourne actuellement : � discuter...

----------------------------------------------------------------------------------------------------


#Gestion des droits des utilisateurs


Pour chaque paire utilisateur/stream, l'utilisateur a un des droits suivants sur la stream :

* NoRights (aucun droit, valeur par d�faut)
* ReadOnly
* WriteOnly
* ReadWrite
* Admin (peut consulter et modifier les droits des autres utilisateurs sur la stream, lire, et �crire)

Ces droits sont d�finis dans l'enum public AccessRights.

Le frontend maintient une table des droits des utilisateurs sur les streams, les m�thodes pour la consulter et la modifier 
sont dans la classe Users. Ces m�thodes ont pour type de retour *void*, et l�vent des exceptions (sp�cifi�es dans leurs commentaires)
en cas d'�chec.

Exemples :
	
	Users.SetRights(string user, string streamName, string targetUser, AccessRights rights)
	Users.AddUser(string userToAdd)
	Users.StreamsOfUser(string user)
	Users.UsersOfStream(string user, string stream)

Modifications � faire : garantir certaines choses comme l'existence d'au moins un Admin par stream (pour l'instant, 
le cr�ateur est un Admin, mais rien ne l'emp�che de se destituer), peut-�tre cr�er un super-utilisateur syst�me ?


