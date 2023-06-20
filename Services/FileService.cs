namespace QuestionTask.Services;

public class FileService
{
    private static void CheckFileExist(string filePath)
    {
        if (!Directory.Exists(filePath))
        {
           Directory.CreateDirectory(filePath);
        }
    }

    public static string SaveQuestionPhoto(IFormFile file)
    {
        return SaveFile(file, "Photos");
    }

    private static string SaveFile(IFormFile file, string folder)
    {
        CheckFileExist(Path.Combine("Files", folder));
        var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
        

            using (var ms = new MemoryStream())
            {
                file.CopyToAsync(ms);
                File.WriteAllBytesAsync(Path.Combine("Files", folder, fileName), ms.ToArray());
            }

            return $"/{folder}/{fileName}";
    }
}
