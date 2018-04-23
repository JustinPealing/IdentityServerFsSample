﻿open System
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.DependencyInjection
open Microsoft.AspNetCore.Builder
open IdentityServer4.Models

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

let configureServices (services : IServiceCollection) =
    services.AddMvc() |> ignore
    services.AddIdentityServer()
        .AddDeveloperSigningCredential()
        .AddInMemoryApiResources(apiResources)
        .AddInMemoryClients(clients) |> ignore

let configureApp (app : IApplicationBuilder) =
    app.UseDeveloperExceptionPage() |> ignore
    app.UseIdentityServer() |> ignore
    app.UseStaticFiles() |> ignore
    app.UseMvcWithDefaultRoute() |> ignore

[<EntryPoint>]
let main argv =
    WebHostBuilder()
        .UseKestrel()
        .ConfigureServices(configureServices)
        .Configure(Action<IApplicationBuilder> configureApp)
        .Build()
        .Run()
    0
