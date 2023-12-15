using MLDbModule.Data;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MLDbModule;

public class DataBaseController
{
    private readonly IMongoDatabase _database;
    
    public DataBaseController(string connectionString, string database)
    {
        _database = new MongoClient(new MongoUrl(connectionString)).GetDatabase(database);
    }

    public void UpsertRecord(string table, DataBaseRecord record)
    {
        var collection = _database.GetCollection<DataBaseRecord>(table);

        if (TryGetRecord(table, record.Id, out DataBaseRecord? baseRecord))
        {
            collection.ReplaceOne(new BsonDocument("_id", record.Id), record, new ReplaceOptions() { IsUpsert = true });
            return;
        }
        
        collection.InsertOne(record);
    }

    public bool TryGetRecord(string table, Guid id, out DataBaseRecord? record) => TryGetRecord(table, DbKeywords.Id, id, out record);

    public bool TryGetRecordByName(string table, string? name, out DataBaseRecord? record) => TryGetRecord(table, DbKeywords.Name, name, out record);

    public bool TryGetRecord(string table, string parameterName, object? parameter, out DataBaseRecord? result)
    {
        result = default;
        
        var collection = _database.GetCollection<DataBaseRecord>(table);
        var filter = Builders<DataBaseRecord>.Filter.Eq(parameterName, parameter);

        var searchResult = collection.Find(filter);

        if (searchResult.CountDocuments() > 0)
            result = searchResult.First();
        
        return result != default;
    }
}