module AuthServer

open System
open System.IO
open Microsoft.AspNetCore
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Http
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Logging
open Giraffe
open Giraffe.Razor
open IdentityServer4.Services
open Microsoft.AspNetCore.Authentication
open IdentityServer4.Stores

type ExternalProvider =
    {
        DisplayName : string;
        AuthenticationScheme : string;
    }

type LoginInputModel =
    {
        Username : string;
        Password : string;
        RememberLogin : Boolean;
        ReturnUrl : string;
    }

type LoginViewModel =
    {
        Input : LoginInputModel;
        AllowRememberLogin : Boolean;
        EnableLocalLogin : Boolean;
        ExternalProviders : ExternalProvider list
    }

let buildLoginViewModel (ctx : HttpContext) returnUrl =
    task {
        let interaction = ctx.GetService<IIdentityServerInteractionService>()
        let! context = interaction.GetAuthorizationContextAsync(returnUrl)
        if isNull context.IdP then
            return {
                Input = { Username = context.LoginHint; Password = ""; RememberLogin = false; ReturnUrl = returnUrl };
                AllowRememberLogin = false;
                EnableLocalLogin = false;
                ExternalProviders =
                [
                    { DisplayName = ""; AuthenticationScheme = context.IdP }
                ]
            }
        else
            let schemeProvider = ctx.GetService<IAuthenticationSchemeProvider>()
            let clientStore = ctx.GetService<IClientStore>()
            let! schemes = schemeProvider.GetAllSchemesAsync()
            let providers =
                query {
                    for scheme in schemes do
                    where (isNotNull scheme.DisplayName || String.Equals(Config.windowsAuthenticationSchemeName, scheme.Name, StringComparison.OrdinalIgnoreCase))
                    select { DisplayName = scheme.DisplayName; AuthenticationScheme = scheme.Name }
                }

            let! client = clientStore.FindEnabledClientByIdAsync(context.ClientId)
            
            return {
                Input = { Username = context.LoginHint; Password = ""; RememberLogin = false; ReturnUrl = returnUrl };
                AllowRememberLogin = Config.allowRememberLogin;
                EnableLocalLogin = false;
                ExternalProviders = Seq.toList providers
            }
    }

let getAccountLogin : HttpHandler = 
    fun (next : HttpFunc) (ctx : HttpContext) -> task {
            let returnUrl = ctx.Request.Query.["returnUrl"].ToString()
            let! vm = buildLoginViewModel ctx returnUrl
            return! razorHtmlView "Account/Login" vm next ctx
        }

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
    
    member __.Configure (app : IApplicationBuilder) (env : IHostingEnvironment) (loggerFactory : ILoggerFactory) =
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
