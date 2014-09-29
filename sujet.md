
Projet Cloud Computing
======================


Présentations
-------------

  Git/Github
  nunit + R# ("resharper") (tests U)
  msbuild                  (new Makefile)
  teamcity                 (Intégration continue)
  ncover                   (Analyse statique)
  powershell               (new Shell)
  TPL + PLinq              (Multicore)
✔ REST avec ASP.NNC        (API)
  .trace                   (profiler CPU)
  Linq                     (SQL C# typé)
  Bootstrap                (Framework Webapp)
  .Peek                    (décompilateur)
  kudu                     (Plateforme de déploiement Azure par Git)
  ndepend


Projet
------

**Event Store**

Service vers lequel on peut pousser des évènements et qui répond avec un ID et une confirmation.
Lectures :

 * Aléatoire
 * Par intervale

### Intérêt

 * Analyse évènementielle de logs
 * Approche différente du *CRUD* dont le problème est d'avoir une sémantique pauvre. Pas d'historique des modifs a priori par exemple.
 * On enregistre les modifications => Pas de suppression (chercher les optimisations grâce à ça)
 * Permet le CQRS

### Objectifs

 * Performance : Évènements poussés par des applications (tester la charge supportée). Limite haute : 5 million d'ev/s.
 * Fiabilité en écriture comme en lecture. 10Mo/s - 1Go/s
 * Service cloud, avec interface d'administration (interface web, API keys)


 * Benchmarking
 * Interface web
 * 


Buts
----

 * Organisation en groupe
 * Découverte du monde du dev Microsoft
 * Génie logiciel, apperçu du monde du logiciel industriel



Répartition des rôles
---------------------

Coordination           : Élie
Build master           : Alex     : Intégration continue et pour tous
Front master           : Raphaël  : API (specification)
Back manager           : Martin   : Back-office storage (performance du stockage)
Admin managment        : Aymeric  : Interface admin, humain, UX
                       : Corentin
Test/Regression master : Rémi     : Tests fonctionnels/unitaires, chasse aux régressions. Intégration des tests dans les différentes parties.
Performance  


Back
Stockage d'évènements

-- --

Front
 + Stockage de comptes

-- API --

REST HTTP

Admin : Utilise l'API utilisateur





truck factor
            ___
 ___________| _\
_|          | \_\
_|  _       |_   \
_|_/.\______/.\__|
   \_/      \_/


Émile
Ikram
Diane
Aurélien
Marc
Corentin
Axelle
Denys

aurelien.delobelle@ens.fr Aurélien Delobelle
ikram.cherigui@ens.fr Ikram Cherigui
denys.kanunikov@ens.fr Denys Kanunikov
axelle.piot@ens.fr Axelle Piot
remi.bricout@ens.fr Rémi Bricout
martin.vassor@ens.fr Martin Vassor
aymeric.fromherz@ens.fr Aymeric Fromherz
emile.enguehard@ens.fr Émile Enguehard
alex.auvolat@ens.fr Alex Auvolat
diane.gallois-wong@ens.fr Diane Gallois-Wong
raphael.rieu-helft@ens.fr Raphaël Rieu-Helft
marc.pasqualetto@ens.fr Marc Pasqualetto
corentin.cadiou@ens.fr Corentin Cadiou
elie.michel@ens.fr Élie Michel

logs, validation des requêtes, automatisation des tests pour ne rien oublier



TODO
----

 * chercher un ndd
 * canal IRC





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


Git (Par Martin)
---

On découvre les commandes au moment où tu les tapes, ça semble un peu parachuté. Enfin pas la peine d'épiloguer à chaque fois pour autant, t'as raison. Live coding bien géré à par ça.
Manque de power tips, mais bon, c'est pas évident de les connaître…

Comment on synchronise les fichiers dans le bazar décentralisé ?
Comment on sait quand commiter/pusher ?
Enregistre des évènements, des actions sur le fs.
Que mettre dans les messages de commit ?
À quoi ça sert que deux personnes fassent la même chose ? Quand utiliser les branches alors ?
Comment ça marche les diffs ? Et pour les binaires ? Comment faire avec les exécutables alors ?
Est-ce qu'il faut être dans une branche pour fusionner vers elle ?
C'est quoi HEAD ?
Dans le merge d'exemple, on ne veut en garder qu'un alors ne vaut-il pas mieux éviter de pourir l'histo ? Parler du rebase et de la propreté de l'historique.
Différence entre `git fetch` et `git pull` ?
GitHub permet aussi de faire un serveur central normal. Choisir comment organiser.

Spikes





struct :
  structure allouée sur la pile (évite l'overhead)
  Permet de rester dans le cache => Plus rapide

JIT :
  Adapter au matériel => Il reste des variables préprocesseur dans l'assembleur en quelque sorte.
  Mettre en commun le GC


Azure
=====

Suite de services :

 * Stockage

   o blob.........Key => Value    À utiliser essentiellement pour le projet
     Fiabilité : (redondance) p.e. 6 copies + hash md5 + relecture toutes les semaines. Sur 2 datacenters.
     Scalabilité : != stockage sur un FS qui dépend du disque et nécessite donc de le changer régulièrement. POssibilité de passer par un CDN. (augmente le débit et la proximité)
     Possibilité de monter le blob comme FS
     De l'ordre de 200Go-1To

   o queue........Producteur => Données => … => Données => Consommateur
     Fiable
     Accès par paquet (de taille 32)
     /!\ Pas FIFO (à cause de problèmes de délais de synchronisation)

   o table........Key => Value
     Taille max : 1Mo
     Écriture atomique sur jusqu'à 100 objets en même temps (pas possible avec le blob).

   o SQL..........BDD relationnelle
     Base multiple.
     Puissance de SQL, mais moins rapide que SQL-Server ou autre.

   o DocumentDB...style MongoDB
     Approche orientée JSON
     Versionning

 => Gérer de façon similaire le service de l'EventStore.
    RikM: Étudie l'interface d'Azure, Google Cloud &co.

 * Calcul

   o cloud service...
     Déploiement d'applicatif .NET transparent.
     Réplication, ne tombe pas en panne.
      - web role
      - worker role

   o vm..............machine virtuelle
     Machine vituelle avec conf sur demande
     => Serveur d'intégration continue

   o website.........Heroku-like pour ASP.NET
     Déploiement web avec un simple push git.
     => Back et Front

 * Virtual Private Network
 * Load Balancer
 * Traffic Manager
 * Active Directory

« Service »


NUnit & R#
==========

Le nom des fonctions a-t-il une importance ?
Est-ce que Assert est vraiment lier à NUnit ?
Quelle est la taille des unitées testées ? Combien de tests faire ?

nugget nunit