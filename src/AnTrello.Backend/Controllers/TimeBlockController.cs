using AnTrello.Backend.Domain.Contracts.Dtos.TimeBlockService.Create;
using AnTrello.Backend.Domain.Contracts.Dtos.TimeBlockService.Delete;
using AnTrello.Backend.Domain.Contracts.Dtos.TimeBlockService.Update;
using AnTrello.Backend.Domain.Contracts.Dtos.TimeBlockService.UpdateOrder;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AnTrello.Backend.Domain.Contracts.Services;
using AnTrello.Backend.Domain.Entities;


namespace AnTrello.Backend.Controllers;

[ApiController]
[Route("/api/user/time-blocks")]
public class TimeBlockController : BaseController
{
    private readonly ITimeBlockService _service;

    public TimeBlockController(ITimeBlockService service)
    {
        _service = service;
    }

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<List<TimeBlock>>> GetAll(CancellationToken token)
    {
        var timeBlocks = await _service.GetAll(UserId, token);

        return Ok(timeBlocks);
    }
        
    /*[Authorize]
    [HttpGet]
    [Route("{id}")]
    public async Task<ActionResult<TimeBlock>> Get(long id, CancellationToken token)
    {
        var task = await _service.get(id, token);
        return task;
    }*/

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<TimeBlock>> Create(CreateTimeBlockRequest request, CancellationToken token)
    {
        request.UserId = UserId;
        var timeBlock = await _service.Create(request, token);
        
        return timeBlock;
    }

    [Authorize]
    [HttpPut]
    [Route("update-order")]
    public async Task<ActionResult> UpdateOrder([FromBody] UpdateOrderRequest request, CancellationToken token)
    {
        request.UserId = UserId;
        
        await _service.UpdateOrder(request, token);

        return Ok();
    }
    

    [Authorize]
    [HttpPut]
    [Route("{id}")]
    public async Task<ActionResult<TimeBlock>> Update(long id, [FromBody] UpdateTimeBlockRequest request, CancellationToken token)
    {
        request.Id = id;
        request.UserId = UserId;
        
        var timeBlock = await _service.Update(request, token);

        if (timeBlock == null)
            return StatusCode(StatusCodes.Status404NotFound, "Time block not found");
            
        return timeBlock;
    }
    
    [Authorize]
    [HttpDelete]
    [Route("{id}")]
    public async Task<ActionResult> Delete(long id, CancellationToken token)
    {
        try
        {
            var request = new DeleteTimeBlockRequest()
            {
                Id = id,
                UserId = UserId
            };

            await _service.Delete(request, token);

            return Ok();
        }
        catch (UnauthorizedAccessException e)
        {
            return StatusCode(StatusCodes.Status403Forbidden, e.Message);
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
        }
    }



}