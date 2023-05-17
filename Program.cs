var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

string[] allowedDevOrigins = new string[] {
    "testServer", "testServer002"
};

string[] allowedProdOrigins = new string[] {
    "testServer", "testServer002"
};

builder.Services.AddCors((options) => {
    options.AddPolicy("CustomDevPolicy", (policeBuilder) => {
        policeBuilder.WithOrigins(allowedDevOrigins)
        .AllowAnyHeader()
        .AllowAnyMethod()
         .AllowCredentials();
    });
});


builder.Services.AddCors((options) => {
    options.AddPolicy("CustomProdPolicy", (policeBuilder) => {
        policeBuilder.WithOrigins(allowedProdOrigins)
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
    });
});


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseCors("CustomDevPolicy");
    app.UseSwagger();
    app.UseSwaggerUI();
} else 
{
    app.UseCors("CustomProdPolicy");
    app.UseHttpsRedirection();

}



app.UseAuthorization();

app.MapControllers();

app.Run();
