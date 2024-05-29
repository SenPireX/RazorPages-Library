using Library.Application.Dto;
using Library.Application.Infrastructure;
using Library.Application.Infrastructure.Repositories;
using Library.Application.Model;
using Library.Webapp.Services;

var builder = WebApplication.CreateBuilder(args);

// *************************************************************************************************
// SERVICES
// *************************************************************************************************
builder.Services.AddSingleton<LibraryContext>();

// * Repositories **********************************************************************************
builder.Services.AddTransient<LibraryRepository>();
builder.Services.AddTransient<LoanRepository>();
builder.Services.AddTransient<UserRepository>();
builder.Services.AddTransient<BookRepository>();

// * Services for authentication *******************************************************************
// To access httpcontext in services
builder.Services.AddHttpContextAccessor();
// Hashing methods
builder.Services.AddTransient<ICryptService, CryptService>();
builder.Services.AddTransient<AuthService>(provider => new AuthService(
    isDevelopment: builder.Environment.IsDevelopment(),
    db: provider.GetRequiredService<LibraryContext>(),
    crypt: provider.GetRequiredService<ICryptService>(),
    httpContextAccessor: provider.GetRequiredService<IHttpContextAccessor>()));
builder.Services.AddAuthentication(
        Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(o =>
    {
        o.LoginPath = "/User/Login";
        o.AccessDeniedPath = "/User/AccessDenied";
    });
builder.Services.AddAuthorization(o =>
{
    o.AddPolicy("OwnerOrAdminRole", p => p.RequireRole(Usertype.Owner.ToString(), Usertype.Admin.ToString()));
});


// * Other Services ********************************************************************************
builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddRazorPages();

// *************************************************************************************************
// MIDDLEWARE
// *************************************************************************************************

var app = builder.Build();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.Run();
