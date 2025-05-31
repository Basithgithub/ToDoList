using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoList;
using TodoList.DTOs.User;
using ToDoList.Services.Contracts;


[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _service;

    public UserController(IUserService service)
    {
        _service = service;
    }

    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] LoginDTO input)
    {
        var token = await _service.Login(input);
        return token is null ? NotFound() : Ok(token);
    }

    [HttpPost("Refresh")]
    public async Task<IActionResult> Refresh()
    {
        var token = await _service.Refresh();
        return Ok(token);
    }

    [Authorize]
    [HttpPost("ChangePassword")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO input)
    {
        var result = await _service.ChangePassword(input);
        return result ? NoContent() : BadRequest();
    }

    [Authorize(Roles = Constants.ROLE_ADMIN)]
    [IgnoreAntiforgeryToken] 
    [HttpPost("Create")]
    public async Task<IActionResult> Create([FromForm] CreateUserDTO input)
    {
        var user = await _service.Create(input);
        return user is null ? Conflict() : Ok(user);
    }

    [Authorize(Roles = Constants.ROLE_ADMIN)]
    [HttpPut("Update")]
    public async Task<IActionResult> Update([FromForm] UpdateUserDTO input)
    {
        var user = await _service.Update(input);
        return user is null ? Conflict() : Ok(user);
    }

    [Authorize(Roles = Constants.ROLE_ADMIN)]
    [HttpDelete("Delete/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var user = await _service.Delete(id);
        return user ? NoContent() : NotFound();
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var users = await _service.GetAll();
        return Ok(users);
    }
}
