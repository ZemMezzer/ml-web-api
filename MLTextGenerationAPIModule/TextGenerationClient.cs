using MLApiCore;
using MLApiCore.Data;
using MLDbModule;
using MLDbModule.Data;
using MLTextGenerationAPIModule.Data;
using MLTextGenerationAPIModule.Requests;
using Newtonsoft.Json;

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

    public override async Task<GenerationResult> Generate(Guid userId, IReadOnlyDictionary<string, string?> input)
    {
        Dictionary<string, string?> output = new Dictionary<string, string?>();
        
        if (!input.TryGetValue(UserInput, out string? userInput) || string.IsNullOrEmpty(userInput))
        {
            LogError($"User input is empty!", output);
            return new GenerationResult(GenerationStatus.UserInvalid, output, userId);
        }
        
        if (!_dataBaseController.TryGetRecord(DbKeywords.Users, userId, out DataBaseRecord? userRecord) || userRecord == null)
        {
            LogError($"User with id {userId} was not found!", output);
            return new GenerationResult(GenerationStatus.UserInvalid, output, userId);
        }
        
        string? characterId = input.GetValueOrDefault(Character, DefaultCharacter);

        if (!_dataBaseController.TryGetRecordByName(DbKeywords.Characters, characterId, out DataBaseRecord? characterRecord) && characterRecord != null)
        {
            LogError($"Character with id \"{characterId}\" was not found!", output);
            return new GenerationResult(GenerationStatus.Failed, output, userId);
        }

        var username = GetUsername(userRecord, input);

        if (string.IsNullOrEmpty(username))
        {
            LogError($"Username is invalid", output);
            return new GenerationResult(GenerationStatus.UserInvalid, output, userId);
        }

        if (!characterRecord.TryGet(DbRecordsDataKeywords.CharacterData, out AiCharacterData characterData))
        {
            LogError($"Character data for character \"{characterId}\" was not found!", output);
            return new GenerationResult(GenerationStatus.Failed, output, userId);
        }

        if (input.TryGetValue(ApiKeywords.CharacterData, out string? rawCharacterData) && !string.IsNullOrEmpty(rawCharacterData))
        {
            try
            {
                characterData = JsonConvert.DeserializeObject<AiCharacterData>(rawCharacterData);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        if (!userRecord.TryGet(GetModelHistoryKey(characterData), out History? history))
            history = new History(characterData.InnerMessage);

        if (input.TryGetValue(ApiKeywords.History, out string? rawHistory) && !string.IsNullOrEmpty(rawHistory))
        {
            try
            {
                history = JsonConvert.DeserializeObject<History>(rawHistory);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        
        bool useHistory = false;

        if (input.TryGetValue(ApiKeywords.UseHistory, out string? value) && !string.IsNullOrEmpty(value))
            bool.TryParse(value, out useHistory);

        var resultHistory = useHistory ? history : new History(characterData.InnerMessage);

        bool isRequestComplete = false;
        
        TextGenerationApiRequest request = new TextGenerationApiRequest(username, userInput, characterData, resultHistory,(_, result) =>
        {
            output.AppendStatus(GenerationStatus.Success);
            output.TryAdd(ApiKeywords.TextGenerationResult, result);

            if (useHistory)
            {
                userRecord.TryUpdate(GetModelHistoryKey(characterData), resultHistory);
                _dataBaseController.UpsertRecord(DbKeywords.Users, userRecord);
                output.TryAdd(ApiKeywords.History, JsonConvert.SerializeObject(resultHistory));
            }

            isRequestComplete = true;
        });
        
        _requestsQueue.AddRequestInQueue(request);

        while (!isRequestComplete)
            await Task.Yield();

        return new GenerationResult(GenerationStatus.Success, output, userId);
    }

    private string GetModelHistoryKey(AiCharacterData characterData)
    {
        return $"{characterData.Name}_{DbRecordsDataKeywords.History}";
    }

    private string GetUsername(DataBaseRecord userRecord, IReadOnlyDictionary<string, string> input)
    {
        string username = userRecord.Name;

        if (userRecord.TryGet("name", out string? customName) && !string.IsNullOrEmpty(customName))
            username = customName;
        
        if (input.TryGetValue("name", out string? name) && !string.IsNullOrEmpty(name))
            username = name;

        return username;
    }
    
    private void LogError(string? message, Dictionary<string, string?> input)
    {
        Console.WriteLine(new Exception(message));
        input.AppendError(message);
    }
}