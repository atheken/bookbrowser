using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace BookBrowser.API;

[ApiController]
[Route("api/error")]
public class ErrorController: ControllerBase
{
    public async Task<ActionResult> HandleError([FromServices] IHostEnvironment hostEnvironment)
    {
        var ex = HttpContext.Features.Get<IExceptionHandlerFeature>();

        return StatusCode(500, "Sorry about the problem, we have logged it and will see what it will take to fix it.");
    }
}