Voici quelques remarques du Front � l'intention du Web.

Premier jet. Il y aura sans doute beaucoup de modifications.

 * Les requ�tes sont repr�sent�es par des classes h�ritant d'une classe `Request`.

 * La classe `Request` poss�de une m�thode publique `Process` qui traite la requ�te et renvoie un objet de type `IAnswer` � pr�ciser.

 * Pour chaque type de requ�te, on donne un constructeur public prenant en argument toutes les informations � fournir par la partie Web. Par exemple :
```
RequestPush(string user, string streamName, Event eventToPush)
RequestPullRandom(string user, string streamName)
RequestPullBetween(string user, string streamName, string eventIdFrom, string eventIdTo)
...
```
et des requ�tes pour cr�er une stream ou modifier les droits d'acc�s, � d�finir.

Pour l'instant on a mis des string partout, mais on aura peut-�tre un autre syst�me d'id.

 * La classe `Event` sera sans doute transmise directement du Web au Back, sans modification par le Front...?

 * R�ponse : "Ack" bien s�r, id de l'�v�nement pour un push, mais aussi message si l'utilisateur n'a pas les droits ad�quats, ou �chec de l'op�ration pour d'autres raisons. � pr�ciser.



Voil� pour le moment, indiquez-nous ici ce que vous en pensez et ce qu'il vous faudrait d'autre =)