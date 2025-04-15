using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;

using BCrypt.Net;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<IdentityDbContext<IdentityUser>>(options => 
    options.UseInMemoryDatabase("ChessUsers"));

builder.Services.AddIdentityCore<IdentityUser>()
    .AddEntityFrameworkStores<IdentityDbContext<IdentityUser>>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("your_super_secret_key_here_12345")),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();
app.UseWebSockets();

app.Use(async (context, next) =>
{
    if (context.WebSockets.IsWebSocketRequest)
    {
        // Проверка токена из query-параметра
        if (!context.Request.Query.TryGetValue("token", out var token) || 
            !ValidateToken(token))
        {
            context.Response.StatusCode = 401;
            return;
        }

        WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
        await HandleWebSocketAsync(webSocket);
    }
    else
    {
        await next();
    }
});

app.MapGet("/", () => "WebSocket Server is running...");

app.MapPost("/register", async (IdentityUser user, IdentityDbContext<IdentityUser> db) =>
{
	user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);
    db.Users.Add(user);
    await db.SaveChangesAsync();
    return Results.Created($"/users/{user.Id}", null);
});

app.MapPost("/login", (IdentityUser loginUser, IdentityDbContext<IdentityUser> db) =>
{
    var user = db.Users.FirstOrDefault(u => u.UserName == loginUser.UserName);
    if (user == null || !BCrypt.Net.BCrypt.Verify(loginUser.PasswordHash, user.PasswordHash))
        return Results.Unauthorized();

    var token = new JwtSecurityToken(
        claims: new[] { new Claim(ClaimTypes.Name, user.UserName ?? "Unknown") },
        expires: DateTime.UtcNow.AddDays(1),
        signingCredentials: new SigningCredentials(
            new SymmetricSecurityKey(Encoding.ASCII.GetBytes("your_super_secret_key_here_12345")), 
            SecurityAlgorithms.HmacSha256)
    );

    return Results.Ok(new 
    { 
        Token = new JwtSecurityTokenHandler().WriteToken(token) 
    });
});

app.Run();

static async Task HandleWebSocketAsync(WebSocket webSocket)
{
	var buffer = new byte[1024 * 4];
	WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

	while (!result.CloseStatus.HasValue)
	{
		var serverMsg = Encoding.UTF8.GetBytes("d7-d5");
		await webSocket.SendAsync(new ArraySegment<byte>(serverMsg, 0, serverMsg.Length), result.MessageType, result.EndOfMessage, CancellationToken.None);

		result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
	}

	await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
}

bool ValidateToken(string token)
{
	if (string.IsNullOrEmpty(token)) return false;

    var tokenHandler = new JwtSecurityTokenHandler();
    try
    {
        tokenHandler.ValidateToken(token, new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("your_super_secret_key_here_12345")),
            ValidateIssuer = false,
            ValidateAudience = false
        }, out _);
        return true;
    }
    catch
    {
        return false;
    }
}