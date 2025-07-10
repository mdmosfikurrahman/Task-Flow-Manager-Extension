using Task_Flow_Manager_Extension.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);
builder.ConfigureProjectSettings();

var app = builder.Build();
app.UseProjectConfiguration();

app.Run();