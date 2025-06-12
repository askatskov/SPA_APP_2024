using Microsoft.EntityFrameworkCore;
using backend.Model;
using Microsoft.IdentityModel.Tokens;

using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<DataContext>(options =>
	options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddAuthentication(options =>
{
	options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
	options.TokenValidationParameters = new TokenValidationParameters
	{
		ValidateIssuer = true,
		ValidateAudience = true,
		ValidateLifetime = true,
		ValidateIssuerSigningKey = true,
		ValidIssuer = "your_issuer",
		ValidAudience = "your_audience",
		IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("your_secret_key_your_secret_key_your_secret_key"))
	};
});

// Other configurations


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(o => o.AddPolicy("MyPolicy", builder => {
	builder
	.SetIsOriginAllowed(_ => true)
	.AllowCredentials()
	.AllowAnyMethod()
	.AllowAnyHeader();
}));

var app = builder.Build();

// Ensure the database is created.
EnsureDatabaseCreated(app.Services);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
	app.UseCors("MyPolicy");
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();


void EnsureDatabaseCreated(IServiceProvider services)
{
	using var scope = services.CreateScope();
	var context = scope.ServiceProvider.GetRequiredService<DataContext>();
	context.Database.EnsureCreated();
}