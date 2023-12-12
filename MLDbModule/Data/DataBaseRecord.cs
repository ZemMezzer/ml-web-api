using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace MLDbModule.Data;

[Serializable]
public class DataBaseRecord
{
    [BsonId] private Guid _id;
    [BsonElement("Name")] private string _name; 
    [BsonElement("Data")] private Dictionary<string, string> _data;

    [BsonIgnore] public Guid Id => _id;
    [BsonIgnore] public string Name => _name;

    public DataBaseRecord(string name, Dictionary<string, object> data)
    {
        _name = name;
        _data = new Dictionary<string, string>();
        
        foreach (var keyValuePair in data)
            _data.TryAdd(keyValuePair.Key, JsonConvert.SerializeObject(keyValuePair.Value));
    }

    public bool TryGet<T>(string key, out T? value)
    {
        value = default;
        
        if (_data.TryGetValue(key, out var rawValue))
        {
            try
            {
                value = JsonConvert.DeserializeObject<T>(rawValue);
                return true;
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        return false;
    }

    public bool TryUpdate<T>(string key, T value)
    {
        if (value == null)
        {
            Console.WriteLine(new NullReferenceException($"Record data value can't be null"));
            return false;
        }
        
        if (_data.TryAdd(key, JsonConvert.SerializeObject(value)))
            return true;
        
        if(!_data.ContainsKey(key))
            return false;

        _data[key] = JsonConvert.SerializeObject(value);
        return true;
    }
}