using Microsoft.AspNetCore.Http;
using WebApp.Application.Common;

namespace CourseWork.Application.FileDtoRequest;

public class UploadFileDtoRequest
{
    public Guid? ParentId { get; init; }
    public IFormFile File { get; init; }
    public Guid UserId { get; init; }

    private UploadFileDtoRequest(Guid? parentId, IFormFile file, Guid userId)
    {
        ParentId = parentId;
        File = file;
        UserId = userId;
    }
    
    public static Result<UploadFileDtoRequest, string> Create(Guid? parentId, IFormFile file, Guid userId)
    {
        var invalidPathChars = Path.GetInvalidFileNameChars();
        if (string.IsNullOrEmpty(file.FileName))
            return "Не введено название файла";

        if (file.FileName.IndexOfAny(invalidPathChars) != -1)
            return "Введены недопустимые символы";

        return new UploadFileDtoRequest(parentId, file, userId);
    }
}