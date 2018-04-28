module AuthServer

open System.IO
open Microsoft.AspNetCore
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Http
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Logging
open Giraffe
open Giraffe.Razor

[<CLIMutable>]
type LoginViewModel =
    {
        ReturnUrl : string
    }

let buildLoginViewModelAsync returnUrl =
    { ReturnUrl = returnUrl }

let getAccountLogin : HttpHandler = 
    fun (next : HttpFunc) (ctx : HttpContext) ->
        let returnUrl = ctx.Request.Query.["returnUrl"].ToString()
        let vm = buildLoginViewModelAsync(returnUrl)
        razorHtmlView "Account/Login" vm next ctx

let webApp =
    choose [
        route "/account/login"   >=> getAccountLogin
        route "/"                >=> razorHtmlView "Home/Index" () ]

type Startup() =
    member __.ConfigureServices (services : IServiceCollection) =
        let env = services.BuildServiceProvider().GetService<IHostingEnvironment>()
        let viewsFolderPath = Path.Combine(env.ContentRootPath, "Views")
        services.AddRazorEngine(viewsFolderPath) |> ignore
        services.AddIdentityServer()
            .AddDeveloperSigningCredential()
            .AddInMemoryApiResources(Config.apiResources)
            .AddInMemoryIdentityResources(Config.identityResources)
            .AddInMemoryClients(Config.clients) |> ignore
    
    member __.Configure (app : IApplicationBuilder)
                        (env : IHostingEnvironment)
                        (loggerFactory : ILoggerFactory) =
        app.UseDeveloperExceptionPage() |> ignore
        app.UseIdentityServer() |> ignore
        app.UseStaticFiles() |> ignore
        app.UseGiraffe webApp |> ignore

[<EntryPoint>]
let main argv =
    WebHost.CreateDefaultBuilder(argv)
        .UseStartup<Startup>()
        .Build()
        .Run()
    0
