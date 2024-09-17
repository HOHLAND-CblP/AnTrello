using AnTrello.Backend.Domain.Contracts.Dtos.UserService.GetProfile;
using AnTrello.Backend.Domain.Contracts.Dtos.UserService.Update;
using AnTrello.Backend.Domain.Contracts.Services;
using AnTrello.Backend.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AnTrello.Backend.Controllers;

[ApiController]
[Route("api/user/profile")]
public class UserController: BaseController
{
    private readonly IUserService _service;

    public UserController(IUserService service)
    {
        _service = service;
    }

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<GetProfileResponse>> Get(CancellationToken token)
    {
        var user = await _service.GetProfile(UserId, token);
        
        return Ok(user);
    }

    [Authorize]
    [HttpPut]
    public async Task<ActionResult<User>> Update(UpdateUserRequest request, CancellationToken token)
    {
        request.Id = UserId;
        
        try
        {
            var user = await _service.Update(request, token);
            
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