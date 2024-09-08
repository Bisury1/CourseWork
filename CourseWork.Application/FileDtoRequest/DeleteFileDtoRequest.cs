using WebApp.Application.Common;

namespace CourseWork.Application.FileDtoRequest;

public class DeleteFileDtoRequest
{
    public Guid FileId { get; init; }
    public Guid UserId { get; init; }        

    private DeleteFileDtoRequest(Guid userId, Guid fileId)
    {
        UserId = userId;
        FileId = fileId;
    }
    
    public static Result<DeleteFileDtoRequest, string> Create(Guid userId, Guid fileId)
    {
        if (userId == Guid.Empty)
            return "Недопустимый userId";
        
        return new DeleteFileDtoRequest(userId, fileId);
    }
}