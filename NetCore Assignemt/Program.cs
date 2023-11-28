using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Facebook;

using NetCore_Assignemt.Data;
// API Key
string CONNECTION_STRING = "DefaultConnection";
// Google
string GOOGLE_CLIENT_SECRET = "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";
string GOOGLE_CLIENT_ID = "1027305466602-6ta3futotkkv4646klci1r1bjj9agama.apps.googleusercontent.com";
// Facebook

/////////////////////////////////////////////////////////////////////////////////////////////////
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString(CONNECTION_STRING) ?? throw new InvalidOperationException("Connection string '"+ CONNECTION_STRING + "' not found.");
builder.Services.AddDbContext<NetCore_Assignemt.Data.DbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Authentication
builder.Services.AddAuthentication().AddGoogle(options =>
{
    options.ClientId = GOOGLE_CLIENT_ID;
    options.ClientSecret = GOOGLE_CLIENT_SECRET;
}
);
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<NetCore_Assignemt.Data.DbContext>();
// Session
builder.Services.AddDistributedMemoryCache(); 
builder.Services.AddSession(cfg => {                    
cfg.Cookie.Name = "Test";             
cfg.IdleTimeout = new TimeSpan(0, 60, 0);
});


builder.Services.AddControllersWithViews();
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

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
