using MLApiCore;
using MLDbModule;
using MLDbModule.Data;
using MLTextGenerationAPIModule.Data;

namespace MLWebApi.Commands.Concrete;

public class ClearHistoryCommand : ICommand
{
    public string CommandName => "clear_history";

    private readonly DataBaseController _controller;
    
    public ClearHistoryCommand(DataBaseController dataBaseController)
    {
        _controller = dataBaseController;
    }
    
    public string Execute(DataBaseRecord sender, string param)
    {
        if (_controller.TryGetRecordByName(DbKeywords.Characters, param, out DataBaseRecord? characterRecord) && characterRecord != null && characterRecord.TryGet(ApiKeywords.CharacterData, out AiCharacterData characterData))
        {
            sender.TryUpdate($"{characterData.Name}_{DbRecordsDataKeywords.History}",
                new History(characterData.InnerMessage));
            
            _controller.UpsertRecord(DbKeywords.Users, sender);

            return $"History for character {characterData.Name} was cleared!";
        }

        return $"character with name {param} not found!";
    }
}