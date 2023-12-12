using MLDbModule;
using MLDbModule.Data;
using MLTextGenerationAPIModule;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace MLWebApi
{
    public class Program
    {
        public static void Main()
        {
            DataBaseController dataBaseController = new DataBaseController("mongodb://localhost:27017", "Xeona");
            TextGenerationClient textGenerationClient = new TextGenerationClient(dataBaseController, "http://localhost:5000/api/v1/chat");
            
            Dictionary<string, string> query = new Dictionary<string, string>()
            {
                {"promt", "Hello"}
            };
            
            textGenerationClient.Generate(new Guid("6ee51261-29ad-4b99-a8eb-44dd26bfe2f0"), query, (result, id) =>
            {
                Console.WriteLine(query);
            });

            Console.ReadKey();
        }
    }
}

