# SqlSyringe
SqlSyringe is a SQL database injection tool, for testing purposes. I built this a an example project for learning MVC core.

Used as a middleware, it allows to deliberately execute SQL command onto a database, using a provided connection string.

*USE WITH CAUTION, and for testing purposes only.*

## Usage

In the target project, configure the SqlSyringe in Startup.cs as a middleware:

    //Enable SylSyringe from a specific IP only.
    app.UseMiddleware<SqlSyringe>("1.1.1.1");

To use the Syringe, browse to the injection page:

    http://your.domain/syringe

:todo: Add Image

## Deploy on Azure
Use this button to automatically deploy this web app as your own Azure App

<a href="https://azuredeploy.net/" target="_blank"><img src="http://azuredeploy.net/deploybutton.png"/></a>
