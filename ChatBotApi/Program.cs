using ChatBotApi.Hubs;
using ChatBotApi.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR(); // Add SignalR service

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder =>
        {
            builder.WithOrigins("http://localhost:4200")
                   .AllowAnyMethod()
                   .AllowAnyHeader()
                   .AllowCredentials(); // Allow credentials
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
// Use the defined CORS policy
app.UseCors("AllowSpecificOrigin");

app.MapControllers();

app.MapGet("/", async context =>
{
    await context.Response.WriteAsync("Hello World!");
});
//app.MapPost("BroadCast", async (string message, IHubContext<ChatHub, IChatClinet> context) =>
//{
//    await context.Clients.All.SendAsync(message);
//    await context.Clients.All.ReceiveMessage(message);
//    return message;     
//});
app.MapHub<ChatHub>("/chathub");

app.MapPost("/broadcast", async (HttpContext context, IHubContext<ChatHub, IChatClient> hubContext) =>
{
    // Read the request body as a JSON object
    using var reader = new StreamReader(context.Request.Body);
    var body = await reader.ReadToEndAsync();
    var data = JsonSerializer.Deserialize<Dictionary<string, string>>(body);

    if (data == null || !data.ContainsKey("user") || !data.ContainsKey("message"))
    {
        context.Response.StatusCode = 400; // Bad Request
        await context.Response.WriteAsync("Invalid input");
        return;
    }

    var user = data["user"];
    var message = data["message"];

    Console.WriteLine($"Received user: {user}, message: {message}");

    await hubContext.Clients.All.ReceiveMessage(user, message);
    await context.Response.WriteAsync("Message sent successfully");
});
//app.MapHub<ChatHub>("/chathub"); // Map the ChatHub endpoint



//app.UseEndpoints(endpoints =>
//{
//    endpoints.MapHub<ChatHub>("/chatsocket"); // Map SignalR hub
//});

app.Run();
