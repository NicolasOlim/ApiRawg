using ApiRawg.Data;
using ApiRawg.Service;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using Serilog; // 1. Biblioteca de logs automáticos

// 2. Configura o Serilog antes de tudo para gravar no terminal e criar a pasta de arquivos
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/log-api.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

try
{
    Log.Information("Iniciando a API RAWG...");

    var builder = WebApplication.CreateBuilder(args);

    // 3. Avisa o projeto para usar o Serilog como sistema oficial de logs
    builder.Host.UseSerilog();

    // --- SEUS SERVIÇOS ORIGINAIS COMEÇAM AQUI ---
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
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
    });

    var app = builder.Build();

    // 4. Ativa o rastreador super leve de requisições do Serilog
    app.UseSerilogRequestLogging();

    // --- CONFIGURAÇÃO DO SWAGGER ADAPTADA ---
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.RoutePrefix = string.Empty; // Mantém na raiz para a MonsterASP funcionar!
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "API RAWG V1");
    });

    // --- MIDDLEWARES ---
    app.UseCors("PermitirTudo");
    app.UseHttpsRedirection();
    app.UseAuthorization();
    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    // Se der qualquer erro fatal que impeça a API de ligar, ele salva no log
    Log.Fatal(ex, "A API falhou ao iniciar.");
}
finally
{
    // Garante que o arquivo seja salvo corretamente antes do programa fechar
    Log.CloseAndFlush();
}