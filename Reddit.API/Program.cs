using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Reddit.API.Configuration;
using Reddit.API.Respositories;
using Reddit.DAL;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.TryAddEnumerable(
    ServiceDescriptor.Singleton<IValidateOptions<ExternalServicesConfiguration>,
        ExternalServicesConfigurationValidation>());

builder.Services.AddOptions<ExternalServicesConfiguration>(
    ExternalServicesConfiguration.RedditApi)
    .Bind(builder.Configuration.GetSection("ExternalServices:RedditApi"))
.ValidateOnStart();

builder.Services.AddOptions<ExternalServicesConfiguration>(
    ExternalServicesConfiguration.RedditTokenApi)
    .Bind(builder.Configuration.GetSection("ExternalServices:RedditTokenApi"))
.ValidateOnStart();

builder.Services
    .AddSubredditProcessing(builder.Configuration);

builder.Services
    .TryAddScoped<IRedditRepository, RedditRepository>();

builder.Services.AddDbContext<RedditDbContext>( o => o.UseInMemoryDatabase("RedditDb")
    .EnableSensitiveDataLogging()
);


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
