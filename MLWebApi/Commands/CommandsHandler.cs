using MLDbModule.Data;

namespace MLWebApi.Commands;

public class CommandsHandler
{
    private readonly List<ICommand> _commands;
    
    public CommandsHandler(IReadOnlyList<ICommand> commands)
    {
        _commands = commands.ToList();
    }

    public bool TryExecuteCommand(DataBaseRecord sender, string commandName, string param, out string result)
    {
        foreach (var command in _commands)
        {
            if (command.CommandName == commandName)
            {
                result = command.Execute(sender, param);
                return true;
            }
        }

        result = $"command {commandName} not found!";
        return false;
    }
}