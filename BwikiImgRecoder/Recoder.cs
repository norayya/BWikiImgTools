using System.Diagnostics;
using System.Numerics;
using OpenCvSharp;
using CvSize = OpenCvSharp.Size;

namespace BwikiImgRecoder;

class Recoder
{
    private static readonly BigInteger MAX_LENGTH = new BigInteger(1024 * 1024) * 8;
    private static readonly int STEPPING = 20;
    static void Main(string[] args)
    {
        if (args.Length < 1)
        {
            Console.WriteLine("Usage: ~.exe <mode> <input-file>[]");
            return;
        }
        
        string ext = args[0] switch
        {
            "0" => ".png",
            "1" => ".jpg",
            _ => throw new ArgumentException("Invalid argument, only 0 and 1 are supported.")
        };
        
        List<string> fileList = new();
        for (int i = 1; i <= args.Length - 1; i++)
        {
            if (!File.Exists(args[i]))
            {
                throw new FileNotFoundException("File not found", args[i]);
            }
            fileList.Add(args[i]);
        }
        
        string targetPath = Directory.CreateDirectory($"Result_{DateTime.Now:yyyy-dd-M--HH-mm-ss}").FullName;
        var genFilename = (string targetDic ,string filename) => Path.Combine($"{targetDic}", $"{DateTime.Now:yyyy-dd-M--HH-mm-ss}_{filename}");
        Console.WriteLine($"Start...");
        Console.WriteLine($"Output file path: {targetPath}");
        
        foreach (var x in fileList)
        {
            string filename = Path.GetFileName(x);
            var fi = new FileInfo(x);
            
            Console.WriteLine("☆ = = = = = = = =");
            Console.WriteLine($"Now processing file {fi.Name}");
            
            Stopwatch sw = Stopwatch.StartNew();
            
            if (fi.Length <= MAX_LENGTH)
            {
                var targetFilename = genFilename(targetPath, filename);
                try
                {
                    File.Copy(x, targetFilename, true);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Failed to copy file {x} to {targetFilename}: {e.Message}");
                    continue;
                }

                Console.WriteLine($"[{filename}], {Math.Round((decimal)fi.Length/1024/1024, 2)} MB, CPY");
                sw.Stop();
                Console.WriteLine($"★ Success. {sw.ElapsedMilliseconds}ms = = = =\r\n");
                
                continue;
                
            }

            try
            {
                var sourceMat = Cv2.ImRead(x);
                Cv2.ImEncode(ext, sourceMat, out byte[] encodedImg);

                if (encodedImg.Length <= MAX_LENGTH)
                {
                    filename = $"{Path.GetFileNameWithoutExtension(x)}{ext}";
                    var targetFilename = genFilename(targetPath, filename);
                    File.WriteAllBytes(targetFilename, encodedImg);
                    Console.WriteLine(
                        $"[{filename}], {Math.Round((decimal)fi.Length / 1024 / 1024, 2)} MB, CPS1");
                }
                else
                {

                    var result = Compress(sourceMat, ext,
                        new CvSize() { Width = sourceMat.Width / STEPPING, Height = sourceMat.Height / STEPPING });
                    filename = $"{Path.GetFileNameWithoutExtension(x)}{ext}";
                    var targetFilename = genFilename(targetPath, filename);
                    File.WriteAllBytes(targetFilename, result);
                    Console.WriteLine(
                        $"[{filename}], {Math.Round((decimal)fi.Length / 1024 / 1024, 2)} MB, CPS2");
                }
                sw.Stop();
                Console.WriteLine($"★ Success. {sw.ElapsedMilliseconds}ms = = = =\r\n");

                sourceMat.Dispose();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Cv2 Error: {e.Message}\r\n");

            }

        }
        
        Console.WriteLine($"Over...");
    }

    static byte[] Compress(Mat src, string ext, CvSize s)
    {
        Cv2.ImEncode(ext, src, out byte[] encodedImg);
        if (encodedImg.Length <= MAX_LENGTH)
        {
            return encodedImg;
        }
        
        Mat newMat = new Mat();
        if (encodedImg.Length >= MAX_LENGTH * 2)
        {

            Cv2.Resize(src, newMat, new CvSize(src.Width*0.75, src.Height*0.75));
            src.Dispose();
            return Compress(newMat, ext, s);
        }
        
        Cv2.Resize(src, newMat, new CvSize(src.Width - s.Width, src.Height - s.Height));
        src.Dispose();
        return Compress(newMat, ext, s);
    }

}