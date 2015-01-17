EvernestWeb
===========

EvernestWeb is a component of the Evernest event store. It is an Azure Website
project, providing a web administration interface for Evernest, including
functionnalities such as : user creation, stream creation and management, source
and right management.

It runs the website presenting Evernest but also the user manager of the official instance of Evernest.


It uses ASP.NET MVC and directory organisation follow the usual conventions of this framework:

 * `App_Start`: Contains the code executed at application start.
 * `Content`: Contains static assets (css, images).
 * `Scripts`: Contains static scripts (js).
 * `Models`, Views, Controllers: Usual MVC units.
 * `ViewModels`: Models related to the rendering of a specific view.

Application starts with `Global.asax.cs`.


