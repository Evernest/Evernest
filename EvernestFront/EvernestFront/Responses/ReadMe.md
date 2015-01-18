Public methods return a Response object, which indicates whether the request was successful, and relevant errors or informations.
These responses are then read by the website or the API to provide the user with the required information.

System commands
=========================================

Commands that create or delete users, streams, sources or keys cannot be answered rightaway : for instance, creating an Id requires
completely accurate information over the system to preserve Id unicity, which means queuing the command for thread safety. 
For such commands, we compute a SystemCommandResponse from the data in the Projections. This response includes a reasonably trustworthy Success boolean, an Error field if Success is false, 
and a command GUID. 
If Success is true, the command is queued. Unless other commands that invalidate it were queued right before it (but the projections still have not been updated), it will be accepted.
If Success is false, the command is dropped, because it is likely invalid (it's invalid unless other recent commands validate it, in which case it should be re-issued later).

Once the command is dequeued, it is treated using completely accurate data, and an accurate response can be found using a CommandResultViewer and the command GUID.
At this point, the relevant objects have indeed been created and can be looked up to find Ids.
