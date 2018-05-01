module Config

open IdentityServer4
open IdentityServer4.Models

let windowsAuthenticationSchemeName = Microsoft.AspNetCore.Server.IISIntegration.IISDefaults.AuthenticationScheme
let allowRememberLogin = true

let apiResources = [
    ApiResource("justin", "Test resource")
]

let identityResources : IdentityResource list = [
    IdentityResources.OpenId();
    IdentityResources.Profile()
]

let clients = [
    Client(
        ClientId = "client",
        AllowedGrantTypes = GrantTypes.Implicit,
        RedirectUris = [| "http://localhost:5002/signin-oidc" |],
        AllowedScopes = [|
            IdentityServerConstants.StandardScopes.OpenId;
            IdentityServerConstants.StandardScopes.Profile
        |]
    )
]
