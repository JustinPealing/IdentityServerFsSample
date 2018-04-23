open System
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.DependencyInjection
open Microsoft.AspNetCore.Builder

let apiResources = []

let clients = []

let configureServices (services : IServiceCollection) =
    services.AddIdentityServer()
        .AddDeveloperSigningCredential()
        .AddInMemoryApiResources(apiResources)
        .AddInMemoryClients(clients) |> ignore

let configureApp (app : IApplicationBuilder) =
    app.UseDeveloperExceptionPage() |> ignore
    app.UseIdentityServer() |> ignore

[<EntryPoint>]
let main argv =
    printfn "Hello World from F#!"
    WebHostBuilder()
        .UseKestrel()
        .ConfigureServices(configureServices)
        .Configure(Action<IApplicationBuilder> configureApp)
        .Build()
        .Run()
    0
