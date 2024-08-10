using System.Security.Authentication;
using AnTrello.Backend.Domain.Contracts.Dtos.Auth.Login;
using AnTrello.Backend.Domain.Contracts.Dtos.Auth.Register;
using AnTrello.Backend.Domain.Contracts.Dtos.AuthService.Login;
using AnTrello.Backend.Domain.Contracts.Dtos.UserService.Create;
using AnTrello.Backend.Domain.Contracts.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AnTrello.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : BaseController
{
    private const string REFRESH_TOKEN_NAME = "refreshToken";   
    
    private readonly IAuthService _service; 
    
    public AuthController(IAuthService service)
    {
        _service = service;
    }
    
    [HttpPost]
    [Route("[action]")]
    public async Task<ActionResult<LoginResponse>> Login(LoginRequest request, CancellationToken token)
    {
        try
        {
            var response = await _service.Login(request, token);
            
            AddRefreshTokenToResponse(Response, response.RefreshToken);
            //response.RefreshToken = null;
            
            return response;
        }
        catch (AuthenticationException e)
        {
            return StatusCode(StatusCodes.Status401Unauthorized, e.Message);
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
        }
    }
    
    [HttpPost]
    [Route("[action]")]
    public async Task<ActionResult<RegisterResponse>> Register(CreateUserRequest request, CancellationToken token)
    {
        var response = await _service.Register(request, token);
        AddRefreshTokenToResponse(Response, response.RefreshToken);
        //response.RefreshToken = null;
        
        return response;
    }

    [HttpPost]
    [Route("login/access-token")]
    public async Task<ActionResult> GetNewTokens(CancellationToken token)
    {
        var refreshToken = Request.Cookies[REFRESH_TOKEN_NAME];

        if (string.IsNullOrEmpty(refreshToken))
        {
            return StatusCode(StatusCodes.Status401Unauthorized, "Refresh token not passed");
        }

        var tokens = await _service.GetNewTokens(refreshToken, token);
        
        if (tokens == null)
            return StatusCode(StatusCodes.Status401Unauthorized, new {Error = "No such user or not valid refresh token"});
        
        AddRefreshTokenToResponse(Response, tokens.RefreshToken);
        
        return Ok(tokens);
    }
    
    [Authorize]
    [HttpPost]
    [Route("[action]")]
    public async Task<ActionResult> Logout(CancellationToken token)
    {
        Console.WriteLine(UserId);
        RemoveRefreshTokenFromResponse(Response);
        return Ok();
    }
    
    private void AddRefreshTokenToResponse(HttpResponse response, string refreshToken)
    {
         response.Cookies.Append(REFRESH_TOKEN_NAME, refreshToken, 
             new CookieOptions
             {
                 Expires = DateTimeOffset.UtcNow + TimeSpan.FromDays(7),
                 HttpOnly = true,
                 Domain = "localhost",
                 SameSite = SameSiteMode.None,
                 Secure = false,
             });
    }

    
    private void RemoveRefreshTokenFromResponse(HttpResponse response)
    {
        response.Cookies.Delete(REFRESH_TOKEN_NAME);/*Append(REFRESH_TOKEN_NAME, refreshToken, 
            new CookieOptions
            {
                Expires = DateTimeOffset.,
                HttpOnly = true,
                Domain = "localhost",
                Secure = false,
                SameSite = SameSiteMode.None
            });*/
    }
}