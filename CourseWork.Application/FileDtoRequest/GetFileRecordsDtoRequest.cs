using WebApp.Application.Common;

namespace CourseWork.Application.FileDtoRequest;

public class GetFileRecordsDtoRequest
{
    public Guid? ParentId { get; init; }
    public Guid UserId { get; init; }      

    private GetFileRecordsDtoRequest(Guid? parentId, Guid userId)
    {
        ParentId = parentId;
        UserId = userId;
    }
    
    public static Result<GetFileRecordsDtoRequest, string> Create(Guid? parentId, Guid userId)
    {
        if (userId == Guid.Empty)
            return "Недопустимый userId";

        return new GetFileRecordsDtoRequest(parentId, userId);
    }
}