
Evernest API
============


 * Overview
    o JSON
    o Access by Object
 * Authentication
    o Keys and Rights
    o Usage jailing
    o Key mechanism for sources and user
 * API Objects
    o General comments
    o Event
    o Stream
    o Source
    o User
    o Right
    o UserRight
 * API Actions
    o Read (`GET`)
    o Create (`POST`)
    o Update (`PATCH`)
    o Delete (`DELETE`)
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


### Event

An event essentially stores a string in a stream.

#### Fields

 * `Id` *int*: Event identifier.
 * `ParentStream` *Stream*: Stream in which the event has been stored.
 * `Content` *string*: Event content.

#### Constraints

Within a given stream, event `Id` is unique. In other words, the pair `Id`/`ParentStream` is unique.

#### Example

```
{
	"Id": 1234,
	"ParentStream": {Stream},
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

#### Example

```
{
	"Id": 8901,
	"ParentUser": {User},
	"Name": "Example of Source",
	"Key": "MqJksh1GMRdo3pbgiKOBxtIrvqf7ikkl"
}
```


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

