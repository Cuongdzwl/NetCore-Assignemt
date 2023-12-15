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
const string LOCAL_CONNECTION_STRING = "WebApp";
// API Key

// Google
const string GOOGLE_CLIENT_SECRET = "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";
const string GOOGLE_CLIENT_ID = "1027305466602-6ta3futotkkv4646klci1r1bjj9agama.apps.googleusercontent.com";
// Facebook
const string FACEBOOK_CLIENT_SECRET = "21bbfc0d23bb25d3ecd68f15f16f4c19";
const string FACEBOOK_CLIENT_ID = "190652060774363";

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
builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    //options.IdleTimeout = TimeSpan.FromSeconds(5);
});

//Email
 builder.Services.AddTransient<IEmailSender, EmailSender>();
sib_api_v3_sdk.Client.Configuration.Default.AddApiKey("api-key", builder.Configuration["BrevoSMTP:api_key"]);

//Payment
builder.Services.AddScoped<IPaymentServices,PaymentServices>();

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

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseForwardedHeaders();

app.UseRouting();
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
