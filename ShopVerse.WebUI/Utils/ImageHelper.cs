namespace ShopVerse.WebUI.Utils;

public class ImageHelper
{
    public readonly IWebHostEnvironment _env;

    public ImageHelper(IWebHostEnvironment env)
    {
        _env = env;
    }

    public async Task<string> UploadFile(IFormFile file, string folderName)
    {
        string fileName = $"{Guid.NewGuid()}_{file.FileName}";
        var uploadPath = Path.Combine(_env.WebRootPath, "img", folderName);

        if(!Directory.Exists(uploadPath))
        {
            Directory.CreateDirectory(uploadPath);
        }
        
        var fullPath = Path.Combine(uploadPath, fileName);

        using(var stream = new FileStream(fullPath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return $"/img/{folderName}/{fileName}";
    }
}
