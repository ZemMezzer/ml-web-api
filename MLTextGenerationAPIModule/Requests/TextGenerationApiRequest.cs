using MLApiCore.Data;
using MLTextGenerationAPIModule.Data;

namespace MLTextGenerationAPIModule.Requests;

public class TextGenerationApiRequest
{
    public string Username { get; }
    public AiCharacterData CharacterData { get; }
    public string Promt { get; }
    public History History { get; }
    public event Action<GenerationStatus, string> OnComplete;

    public TextGenerationApiRequest(string username, string promt, AiCharacterData characterData, History history, Action<GenerationStatus, string> onComplete)
    {
        Username = username;
        CharacterData = characterData;
        History = history;
        Promt = promt;
        
        OnComplete = onComplete;
    }

    internal void OnRequestCompleted(GenerationStatus status, string result)
    {
        OnComplete?.Invoke(status, result);
    }
}