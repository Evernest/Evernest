
Projet Cloud Computing
======================


Présentations
-------------

| Sujet         | Commentaire               | Présentateur |
|---------------|---------------------------|--------------|
| Git/Github    |                           | Martin       |
| nunit + R#    | tests U                   | Axelle       |
| msbuild       | ~ Makefile                |              |
| teamcity      | Intégration continue      |              |
| ncover        | Analyse statique          | Alex         |
| powershell    |                           |              |
| TPL + PLinq   | Multicore                 |              |
| REST, ASP.NNC | API                       | Élie         |
| .trace        | profiler CPU              |              |
| Linq          | SQL C# typé               |              |
| Bootstrap     | Framework Webapp          |              |
| .Peek         | décompilateur             |              |
| kudu          | déploiement Azure par Git |              |
| ndepend       |                           | Nicolas      |


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


Buts
----

 * Organisation en groupe
 * Découverte du monde du dev Microsoft
 * Génie logiciel, apperçu du monde du logiciel industriel



Répartition des rôles
---------------------

|                        |                   |                                                       |
|------------------------|-------------------|-------------------------------------------------------|
| Coordination           | Élie              |                                                       |
| Build master           | Alex              | Intégration continue et pour tous                     |
| Front master           | Raphaël           | API (specification)                                   |
| Back manager           | Martin            | Back-office storage (performance du stockage)         |
| Admin managment        | Aymeric  Corentin | Interface admin, humain, UX                           |
| Test/Regression master | Rémi              | Tests fonctionnels/unitaires, chasse aux régressions.  Intégration des tests dans les différentes parties. |
| Performance            |                   |                                                       |


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


logs, validation des requêtes, automatisation des tests pour ne rien oublier



TODO
----

 * chercher un ndd
 * canal IRC
