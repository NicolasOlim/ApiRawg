using ApiRawg.Data;
using ApiRawg.Models;
using Google.Cloud.Firestore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace ApiRawg.Service
{
    public class JogoService
    {
        private readonly ILogger<JogoService> _logger;
        private readonly FirestoreData _firestoreData;
        private readonly string _collectionName = "jogos";
        private readonly HttpClient _httpClient;
        private readonly string _apiKey = "53dad1fcdc4540d2a983ce2451f45b68"; // Chave do colega

        public JogoService(ILogger<JogoService> logger, FirestoreData firestoreData, HttpClient httpClient)
        {
            _logger = logger;
            _firestoreData = firestoreData;
            _httpClient = httpClient;
            // O RAWG exige um User-Agent, isso garante que eles não bloqueiem a sua requisição
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "ApiRawg_App_Academico/1.0");
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
            // 1. Chama a validação da API RAWG
            bool jogoReal = await ValidarJogo(jogo.Nome, jogo.Descricao);

            if (!jogoReal)
            {
                _logger.LogWarning("Jogo '{Nome}' não encontrado na API RAWG. Criação cancelada.", jogo.Nome);
                throw new Exception($"Jogo '{jogo.Nome}' não encontrado na API RAWG. Criação cancelada.");
            }

            // 2. Se for real, prossegue com a criação no Firestore
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

            string idString = novoId.ToString();
            DocumentReference docRef = _firestoreData.Db.Collection(_collectionName).Document(idString);

            jogo.Id = idString;
            await docRef.SetAsync(jogo);

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

        private async Task<bool> ValidarJogo(string nome, string descricao)
        {
            try
            {
                string nomeCodificado = Uri.EscapeDataString(nome);
                string apiUrl = $"https://api.rawg.io/api/games?key={_apiKey}&search={nomeCodificado}";

                var consulta = await _httpClient.GetAsync(apiUrl);

                if (!consulta.IsSuccessStatusCode)
                {
                    return false;
                }

                var resposta = await consulta.Content.ReadAsStringAsync();
                using JsonDocument doc = JsonDocument.Parse(resposta);

                int quantidadeResultados = doc.RootElement.GetProperty("count").GetInt32();

                if (quantidadeResultados == 0)
                {
                    return false;
                }

                JsonElement resultados = doc.RootElement.GetProperty("results");

                foreach (JsonElement resultado in resultados.EnumerateArray())
                {
                    string nomeDaApi = resultado.GetProperty("name").GetString();

                    if (nomeDaApi != null && nomeDaApi.Equals(nome, StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }

                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}