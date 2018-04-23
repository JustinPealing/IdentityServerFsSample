namespace Fs.Identity.AuthServer.UI

open Microsoft.AspNetCore.Mvc

type HomeController() = 
    inherit Controller()
    member this.Index() = 
        this.View()
