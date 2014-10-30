
Evernest API
============

 * Overview
    o JSON
    o Access by Object
 * Keys and Rights
    o Usage jailing
    o Key mechanism for sources and user
 * API Objects
    o Event
    o Stream
    o Source
    o User
    o Right
    o UserRight
 * Request/Response overview
    o Different HTTP methods (at least GET and POST since user send data)
    o HTTP response code + error/success field in answer
 * Data selection (partials)
 * Paging
    o Cursoring
    o Since/After
 * Visibility
 * Rate limits
    o Limitation on returned data length
    o Quota Cost
 * High-level
    o Statistics
    o Search
 * Best practices






## Objects

This is a full description of Evernest API Objects.
As appropriate, objects effectively returned by API calls can be only partial.


### Event

```
{
	"Id": 1234,
	"ParentStream": {Stream},
	"Content": "This is a sample event."
}
```

`Id`/`ParentStream` is unique.


### Stream

```
{
	"Id": 4567,
	"Name": "Example of Stream"
}
```

`Id` is unique.


### Source

```
{
	"Id": 8901,
	"ParentUser": {User},
	"Name": "Example of Source",
	"Key": "MqJksh1GMRdo3pbgiKOBxtIrvqf7ikkl"
}
```

`Id`/`ParentUser` is unique.
`Name` and `Key` are private.


### User

```
{
	"Id": 2345,
	"Name": "Doe",
	"FirstName": "John",
	"AdminStreams": [{Stream}, {Stream}, …],
	"Sources": [{Source}, {Source}, …],
	"Key": "srAADi3VgIVSMZCd4TcRRi24ZcjQP13i"
}
```

`Id` is unique.


### Right

```
{
	"Id": 6789,
	"Source": {Source},
	"Stream": {Stream},
	"Type": "None|Read|Write|ReadWrite|Admin"
}
```


### UserRight

```
{
	"Id": 123,
	"User": {User},
	"Stream": {Stream},
	"Type": "None|Read|Write|ReadWrite|Admin"
}
```


## Request template

`{action} /{object}/{relation}`

Where:

 * `{action}` can be `GET`, `POST`, `PUT` or `DELETE`
 * `{object}` can be `Event`, `Stream`, `Source`, `User`, `Right`, `UserRight`


## Answer template

```
{
	"Status": "Success|Error",
	"FieldErrors": ["Foo", "Bar/Baz"],

	"Events": [{Event}, {Event}, …],
	"Streams": [{Stream}, {Stream}, …],
	"Sources": [{Source}, {Source}, …],
	"Users": [{User}, {User}, …],
	"Rights": [{Right}, {Right}, …],
	"UserRights": [{UserRight}, {UserRight}, …]
}
```

Header and content.
An empty field can not being here at all (e.g. there is no `Events` field in a response to a query about rights).


## Visibility rules

Say what is hidden?

### From a source key

If rule not set to `None`, can see other sources in relation with the stream.
See source = `Id, ParentUser`


### From a user key

Do anything the user can do.
Must be used for administration only. Exists for people wanting to create Evernest client, not for applications.



## Granting rights

A user cannot give more rights to its sources than it has on a stream.
What if rights change?


## Core principles

 * Personal accounts
 * Jail usages


## High-level API

Statistics?
Search?

