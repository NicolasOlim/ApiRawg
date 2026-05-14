using ApiRawg.Data;
using ApiRawg.Models;
using Google.Cloud.Firestore;

namespace ApiRawg.Service
{
    public class JogoService
    {

        private readonly ILogger<JogoService> _logger;
        private readonly FirestoreData _firestoreData;
        private readonly string _collectionName = "jogos";
        private readonly HttpClient _httpClient;

        public JogoService(ILogger<JogoService> logger, FirestoreData firestoreData, HttpClient httpClient)
        {
            _logger = logger;
            _firestoreData = firestoreData;
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "ApiRawg/1.0");
        }

        public async Task<List<Jogo>> Listar()
        {
            CollectionReference collection = _firestoreData.Db.Collection(_collectionName);
            QuerySnapshot snapshot = await collection.GetSnapshotAsync();

            List<Jogo> jogos = new List<Jogo>();

            foreach (DocumentSnapshot document in snapshot.Documents)
            {
                if (document.Exists)
                {
                    var jogo = document.ConvertTo<Jogo>();
                    jogo.Id = document.Id;
                    jogos.Add(jogo);
                }
            }

            return jogos;
        }

    }
}
