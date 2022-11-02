global using registration_api.Data;
global using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddCors(options => options.AddDefaultPolicy(p => p.WithOrigins("*").AllowAnyMethod().AllowAnyHeader()));

builder.Services.AddControllers();

#region [ MY SQL ]
string mySqlConection = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContextPool<DbContext>(options =>
                options.UseMySql(mySqlConection,
                    ServerVersion.AutoDetect(mySqlConection)));
#endregion

#region [ Teste in SQL Server ]
//builder.Services.AddDbContext<DataContext>(options =>
//{
//    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnectionSQLServer"));
//});
#endregion

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseCors();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
