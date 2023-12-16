using Microsoft.AspNetCore.Mvc;
using MLApiCore;
using MLApiCore.Data;
using MLDbModule;
using MLDbModule.Data;
using MLTextGenerationAPIModule;
using MLWebApi.Commands;
using Newtonsoft.Json;

namespace MLWebApi.Controllers;

[ApiController, Route("api/")]
public class GenerateController : MlApiControllerBase
{
    private readonly DataBaseController _dataBaseController;
    private readonly TextGenerationClient _textGenerationClient;
    private readonly CommandsHandler _commandsHandler;
    
    public GenerateController(DataBaseController dataBaseController, TextGenerationClient client, CommandsHandler commandsHandler)
    {
        _dataBaseController = dataBaseController;
        _textGenerationClient = client;
        _commandsHandler = commandsHandler;
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

    [HttpPost("execute")]
    public async Task<string> ExecuteCommand()
    {
        var requestString = await GetRequestString();
        
        var input = JsonConvert.DeserializeObject<Dictionary<string, string?>>(requestString);
        
        if(input == null)
            return GetEmptyDictionaryWithStatus(GenerationStatus.InvalidRequest).ToJson();

        if (!input.TryValidateAuth(_dataBaseController, out DataBaseRecord? userRecord) || userRecord == null)
            return GetEmptyDictionaryWithStatusAndErrorMessage(GenerationStatus.AccessDenied,"Invalid Username Or Password!").ToJson();

        if (!input.TryGetValue("command", out string? command) || string.IsNullOrEmpty(command))
            return GetEmptyDictionaryWithStatus(GenerationStatus.InvalidRequest).ToJson();

        Dictionary<string, string> output = new Dictionary<string, string>();

        string commandParam = string.Empty;

        if (input.TryGetValue("parameter", out string? param) && !string.IsNullOrEmpty(param))
            commandParam = param;

        if (_commandsHandler.TryExecuteCommand(userRecord, command, commandParam, out string result))
        {
            output.AppendStatus(GenerationStatus.Success);
            output.Add("result", result);
        }

        return output.ToJson();
    }

    private async Task<GenerationResult> GetGenerationResult(DataBaseRecord userRecord, Dictionary<string, string?> input)
    {
        var result = await _textGenerationClient.Generate(userRecord.Id, input);
        return result;
    }
}