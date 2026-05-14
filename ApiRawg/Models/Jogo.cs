using Google.Cloud.Firestore;

namespace ApiRawg.Models
{
    [FirestoreData]
    public class Jogo
    {
        [FirestoreProperty]
        public string Id { get; set; }
        [FirestoreProperty] 
        public string Nome { get; set; }
        [FirestoreProperty]
        public string Descricao { get; set; }
        [FirestoreProperty]
        public string ImagemUrl { get; set; }
        [FirestoreProperty]
        public double Avaliacao { get; set; }
        [FirestoreProperty]
        public int Classificacao { get; set; }
        [FirestoreProperty]
        public DateTime Upload { get; set; }


    }
}
