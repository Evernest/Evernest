Voici quelques remarques du Front à l'intention du Web.
Premier jet. Il y aura sans doute beaucoup de modifications.

 * Les requêtes sont représentées par des classes héritant d'une classe `Request`.

 * La classe `Request` possède une méthode publique `Process` qui traite la requête et renvoie un objet de type IAnswer à préciser.

 * Pour chaque type de requête, on donne un constructeur public prenant en argument toutes les informations à fournir par la partie Web. Par exemple :
```
RequestPush(string user, string streamName, Event eventToPush)
RequestPullRandom(string user, string streamName)
RequestPullBetween(string user, string streamName, Event eventFrom, Event eventTo)
...
```
et des requêtes pour créer une stream ou modifier les droits d'accès, à définir.

 * La classe `Event` sera sans doute transmise directement du Web au Back, sans modification par le Front...?

 * Réponse : "Ack" bien sûr, id de l'événement pour un push ? mais aussi message si l'utilisateur n'a pas les droits adéquats, et transmission éventuelle d'erreurs du Back. Simple enum ou classe selon la complexité dont on aura besoin, on ne sait pas trop encore.



Voilà pour le moment, indiquez-nous ici les précisions dont vous aurez besoin =)