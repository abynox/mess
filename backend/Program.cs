using System.Net;
using System.Security.Claims;
using System.Text;
using Mess;
using Mess.Api;
using Mess.Auth;
using Mess.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using IPNetwork = Microsoft.AspNetCore.HttpOverrides.IPNetwork;

var builder = WebApplication.CreateBuilder(args);

Config.GetFromEnvironment();
Console.WriteLine("Connection string: " + Config.Instance.DbConnectionString);
if (File.Exists(Config.Instance.DbFilePath))
{
    Console.WriteLine("Migrating Database as a database already exists");
    using (var db = new AppDatabaseContext())
    {
        db.Database.Migrate();
    }
    Console.WriteLine("Migrated Database");
}


// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddControllersWithViews();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddDbContext<AppDatabaseContext>();
builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = "SmartScheme";
        options.DefaultChallengeScheme = "oidc";
        
    }).AddPolicyScheme("SmartScheme", "Cookie or JWT", options =>
    {
        options.ForwardDefaultSelector = context =>
        {
            // Check whether Bearer authorization header exists.
            string authHeader = context.Request.Headers["Authorization"];
            if (!string.IsNullOrEmpty(authHeader) &&
                authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                // If so use JWT authorization
                return JwtBearerDefaults.AuthenticationScheme;
            }

            // Otherwise fallback to cookies
            return CookieAuthenticationDefaults.AuthenticationScheme;
        };
    }).AddJwtBearer(options =>
    {
        // Create the JwtBearer Authorization scheme
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = Config.Instance.JwtIssuer,
            ValidAudience = Config.Instance.JwtIssuer,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Config.Instance.Secret))
        };
    })
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
    {
        // Create the Cookie Authorization scheme
        options.Events.OnRedirectToAccessDenied =
        options.Events.OnRedirectToLogin = c =>
        {
            c.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Task.FromResult<object>(null);
        };
        options.LoginPath = "/login";  // where unauthenticated users are redirected
        options.AccessDeniedPath = "/forbidden";
    })
    .AddOpenIdConnect("oidc", options =>
    {
        options.Authority = Config.Instance.OAuthAuthority;
        options.ClientId = Config.Instance.OAuthClientId;
        options.ClientSecret = Config.Instance.OAuthClientSecret; // Securely store and load this value
        options.ResponseType = "code";
        options.RequireHttpsMetadata = false; // remove before public deploy

        options.SaveTokens = true;

        options.Scope.Add("openid");
        options.Scope.Add("profile");
        options.Scope.Add("email");

        options.GetClaimsFromUserInfoEndpoint = true;

        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            NameClaimType = "name",
            ValidateIssuerSigningKey = false,
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true
        };
        
        options.TokenValidationParameters.ValidateIssuerSigningKey = false;
        options.TokenValidationParameters.SignatureValidator = (token, parameters) =>
        {
            return new JsonWebToken(token);
        };

        // Optionally handle events
        options.Events = new OpenIdConnectEvents
        {
            OnRedirectToIdentityProvider = context =>
            {
                if (context.Request.Path.ToString().StartsWith("/api/") && context.Request.Path.ToString() != "/api/v1/sso/login" && context.Request.Path.ToString() != "/api/v1/sso/cookie")
                {
                    context.Response.StatusCode = 401;
                    context.HandleResponse();
                }
                return Task.FromResult(0);
            },
            OnAuthenticationFailed = context =>
            {
                // Log or handle authentication failures
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                // Additional token validation can go here
                var claims = context.Principal!.Identities.First().Claims;

                var sub = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                var name = claims.FirstOrDefault(c => c.Type == "name")?.Value;

                if (sub == null)
                {
                    throw new Exception("A fatal error occured while authenticating the user. No id is associated with them");
                }
                
                // Resolve your DbContext via DI
                var db = context.HttpContext.RequestServices.GetRequiredService<AppDatabaseContext>();

                var user = db.Users.SingleOrDefault(u => u.OidcId == sub);
                if (user == null)
                {
                    user = new User
                    {
                        OidcId = sub!,
                        Name = name ?? "New user"
                    };
                    db.Users.Add(user);
                    db.SaveChanges();
                }
                else
                {
                    user.Name = name ?? user.Name;
                    db.SaveChanges();
                }
                return Task.CompletedTask;
            }
        };
    });
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("user", policy =>
    {
        policy.RequireClaim(ClaimTypes.NameIdentifier);
    });
});

// Middleware for getting the group and checking access, ...
builder.Services.AddScoped<GroupAccessFilter>();

var app = builder.Build();
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedProto,
    KnownNetworks = { new IPNetwork(IPAddress.Any, 0) },
    ForwardedProtoHeaderName = "X-Forwarded-Proto",
});

// Configure the HTTP request pipeline.
app.UseRouting();
app.MapOpenApi();
app.MapScalarApiReference();
app.UseFileServer();
app.UseWebSockets();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();



app.Run();

