

EvernestFront
=============

EvernestFront provides to Website and Web API functions handling Evernest core objects. (See [Concepts](https://github.com/Evernest/Evernest/wiki/Concepts:-Events,-Streams-and-Rights))

It uses event streams provided by EvernestBack to store user events but also meta data such as object (user, source, stream) creation and deletion.

It checks for available rights at each request for data or action.

Response
================

Public methods return a Response object, which indicates whether the request was successful, the requested information if it was, and relevant errors if it wasn't.
These responses are then read by the website or the API to provide the user with the required information.


Projections
====================

Projections are data needed to answer requests, which are quick to access, at the cost of not being always fully up-to-date.
It is used to display information for clients. When exactness is critical in order to validate an action, we first ask the projections: if they refuse it we directly return an error; if they accept, we tell the client it is probably accepted, and double-check by issuing a system command and waiting until it is treated using fully up-to-date information to perform the action.

System commands
=========================================

Commands that create or delete users, streams, sources or keys cannot be answered rightaway : for instance, creating an Id requires
completely accurate information over the system to preserve Id unicity, which means queuing the command for thread safety. 
For such commands, we compute a Response<System.Guid> from the data in the Projections. This response includes a reasonably trustworthy Success boolean, an Error field if Success is false, 
and a command GUID. 
If Success is true, the command is queued. Unless other commands that invalidate it were queued right before it (but the projections still have not been updated), it will be accepted.
If Success is false, the command is dropped, because it is likely invalid (it's invalid unless other recent commands validate it, in which case it should be re-issued later).

Once the command is dequeued, it is treated using completely accurate data, and an accurate response can be found using a CommandResultViewer and the command GUID.
At this point, the relevant objects have indeed been created and can be looked up to find Ids.

