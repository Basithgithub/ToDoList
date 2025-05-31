using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoList.DTOs.User;
using TodoList.Models;
using TodoList.Services.Contracts;
using ToDoList.Services.Contracts;

namespace TodoList.Services;

public class UserService(
    TodoListDBContext context,
    TokenService tokenService,
    IPhotoService photo,
    CurrentUser currentUser) : IUserService
{
    public async Task<string?> Login(LoginDTO input)
    {
        var user = await context.Users.FirstOrDefaultAsync(x =>
            x.Username == input.Username &&
            x.HashedPassword == MethodHelper.ComputeSHA512Hash(input.Password));

        if (user is null)
        {
            return null;
        }

        var token = tokenService.Generate(user);
        return token;
    }

    public async Task<string> Refresh()
    {
        var user = await context.Users.FirstAsync(x =>
            x.Id == int.Parse(currentUser.Id));

        var token = tokenService.Generate(user);
        return token;
    }

    public async Task<bool> ChangePassword(ChangePasswordDTO input)
    {
        var user = await context.Users.FirstOrDefaultAsync(x =>
            x.Id == int.Parse(currentUser.Id) &&
            x.HashedPassword == MethodHelper.ComputeSHA512Hash(input.CurrentPassword)
            );

        if (user is null)
        {
            return false;
        }

        user.HashedPassword = MethodHelper.ComputeSHA512Hash(input.NewPassword);
        await context.SaveChangesAsync();

        return true;
    }

    public async Task<User?> Create([FromForm] CreateUserDTO input)
    {
        var exists = await context.Users.AnyAsync(x =>
            x.Username == input.Username);

        var uploadPhotoToCloud = await photo.AddPhoto(input.Photo);

        if (exists)
        {
            return null;
        }

        var user = User.CreateCustomer(input.Username,input.Password, uploadPhotoToCloud.SecureUrl.AbsoluteUri,uploadPhotoToCloud.PublicId);

        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();
        return user;
    }

    public async Task<User?> Update(UpdateUserDTO input)
    {
        var user = await context.Users.FirstOrDefaultAsync(x =>
            x.Id == input.Id);

        if (user is null)
        {
            return null;
        }

        //Remove the existing photo in cloud and adding the new photo
        var deletePhotoInCloud = await photo.DeletePhoto(user.PhotoPublicId);
        var updateNewPhoto = await photo.AddPhoto(input.Photo);
        //update the newly updated image and public id  in the database
        user.photo = updateNewPhoto.SecureUrl.AbsoluteUri;
        user.PhotoPublicId = updateNewPhoto.PublicId;
        user.Username = input.Username;
        user.Role = input.Role;
        await context.SaveChangesAsync();

        return user;
    }

    public async Task<bool> Delete(int id)
    {
        var user = await context.Users.FirstOrDefaultAsync(x =>
            x.Id == id);

        if (user is null)
        {
            return false;
        }

        var uploadPhotoToCloud = await photo.DeletePhoto(user.PhotoPublicId);

        context.Users.Remove(user);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<UserDTO>> GetAll()
    {
        return await context.Users.Select(x => new UserDTO()
        {
            Username = x.Username,
            Id = x.Id,
            Role = x.Role
        }).ToListAsync();

    }
}
