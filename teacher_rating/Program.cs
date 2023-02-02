using teacher_rating.Models;
using teacher_rating.Mongodb.Data.Interfaces;
using teacher_rating.Mongodb.Data.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<TeacherRatingDatabaseSettings>(
    builder.Configuration.GetSection("TeacherRatingDatabase"));

builder.Services.AddScoped<ITeacherRepository, TeacherRepository>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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