# SqlSyringe (alpha/pre-release status)

<img src="https://raw.githubusercontent.com/suterma/SqlSyringe/master/doc/icon.gif" alt="icon" width="100" align="right">

**USE WITH CAUTION**

SqlSyringe is a SQL database exploration tool, for testing and administration purposes. It allows specific users to directly execute SQL commands to a database.

Implemented as a middleware component for the ASP.NET request pipeline, it serves specific HTML pages and code that executes SQL commands directly to a database.

It uses some minimal security measures:

  * A specific single source IP address must be configured and any request is checked against it.
  * Only requests via HTTPS are accepted
  * The connection string must be provided by the user and is never stored

I built this as an example project for learning ASP.NET core.

## Application
### .NET Core
In the target project, configure SqlSyringe in Startup.cs as a middleware:

```csharp
            //Use and configure SqlSyringe.
            app.UseMiddleware<Syringe>(new InjectionOptions() {
                //Enable SqlSyringe from a specific source IP only (will pass over request otherwise). Example: "::1" is IPv6 localhost
                FromIp = IPAddress.Parse("::1"), 
                //The connection string to use for queries. If omitted here, the user must provide it with each request.
                ConnectionString = "..."
            });
```
This registers the middleware in the ASP.NET Core request pipeline, waiting to handle appropriate requests.

### .NET 4.5

In the HttpApplication, create and register the SqlSyringe Module in the Global.asax.cs file:
```csharp
        /// <summary>
        ///     The SQL syringe module, for use in this application.
        /// </summary>
        /// <devdoc>Provide the options from a config file as necessary.</devdoc>
        private static readonly IHttpModule SqlSyringeModule = new Syringe(new InjectionOptions {
            //Enable SqlSyringe from a specific source IP only (will pass over request otherwise). Example: "::1" is IPv6 localhost
            FromIp = IPAddress.Parse("::1"), 
            //The connection string to use for queries. If omitted here, the user must provide it with each request.
            ConnectionString = "..."
            FromIp = IPAddress.Parse("::1")
        });
        
        //...
        
        /// <summary>
        ///     Executes custom initialization code after all event handler modules have been added.
        /// </summary>
        public override void Init() {
            base.Init();
            SqlSyringeModule.Init(this);
        }
```
This registers the module in the ASP.NET request pipeline, waiting to handle appropriate requests.

## Usage

To use the SqlSyringe on the running target, browse to the SqlSyringe injection page (https is required):

    https://your.domain/sql-syringe
    
This triggers the middleware component, which then serves the injection page:    

<img src="https://raw.githubusercontent.com/suterma/SqlSyringe/master/doc/sql-syringe-input.PNG" alt="The SQL injection page of SqlSyringe" width="480">

On submit and after succesful injection, you get a result either with data or the rows affected:

<img src="https://raw.githubusercontent.com/suterma/SqlSyringe/master/doc/sql-syringe-select-output.PNG" alt="sql-syringe-select-output.PNG" width="480">

## Demo
To see SqlSyringe in action you may want to use the [SqlSyringe practice project](https://github.com/suterma/SqlSyringe-Practice)

## Deploy on Azure
Use this button to automatically deploy this web app as your own Azure App

<a href="https://azuredeploy.net/" target="_blank"><img src="http://azuredeploy.net/deploybutton.png"/></a>

## Differentiation
Although named equally, this project is very different from the PHP project "SQL Syringe" by kkoivisto on https://sourceforge.net/projects/sqlsyringe/
