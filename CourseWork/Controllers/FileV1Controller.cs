using System.Net;
using CourseWork.Application.FileDtoRequest;
using CourseWork.Application.Services.FileService;
using CourseWork.Domain.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace CourseWork.Controllers;

[Authorize]
public class FileV1Controller(IFileService fileService) : BaseAppController
{
    [HttpPost("v1/file")]
    [RequestSizeLimit((long)1e+10)]
    [RequestFormLimits(MultipartBodyLengthLimit = (long)1e+10)]
    public async Task<ActionResult<FileRecord>> UploadFileAsync([FromForm] Guid? parentId, IFormFile file)
    {
        var uploadFileRequest = UploadFileDtoRequest.Create(parentId, file, UserId);
        return uploadFileRequest.IsSuccess ? Ok(await fileService.UploadFileAsync(uploadFileRequest.Value)) : BadRequest(uploadFileRequest.Error);
    }

    [HttpPost("v1/directory")]
    public async Task<ActionResult<FileRecord>> CreateDirectoryAsync([FromForm] Guid? parentId, string fileName)
    {
        var createDirectoryRequest = CreateDirectoryDtoRequest.Create(parentId, fileName, UserId);
        return createDirectoryRequest.IsSuccess ? Ok(await fileService.CreateDirectoryAsync(createDirectoryRequest.Value)) : BadRequest(createDirectoryRequest.Error);
    }
    
    [HttpGet("v1/file/list")]
    public async Task<ActionResult<IEnumerable<FileRecord>>> GetFilesRecords([FromQuery] Guid? parentId)
    {
        var getFileRecordsRequest = GetFileRecordsDtoRequest.Create(parentId, UserId);
        return getFileRecordsRequest.IsSuccess ? Ok(await fileService.GetFileRecords(getFileRecordsRequest.Value)) : BadRequest(getFileRecordsRequest.Error);
    }

    [HttpGet("v1/file/{fileId:guid}")]
    public async Task<IActionResult> GetFile([FromRoute] Guid fileId)
    {
        var (fileStream, fileName) = await fileService.GetFile(fileId, UserId);
        return File(fileStream, "application/octet-stream", fileName);
    }

    [HttpPatch("v1/file/path")]
    public async Task<ActionResult<FileRecord>> UpdateFilePath([FromForm] Guid? currentParentId, [FromForm] Guid fileId, [FromForm] Guid? updatedParentId)
    {
        var updateFilePathRequest = UpdateFilePathDtoRequest.Create(currentParentId, UserId, updatedParentId, fileId);
        return updateFilePathRequest.IsSuccess ? Ok(await fileService.UpdateFilePathAsync(updateFilePathRequest.Value)) : BadRequest(updateFilePathRequest.Error);
    }

    [HttpDelete("v1/file/{fileId:guid}")]
    public async Task<ActionResult<bool>> DeleteFile(Guid fileId)
    {
        var deleteFileRequest = DeleteFileDtoRequest.Create(UserId, fileId);
        return deleteFileRequest.IsSuccess ? Ok(await fileService.DeleteFileAsync(deleteFileRequest.Value)) : BadRequest(deleteFileRequest.Error);
    }

    [HttpGet("v1/file/name/{fileName}")]
    public async Task<ActionResult<IEnumerable<FileRecord>>> GetFileRecordByName(string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
        {
            return BadRequest();
        }

        return Ok(await fileService.GetFilesByName(fileName, UserId));
    }
}