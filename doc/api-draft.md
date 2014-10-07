
API
===

*[Draft]*


Ce fichier est une ébauche de description des différentes APIs. N'hésitez pas à proposer des modifications.


API utilisateur
---------------

Cette API est celle utilisée par les clients pour utiliser les services d'Evernest au sein de leurs programmes. Elle peut égelement être utilisée par l'interface web pour communiquer avec le serveur.

Dans un premier temps, l'API est basée sur le protocole HTTP mais pourra dans l'avenir être également portée sur un protocole plus léger, notamment au niveau de l'*overhead* (HTTP a des headers un peu lourds) et du [*round-trip time*](https://fr.wikipedia.org/wiki/Round-trip_delay_time) dû à l'ouverture d'une nouvelle connexion à chaque requête.

L'avantage d'utiliser HTTP est de pouvoir utiliser les URLs pour représenter les différentes actions possibles, de pouvoir utiliser les en-têtes comme le statut (pour dire si tout s'est bien passer ou signaler ce qui ne va pas).

Les différentes actions porposées par l'API sont donc accessibles par l'intermédiaire de différentes URLs et les données envoyées et reçues sont contenues dans le corps des requêtes et réponses. Seule la méthode POST est donc utilisée par l'API (GET ne sert en principe pas à envoyer des données).

Les formats de données les plus utilisés usuellement pour décrire les données échangées sont [XML](https://fr.wikipedia.org/wiki/Extensible_Markup_Language) et [JSON](https://fr.wikipedia.org/wiki/JavaScript_Object_Notation). JSON a de plus en plus tendance à être utilisé, notamment parce qu'il est baaucoup moins verbeux que XML et bien plus simple à parser (et donc plus rapide), c'est pourquoi nous l'utilisons.

Les points d'entrée de l'API sont tous situés dans dans `www.evernest.org/api/`. On aurait pu envisager de mettre ça dans `api.evernest.org/` mais ça pose des problèmes de bloquage [XSS](https://fr.wikipedia.org/wiki/Cross-site_scripting) si on veut utiliser l'API dans l'interface web (avec [Ajax](https://fr.wikipedia.org/wiki/Ajax_%28informatique%29)).

Les points d'entrée sont donc les suivants :

 * `/api/*`
   Racine de l'API

 * `/api/pull/*`
   Entrées permettant de récupérer des données.
   Le corps des requêtes peut comporter un champ `filter` indiquant une liste de tags à prendre en compte. *[Note: À ignorer tant qu'on n'a pas de système de tag]*

 * `/api/pull/event/random`
   Retourne un évènement au hasard.

 * `/api/pull/event/<id1>/<id2>`
   Retourne les évènements entre les identifiants `<id1>` et `<id2>` inclus.
   Si `<id2>` est omis, il est considéré comme égal à `<id1>`.

 * `/api/push/*`
   Entrées permettant de poster des données.

 * `/api/push/event`
   Ajoute un évènement. La requête doit contenir un champ `event` contenant lui-même un champ `content` (et un champ `tags` ?). La réponse contient un champ `id` indiquant l'identifiant de l'évènement nouvellement enregistré.

 * `/api/login`
   Identification. La requête comporte les champs `user` et `password` et la réponse un jeton de session dans un champ `token` ainsi que sa date de péromption, dans un champ `token_timeout` (sous forme de timestamp). Cet horaire est présent à titre indicatif et est basée sur l'heure du serveur.


Les formats JSON des corps de requêtes et réponses sont :

 * Requête générale
   ```
       {
           'token': <token>
       }
   ```
   Toute requête sur l'API, à l'exception des requêtes d'identification, doit contenir un jeton de session (`token`). Cela permet d'une part d'éviter les attaques [CSRF](https://en.wikipedia.org/wiki/Cross-site_request_forgery) et d'autre part d'établir un lien entre les différentes requêtes d'un même utilisateur.

 * Réponse générale
   ```
       {
           'new_token': <token>,
           'token_timeout': <timestamp>
       }
   ```
   Le principe d'un jeton est de n'être utilisable qu'une seule fois. Pour éviter d'avoir à refaire à chaque fois une requête de login, un nouveau jeton est généré pour une éventuelle requête suivante. On peut éventuellement ajouter un champ `no_new_token: true` à la requête pour ne pas générer de nouveau token. À moins qu'il ne vaille mieux que par défaut aucun jeton ne soit généré ?

 * Retour de Pull
   ```
       {
           'token': <token>,
           'events': [<event1>, <event2>, …]
       }
   ```

 * Évènement
   ```
      {
          'id': <id>,
          'content': <content>,
          'tags': [<tag1>, <tag2>, …]
      }
   ```


**Remarque :**
Pour des raisons de sécurité, on ne met **jamais** de liste à la racine d'un JSON.
Ce choix permet également plus de souplesse car il permet d'ajouter de nouveaux champs de données sans casser les champs déjà existants.

**Remarque :**
L'API ne doit être accessible que par HTTPS et non en HTTP. Sinon, il est simple de modifier une requête sans modifier le jeton ou d'intercepter les identifiants lors de la connexion…
