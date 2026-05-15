using ApiRawg.Data;
using ApiRawg.Service;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("PermitirTudo");
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
