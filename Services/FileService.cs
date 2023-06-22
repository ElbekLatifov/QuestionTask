namespace QuestionTask.Services;

public class FileService
{
    private const string FolderName = "wwwroot";
    private void CheckFileExist(string filePath)
    {
        string path = Path.Combine(FolderName, filePath);

        if (!Directory.Exists(path))
        {
           Directory.CreateDirectory(path);
        }
    }

    public async Task<string> SaveQuestionPhoto(IFormFile file)
    {
        return await SaveFile(file, "Photos");
    }

    private async Task<string> SaveFile(IFormFile file, string folder)
    {
        CheckFileExist(folder);
        var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);

        using (var ms = new MemoryStream())
        {
            await file.CopyToAsync(ms);
            await File.WriteAllBytesAsync(Path.Combine(FolderName, folder, fileName), ms.ToArray());
        }

        return $"/{folder}/{fileName}";
    }
}
