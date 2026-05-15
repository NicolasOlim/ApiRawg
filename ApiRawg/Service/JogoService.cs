using ApiRawg.Data;
using ApiRawg.Models;
using Google.Cloud.Firestore;
using System.Reflection.Metadata.Ecma335;

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

        public async Task<Jogo> ObterPorId(string id)
        {
            DocumentReference docRef = _firestoreData.Db.Collection(_collectionName).Document(id);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

            if (snapshot.Exists)
            {
                var jogo = snapshot.ConvertTo<Jogo>();
                jogo.Id = snapshot.Id;
                return jogo;
            }

            return null;

        }

        public async Task<Jogo> Criar(Jogo jogo)
        {

            DocumentReference contadorId = _firestoreData.Db.Collection("contador").Document("contador_jogos");

            int novoId = await _firestoreData.Db.RunTransactionAsync(async transaction =>
            {


                DocumentSnapshot snapshot = await transaction.GetSnapshotAsync(contadorId);

                int idAtual = 0;

                if (snapshot.Exists)
                {
                    snapshot.TryGetValue("ultimoId", out idAtual);
                }

                int proximoId = idAtual + 1;

                Dictionary<string, object> atualizacaoContador = new Dictionary<string, object>
                {
                    { "ultimoId", proximoId }
                };

                transaction.Set(contadorId, atualizacaoContador, SetOptions.MergeAll);
                return proximoId;
            });

            return jogo;
        }

        public async Task Atualizar(string id, Jogo jogo)
        {
            DocumentReference docRef = _firestoreData.Db.Collection(_collectionName).Document(id);
            await docRef.SetAsync(jogo, SetOptions.MergeAll);
        }

        public async Task Excluir(string id)
        {
            DocumentReference docRef = _firestoreData.Db.Collection(_collectionName).Document(id);
            await docRef.DeleteAsync();

        }
    }
}
