
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
 * Request/Response overview
 * API Endpoints
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

Not case-sensitive


## Authentication

Access to API is of course restricted and so requires some way of authentication.

The most basic authentication would be to provide your personnal credentials everytime you request the API. But the major problem is that API are designed to be accessed by programs, not humans. So your credentials would be found inside your application code, and even binary, which is a very bad design.

Actually your credentials must remains in the human field. Programs would be identified by unique keys. Thus, when a program's key get corrupted, you can invalidate it without breaking all other running projects.

This leads to a fundamental principle of Evernest: *Usage jailing*. Evernest has been designed such as a problem in a given project can never affect other ones. And the key system is the main feature providing usage jailing.

These keys are called *sources* keys. A source represents a program, are a unit of a program that needs to access some stream. You can select which rights source has on which streams.

It differes from the *user* key, which is unique and has all the rights you have when you are logged in the administration interface.

Although it wants to insure this jailing, the API allow you to do everything from the API, so you better follow simple rules:

 * Although you can distribute your keys between programs as you wish, and even have a single key with rights uppon all your streams, don't do so. A key must have priviledges on streams related to one project at a time.

 * Never use your User key in programs meant to be packaged. This key is an administration key and must used by your administration tools only.


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
 * `UserName` *string*: User personnal name.
 * `Password` *hash*: Hash of user password concatenated to `PasswordSalt`.
 * `PasswordSalt` *hash*: Random string used to avoid pattern recognition in password hash.
 * `Name` *string*: User personnal name.
 * `FirstName` *string*: User personnal first name.
 * `RelatedStreams` *{Stream} list*: List of streams that are related to this user. A related stream is a stream that is either readable, writable or administrated by the user.
 * `OwnedSources` *{Stream} list*: List of streams that are administrated by this user.


#### Constraints

`Id` is unique.

`RelatedStreams` and `OwnedSources` are redundant information and may not fully up to date.

#### Default selector

`Id, UserName, Name, FirstName, Key, RelatedStreams(items(Id)), OwnedSources(items(Id))`

Note that password fields are not returned by default.

#### Example

```
{
	"Id": 2345,
	"UserName": "jdo",
	"Password": "j3M36oxUiV719mrqXyBaSRPm6nOhRdch",
	"PasswordSalt": "zMQJYyioafGRTXt5g4ehinz5l8vfljLp",
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
	"Source": {
		"Id": 123
	},
	"Stream": {
		"Id": 456
	},
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




## Request/Response overview

For more consistency, all requests and responses follow the same pattern. This pattern can be slightly different according to the HTTP method used.

### Request

#### Authentication

Requests to the API **always** require the following information an authentication key, or credentials. Human credentials must be used only when there is no other way to do, e.g. for the first API access ever. After that, you should use the user key.

When authenticating though a key, use the `key` field. For credentials, use `login` and `password` fields.

#### URL

Although API endpoints can varie due to the differences between objects, they try to follow the template `/{object}/{selector}/{action}/{arguments}`.

`{object}` can be `Event`, `Stream`, `Source`, `User`, `Right`, `UserRight`.

`{selector}` indicates which object is targeted. Some actions may not need any selection.

Available values for `{action}` depends on the object and `{arguments}` depend on the action itself.


## Response

HTTP response code + error/success field in answer

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




## API Endpoints

### Overview

Each endpoint is presented with a uniform information. The following subsections describes what are those fields.

#### URL

The *URL* field indicates what method you should use to access the endpoint. Note that this is just an indication. You can replace a POST request by a GET request with options inside the query string.

To do so, encode each JSON field in the `Parent(Field)` form. For example, the following POST body would be encoded as `?Event(Content)=This%20is%20a%20new%20event!`

```
{
	"Event": {
		"Content": "This is a new event!"
	}
}
```

#### Input data

The *Input data* describes the information that the request could be provided. It is encoded in the `Parent(Field)` form.

Mandatory fields are explicitely signaled by the **[required]** tag.

When not specified, no input data is processed.

#### Required rights

The *Required rights* are the least rights that the key used for authentication must have to be able to perform the request.

When not specified, access is public.

#### Returned value

The *Returned value* is always *single Something* or *list of Something*. It indicates if the value fields of the response template are empty or contains one or more elements.

When not specified, all lists are empty.

### Stream

#### Get stream info

Get stream by its Id.

**URL:** `GET /Stream/{StreamId}`

**Required rights:** `Read` right on the stream.

**Returned value:** single Stream

#### Get a random event

Get a random event from stream `{StreamId}`

**URL:** `GET /Stream/{StreamId}/Pull/Random`

**Required rights:** `Read` right on the stream.

**Returned value:** single Event

#### Get a single event

Get an event from `{StreamId}` by its Id.

If {Id} is negative, `{LastId}` is added to it so that it is counted from the end of the stream.

**URL:** `GET /Stream/{StreamId}/Pull/{Id}`

**Required rights:** `Read` right on the stream.

**Returned value:** single Event

#### Get a range of events

Get all events from stream `{StreamId}` whose Ids are between `{StartId}` and `{StopId}` (including bounds).

If these Ids are negatives, `{LastId}` is added to them so that they are counted from the end of the stream.

**URL:** `GET /Stream/{StreamId}/Pull/{StartId}/{StopId}`

**Input data:**

 * `Since` *int*: See paging section
 * `Before` *int*: See paging section

**Required rights:** `Read` right on the stream.

**Returned value:** list of Events

#### Post a new event

**URL:** `POST /Stream/{StreamId}/Push`

**Input data:**

 * `Event(Content)` *string* **[required]**: New event content.

**Required rights:** `Write` right on the stream

**Example:**

```
POST /Stream/19/Push HTTP/1.1
Host: api.evernest.org

{
	"Event": {
		"Content": "This is a new event!"
	}
}
```

### Sources

#### Get source info

Get source by its Id.

**URL:** `GET /Source/{SourceId}`

**Required rights:** `Read` or `Right` right on a stream in common between both the used key and the target source.

**Returned value:** single Source


### Users

#### Get user info

Get user by its Id.

**URL:** `GET /User/{UserId}`

**Returned value:** single User

*todo*: Returned fields depending on rights (common stream)




## Data selection (partials)

*todo*

## Paging

*todo*

## Visibility

*todo*

## Rate limits

*todo*

## High-level

*todo*

## Best practices

*todo*

## Request template

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

