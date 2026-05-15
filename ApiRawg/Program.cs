using ApiRawg.Data;
using ApiRawg.Service;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.HttpLogging;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpLogging(logging =>
{

    logging.LoggingFields = HttpLoggingFields.All;

});

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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

var caminhoChave = Path.Combine(Directory.GetCurrentDirectory(),
    "chave_API/firebase-key.json");
var credential = GoogleCredential.FromFile(caminhoChave);

var firestoreData = new FirestoreDbBuilder
{
    ProjectId = "apirawg",
    Credential = credential,
}.Build();

builder.Services.AddCors(options =>
{
   options.AddPolicy("PermitirTudo", policy=>
    {
        policy.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});
var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseHttpLogging();
}


    app.UseSwagger();
app.UseSwaggerUI(options => { 

    options.RoutePrefix = string.Empty; // Define a raiz para o Swagger UI
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "API RAWG V1"); // Define o endpoint do Swagger JSON

});

// Se alguém entrar na raiz do site (/), será levado direto para o Swagger
// Redireciona, mas avisa o Swagger para não mostrar isso na tela
app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();

app.UseCors("PermitirTudo");
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
