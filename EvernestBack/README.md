

EvernestBack
============

Features
--------

EvernestBack manages Evernest hard storage using
[Microsoft Azure](http://azure.microsoft.com/).
It defines event streams which allow user to either push or pull events
(basically, a combination of an index and a string) to the cloud asynchronously.
EvernestBack also takes care to always keep the cloud in a valid readable state:
no crash or connexion interruption will corrupt any event already pushed on the
server; and deals with efficiency issues thanks to buffered writes and a simple
cache mechanism.
EvernestBack also provides a dummy mode which doesn't use the Microsoft Azure
cloud services and stores EventStreams locally for testing purposes.

It enables writing and reading in event streams. New events are always append
to the end of the streams and event reading is done by slice of consecutive
events.

Garantees...
------------

... that should be provided : 

 - EvernestBack garantee that all acknowledge messages is recoverable.
 - EvernestBack garantee that all recoverd message is identical to the original message.

If something doesn't behave as it should, please report the bug.

Undefined behavior
------------------

 - No garantee on the response time


Fast how to ?
-------------

 - You can instanciate a new stream using AzureStorageClient.singleton.GetNewEventStream()
 - You can push to a stream using stream.push()
 - You can pull from a stream using stream.pull()

For further informations, take a look at the inline doc ;-)

Project organisation
--------------------

The source files of the project are in Evernest/EvernestBack, unit tests are
located in Evernest/test/EvernestBackTests.

The two main components of the project are:
- the AzureStorageClient singleton which handles the connection with
 the Microsoft Azure cloud services and manages event streams
- the IEventStream interface, the facade you'll be using most of the time
 if you plan to use EvernestBack
- LowLevelEvent and EventRange describes the basic structure of events
EvernestBack offers you
EventStream and MemoryEventStream implements the IEventStream interface and will
be used depending on whether you plan to use the local mode or not. Most of the
other components are used by EventStream to efficiently store and retrieve events.

Technical choices
-----------------

As pushing and pulling events to or from a remote server may be time-consuming,
these operations are handled asynchronously by EvernestBack. Each stream will
thereby use two worker threads to either push or pull data.
When pushing events, they are temporarily copied in a buffer to speed-up writing
operations. Those events are immediately accessible for reading, but won't be
sent to the server until the next buffer flush (thereby potentially lost in a
crash). When pushed to the server, the events are written in a pageblob (as
blockblobs would imply either smaller blobs or slower transactions with the cloud)
at the end of the already written events, then the new number of bytes written in
the pageblob is written in the first page of the pageblob so that we can keep
trace of the pageblob size.
Each push also notifies an indexer of the location where the event is expected to be
written on the server, which may be stored in a binary search tree or discarded.
When pulling an event, reasonably accurate bounds on the event location are then 
retrieved from the tree, and used to pull a whole range of events containing the
requested event. If the pull is successfull, that range is then used by a cache
mechanism which keeps the latest retrieved ranges and takes advantage of these 
moderately big ranges instead of a more targeted pull, thereby speeding up further
pulls (assuming events pushed in a same small timespan will also be retrieved in
a same small timespan).
To speed up event streams reloading at startup, additional indexing information is
regularly pushed in a blockblob (and retrieved at startup). This information are
totally optional, may be incomplete, but must be somewhat coherent with the
corresponding pageblob for the events to be properly retrieved. If this information
was however to be discarded, reloading the whole stream may be awfully slow
depending on the stream's size.

Future improvments
------------------

EvernestBack doesn't properly handle the creation of a new blob when exceeding
the maximum capacity of the current blob.
EvernestBack doesn't use thread pools, which may cause slowdowns when using a
moderately big number of streams.
Indexing information updates and retrieval aren't robust enough.
