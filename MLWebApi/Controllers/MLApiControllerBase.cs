using Microsoft.AspNetCore.Mvc;
using MLApiCore;
using MLApiCore.Data;

namespace MLWebApi.Controllers;

public class MlApiControllerBase : Controller
{
    protected async Task<string> GetRequestString()
    {
        StreamReader reader = new StreamReader(Request.Body, leaveOpen: false);
        var requestString = await reader.ReadToEndAsync();

        return requestString;
    }

    protected Dictionary<string, string?> GetEmptyDictionaryWithStatus(GenerationStatus status)
    {
        Dictionary<string, string?> output = new Dictionary<string, string?>();
        
        output.AppendStatus(status);

        return output;
    }

    protected Dictionary<string, string?> GetEmptyDictionaryWithStatusAndErrorMessage(GenerationStatus status, string errorMessage)
    {
        var output = GetEmptyDictionaryWithStatus(status);
        output.AppendError(errorMessage);

        return output;
    }
}