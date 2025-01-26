using System.CommandLine;
using System.Diagnostics;
using System.IO.MemoryMappedFiles;
using System.Text;

namespace BWikiImgTools;

internal static class Test
{
    public static readonly Func<Command> TestCmd = () =>
    {
        var subCmd = new Command("test", "Test Command");
        
        subCmd.SetHandler(() =>
        {
           
        });
        
        return subCmd;
    };
}