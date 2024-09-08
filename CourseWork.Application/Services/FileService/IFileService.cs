using CourseWork.Application.FileDtoRequest;
using CourseWork.Domain.Entity;

namespace CourseWork.Application.Services.FileService;

public interface IFileService
{
    Task<FileRecord> UploadFileAsync(UploadFileDtoRequest uploadFileRequest);
    Task<FileRecord> CreateDirectoryAsync(CreateDirectoryDtoRequest createDirectoryRequest);
    Task<IEnumerable<FileRecord>> GetFileRecords(GetFileRecordsDtoRequest getFileRecordsRequest);
    Task<FileRecord> UpdateFilePathAsync(UpdateFilePathDtoRequest updateFilePathRequest);
    Task<IEnumerable<FileRecord>> GetFilesByName(string fileName, Guid userId);
    Task<(Stream, string fileName)> GetFile(Guid fileId, Guid userId);
    Task<bool> DeleteFileAsync(DeleteFileDtoRequest deleteFileRequest);
}