using Google.Cloud.Firestore;
using System.Text.Json;

namespace ApiRawg.Data
{
    public class FirestoreData
    {

        public FirestoreDb Db { get; private set;}

        public FirestoreData()
        {

            var chave = "chave_API/firebase-key.json";
            var fullPath = Path.Combine(AppContext.BaseDirectory, chave);

            if (!File.Exists(fullPath))
            {
                throw new FileNotFoundException($"O arquivo de chave do Firebase não foi encontrado: {fullPath}");
            }

            string jsonList = File.ReadAllText(fullPath);
            using JsonDocument doc = JsonDocument.Parse(jsonList);

            if(!doc.RootElement.TryGetProperty("project_id", out var projectIdElement))
            {
                throw new Exception("O arquivo de chave do Firebase não contém a propriedade 'project_id'.");
            }


            string projectId = projectIdElement.GetString();

            var builder = new FirestoreDbBuilder
            {
              
                ProjectId = projectId,
                CredentialsPath = fullPath

            };

            Db = builder.Build();
        }

    }
}
