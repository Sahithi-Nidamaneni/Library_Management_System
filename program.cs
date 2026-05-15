using Library_Management.Helpers;

using Library_Management.Repositories;

using Library_Management.Services;

using Azure.Messaging.ServiceBus;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

builder.Services.AddScoped<DbHelper>();

builder.Services.AddScoped<UserRepository>();

builder.Services.AddScoped<MembershipRepository>();

builder.Services.AddScoped<BookRepository>();

builder.Services.AddScoped<BorrowRepository>();

//builder.Services.AddScoped<NotificationRepository>();

builder.Services.AddScoped<AzureStorageService>();

// ✅ ServiceBusClient ko Singleton register karo

builder.Services.AddSingleton(sp =>

{

	var config = sp.GetRequiredService<IConfiguration>();

	var connectionString = config["ServiceBus:ConnectionString"];

	return new ServiceBusClient(connectionString);

});

var app = builder.Build();
app.UseDeveloperExceptionPage();

app.UseSwagger();

app.UseSwaggerUI();

app.UseAuthorization();

app.MapControllers();

app.Run();
