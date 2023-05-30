using Microsoft.EntityFrameworkCore;
using SecurityLite.Models;
using Microsoft.AspNetCore.Identity;
using SecurityLite.Data;
using SecurityLite.Areas.Identity.Data;
using Quartz;
using SecurityLite.Jobs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages().AddRazorRuntimeCompilation();
builder.Services.AddDbContext<ModelsContext>(options=>options.UseSqlite(
    builder.Configuration.GetConnectionString("localDb")));
builder.Services.AddDbContext<SecurityLiteContext>(options =>
 options.UseSqlite(builder.Configuration.GetConnectionString("localDb")));

builder.Services.AddIdentity<SecurityLiteUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 4;
}).AddEntityFrameworkStores<SecurityLiteContext>().AddDefaultUI();
builder.Services.ConfigureApplicationCookie(opt =>
{
    opt.AccessDeniedPath = new PathString("/Identity/Account/AccessDenied");
    opt.LoginPath = new PathString("/Identity/Account/Login");
});
/*builder.Services.AddQuartz(q =>
{
    q.UseMicrosoftDependencyInjectionJobFactory();
    var jobKey = new JobKey("ReportSender");
    q.AddJob<ReportSender>(opts=>opts.WithIdentity(jobKey));
    q.AddTrigger(t =>
        t.ForJob(jobKey)
        .WithIdentity("ReportSender-trigger")
        .StartNow()
        .WithSimpleSchedule(x => x.WithIntervalInHours(24)
        .RepeatForever())
        );
}
);
builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);*/



var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
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
