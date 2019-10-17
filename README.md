# SqlSyringe (alpha/pre-release status)

**USE WITH CAUTION, and for testing purposes only.**

SqlSyringe is a SQL database injection tool, for testing purposes. 

Used as a middleware, it allows to deliberately execute SQL command onto a database, using a provided connection string. 

It uses some minimal security measures:

  * A specific single source IP address must be configured and any request is checked against it.
  * Only requests via HTTPS are accepted
  * The connection string must be provided by the user and is never stored

I built this as an example project for learning .NET core.

## Application

In the target project, configure the SqlSyringe in Startup.cs as a middleware:

```csharp
    //Enable SylSyringe from a specific source IP only (will fail silently otherwise)
    app.UseMiddleware<SqlSyringe>("1.1.1.1");
```

This registers the middleware in the request pipeline, waiting to handle appropriate requests.

## Usage

To use the SqlSyringe on the running target, browse to the SqlSyringe injection page (https is required):

    https://your.domain/sql-syringe
    
This triggers the middelware component, which then serves the injection page:    

:todo: Add Image
![The SQL injection page of syringe](https://github.com/adam-p/markdown-here/raw/master/src/common/images/icon48.png "The SQL injection page of syringe")

On succesful injection, you get a result either with data or the rows affected:

:todo: Add Image
![The SQL injection page of syringe](https://github.com/adam-p/markdown-here/raw/master/src/common/images/icon48.png "The SQL injection page of syringe")

## Deploy on Azure
Use this button to automatically deploy this web app as your own Azure App

<a href="https://azuredeploy.net/" target="_blank"><img src="http://azuredeploy.net/deploybutton.png"/></a>
