using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Linq;
using System.Text;
using TodoList;
using TodoList.DTOs.CloudinarySettings;
using TodoList.Extensions;
using TodoList.Services;
using TodoList.Services.Contracts;

var builder = WebApplication.CreateBuilder(args);
//Middleware
builder.Services.AddApplicationExtensions(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
//app.MapUserEndpoints();
//app.MapTodoEndpoints();

app.Run();
