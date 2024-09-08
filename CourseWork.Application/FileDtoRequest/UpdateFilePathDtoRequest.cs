using WebApp.Application.Common;

namespace CourseWork.Application.FileDtoRequest;

public class UpdateFilePathDtoRequest
{
    public Guid? CurrentParentId { get; init; }
    public Guid FileId { get; init; }
    public Guid? UpdatedParentId { get; init;}
    public Guid UserId { get; init; } 

    private UpdateFilePathDtoRequest(Guid? currentParentId, Guid userId, Guid? updatedParentId, Guid fileId)
    {
        CurrentParentId = currentParentId;
        UserId = userId;
        UpdatedParentId = updatedParentId;
        FileId = fileId;
    }

    public static Result<UpdateFilePathDtoRequest, string> Create(Guid? currentParentId, Guid userId, Guid? updatedParentId, Guid fileId)
    {
        if (currentParentId == updatedParentId)
            return "Нельзя переместить файл в ту же папку";

        if (userId == Guid.Empty)
            return "Недопустимый userId";

        return new UpdateFilePathDtoRequest(currentParentId, userId, updatedParentId, fileId);
    }
}