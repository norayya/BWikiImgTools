using System.CommandLine;

namespace BWikiImgTools;

internal static class CommandExtensions
{
    internal static Command AddChildCommand(this Command parentCommand, Command childCommand)
    {
        parentCommand.Add(childCommand);
        return parentCommand;
    }
    internal static Command AddChildCommand(this Command parentCommand, Func<Command> childCommandFactory)
    {
        var childCommand = childCommandFactory.Invoke();
        parentCommand.Add(childCommand);
        return parentCommand;
    }

}