# SqlSyringe (alpha/pre-release status)

<img src="https://raw.githubusercontent.com/suterma/SqlSyringe/master/doc/icon.gif" alt="icon" width="100" align="right">

**USE WITH CAUTION**

SqlSyringe is a SQL database exploration tool, for testing and administration purposes. It allows specific users to directly execute SQL commands to a database.

Implemented as a middleware component for the ASP.NET request pipeline, it serves specific HTML pages and code that executes SQL commands directly to a database.

It uses some minimal security measures:

  * A specific single source IP address must be configured and any request is checked against it.  
  * Only requests via HTTPS are accepted
  * The connection string is either configured or must be provided by the user
  * With .NET Core, conditional mapping allows customized access control
  * With .NET Framework 4.5, the ASP.NET authentication is supported

I built this as an example project for learning ASP.NET Core and multi-targeting NuGet packages.

## Application
These examples use a minimal configuration
### .NET Core
In the target project, configure SqlSyringe in Startup.cs as a middleware:

```csharp
//Use and configure SqlSyringe.
app.UseMiddleware<Syringe>(new InjectionOptions() {
    //Enable SqlSyringe from a specific source IP only. If not provided, IPv6 localhost is used (::1)
    FromIp = IPAddress.Parse("1.2.3.4"), 
    //The connection string to use for queries. If omitted here, the user must provide it with each request.
    ConnectionString = Configuration.GetConnectionString("DefaultConnection")
});
```
This registers the middleware in the ASP.NET Core request pipeline, waiting to handle appropriate requests.

### .NET Framework 4.5

In the target HttpApplication, create and register the SqlSyringe Module in the Global.asax.cs file:
```csharp
/// <summary>
///     The SQL syringe module, for use in this application.
/// </summary>
/// <devdoc>Provide the options from a config file as necessary.</devdoc>
private static readonly IHttpModule SqlSyringeModule = new Syringe(new InjectionOptions {
    //Enable SqlSyringe from a specific source IP only. If not provided IPv6 localhost is used (::1)
    FromIp = IPAddress.Parse("1.2.3.4"), 
    //The connection string to use for queries. If omitted here, the user must provide it with each request.
    ConnectionString = ConfigurationManager.ConnectionStrings["DbConnectionString"].ConnectionString
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

## Security considerations & advanced configuration

SqlSyringe is a powerful tool and with power comes responsibility. Make sure, you configure it appropriate to your access requirements.

### Options with .NET Core

* IP-Address
  
  Default is IPv6 localhost (`::1`). By restricting to serve responses only to a specific origin IP, you can make sure only a select organisation has access. However, IP addresses tend to change and in case you use e.g a firewall, load balancer, or reverse proxy, restricting the IP alone may not be helpful.

* Connection string
  
  Providing the connection string in the configuration saves the user from typing it in manually in each request, thus it is generally recommended. On the other hand, requiring the connection string from the user may serve as some kind of access control and/or allowing to deliberately use different connection variants on purpose.

* URL slug
  
  While the default path to SqlSyringe is `/sql-syringe`, you can provide a different slug, e.g. to make use of already exising path-based access-checks and/or to provide different instantiations of SqlSyringe with different configurations (e.g. for different databases)

* Authentication & Authorization
  
  SqlSyringe allows you to implement arbitrary custom requirements via the conditional middleware branching in .NET Core. See the below example for additional role- and username-based authentication and authorization. You could also require e.g. client certificates, cookie values etc.

### Advanced example with .NET Core
```csharp
// Conditionally handle requests (exclusively) with the SqlSyringe, based on additional request properties            
app.MapWhen(context =>
    context.Request.HttpContext.User.Identity.IsAuthenticated &&
    context.Request.HttpContext.User.IsInRole("sqladmin") &&
    context.Request.HttpContext.User.Identity.Name == "test@marcelsuter.ch",
    appBuilder => {
        //Use and configure SqlSyringe with it's own options.
        appBuilder.UseMiddleware<Syringe>(new InjectionOptions() {
            //Enable SqlSyringe from a specific source IP only. If not provided, IPv6 localhost is used (::1)
            FromIp = IPAddress.Parse("::1"),
            //Only handle a specific path ("sql-syringe" is used as default)
            UrlSlug = "/sql-syringe-to-my-database",
            //The connection string to use for queries. If omitted here, the user must provide it with each request.
            ConnectionString = Configuration.GetConnectionString("MyCustomConnection")
        });
    }
);
```
Configure the SqlSyringe at the end of the pipeline to make sure it does not interfere with existing middlewares.

### Options with .NET Framework 4.5

* IP-Address
  
  Default is IPv6 localhost (`::1`). By restricting to serve responses only to a specific origin IP, you can make sure only a select organisation has access. However, IP addresses tend to change and in case you use e.g a firewall, load balancer, or reverse proxy, restricting the IP alone may not be helpful.

* Connection string
  
  Providing the connection string in the configuration saves the user from typing it in manually in each request, thus it is generally recommended. On the other hand, requiring the connection string from the user may serve as some kind of access control and/or allowing to deliberately use different connection variants on purpose.

* URL slug
  
  While the default path to SqlSyringe is `/sql-syringe` you can provide a different slug, e.g. to make use of already exising path-based access-checks and/or to provide different instantiations of SqlSyringe with different configurations (e.g. for different databases)

* Role & user name

  Authentication
  
  SqlSyringe supports the built-in ASP.NET authentication schemes (implementing, the IPrincipal interface) to gather role and user information for the request. If you have other authentication schemes, you need to implement IPrincipal to have authorization with SqlSyringe. 

  Authorization
  
  SqlSyringe does not use the built-in authorizaton: If either a specific role and/or username(s) one is provided in the options, SylSyringe checks whether the request has `Identity.IsAuthenticated` on true and then authorizes access based on the provided role and username information.

### Advanced example with .NET Framework 4.5
```csharp
private static readonly IHttpModule SqlSyringeModule = new Syringe(new InjectionOptions {
    //Access path (default is "/sql-syringe")
    UrlSlug = "/admin/database/sql-syringe",
    //Enable SqlSyringe from a specific source IP only (default is IPv6 localhost)
    FromIp = IPAddress.Parse("8.8.8.8"), 
    ConnectionString = ConfigurationManager.ConnectionStrings["DbConnectionString"].ConnectionString,
    //SqlSyringe authorizes the request for itself when a user with one of the given role and name is authenticated.
    UserName = "jon.doe@example.com,bob,nancy",
    Role = "database-admin"
});
```

## Demo
To see SqlSyringe in action you may want to use the [SqlSyringe practice project](https://github.com/suterma/SqlSyringe-Practice)

## Deploy on Azure
Use this button to automatically deploy this web app as your own Azure App

<a href="https://azuredeploy.net/" target="_blank"><img src="http://azuredeploy.net/deploybutton.png"/></a>

## Differentiation
Although named equally, this project is very different from the PHP project "SQL Syringe" by kkoivisto on https://sourceforge.net/projects/sqlsyringe/
