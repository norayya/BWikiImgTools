using System.CommandLine;
using System.Diagnostics;
using System.IO.MemoryMappedFiles;
using System.Text;
using AlibabaCloud.OpenApiClient.Models;
using AlibabaCloud.SDK.Ocr_api20210707;
using AlibabaCloud.SDK.Ocr_api20210707.Models;
using AlibabaCloud.TeaUtil.Models;

namespace BWikiImgTools;

internal static class MultiOCR
{
    public static readonly Func<Command> Ocr = () =>
    {
        var subCmd = new Command("ocr", "Ocr");

        return subCmd;
    };

    public static class Aliyun
    {
        public static readonly Func<Command> AliyunOcr = () =>
        {
            var subCmd = new Command("aliyunocr", "AliyunOcr");

            var input = new Option<string>(["--input", "-i"], "Image file path.");
            var aliyunAccessKey = new Option<string>(["--key", "-k"], "Aliyun API access key.");
            var aliyunAccessSecret = new Option<string>(["--secret", "-s"], "Aliyun API access secret.");
            var editor = new Option<string>(["--editor", "-e"], "Editor path.");

            subCmd.AddOption(input);
            subCmd.AddOption(aliyunAccessKey);
            subCmd.AddOption(aliyunAccessSecret);
            subCmd.AddOption(editor);

            subCmd.SetHandler(Handler, input, aliyunAccessKey, aliyunAccessSecret, editor);
            return subCmd;
        };

        private static async Task<int> Handler(string? input, string? accessKey, string? accessSecret, string? editor)
        {
            if (input == null)
            {
                Console.WriteLine("input is null");
                return 2;
            }

            if (accessKey == null || accessSecret == null)
            {
                Console.WriteLine("access or secret is null");
                return 3;
            }
            
            var config = new Config()
            {
                AccessKeyId = accessKey,
                AccessKeySecret = accessSecret,
                Endpoint = @"ocr-api.cn-hangzhou.aliyuncs.com"
            };

            try
            {
                var client = new Client(config);

                var request = new RecognizeAdvancedRequest()
                {
                    Body = new FileStream(input, FileMode.Open, FileAccess.Read, FileShare.Read),
                };

                var runtimeOpt = new RuntimeOptions();

                var result = await client.RecognizeAdvancedWithOptionsAsync(request, runtimeOpt);
                
                if(result is null)
                {
                    Console.WriteLine("No Result.");
                    return 4;
                }
                
                if (editor == null || !File.Exists(editor))
                {
                    Console.WriteLine(result.Body.Data);
                    return 0;
                }
                
                string memoryName = Guid.NewGuid().ToString();
                string msg = result.Body.Data; // json
                var buffer = Encoding.UTF8.GetBytes(msg);
                int memorySize = buffer.Length + 4;

                var memoryMap = MemoryMappedFile.CreateNew(memoryName, memorySize);
                
                #if DEBUG
                  Console.WriteLine($"Memory Name: {memoryName}");
                #endif

                using (var accessor = memoryMap.CreateViewAccessor())
                {
                    accessor.Write(0, buffer.Length);
                    accessor.WriteArray(4, buffer, 0, buffer.Length);
                }
                
                Process.Start(editor, [memoryName, $"Ocr"]);
                
                using (var accessor = memoryMap.CreateViewAccessor())
                {
                    while (accessor.ReadInt32(0) != 0)
                    {
                        Thread.Sleep(500);
                    }
                }
                
                Console.WriteLine("Exit...");
                
                memoryMap.Dispose();    
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 1;
            }

        }
    }
}
