using AnTrello.Backend.Domain.Contracts.Dtos.PomodoroService.DeleteSession;
using AnTrello.Backend.Domain.Contracts.Dtos.PomodoroService.UpdateRound;
using AnTrello.Backend.Domain.Contracts.Dtos.PomodoroService.UpdateSession;
using AnTrello.Backend.Domain.Contracts.Services;
using AnTrello.Backend.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AnTrello.Backend.Controllers;

[ApiController]
[Route("api/user/timer")]
public class PomodoroController : BaseController
{
    private readonly IPomodoroService _service;

    public PomodoroController(IPomodoroService service)
    {
        _service = service;
    }

    [Authorize]
    [HttpGet]
    [Route("today")]
    public async Task<ActionResult<PomodoroSession>> GetTodaySession(CancellationToken token)
    {
        var session = await _service.GetTodaySession(UserId, token);

        return Ok(session);
    }
    
    [Authorize]
    [HttpPost]
    public async Task<ActionResult<PomodoroSession>> Create(CancellationToken token)
    {
        var session = await _service.Create(UserId, token);
        
        return Ok(session);
    }
    
    [Authorize]
    [HttpPut]
    [Route("round/{id}")]
    public async Task<ActionResult> UpdateRound(long id, [FromBody] UpdateRoundRequest request, CancellationToken token)
    {
        request.Id = id;
        request.UserId = UserId;
        
        await _service.UpdateRound(request, token);

        return Ok();
    }
    
    [Authorize]
    [HttpPut]
    [Route("{id}")]
    public async Task<ActionResult> Update(long id, [FromBody] UpdateSessionRequest request, CancellationToken token)
    {
        request.Id = id;
        request.UserId = UserId;
        Console.WriteLine(request.IsCompleted);
        await _service.UpdateSession(request, token);

        return Ok();
    }
    
    [Authorize]
    [HttpDelete]
    [Route("{id}")]
    public async Task<ActionResult> Delete(long id, CancellationToken token)
    {
        var request = new DeleteSessionRequest
        {
            Id = id,
            UserId = UserId
        };
        
        await _service.DeleteSession(request, token);

        return Ok();
    }
}