using System.Text.RegularExpressions;

namespace GatilDosResgatadosApi.Infrastructure;

public static class FormFileExtensions
{
    public const int ImageMinimumBytes = 512;

    public static bool IsImage(this IFormFile postedFile)
    {
        if (!postedFile.ContentType.Equals("image/jpg", StringComparison.CurrentCultureIgnoreCase) &&
            !postedFile.ContentType.Equals("image/jpeg", StringComparison.CurrentCultureIgnoreCase) &&
            !postedFile.ContentType.Equals("image/pjpeg", StringComparison.CurrentCultureIgnoreCase) &&
            !postedFile.ContentType.Equals("image/gif", StringComparison.CurrentCultureIgnoreCase) &&
            !postedFile.ContentType.Equals("image/x-png", StringComparison.CurrentCultureIgnoreCase) &&
            !postedFile.ContentType.Equals("image/png", StringComparison.CurrentCultureIgnoreCase))
        {
            return false;
        }

        if (!Path.GetExtension(postedFile.FileName).Equals(".jpg", StringComparison.CurrentCultureIgnoreCase)
            && !Path.GetExtension(postedFile.FileName).Equals(".png", StringComparison.CurrentCultureIgnoreCase)
            && !Path.GetExtension(postedFile.FileName).Equals(".gif", StringComparison.CurrentCultureIgnoreCase)
            && !Path.GetExtension(postedFile.FileName).Equals(".jpeg", StringComparison.CurrentCultureIgnoreCase))
        {
            return false;
        }

        try
        {
            if (!postedFile.OpenReadStream().CanRead)
            {
                return false;
            }

            byte[] buffer = new byte[ImageMinimumBytes];
            postedFile.OpenReadStream().Read(buffer, 0, ImageMinimumBytes);

            Dictionary<string, byte[][]> imageHeader = new()
            {
                {
                    "JPG",
                    [[0xFF, 0xD8, 0xFF, 0xE0],
                    [0xFF, 0xD8, 0xFF, 0xE1],
                    [0xFF, 0xD8, 0xFF, 0xE2],
                    [0xFF, 0xD8, 0xFF, 0xE3],
                    [0xFF, 0xD8, 0xFF, 0xE8],
                    [0xFF, 0xD8, 0xFF, 0xDB] ]
                },
                {
                    "JPEG",
                    [[0xFF, 0xD8, 0xFF, 0xE0],
                    [0xFF, 0xD8, 0xFF, 0xE1],
                    [0xFF, 0xD8, 0xFF, 0xE2],
                    [0xFF, 0xD8, 0xFF, 0xE3],
                    [0xFF, 0xD8, 0xFF, 0xE8],
                    [0xFF, 0xD8, 0xFF, 0xDB]]
                },
                { "PNG", [[0x89, 0x50, 0x4E, 0x47]] },
                {
                    "GIF",
                    [[0x47, 0x49, 0x46, 0x38, 0x37, 0x61],
                    [0x47, 0x49, 0x46, 0x38, 0x39, 0x61]]
                },
                { "BMP", [[0x42, 0x4D]] },
                { "ICO", [[0x00, 0x00, 0x01, 0x00]] }
            };

            string fileExt = postedFile.FileName[(postedFile.FileName.LastIndexOf('.') + 1)..].ToUpper();
            byte[][] tmp = imageHeader[fileExt];
            foreach (byte[] validHeader in tmp)
            {
                if (CompareArray(validHeader, buffer[..validHeader.Length]))
                {
                    return true;
                }
            }
        }
        catch (Exception)
        {
            return false;
        }

        return true;
    }
    private static bool CompareArray(byte[] a1, byte[] a2)
    {
        if (a1.Length != a2.Length)
            return false;

        for (int i = 0; i < a1.Length; i++)
        {
            if (a1[i] != a2[i])
                return false;
        }

        return true;
    }

}