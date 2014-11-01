
Evernest API
============


 * Overview
    - JSON
    - Access by Object
 * Authentication
    - Keys and Rights
    - Usage jailing
    - Key mechanism for sources and user
 * API Objects
    - General comments
    - Event
    - Stream
    - Source
    - User
    - Right
    - UserRight
 * API Actions
    - Read (`GET`)
    - Create (`POST`)
    - Update (`PATCH`)
    - Delete (`DELETE`)
 * Request/Response overview
    - Different HTTP methods (at least GET and POST since user send data)
    - HTTP response code + error/success field in answer
 * Data selection (partials)
 * Paging
    - Cursoring
    - Since/After
 * Visibility
 * Rate limits
    - Limitation on returned data length
    - Quota Cost
 * High-level
    - Statistics
    - Search
 * Best practices



## Overview

*todo*


## Authentication

*todo*


## API Objects

This section is a full description of Evernest API Objects. This page does not explains what the objets are, which is the role of the [Concepts](https://github.com/Evernest/Evernest/wiki/Concepts:-Events,-Streams-and-Rights) page, but shows how they are transmitted by the API.

As appropriate, objects effectively returned by API calls can be only partial, due to right restriction. For example, a given user can not see information about an unrelated stream.

Objects are described as a list of properties, which can be either integers, strings or other objects. In examples, `{Foo}` stands for an object of type `Foo`.


### General comments

#### Ids

Every objet has an `Id` field. It is not always totally unique since it can be related to a parent object (as events are related to streams) but are a way to identify the object instance.

#### Names

Some objects have a `Name` field. This is about to be like an identifier but for humans. Numerical Ids are not easy to remember so objects that are exposed to the User Interface can be named by the user.

There is technically no problem with multiple objects sharing the same name, although this is obviously not recommanded.

Name can be either public (for streams) or private (for sources). For more information, see the Visibility section.

#### Default selector

Selectors define what fields are returned by the server. For more information about selectors, see the Data selection section.

For each object, a default selector is given.


### Event

An event essentially stores a string in a stream.

#### Fields

 * `Id` *int*: Event identifier.
 * `ParentStream` *Stream*: Stream in which the event has been stored.
 * `Content` *string*: Event content.

#### Constraints

Within a given stream, event `Id` is unique. In other words, the pair `Id`/`ParentStream` is unique.

#### Default selector

`Id, ParentStream(Id), Content`

#### Example

```
{
	"Id": 1234,
	"ParentStream": {
		"Id": 4567
	},
	"Content": "This is a sample event."
}
```


### Stream

Streams are series of events.

#### Fields

 * `Id` *int*: Stream identifier.
 * `LastId` *int*: Id of the last inserted event.
 * `Count` *int*: Number of events within the stream.
 * `Name` *string*: Stream user name.

#### Constraints

`Id` is unique. It is not unique for a given user only since streams can be symetrically shared by many users as soon as they have the `Admin` rights on it.

`Name` is a public field since it may be shared by every users working on it.

#### Default selector

`Id, LastId, Count, Name`

#### Example

```
{
	"Id": 4567,
	"LastId": 45,
	"Count": 12,
	"Name": "Example of Stream"
}
```


### Source

A source represents a program access key to the API. It may be used by a program, or a module of a program.

A user can create as sources as he wishes in order to give different rights to different programs.

#### Fields

 * `Id` *int*: Stream identifier.
 * `ParentUser` *User*: User who owns the source.
 * `Name` *string*: Source name.
 * `Key` *base64-encoded int*: API access key.

#### Constraints

`Id`/`ParentUser` is unique.
`Name` and `Key` are private.

For a given user, source `Id` is unique. In other words, the pair `Id`/`ParentUser` is unique.

`Name` can not be seen by other users, which means you can manage your names as you wish. Nobody will care and it will not be a way to discover what program you are using.

#### Default selector

`Id, ParentUser(Id), Name, Key`

#### Example

```
{
	"Id": 8901,
	"ParentUser": {
		"Id": 234
	},
	"Name": "Example of Source",
	"Key": "MqJksh1GMRdo3pbgiKOBxtIrvqf7ikkl"
}
```


### User

A user represents a physical person, e.g. a user account.

It owns sources and administrates streams.

#### Fields

 * `Id` *int*: User identifier.
 * `Name` *string*: User personnal name.
 * `FirstName` *string*: User personnal first name.
 * `RelatedStreams` *{Stream} list*: List of streams that are related to this user. A related stream is a stream that is either readable, writable or administrated by the user.
 * `OwnedSources` *{Stream} list*: List of streams that are administrated by this user.


#### Constraints

`Id` is unique.

`RelatedStreams` and `OwnedSources` are redundant information and may not fully up to date.

#### Default selector

`Id, Name, FirstName, Key, RelatedStreams(items(Id)), OwnedSources(items(Id))`

#### Example

```
{
	"Id": 2345,
	"Name": "Doe",
	"FirstName": "John",
	"Key": "srAADi3VgIVSMZCd4TcRRi24ZcjQP13i"
	"RelatedStreams": [
		{ "Id": 678 },
		{ "Id": 901 }
	],
	"OwnedSources": [
		{ "Id": 234 },
		{ "Id": 567 },
		{ "Id": 890 }
	]
}
```


### Right

A right defines the priviledges a given source has on a given stream.

When no right entry exists for a pair of a source and a stream, that means that the source has no rights on the stream.

#### Fields

 * `Id` *int*: Right identifier.
 * `Source` *{Source}*: Source to which the right provides priviledges.
 * `Stream` *{Stream}*: Stream to which the right is related.
 * `Type` *None|Read|Write|ReadWrite|Admin*: Right type. This is an enumerable field. `Read` means that the source can only read events from stream. `Right` means that it can append events but not read it. `ReadWrite` is the addition of both previous rights and `Admin` means that the source can edit rights related to the target stream while still being able to read from and write to it.

#### Constraints

`Id` is unique.

The pair `Source`/`Stream` is unique too.

#### Default selector

`Id, Source(Id), Stream(Id), Type`

#### Example

```
{
	"Id": 6789,
	"Source": {Source},
	"Stream": {Stream},
	"Type": "ReadWrite"
}
```


### UserRight

User rights are very similar to rights except that they provides rights to *user* and not to *sources*.

#### Fields

 * `Id` *int*: User right identifier.
 * `User` *{User}*: User to which the user right provides priviledges.
 * `Stream` *{Stream}*: Stream to which the right is related.
 * `Type` *None|Read|Write|ReadWrite|Admin*: Right type. See `Right` `Type` field for more information.

#### Constraints

`Id` is unique.

The pair `User`/`Stream` is unique too.

#### Default selector

`Id, Source(Id), Stream(Id), Type`

#### Example

```
{
	"Id": 123,
	"User": {
		"Id": 456
	},
	"Stream": {
		"Id": 789
	},
	"Type": "Admin"
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

