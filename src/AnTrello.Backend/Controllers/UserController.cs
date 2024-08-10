using AnTrello.Backend.Domain.Contracts.Dtos.UserService.Update;
using AnTrello.Backend.Domain.Contracts.Services;
using AnTrello.Backend.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AnTrello.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController(IUserService service) : BaseController
{
    private readonly IUserService _service = service;

    [Authorize]
    [HttpGet("")]
    public async Task<ActionResult> GetUser(CancellationToken token)
    {
        var user = await _service.GetById(UserId, token);
        
        return Ok(user);
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult> Update(UpdateUserRequest request, CancellationToken token)
    {
        if (request.User.Id != UserId)
            return StatusCode(StatusCodes.Status403Forbidden, "You can`t get info about another user");

        try
        {
            var user = _service.Update(request, token);
            
            return Ok(user);
        }
        catch (KeyNotFoundException e)
        {
            return StatusCode(StatusCodes.Status404NotFound, "No such user");
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
        }
    }
}