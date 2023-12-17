using MLDbModule;
using MLDbModule.Data;
using MLTextGenerationAPIModule.Data;
using Newtonsoft.Json;

namespace MLWebApi.Commands.Concrete;

public class GetHistoryCommand : ICommand
{
    public string CommandName => "get_history";
    
    public string Execute(DataBaseRecord sender, string param)
    {
        if (sender.TryGet($"{param}_{DbRecordsDataKeywords.History}", out History? history) && history != null)
            return JsonConvert.SerializeObject(history);

        return $"History of character {param} was not found!";
    }
}