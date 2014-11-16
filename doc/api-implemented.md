
Evernest API
============

Stream
------

GET: /Stream/{streamId}
GET: /Stream/{streamId}/Pull/Random
GET: /Stream/{streamId}/Pull/{id}
GET: /Stream/{streamId}/Pull/{startId}/{stopId}

POST: /Stream/{streamId}/Push

Source
------

GET: /Source/{sourceId}

POST: /Source/New

User
----

GET: /User/{userId}

Right
-----

GET: /Right/{sourceId}/{streamId}
GET: /Right/{sourceId}/{streamId}/Set/{right}

UserRight
---------

GET: /UserRight/{userId}/{streamId}
GET: /UserRight/{userId}/{streamId}/Set/{right}
