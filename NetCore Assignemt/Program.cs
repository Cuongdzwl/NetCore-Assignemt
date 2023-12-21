using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NetCore_Assignemt.Areas.Identity.Data;
using Microsoft.AspNetCore.Authentication.Facebook;
using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using NetCore_Assignemt.Models;
using NetCore_Assignemt.Data;
using Microsoft.AspNetCore.Identity.UI.Services;
using NetCore_Assignemt.Services;   
using sib_api_v3_sdk;
using sib_api_v3_sdk.Client;
using System.Text.Json.Serialization;

const string CLOUD_CONNECTION_STRING = "Azure";
const string LOCAL_CONNECTION_STRING = "DefaultConnection";
// API Key

// Google
const string GOOGLE_CLIENT_SECRET = "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";
const string GOOGLE_CLIENT_ID = "1027305466602-6ta3futotkkv4646klci1r1bjj9agama.apps.googleusercontent.com";
// Facebook
const string FACEBOOK_CLIENT_SECRET = "bb0bc7fcdc0344575d2131530b6b8c3d";
const string FACEBOOK_CLIENT_ID = "3682826011964025";

const string BREVO_API = "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";
//"AppId": "3682826011964025",
//      "AppSecret": "bb0bc7fcdc0344575d2131530b6b8c3d"
/////////////////////////////////////////////////////////////////////////////////////////////////
var builder = WebApplication.CreateBuilder(args);

var conn = CLOUD_CONNECTION_STRING;

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString(conn) ?? throw new InvalidOperationException("Connection string '"+ conn + "' not found.");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Authentication
builder.Services.AddAuthentication().AddCookie();
builder.Services.AddAuthentication().AddGoogle(options =>
{
    options.ClientId = GOOGLE_CLIENT_ID;
    options.ClientSecret = GOOGLE_CLIENT_SECRET;
}
);

builder.Services.AddAuthentication().AddFacebook(options =>
{
    options.ClientId = FACEBOOK_CLIENT_ID;
    options.ClientSecret = FACEBOOK_CLIENT_SECRET;
    options.CallbackPath = "/signin-facebook";
});

builder.Services.AddAuthentication().AddTwitter(options =>
{
    options.ConsumerKey = "pWwUer4GtIHvI5nijxgKETDa4";
    options.ConsumerSecret = "U9GZl43A4zGsOafO03TSFn8N14Zncnn7fTTiy0W5LACphmK1mD"; 
});


// Identity
builder.Services.AddDefaultIdentity<User>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
    options.SignIn.RequireConfirmedEmail = true;
    options.User.RequireUniqueEmail = true;
})
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>();

// Session
builder.Services.AddDistributedMemoryCache(); 
builder.Services.AddSession(cfg => {                    
cfg.Cookie.Name = "Test";             
cfg.IdleTimeout = new TimeSpan(0, 60, 0);
});

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.Cookie.Name = ".AdventureWorks.Session";
    options.IdleTimeout = TimeSpan.FromSeconds(10);
    options.Cookie.IsEssential = true;
});

//Email
builder.Services.AddTransient<IEmailSender, EmailSender>();
sib_api_v3_sdk.Client.Configuration.Default.AddApiKey("api-key", BREVO_API);

//Payment
builder.Services.AddScoped<IVnPayServices,VnPayServices>();

builder.Services.AddControllersWithViews().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
}); 

// Api documentation generator
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
    // Api Listing
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    });
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.UseSession();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseForwardedHeaders();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();

