using WebApp.Application.Common;

namespace CourseWork.Application.FileDtoRequest;

public class CreateDirectoryDtoRequest
{
    public Guid? ParentId { get; init; }
    public string FileName { get; init; }
    public Guid OwnerId { get; init; }        

    private CreateDirectoryDtoRequest(Guid? parentId, string fileName, Guid ownerId)
    {
        ParentId = parentId;
        FileName = fileName;
        OwnerId = ownerId;
    }
    
    public static Result<CreateDirectoryDtoRequest, string> Create(Guid? parentId, string fileName, Guid ownerId)
    {
        var invalidPathChars = Path.GetInvalidFileNameChars();
        if (string.IsNullOrEmpty(fileName))
            return "Не введено название папки";

        if (fileName.IndexOfAny(invalidPathChars) != -1)
            return "Введены недопустимые символы";

        return new CreateDirectoryDtoRequest(parentId, fileName, ownerId);
    }
}