# SqlSyringe (alpha/pre-release status)

**USE WITH CAUTION, and for testing purposes only.**

SqlSyringe is a SQL database injection tool, for testing purposes. 

Used as a middleware, it allows to deliberately execute SQL command onto a database, using a provided connection string. As minimal security measure, it requires the use of the https protocol and of a specified source IP address.

I built this a an example project for learning MVC core.

## Usage

In the target project, configure the SqlSyringe in Startup.cs as a middleware:

```csharp
    //Enable SylSyringe from a specific source IP only (will fail silently otherwise)
    app.UseMiddleware<SqlSyringe>("1.1.1.1");
```

To use the Syringe, browse to the injection page (https is required):

    https://your.domain/syringe

:todo: Add Image
![The SQL injection page of syringe](https://github.com/adam-p/markdown-here/raw/master/src/common/images/icon48.png "The SQL injection page of syringe")


## Deploy on Azure
Use this button to automatically deploy this web app as your own Azure App

<a href="https://azuredeploy.net/" target="_blank"><img src="http://azuredeploy.net/deploybutton.png"/></a>
