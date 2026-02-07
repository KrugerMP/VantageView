using Microsoft.EntityFrameworkCore;
using VantageView.Auth.Domain.Auth.Commands;
using VantageView.Data;
using VantageView.Data.Entities;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.SetIsOriginAllowed(origin => new Uri(origin).Host is "localhost" or "127.0.0.1")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddScoped<IAuthCommands, AuthCommands>();

WebApplication app = builder.Build();


using (IServiceScope scope = app.Services.CreateScope())
{
    AppDbContext db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
    if (!db.Users.Any())
    {
        db.Users.AddRange(
            new Users()
            {
                UserName = "admin",
                Password = "b03ddf3ca2e714a6548e7495e2a03f5e824eaac9837cd7f159c67b90fb4b7342" // P@ssw0rd
            });
        db.SaveChanges();
    }
}

app.UseCors();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
