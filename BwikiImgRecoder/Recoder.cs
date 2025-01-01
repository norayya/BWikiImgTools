using System.Numerics;
using OpenCvSharp;

namespace BwikiImgRecoder;

class Recoder
{
    private static readonly BigInteger MAX_LENGTH = new BigInteger(1024 * 1024) * 8;
    static void Main(string[] args)
    {
        if (args.Length < 1)
        {
            Console.WriteLine("Usage: ~.exe <mode> <input-file>[] ");
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

        foreach (var x in fileList)
        {
            string filename = Path.GetFileName(x);
            var fi = new FileInfo(x);
            if (fi.Length <= MAX_LENGTH)
            {
                var targetFilename = genFilename(targetPath, filename);
                Console.WriteLine($"[{filename}], {Math.Round((decimal)fi.Length/1024/1024, 2)} MB, CPY  -> {targetFilename}");
                File.Copy(x, targetFilename, true);
                continue;
            }
            
            var sourceMat = Cv2.ImRead(x);
            Cv2.ImEncode(ext, sourceMat, out byte[] encodedImg);
            
            if (encodedImg.Length <= MAX_LENGTH)
            {
                filename = $"{Path.GetFileNameWithoutExtension(x)}{ext}";
                var targetFilename = genFilename(targetPath, filename);
                Console.WriteLine($"[{filename}], {Math.Round((decimal)fi.Length/1024/1024, 2)} MB, CPS1  -> {targetFilename}");
                File.WriteAllBytes(targetFilename, encodedImg);
                sourceMat.Dispose();
    
            }
            else
            {
                var result = Compress(sourceMat, ext);
                var targetFilename = genFilename(targetPath, filename);
                filename = $"{Path.GetFileNameWithoutExtension(x)}{ext}";
                Console.WriteLine($"[{filename}], {Math.Round((decimal)fi.Length/1024/1024, 2)} MB, CPS2  -> {targetFilename}");
                File.WriteAllBytes(genFilename(targetPath, filename), result);
             
            }
            
        }

    }

    static byte[] Compress(Mat src, string ext)
    {
        Cv2.ImEncode(ext, src, out byte[] encodedImg);
        if (encodedImg.Length <= MAX_LENGTH)
        {
            return encodedImg;
        }
        
        Mat newMat = new Mat();
        if (encodedImg.Length >= MAX_LENGTH * 2)
        {

            Cv2.Resize(src, newMat, new Size(src.Width*0.75, src.Height*0.75));
            src.Dispose();
            return Compress(newMat, ext);
        }
        
        Cv2.Resize(src, newMat, new Size(src.Width*0.9, src.Height*0.9));
        src.Dispose();
        return Compress(newMat, ext);
    }
    
}