using VantageView.Admin.Portal.Components;

using VantageView.Admin.Portal.Services;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.
string apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? "https://localhost:7021";
string authBaseUrl = builder.Configuration["AuthBaseUrl"] ?? "https://localhost:7128";

builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<AuthHandler>();
builder.Services.AddHttpClient("Api", client => client.BaseAddress = new Uri(apiBaseUrl))
    .AddHttpMessageHandler<AuthHandler>();
builder.Services.AddHttpClient("Auth", client => client.BaseAddress = new Uri(authBaseUrl));

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
