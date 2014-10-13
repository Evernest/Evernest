EvernestWeb-virtual
===================

This program is a very simple implementation of the Evernest User API.
It is intended to be use for testing only. Tests can be related to the API itself (to check its consistency) or to developp tests while there is no current version of the API in the EvernestWeb project.

This API is available at `www.evernest.org/api/…`.

Current state
-------------

The following entry points have been implemented:

 * `/api/login`
 * `/api/push/event`
 * `/api/pull/event/random`
 * `/api/pull/event/<id1>/<id2>`

Please ask me for credentials to use it.

See [API specification](https://github.com/Evernest/Evernest/blob/master/doc/api-draft.md) for more information.
Tags are ignored.

Differences with documented API
-------------------------------

For testing purpose, an additionnal entry point has been added:

`/api/reset`

It simply cleans all tokens and events.

**Warning** Events are stored in RAM. It will be cleaned each time the server restart so do not rely on it.

If you find out more differences, please notify me!
