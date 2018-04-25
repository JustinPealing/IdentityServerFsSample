﻿open System
open System.IO
open Microsoft.AspNetCore
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Logging
open Microsoft.AspNetCore.Builder
open IdentityServer4.Models
open Giraffe
open Giraffe.Razor

let webApp =
    choose [
        route "/ping"   >=> text "pong"
        route "/"       >=> razorView "text/html" "Home/Index" () ]

let apiResources = [
    ApiResource("justin", "Test resource")
]

let clients = [
    Client(
        ClientId = "client",
        AllowedGrantTypes = GrantTypes.ClientCredentials,
        AllowedScopes = [|"justin"|],
        ClientSecrets = [|
            Secret("secret".Sha256())
        |]
    )
]

type Startup() =
    member __.ConfigureServices (services : IServiceCollection) =
        let env = services.BuildServiceProvider().GetService<IHostingEnvironment>()
        let viewsFolderPath = Path.Combine(env.ContentRootPath, "Views")
        services.AddRazorEngine(viewsFolderPath) |> ignore
        services.AddIdentityServer()
            .AddDeveloperSigningCredential()
            .AddInMemoryApiResources(apiResources)
            .AddInMemoryClients(clients) |> ignore
    
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
