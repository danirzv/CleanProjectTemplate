using CleanProjectTemplate.Api.Authorization;
using CleanProjectTemplate.Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleanProjectTemplate.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class TestController : ControllerBase
{
    [HttpGet("Hello")]
    public string HelloWorld() => "Hello world!";

    [Authorize(Policies.SomePolicy)]
    [HttpGet("Restricted")]
    public string Restricted() => "Restricted!";

    [Authorize(Policies.SomePolicy)]
    [AllowAnonymous]
    [HttpGet("Unrestricted")]
    public string Unrestricted() => "here Anonymous is allowed too!";

    [HttpPost("HandledError")]
    public string HandledError() => throw new SampleHandledError("Some Handled-Error happened here!");

    [HttpPost("UnhandledError")]
    public string UnhandledError() => throw new SampleUnhandledError("Some Unhandled-Error happened here!");
}

public class SampleHandledError(string message) : HandledException(message);
public class SampleUnhandledError(string message) : Exception(message);
