using ImageMagick;
using Microsoft.AspNetCore.Http;

namespace Application.Common;

public static class UploadTools
{
    private static string NormalizePath(string path)
    {
        if (!path.EndsWith("/")) {
            path += "/";
        }

        if (path.StartsWith("/")) {
            path = path.Substring(1);
        }

        return path;
    }

    private static string NormalizeFile(string path)
    {
        if (path.StartsWith("/")) {
            path = path.Substring(1);
        }

        return path;
    }

    public static async Task<string> UploadAsync(this IFormFile file, string path, string name,
        string dir = "wwwroot", string prefix = "/", MagickGeometry geometry = null)
    {
        try {
            path = NormalizePath(path);

            var absoluteDir = Path.Combine(Directory.GetCurrentDirectory(), dir, path);
            if (!Directory.Exists(absoluteDir)) {
                Directory.CreateDirectory(absoluteDir);
            }

            var fileName = name + Path.GetExtension(file.FileName);
            var filePath = path + fileName;
            var absolutePath = Path.Combine(Directory.GetCurrentDirectory(), dir, filePath);

            await using var stream = File.Create(absolutePath);
            await file.CopyToAsync(stream);
            stream.Close();

            if (geometry != null) {
                await ResizeImageAsync(absolutePath, geometry);
            }

            return $"{prefix}{filePath}";
        }
        catch (Exception e) {
            await Console.Out.WriteAsync(e.Message);
            return null;
        }
    }

    public static string Upload(this IFormFile file, string path, string name, string dir = "wwwroot",
        string prefix = "/", MagickGeometry geometry = null)
    {
        try {
            path = NormalizePath(path);

            var absoluteDir = Path.Combine(Directory.GetCurrentDirectory(), dir, path);
            if (!Directory.Exists(absoluteDir)) {
                Directory.CreateDirectory(absoluteDir);
            }

            var fileName = name + Path.GetExtension(file.FileName);
            var filePath = path + fileName;
            var absolutePath = Path.Combine(Directory.GetCurrentDirectory(), dir, filePath);

            using var stream = File.Create(absolutePath);
            file.CopyTo(stream);
            stream.Close();

            if (geometry != null) {
                ResizeImage(absolutePath, geometry);
            }

            return $"{prefix}{filePath}";
        }
        catch (Exception e) {
            Console.Out.WriteAsync(e.Message);
            return null;
        }
    }

    public static async Task DeleteFile(string path, string dir = "wwwroot/")
    {
        if (path == null) {
            return;
        }

        if (path.Contains("1.png")) {
            return;
        }

        try {
            path = NormalizeFile(path);

            File.Delete(Path.Combine(Directory.GetCurrentDirectory(), dir, path));
        }
        catch (Exception e) {
            await Console.Error.WriteLineAsync(e.Message);
        }
    }

    public static async Task DeleteFiles(IEnumerable<string> paths, string dir = "wwwroot/")
    {
        foreach (var path in paths) {
            var newPath = path;
            if (newPath == null) {
                return;
            }

            try {
                newPath = NormalizeFile(newPath);

                File.Delete(Path.Combine(Directory.GetCurrentDirectory(), dir, newPath));
            }
            catch (Exception e) {
                await Console.Error.WriteLineAsync(e.Message);
            }
        }
    }

    public static async Task ResizeImageAsync(string absolutePath, MagickGeometry geometry)
    {
        var imageFileInfo = new FileInfo(absolutePath);
        using var imageMagick = new MagickImage(imageFileInfo);

        imageMagick.Resize(geometry);

        if (absolutePath.ToLower().EndsWith("jpg") || absolutePath.ToLower().EndsWith("jpeg")) {
            imageMagick.SetCompression(CompressionMethod.JPEG);
        }

        await imageMagick.WriteAsync(imageFileInfo);
        imageMagick.Dispose();
    }

    public static void ResizeImage(string absolutePath, MagickGeometry geometry)
    {
        var imageFileInfo = new FileInfo(absolutePath);
        using var imageMagick = new MagickImage(imageFileInfo);

        imageMagick.Resize(geometry);

        if (absolutePath.ToLower().EndsWith("jpg") || absolutePath.ToLower().EndsWith("jpeg")) {
            imageMagick.SetCompression(CompressionMethod.JPEG);
        }

        imageMagick.Write(imageFileInfo);
        imageMagick.Dispose();
    }

    public static bool IsImage(this IFormFile file)
    {
        if (file == null) {
            return false;
        }

        return new List<string> {
            "image/jpeg",
            "image/jpg",
            "image/png",
            "image/webp"
        }.Contains(file.ContentType);
    }

    public static bool IsVideo(this IFormFile file)
    {
        if (file == null) {
            return false;
        }

        return new List<string> {
            "video/3gpp",
            "video/mp4",
            "video/mpeg",
            "video/mpeg",
            "video/ogg",
            "video/webm",
            "video/x-ms-wmv",
            "video/ms-asf",
            "video/x-m4v",
            "video/x-msvideo",
        }.Contains(file.ContentType);
    }

    public static bool IsAudio(this IFormFile file)
    {
        if (file == null) {
            return false;
        }

        return new List<string> {
            "audio/mpeg",
            "audio/mp3",
            "audio/aac",
        }.Contains(file.ContentType);
    }
}