using MLDbModule;
using MLTextGenerationAPIModule;
using MLWebApi.Commands;
using MLWebApi.Commands.Concrete;

namespace MLWebApi
{
    public static class Program
    {
        public static void Main(params string[] args)
        {
            //TODO Config file
            DataBaseController dataBaseController = new DataBaseController("mongodb://localhost:27017", "Xeona");
            TextGenerationClient textGenerationClient = new TextGenerationClient(dataBaseController, "http://localhost:5000/api/v1/chat");
            CommandsHandler commandsHandler = InitializeCommands(dataBaseController);
            
            var builder = WebApplication.CreateBuilder(args);
            
            builder.Services.AddSingleton(dataBaseController);
            builder.Services.AddSingleton(textGenerationClient);
            builder.Services.AddSingleton(commandsHandler);
            
            builder.Services.AddControllers();
            var app = builder.Build();
            app.UseRouting();
            app.MapDefaultControllerRoute();
            
            app.Run("http://localhost:6666");
        }

        private static CommandsHandler InitializeCommands(DataBaseController dataBaseController)
        {
            ICommand clearCommand = new ClearHistoryCommand(dataBaseController);
            ICommand getHistoryCommand = new GetHistoryCommand();

            CommandsHandler commandsHandler =
                new CommandsHandler(new List<ICommand>() { clearCommand, getHistoryCommand });

            return commandsHandler;
        }
    }
}

