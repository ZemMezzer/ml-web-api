using Microsoft.AspNetCore.Mvc;
using MLApiCore;
using MLApiCore.Data;
using MLDbModule;
using MLDbModule.Data;
using MLTextGenerationAPIModule;
using Newtonsoft.Json;

namespace MLWebApi.Controllers;

[ApiController, Route("api/")]
public class GenerateController : MlApiControllerBase
{
    private readonly DataBaseController _dataBaseController;
    private readonly TextGenerationClient _textGenerationClient;
    
    public GenerateController(DataBaseController dataBaseController, TextGenerationClient client)
    {
        _dataBaseController = dataBaseController;
        _textGenerationClient = client;
    }
    
    
    [HttpPost("generate")]
    public async Task<string> Generate()
    {
        var requestString = await GetRequestString();
        
        try
        {
            var input = JsonConvert.DeserializeObject<Dictionary<string, string?>>(requestString);
            
            if(input == null)
                return GetEmptyDictionaryWithStatus(GenerationStatus.InvalidRequest).ToJson();

            if (!input.TryValidateAuth(_dataBaseController, out DataBaseRecord? userRecord) || userRecord == null)
                return GetEmptyDictionaryWithStatusAndErrorMessage(GenerationStatus.AccessDenied,"Invalid Username Or Password!").ToJson();

            var output = await GetGenerationResult(userRecord, input);
            return JsonConvert.SerializeObject(output.Output);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return string.Empty;
        }
    }

    private async Task<GenerationResult> GetGenerationResult(DataBaseRecord userRecord, Dictionary<string, string?> input)
    {
        var result = await _textGenerationClient.Generate(userRecord.Id, input);
        return result;
    }
}