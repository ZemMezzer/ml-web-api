using MLDbModule.Data;

namespace MLWebApi.Commands;

public interface ICommand
{
    public string CommandName { get; }
    public string Execute(DataBaseRecord sender, string param);
}