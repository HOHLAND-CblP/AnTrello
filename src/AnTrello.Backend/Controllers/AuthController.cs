using AnTrello.Backend.Domain.Contracts.Dtos.User.Login;
using AnTrello.Backend.Domain.Contracts.Services;
using Microsoft.AspNetCore.Mvc;

namespace AnTrello.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController
{
    private readonly IAuthService _service; 
    
    public AuthController(IAuthService service)
    {
        _service = service;
    }
    
    [HttpPost]
    [Route("[action]")]
    public async Task<ActionResult<LoginResponse>> Login(LoginRequest request, CancellationToken token)
    {
        var response = await _service.Login(request, token);
        
        return response;
    }
}