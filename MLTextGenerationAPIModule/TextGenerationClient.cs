using MLApiCore;
using MLApiCore.Data;
using MLDbModule;
using MLDbModule.Data;
using MLTextGenerationAPIModule.Data;
using MLTextGenerationAPIModule.Requests;

namespace MLTextGenerationAPIModule;

public class TextGenerationClient : TextMlClientBase
{
    private readonly DataBaseController _dataBaseController;
    private readonly TextGenerationRequestsQueue _requestsQueue;

    private const string Character = "character";
    private const string DefaultCharacter = "Xeona";

    private const string UserInput = "promt";
    
    public TextGenerationClient(DataBaseController dataBaseController, string textGenerationApiUrl)
    {
        _dataBaseController = dataBaseController;
        _requestsQueue = new TextGenerationRequestsQueue(textGenerationApiUrl);
    }

    public override void Generate(Guid userId, Dictionary<string, string> input, Action<GenerationResult, Guid> onComplete)
    {
        if (!input.TryGetValue(UserInput, out string? userInput) || string.IsNullOrEmpty(userInput))
        {
            LogError($"User input is empty!", input);
        }
        
        if (!_dataBaseController.TryGetRecord(DbKeywords.Users, userId, out DataBaseRecord? record))
        {
            LogError($"User with id {userId} was not found!", input);
            
            onComplete?.Invoke(GenerationResult.UserInvalid, userId);
            return;
        }
        
        string? characterId = input.GetValueOrDefault(Character, DefaultCharacter);

        if (!_dataBaseController.TryGetRecordByName(DbKeywords.Characters, characterId, out DataBaseRecord? characterRecord))
        {
            LogError($"Character with id \"{characterId}\" was not found!", input);
            
            onComplete?.Invoke(GenerationResult.Failed, userId);
            return;
        }
            

        if (!characterRecord.TryGet(DbRecordsDataKeywords.CharacterData, out AiCharacterData characterData))
        {
            LogError($"Character data for character \"{characterId}\" was not found!", input);
            
            onComplete?.Invoke(GenerationResult.Failed, userId);
            return;
        }

        TextGenerationApiRequest request = new TextGenerationApiRequest(record, characterData, userInput, (_, result) =>
        {
            input.AppendStatus(GenerationResult.Success);
            input.TryAdd("result", result);
            onComplete?.Invoke(GenerationResult.Success, userId);
        });
        
        _requestsQueue.AddRequestInQueue(request);
    }

    public void UpdateHistoryForUser(Guid userId, string botMessage)
    {
        if (!_dataBaseController.TryGetRecord(DbKeywords.Users, userId, out DataBaseRecord? record))
        {
            Console.WriteLine(new Exception($"Can't find user with id {userId}"));
            return;
        }
    }
    
    private void LogError(string message, Dictionary<string, string> input)
    {
        Console.WriteLine(new Exception(message));
        input.AppendError(message);
    }
}