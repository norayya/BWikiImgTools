using System.Diagnostics;
using OpenCvSharp;
using CvSize = OpenCvSharp.Size;
using System.CommandLine;


namespace BWikiImgReCoder;

internal static class ReCoder
{
    static async Task<int> Main(string[] args)
    {
        var rootCmd = new RootCommand();

        var subCmdCompress = new Func<Command>(() =>
        {
            var subCmd = new Command("compress", "Compress the image by resize and changing format.");
            var inputs = new Option<string[]>(["--inputs", "-i"], "The input image files.")
            {
                Arity = ArgumentArity.OneOrMore,
                AllowMultipleArgumentsPerToken = true
            };
            var output = new Option<string>(["--output", "-o"], () => Path.Combine([ AppDomain.CurrentDomain.BaseDirectory ,"Outputs", $"Result_{DateTime.Now:yyyy-mm-dd-HH-MM-ss}"]),
                "The output image path.");
            var format = new Option<int>(["--format", "-f"], () => 1, "The output image format. [PNG:0, JPEG:1].");
            var length = new Option<decimal>(["--length", "-l"], () => 8, "Length limit for the output image, MB, [Default: 8]");
            var depth = new Option<int>(["--depth", "-d"], () => 50, "Depth limit for the compression, [Default: 50]");
            var stepping = new Option<int>(["--stepping", "-s"], () => 50, "Resize stepping. [Default: 50]");
            var force = new Option<bool>(["--force"], () => false, "If the file size before compression is smaller than the length limit, the compression will also forced.");
                
            subCmd.AddOption(inputs);
            subCmd.AddOption(output);
            subCmd.AddOption(format);
            subCmd.AddOption(length);
            subCmd.AddOption(depth);
            subCmd.AddOption(stepping);
            subCmd.AddOption(force);
        
            subCmd.SetHandler(Compress, inputs, output, format, length, depth, stepping, force);
            
            return subCmd;
        }).Invoke();
        
        rootCmd.Add(subCmdCompress);
        
        return await rootCmd.InvokeAsync(args);
    }

    static void PrintConsoleColorText(string text, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(text);
        Console.ResetColor();
    }

    static async Task<int> Compress(string[] inputs, string output, int format, decimal length, int depth, int stepping, bool force)
    {
        if (!Enum.IsDefined(typeof(Format), format))
        {
            PrintConsoleColorText($"Invalid format: {format}", ConsoleColor.Red);
            return 1;
        }

        if (!Path.IsPathFullyQualified(output))
        {
            PrintConsoleColorText($"Invalid output path: {output}", ConsoleColor.Red);
            return 2;
        }

        foreach (var input in inputs)
        {
            if (File.Exists(input)) continue;
            PrintConsoleColorText($"File {input} does not exist", ConsoleColor.Red);
            return 3;
        }

        var outputFormat = (Format)format switch
        {
            Format.PNG => ".png",
            Format.JPEG => ".jpg",
            _ => throw new ArgumentException($"Invalid format: {format}"),
        };
        
        if (!Path.Exists(output))
            Directory.CreateDirectory(output);
        var max_length = (long)Math.Ceiling((1024 * 1024) * length);

        var index = 0;
        // start 
        foreach (var input in inputs)
        {
            Stopwatch sw = Stopwatch.StartNew();
            var fileInfo = new FileInfo(input);
            
            PrintConsoleColorText($"Now Compressing: {fileInfo.Name}", ConsoleColor.Yellow);
            if (!force && fileInfo.Length < max_length)
            {
                var filename = Path.Combine(output, $"{index}_{Path.GetFileNameWithoutExtension(input)}{outputFormat}");
                try
                {
                    File.Copy(input, filename, overwrite: true);
                }
                catch (Exception ex)
                {
                    PrintConsoleColorText($"Failed to copy {input} to {filename}: {ex.Message}", ConsoleColor.Red);
                    sw.Stop();
                    index += 1;
                    continue;
                }

                sw.Stop();
                PrintConsoleColorText($"Copy {input} to {filename}, took {sw.ElapsedMilliseconds}ms", ConsoleColor.Green);
                index += 1;
                continue;
            }
            
            try
            {
                var filename = $"{index}_{Path.GetFileNameWithoutExtension(input)}{outputFormat}";
                
                var raw = Cv2.ImRead(input);
                Cv2.ImEncode(outputFormat, raw, out var cpsBuff);
                
                if (cpsBuff.Length <= max_length)
                {
                    await File.WriteAllBytesAsync(Path.Combine(output, filename), cpsBuff);
                }
                else
                {
                    var cpsResult = cps(raw, outputFormat, new CvSize()
                    {
                        Width = raw.Width / stepping,
                        Height = raw.Height / stepping,
                    }, max_length, depth);

                    if (cpsResult is null)
                    {
                        sw.Stop();
                        PrintConsoleColorText($"Failed, depth limit.", ConsoleColor.Red);
                        index += 1;
                        raw.Dispose();
                        continue;
                    }
                    
                    await File.WriteAllBytesAsync(Path.Combine(output, filename), cpsResult);
                }

                sw.Stop();
                PrintConsoleColorText($"Wrote {filename}, took {sw.ElapsedMilliseconds}ms", ConsoleColor.Green);
                index += 1;
                raw.Dispose();
            }
            catch (Exception ex)
            {
                PrintConsoleColorText($"Failed to copy {input} to {output}: {ex.Message}", ConsoleColor.Red);
                sw.Stop();
                index += 1;
            }

        }
        
        PrintConsoleColorText($"Output: {output}, Task completed, Exit.", ConsoleColor.Cyan);
        return 0;
    }

    private enum Format
    {
        PNG = 0,
        JPEG = 1,
    }
    

    static byte[]? cps(Mat src, string ext, CvSize s, long max_length, int max_depth, int depth = 0)
    {
        if (depth >= max_depth)
        {
            return null;
        }
        
        #if DEBUG
            Console.WriteLine($"depth: {depth}, {src.Width}x{src.Height} , CvSize: {s.Width} x {s.Height}");
        #endif
        
        Cv2.ImEncode(ext, src, out byte[] encodedImg);
        if (encodedImg.Length <= max_length)
        {
            return encodedImg;
        }
        
        Mat newMat = new Mat();
        if (encodedImg.Length >= max_length * 2)
        {

            Cv2.Resize(src, newMat, new CvSize(src.Width*0.75, src.Height*0.75));
            src.Dispose();
            return cps(newMat, ext, s, max_length, max_depth, depth + 1);
        }
        
        Cv2.Resize(src, newMat, new CvSize(src.Width - s.Width, src.Height - s.Height));
        src.Dispose();
        return cps(newMat, ext, s, max_length, max_depth, depth + 1);
    }

}