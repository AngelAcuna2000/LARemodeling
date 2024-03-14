using BusinessSolutionShared;
using MySql.Data.MySqlClient;
using System.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.SetBasePath(AppContext.BaseDirectory)
                     .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                     .AddEnvironmentVariables();

builder.Services.AddControllersWithViews();

builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

builder.Services.AddScoped<MySqlConnection>(provider =>
{
    var configuration = provider.GetRequiredService<IConfiguration>();

    var connectionString = configuration.GetConnectionString("client_inquiries");

    return new MySqlConnection(connectionString);
});

builder.Services.AddScoped<DbConnector>();

builder.Services.AddScoped<IDbConnection>(provider =>
{
    var dbConnector = provider.GetRequiredService<DbConnector>();

    return dbConnector.CreateConnection();
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error")
       .UseHsts();
}

app.UseHttpsRedirection()
   .UseStaticFiles()
   .UseRouting()
   .UseAuthorization();

app.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");

app.Run();
