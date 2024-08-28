using AnTrello.Backend.Domain.Contracts.Dtos.TaskService.CreateTask;
using AnTrello.Backend.Domain.Contracts.Dtos.TaskService.DeleteTask;
using AnTrello.Backend.Domain.Contracts.Dtos.TaskService.UpdateTask;
using AnTrello.Backend.Domain.Contracts.Services;
using AnTrello.Backend.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AnTrello.Backend.Controllers;

[ApiController]
[Route("api/user/tasks")]
public class TaskController : BaseController
{
    private readonly ITaskService _service;

    public TaskController(ITaskService service)
    {
        _service = service;
    }

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<List<UserTask>>> GetAll(CancellationToken token)
    {
        var users = await _service.GetAllUsersTasks(UserId, token);

        return Ok(users);
    }
        
    [Authorize]
    [HttpGet]
    [Route("{id}")]
    public async Task<ActionResult<UserTask>> Get(long id, CancellationToken token)
    {
        var task = await _service.GetTask(id, token);
        return task;
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<UserTask>> Create(CreateTaskRequest request, CancellationToken token)
    {
        request.UserId = UserId;
        var task = await _service.Create(request, token);
        
        return task;
    }


    [Authorize]
    [HttpPut]
    [Route("{id}")]
    public async Task<ActionResult<UserTask>> Update(long id, [FromBody] UpdateTaskRequest request, CancellationToken token)
    {
        request.Id = id;
        request.UserId = UserId;
        
        var task = await _service.Update(request, token);

        if (task == null)
            return StatusCode(StatusCodes.Status404NotFound, "Task not found");
            
        return task;
    }
    
    [Authorize]
    [HttpDelete]
    [Route("{id}")]
    public async Task<ActionResult> Delete(long id, CancellationToken token)
    {
        try
        {
            var request = new DeleteTaskRequest
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