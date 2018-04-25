open System
open Microsoft.AspNetCore
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Logging
open Microsoft.AspNetCore.Builder
open IdentityServer4.Models
open Giraffe

let webApp =
    choose [
        route "/ping"   >=> text "pong"
        route "/"       >=> text "Hello, World!" ]

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
        services.AddIdentityServer()
            .AddDeveloperSigningCredential()
            .AddInMemoryApiResources(apiResources)
            .AddInMemoryClients(clients) |> ignore
    
    member __.Configure (app : IApplicationBuilder)
                        (env : IHostingEnvironment)
                        (loggerFactory : ILoggerFactory) =
        app.UseDeveloperExceptionPage()
        app.UseIdentityServer()
        app.UseStaticFiles()
        app.UseGiraffe webApp

[<EntryPoint>]
let main argv =
    WebHost.CreateDefaultBuilder(argv)
        .UseStartup<Startup>()
        .Build()
        .Run()
    0
