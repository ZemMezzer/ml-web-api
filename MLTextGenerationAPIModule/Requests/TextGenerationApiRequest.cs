using MLDbModule.Data;
using MLTextGenerationAPIModule.Data;

namespace MLTextGenerationAPIModule.Requests;

public class TextGenerationApiRequest
{
    public DataBaseRecord Record { get; }
    public AiCharacterData AiCharacterData { get; }
    public string FormattedInput { get; }

    public event Action<DataBaseRecord, string> OnComplete;

    public TextGenerationApiRequest(DataBaseRecord record, AiCharacterData characterData, string formattedInput, Action<DataBaseRecord, string> onComplete)
    {
        Record = record;
        FormattedInput = formattedInput;
        AiCharacterData = characterData;

        OnComplete = onComplete;
    }

    internal void OnRequestCompleted(string result)
    {
        OnComplete?.Invoke(Record, result);
    }
}