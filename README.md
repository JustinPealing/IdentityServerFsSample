# Fs.Identity

Using Identity Server 4 from F#.

To run (from `AuthServer` directory):

    dotnet watch run

Example requests

    http://localhost:5000/connect/authorize?response_type=id_token&state=&client_id=client&scope=openid%20profile&redirect_uri=http%3A%2F%2Flocalhost%3A5002%2Fsignin-oidc&nonce=12345