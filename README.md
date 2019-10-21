# SqlSyringe (alpha/pre-release status)

<img src="https://raw.githubusercontent.com/suterma/SqlSyringe/master/doc/icon.gif" alt="icon" width="100" align="right">

**USE WITH CAUTION, and for testing purposes only.**

SqlSyringe is a SQL database injection tool, for testing purposes. 

Used as a middleware in the ASP.NET request pipeline, it allows you to deliberately execute SQL command onto a database, using a provided connection string. 

It uses some minimal security measures:

  * A specific single source IP address must be configured and any request is checked against it.
  * Only requests via HTTPS are accepted
  * The connection string must be provided by the user and is never stored

I built this as an example project for learning .NET core.

## Application

In the target project, configure SqlSyringe in Startup.cs as a middleware:

```csharp
            //Enable SylSyringe from a specific source IP only (will pass over request otherwise)
            app.UseMiddleware<Syringe>(new InjectionOptions()
            {
                FromIp = IPAddress.Parse("::1") //Use your IP, e.g. ::1 is IPv6 localhost
            });
```

This registers the middleware in the request pipeline, waiting to handle appropriate requests.

## Usage

To use the SqlSyringe on the running target, browse to the SqlSyringe injection page (https is required):

    https://your.domain/sql-syringe
    
This triggers the middelware component, which then serves the injection page:    

<img src="https://raw.githubusercontent.com/suterma/SqlSyringe/master/doc/sql-syringe-input.PNG" alt="The SQL injection page of SqlSyringe" width="480">

On submit and after succesful injection, you get a result either with data or the rows affected:

<img src="https://raw.githubusercontent.com/suterma/SqlSyringe/master/doc/sql-syringe-select-output.PNG" alt="sql-syringe-select-output.PNG" width="480">

## Deploy on Azure
Use this button to automatically deploy this web app as your own Azure App

<a href="https://azuredeploy.net/" target="_blank"><img src="http://azuredeploy.net/deploybutton.png"/></a>

## Credits

 * The practice demo project is based on the [Contoso University example project by Microsoft](https://docs.microsoft.com/en-us/aspnet/mvc/overview/getting-started/getting-started-with-ef-using-mvc/creating-an-entity-framework-data-model-for-an-asp-net-mvc-application) (Apache License Version 2.0 licensed)

## Differentiation
Although named equally, this project is very different from the PHP project "SQL Syringe" by kkoivisto on https://sourceforge.net/projects/sqlsyringe/
