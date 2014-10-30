
API study
=========

This is a quick overview of existing APIs. Just for inspiration.

Facebook Graph
--------------

 * Access by object then relation (node then edge of the Graph)
 * HTTP methods (GET, POST, DELETE)
 * HTTP response codes?
 * JSON
 * Heavy Access Token mechanism. Rights are granted on tokens and there is different ways to get it.
 * Query variables OR message content
 * API testing user interface


Lokad
-----

 * Access by action
 * Query variables
 * Basic Authentication
 * GET only (So no heavy content sent)
 * No response code
 * JSON
 * Delayed operations
 * Very few entry points
 * error/success field in answer


Twitter
-------

 * Access by object then location or action or attribute; or category/object (well, there is no consistency at all…)
 * Streaming API (avoid round trip)
 * JSON
 * OAuth
 * Multiple HTTP methods
 * Rate limiting of API use (to avoid security issues?)
 * Token timeout by *window*
 * HTTP response code and special headers + success/error fields
 * Limitation on returned data length
 * Cursoring
 * Request Syntax inside GET querystring
 * URL-encode everything


YouTube
-------

 * Access by object (called "resource") and operation (though HTTP method)
 * Reference by ID
 * HTTP methods (GET, POST, PUT, DELETE)
 * JSON
 * Register to get API key (i.e. associate the rights you want to your key)
 * Rate limits (quota cost for each operation) to force people to cache data and not to abuse from server computation power
 * Partial resources: Ask for each piece of data. Leads to a more evolutive API while always being backward compatible.
 * Additionnal Syntax for `field` parameters
 * Consistent request format
 * ETags for caching and to avoid conflicts
 * Possible GZipping


Evernest (expected)
--------

 * Access by ???
 * JSON
 * Different HTTP methods (at least GET and POST since user send data)
 * Token mechanism for sources and user. Token of Keys (token are limited in time) (like Google dev API key registration)
 * HTTP response code + error/success field in answer
 * API testing user interface?
 * Limitation on returned data length. +quota cost?
 * Cursoring (Twitter Timeline <=> Evernest Stream) + Paging system => More high-level API?
 * Streaming API for event pushing?
 * w@ time zones?
 * Documentation API (get info about the API though the API)?
 * Consistent request format




Misc though
-----------

 * Facebook: Overcomplicated token system, but quite clean API then.
 * Lokad: Actually not really relevant here, since it is a tiny read-only API
 * Twitter: lack of consistency and losing documentation. But interesting approach for timeline, which should be useful for Everest
 * Youtube: Well-defined concepts and clean API. Good documentation.

