using ApiRawg.Data;
using ApiRawg.Service;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using Serilog;

// 1. O Serilog vai gravar na sua tela (Console) e também criar um arquivo de texto no servidor sozinho
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/api-log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog(); // Substitui o log padrão do .NET pelo Serilog

    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();

    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
        {
            Version = "v1",
            Title = "API RAWG",
            Description = "API para gerenciamento de jogos usando Firestore",
        });

        var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        options.IncludeXmlComments(xmlPath);
    });

    builder.Services.AddSingleton<FirestoreData>();
    builder.Services.AddScoped<JogoService>();
    builder.Services.AddHttpClient();

    var caminhoChave = Path.Combine(Directory.GetCurrentDirectory(), "chave_API/firebase-key.json");
    var credential = GoogleCredential.FromFile(caminhoChave);

    var firestoreData = new FirestoreDbBuilder
    {
        ProjectId = "apirawg",
        Credential = credential,
    }.Build();

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("PermitirTudo", policy =>
        {
            policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
        });
    });

    var app = builder.Build();

    app.UseSerilogRequestLogging(); // Registra automaticamente as rotas chamadas e os status codes

    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.RoutePrefix = string.Empty;
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "API RAWG V1");
    });

    app.UseCors("PermitirTudo");
    app.UseHttpsRedirection();
    app.UseAuthorization();
    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Falha crítica na API");
}
finally
{
    Log.CloseAndFlush();
}