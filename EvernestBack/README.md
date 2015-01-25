

EvernestBack
============


EvernestBack manage Evernest hard storage using
[Microsoft Azure](http://azure.microsoft.com/).

It enables writing and reading in event streams. New events are always append
to the end of the streams and event reading is done by slice of consecutive
events.

Garantees...
------------

... that should be provided : 

 - EvernestBack garantee that all acknowledge messages is recoverable.
 - EvernestBack garantee that all recoverd message is identical to the original message.

If something doesn't behave as it should, please report the bug.

Undefined behavior.
-------------------

 - No garantee on the response time


Fast how to ?
-------------

 - You can instanciate a new stream using AzureStorageClient.singleton.GetNewEventStream()
 - You can push to a stream using stream.push()
 - You can pull from a stream using stream.pull()

For further informations, take a look at the inline doc ;-)
