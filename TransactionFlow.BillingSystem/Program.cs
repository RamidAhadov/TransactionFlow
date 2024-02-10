using Autofac;
using Autofac.Extensions.DependencyInjection;
using log4net.Config;
using TransactionFlow.BillingSystem.DependencyResolvers.Autofac;
using TransactionFlow.BillingSystem.Utilities.AutoMapper;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper(typeof(AutoMapperProfile).Assembly);

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory())
    .ConfigureContainer<ContainerBuilder>(builder =>
    {
        builder.RegisterModule(new AutofacBusinessModule());
    });

//builder.Services.AddLog4Net();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//XmlConfigurator.Configure(new FileInfo("log4net.config"));

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();