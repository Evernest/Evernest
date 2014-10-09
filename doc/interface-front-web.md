 * Le namespace `Request` contient les classes relatives aux requêtes venant de l'API et de l'interface Web.

 * Les requêtes sont représentées par des classes héritant d'une classe `Request`.

 * La classe `Request` possède une méthode publique `Process` qui traite la requête et renvoie un objet de type `IAnswer` à préciser.

 * Pour chaque type de requête (classes Push, PullRandom, PullRange...), on donne un constructeur public prenant en argument toutes les informations à fournir par la partie Web. 
 
 Par exemple :
```
Request.Push(string user, string streamName, Event eventToPush)
Request.PullRandom(string user, string streamName)
Request.PullRange(string user, string streamName, string eventIdFrom, string eventIdTo)
Request.CreateStream(string user, string streamName)
Request.SetRights(string user, string streamName, string targetUser, StreamRights rights)
...
```

L'idée est donc que pour chaque requête de client, le site Web crée un objet du type Request.* approprié, puis appelle sa méthode Process et obtient une réponse en valeur de retour. 

Pour l'instant on a mis des string partout, mais on aura peut-être un autre système d'id.

 * La classe `Event` sera sans doute transmise directement du Web au Back, sans modification par le Front...?

 * Le type `StreamRights` sera a priori implémenté sous forme d'un enum public, il faudrait définir exactement quels types de droits existent... 
 
 * Réponse : objet de type IAnswer (pas encore implémenté) contenant les informations pertinentes :
	succès ou échec de l'opération, id de l'événement pour un push, mais aussi message d'erreur si l'utilisateur n'a pas les droits adéquats, ou échec de l'opération pour d'autres raisons. À préciser.

 * Il y aura probablement une classe différente implémentant l'interface IAnswer pour chaque type de requête.