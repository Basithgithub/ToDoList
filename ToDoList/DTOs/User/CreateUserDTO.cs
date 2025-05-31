namespace TodoList.DTOs.User;

public class CreateUserDTO
{
    public required string Username { get; set; }
    public string? Password { get; set; }
    public IFormFile? Photo { get; set; }
}
