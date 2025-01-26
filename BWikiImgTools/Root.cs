using System.CommandLine;
using Cps = BWikiImgTools.Compression;

namespace BWikiImgTools;

internal static class Root
{
    private static async Task<int> Main(string[] args)
    {
        return await new RootCommand()
            .AddChildCommand(Test.TestCmd)
            .AddChildCommand(Cps.Compress)
            .AddChildCommand(MultiOCR.Ocr()
                .AddChildCommand(MultiOCR.Aliyun.AliyunOcr))
            .InvokeAsync(args);
            
    }
}
