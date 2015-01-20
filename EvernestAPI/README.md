EvernestAPI
===========

EvernestAPI provides a web API for Evernest to be used by users' programs.

Almost everything that can be done though graphical user console should be possible to
do though the API. (user creation is **not** possible).

The API reference is available in the wiki: [there][APIref].

The most important files are the following:

- `/App_Start/WebApiConfig.cs`, that defines the routes
- `/Controllers/*`, that contain all the controllers of the API
- `/Models/Tools.cs`, that contain some tools (eg requests' parsing)

Return values are serialized automatically, so there is no source file in the project for this serialization.

[APIref]: https://github.com/Evernest/Evernest/wiki/API-Reference "Go to API reference"
