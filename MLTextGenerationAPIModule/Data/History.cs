using Newtonsoft.Json;

namespace MLTextGenerationAPIModule.Data;

[Serializable]
public class History
{
    [JsonProperty("internal")] private List<List<string>> _internalMessages;
    [JsonProperty("visible")] private List<List<string>> _visibleMessages;

    [JsonIgnore] public List<List<string>> Messages => _visibleMessages;

    public History(string innerMessage = "")
    {
        _internalMessages = new List<List<string>>();
        _visibleMessages = new List<List<string>>();

        _internalMessages.Add(new List<string>(){"<|BEGIN-VISIBLE-CHAT|>", innerMessage});
        _visibleMessages.Add(new List<string>(){"", innerMessage});
    }

    public void AddPromt(string message)
    {
        _internalMessages.Add(new List<string>(){$"{message}", ""});
        _visibleMessages.Add(new List<string>(){$"{message}", ""});
    }

    public void SetModelMessage(string message)
    {
        var internalMessagesPair = _internalMessages[^1];
        var visibleMessagesPair = _visibleMessages[^1];
        
        internalMessagesPair[^1] = $"{message}";
        visibleMessagesPair[^1] = $"{message}";
    }

    public void RemoveLast()
    {
        _internalMessages.RemoveAt(_internalMessages.Count-1);
        _visibleMessages.RemoveAt(_visibleMessages.Count-1);
    }

    public void Clear(string innerMessage)
    {
        _internalMessages = new List<List<string>>();
        _visibleMessages = new List<List<string>>();

        _internalMessages.Add(new List<string>(){"<|BEGIN-VISIBLE-CHAT|>", innerMessage});
        _visibleMessages.Add(new List<string>(){"", innerMessage});
    }
}