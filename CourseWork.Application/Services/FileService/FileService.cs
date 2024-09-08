using CourseWork.Application.Adapters;
using CourseWork.Application.Common;
using CourseWork.Application.FileDtoRequest;
using CourseWork.Domain.Entity;
using CourseWork.Domain.Enum;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using DirectoryNotFoundException = System.IO.DirectoryNotFoundException;
using FileNotFoundException = System.IO.FileNotFoundException;

namespace CourseWork.Application.Services.FileService;

public class FileService(IFileRecordContext fileRecordContext, ISaveDbContext saveDbContext, UserManager<IdentityUser> userManager, IConfiguration configuration) : IFileService
{
    private string? FilePath { get; init; } = configuration["FILES_PATH"];
    private string? Key { get; init; } = configuration["KEY"];
    private byte[] _key = [222, 179, 63, 137, 193, 181, 28, 99, 87, 75, 76, 11, 214, 56, 238, 203, 114, 41, 50, 249, 168, 202, 44, 53, 62, 246, 117, 137, 99, 32, 97, 88];
    
    public async Task<FileRecord> UploadFileAsync(UploadFileDtoRequest uploadFileRequest)
    {
        var user = await userManager.FindByIdAsync(uploadFileRequest.UserId.ToString());

        if (user is null)
        {
            throw new UserNotFoundException($"Пользователь с id: {uploadFileRequest.UserId} не найден");
        }

        var parentId = uploadFileRequest.ParentId;
        if (parentId is not null)
        {
            var directory = await fileRecordContext.FileRecords.FirstOrDefaultAsync(record => record.Id == parentId && record.FileType == FileType.Directory);
            if (directory is null)
            {
                throw new DirectoryNotFoundException("Не найдена папка");
            }
        }
        
        var currentDirectory = await fileRecordContext.FileRecords.Where(record => record.ParentId == parentId && record.Owner.Id == user.Id).ToListAsync();
        if (currentDirectory.FirstOrDefault(record => record.FileName == uploadFileRequest.File.FileName) is not null)
        {
            FileNameException.ThrowFileNameException(uploadFileRequest.File.FileName);
        }
        
        var dirPath = Path.Combine(FilePath!, user.Id);
        if (!Directory.Exists(dirPath))
            Directory.CreateDirectory(dirPath);

        var fileGuid = Guid.NewGuid();
        var filePath = Path.Combine(dirPath, fileGuid.ToString());

        await using var fileStream = File.OpenWrite(filePath);

        var s = uploadFileRequest.File.OpenReadStream();
        await FileEncryptor.EncryptFile(s, fileStream, _key);
        var fileRecord = new FileRecord
        {
            Id = fileGuid,
            FileName = uploadFileRequest.File.FileName,
            FileSize = uploadFileRequest.File.Length,
            FileType = FileType.File,
            Owner = user,
            ParentId = parentId,
            UploadedAt = DateTime.UtcNow
        };
        await fileRecordContext.FileRecords.AddAsync(fileRecord);
        await saveDbContext.SaveChangesAsync(CancellationToken.None);

        return fileRecord;
    }

    public async Task<FileRecord> CreateDirectoryAsync(CreateDirectoryDtoRequest createDirectoryRequest)
    {
        var user = await userManager.FindByIdAsync(createDirectoryRequest.OwnerId.ToString());

        if (user is null)
        {
            throw new UserNotFoundException($"Пользователь с id: {createDirectoryRequest.OwnerId} не найден");
        }

        var parentId = createDirectoryRequest.ParentId;
        var currentDirectory = await fileRecordContext.FileRecords.Where(record => record.ParentId == parentId && record.Owner.Id == user.Id).ToListAsync();
        if (currentDirectory.FirstOrDefault(record => record.FileName == createDirectoryRequest.FileName) is not null)
        {
            FileNameException.ThrowFileNameException(createDirectoryRequest.FileName);
        }
        var fileRecord = new FileRecord
        {
            Id = Guid.NewGuid(),
            FileName = createDirectoryRequest.FileName,
            FileSize = null,
            FileType = FileType.Directory,
            Owner = user,
            ParentId = parentId,
            UploadedAt = DateTime.UtcNow
        };
        
        await fileRecordContext.FileRecords.AddAsync(fileRecord);
        await saveDbContext.SaveChangesAsync(CancellationToken.None);

        return fileRecord;
    }

    public async Task<IEnumerable<FileRecord>> GetFileRecords(GetFileRecordsDtoRequest getFileRecordsRequest)
    {
        var user = await userManager.FindByIdAsync(getFileRecordsRequest.UserId.ToString());

        if (user is null)
        {
            throw new UserNotFoundException($"Пользователь с id: {getFileRecordsRequest.UserId} не найден");
        }
        
        return await fileRecordContext.FileRecords.Where(record => record.Owner.Id == user.Id && record.ParentId == getFileRecordsRequest.ParentId).ToListAsync();
    }

    public async Task<FileRecord> UpdateFilePathAsync(UpdateFilePathDtoRequest updateFilePathRequest)
    {
        var user = await userManager.FindByIdAsync(updateFilePathRequest.UserId.ToString());

        if (user is null)
        {
            throw new UserNotFoundException($"Пользователь с id: {updateFilePathRequest.UserId} не найден");
        }

        var fileRecord = await fileRecordContext.FileRecords.FirstOrDefaultAsync(record =>
            updateFilePathRequest.FileId == record.Id && record.Owner.Id == user.Id && record.ParentId == updateFilePathRequest.CurrentParentId);
        if (fileRecord is null)
        {
            throw new FileNotFoundException();
        }

        var currentDirectory = await fileRecordContext.FileRecords.Where(record => record.ParentId == updateFilePathRequest.UpdatedParentId && record.Owner.Id == user.Id)
            .ToListAsync();
        if (currentDirectory.FirstOrDefault(record => record.FileName == fileRecord.FileName) is not null)
        {
            FileNameException.ThrowFileNameException(fileRecord.FileName);
        }

        fileRecord.ParentId = updateFilePathRequest.UpdatedParentId; 
        fileRecord.UploadedAt = DateTime.UtcNow;
        fileRecord = fileRecordContext.FileRecords.Update(fileRecord).Entity;
        await saveDbContext.SaveChangesAsync(CancellationToken.None);

        return fileRecord;
    }

    public async Task<IEnumerable<FileRecord>> GetFilesByName(string fileName, Guid userId)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());

        if (user is null)
        {
            throw new UserNotFoundException($"Пользователь с id: {userId} не найден");
        }

        var userRoles = await userManager.GetRolesAsync(user);
        var userIsAdmin = userRoles.Contains(nameof(UserRoles.Admin));
        if (userIsAdmin)
        {
            return await fileRecordContext.FileRecords.Where(record => record.FileName.Contains(fileName)).Include(record => record.Owner).ToListAsync();
        }

        return await fileRecordContext.FileRecords.Where(record => record.FileName.Contains(fileName) && record.Owner.Id == user.Id).ToListAsync();
    }

    public async Task<(Stream, string fileName)> GetFile(Guid fileId, Guid userId)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());

        if (user is null)
        {
            throw new UserNotFoundException($"Пользователь с id: {userId} не найден");
        }

        var file = await fileRecordContext.FileRecords.FirstOrDefaultAsync(record => record.Id == fileId && record.Owner.Id == user.Id);
        if (file is null)
        {
            throw new FileNotFoundException();
        }

        if (file.FileType == FileType.Directory)
        {
            throw new NotImplementedException();
        }

        var filePath = Path.Combine(Path.Combine(FilePath!, user.Id), file.Id.ToString());
        
        await using var encryptedFileStream = File.OpenRead(filePath);
        var result = await FileEncryptor.DecryptFile(encryptedFileStream, _key);
        return (result, file.FileName);
    }

    public async Task<bool> DeleteFileAsync(DeleteFileDtoRequest deleteFileRequest)
    {
        var user = await userManager.FindByIdAsync(deleteFileRequest.UserId.ToString());

        if (user is null)
        {
            throw new UserNotFoundException($"Пользователь с id: {deleteFileRequest.UserId} не найден");
        }

        var userRoles = await userManager.GetRolesAsync(user);
        var userIsAdmin = userRoles.Contains(nameof(UserRoles.Admin));
        FileRecord? file;
        if (userIsAdmin)
        {
            file = await fileRecordContext.FileRecords.Include(record => record.Owner).FirstOrDefaultAsync(record => record.Id == deleteFileRequest.FileId);
        }
        else
        {
            file = await fileRecordContext.FileRecords.FirstOrDefaultAsync(record => record.Id == deleteFileRequest.FileId && record.Owner.Id == user.Id);
        }
        
        if (file is null)
        {
            return false;
        }

        var deletedFiles = await GetAllSubsidiariesFiles(file);
        foreach (var deletedFile in deletedFiles)
        {
            if (deletedFile.FileType == FileType.File)
            {
                var filePath = Path.Combine(FilePath, deletedFile.Owner.Id, deletedFile.Id.ToString());
                File.Delete(filePath);
            }
        }
        
        fileRecordContext.FileRecords.RemoveRange(deletedFiles);
        await saveDbContext.SaveChangesAsync(CancellationToken.None);
        return true;
    }

    private async ValueTask<IEnumerable<FileRecord>> GetAllSubsidiariesFiles(FileRecord parentFile)
    {
        if (parentFile.FileType == FileType.File)
        {
            return [parentFile];
        }

        var fileStack = new Stack<Guid>();
        var fileDictionary = new Dictionary<Guid, FileRecord>
        {
            { parentFile.Id, parentFile }
        };
        
        fileStack.Push(parentFile.Id);
        do
        {
            var fileId = fileStack.Pop();
            var files = await fileRecordContext.FileRecords.Where(record => record.ParentId == fileId).ToListAsync();
            foreach (var file in files.Where(file => !fileDictionary.ContainsKey(file.Id)))
            {
                fileStack.Push(file.Id);
                fileDictionary[file.Id] = file;
            }
        } while (fileStack.Count != 0);

        return fileDictionary.Values;
    }
    
    
}