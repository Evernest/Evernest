Cours
-----


Beaucoup d'échec et de surcoût dans le monde logiciel. Le côté abstrait rend difficiles les pronostics. Aspect social dans l'adoption des logiciels.

AntiPatterns : Non-solutions qui peuvent garantir un échec
 - Nom : Pour que les gens soient attirés et retiennent le projet.
 - Échelle : Code, Archi, Entreprise, Industrie. Bogue de l'an 2000, Fin du timestamp, JavaScript, Word

 1. Lava flow
    Nouvelle fonctionnalité ajouté puis non maintenue.
    Aggrégation de nombreuse fonctionnalités, puis abandon ou perte de temps à maintenance par d'autres.

 2. Blob, a.k.a. The God Class
    Gros blocs de code. Classe « cœur » qui contient un peu tout le code.

 3. Golden Hammer
    Rester sur les recette d'un succès passé. Ne pas changer une équipe qui gagne.
    (Utiliser une arme lourde pour faire des choses simples.)

Livre : *JavaScript, the good parts.*

jlaurent: « Un développeur ne peut pas ignorer le JavaScript. »
« Javascript, c'est le VBA du web. »

 4. Code spaghetti
    Pour étudier un morceau de code, on doit **tout** lire.
    Dépendance des classes en plan de métro.

 5. Stovepipe system
    Dans les grandes organisations. Développement à large échelle temporelle. Intégration point à point des nouveaux projets.

 6. Design by committee
    Mettre tout le monde d'accord en faisant moit'-moit'. Plus de design global. Bonne solutions indépendemment, mais mélange mauvais.

 7. Cut and paste programing
 8. Cargo Cult programing
 9. Project mismanagement
 10. Architecture by implication
 11. Reinvent the wheel
 12. Corncob
 13. Gold platine











struct :
  structure allouée sur la pile (évite l'overhead)
  Permet de rester dans le cache => Plus rapide

JIT :
  Adapter au matériel => Il reste des variables préprocesseur dans l'assembleur en quelque sorte.
  Mettre en commun le GC