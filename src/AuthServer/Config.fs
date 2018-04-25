module Config

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