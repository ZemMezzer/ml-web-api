using Newtonsoft.Json;

namespace MLTextGenerationAPIModule.Data;

[Serializable]
public class History
{
    [JsonProperty("internal")] private List<List<string>> _internalMessages;
    [JsonProperty("visible")] private List<List<string>> _visibleMessages;

    public List<List<string>> Messages => _visibleMessages;

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
        int internalMessagesCount = _internalMessages.Count - 1;
        int visibleMessagesCount = _internalMessages.Count - 1;
            
        _internalMessages[internalMessagesCount][_internalMessages[internalMessagesCount].Count-1] = $"{message}";
        _visibleMessages[visibleMessagesCount][_visibleMessages[visibleMessagesCount].Count-1] = $"{message}";
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