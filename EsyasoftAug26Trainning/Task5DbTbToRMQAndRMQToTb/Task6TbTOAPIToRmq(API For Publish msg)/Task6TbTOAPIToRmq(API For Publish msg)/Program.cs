using Serilog;
Log.Logger = new LoggerConfiguration().WriteTo.Console().WriteTo.File("logs/app.log", rollingInterval: RollingInterval.Day).CreateLogger();
Log.Information("API Creation Service Start");

var builder = WebApplication.CreateBuilder(args);
Log.Information("Register Services...");
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


// Configure the HTTP request pipeline.
Log.Information("check api is development mode if true then enable swagger ");
if (app.Environment.IsDevelopment())
{
    Log.Information("sweeger midileware enable ");
    app.UseSwagger();
    app.UseSwaggerUI();
}
Log.Information("Enable Redirection middleware");
app.UseHttpsRedirection();
Log.Information("enable authoraization middleware");
app.UseAuthorization();

app.MapControllers();

app.Run();
