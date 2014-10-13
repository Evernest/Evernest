 * Le namespace `Request` contient les classes relatives aux requ�tes venant de l'API et de l'interface Web.

 * Les requ�tes sont repr�sent�es par des classes h�ritant d'une classe `Request`.

 * La classe `Request` poss�de une m�thode publique `Process` qui traite la requ�te et renvoie un objet de type `IAnswer` � pr�ciser.

 * Pour chaque type de requ�te (classes Push, PullRandom, PullRange...), on donne un constructeur public prenant en argument toutes les informations � fournir par la partie Web. 
 
 Par exemple :
```
Request.Push(string user, string streamName, Event eventToPush)
Request.PullRandom(string user, string streamName)
Request.PullRange(string user, string streamName, string eventIdFrom, string eventIdTo)
Request.CreateStream(string user, string streamName)
Request.SetRights(string user, string streamName, string targetUser, StreamRights rights)
...
```

L'id�e est donc que pour chaque requ�te de client, le site Web cr�e un objet du type Request.* appropri�, puis appelle sa m�thode Process et obtient une r�ponse en valeur de retour. 

Pour l'instant on a mis des string partout, mais on aura peut-�tre un autre syst�me d'id.

 * La classe `Event` sera sans doute transmise directement du Web au Back, sans modification par le Front...?

 * Le type `StreamRights` sera a priori impl�ment� sous forme d'un enum public, il faudrait d�finir exactement quels types de droits existent... 
 
 * R�ponse : objet de type IAnswer (pas encore impl�ment�) contenant les informations pertinentes :
	succ�s ou �chec de l'op�ration, id de l'�v�nement pour un push, mais aussi message d'erreur si l'utilisateur n'a pas les droits ad�quats, ou �chec de l'op�ration pour d'autres raisons. � pr�ciser.

 * Il y aura probablement une classe diff�rente impl�mentant l'interface IAnswer pour chaque type de requ�te.