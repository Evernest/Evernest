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
