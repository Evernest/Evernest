

EvernestFront
=============

EvernestFront provides to Website and Web API functions handling Evernest core objects. (See [Concepts](https://github.com/Evernest/Evernest/wiki/Concepts:-Events,-Streams-and-Rights))

It uses event streams provided by EvernestBack to store user events but also meta data such as object (user, source, stream) creation and deletion.

It checks for available rights at each request for data or action.

Response
================

Public methods return a Response object, which indicates whether the request was successful, the requested information if it was, and relevant errors if it wasn't.
These responses are then read by the website or the API to provide the user with the required information.

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

User public methods

=====================================


    Response<User> UserBuilder.GetUser(string userName)

    Response<User> UserBuilder.GetUser(long userId)

    Response<Guid> UserBuilder.AddUser(string userName)

    Response<Guid> UserBuilder.AddUser(string userName, string password)


Source public methods
=======================================

    Response<Tuple<String, Guid>> User.CreateSource(string sourceName)
    Response<Source> User.GetSource(string sourceName)
    Response<Source> User.GetSource(long sourceId)
    Response<Source> SourcesBuilder.GetSource(string sourceKey)
    Response<Guid> User.SetSourceRight(string sourceKey, long streamId, AccessRight right)
    Response<Guid> User.DeleteSource(long sourceId)
