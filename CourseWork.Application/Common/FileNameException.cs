using System.Diagnostics.CodeAnalysis;

namespace CourseWork.Application.Common;

public class FileNameException(string message) : Exception(message)
{
    [DoesNotReturn]
    public static void ThrowFileNameException(string fileName)
    {
        throw new FileNameException($"Файл с именем {fileName} уже существует в этой папке");
    }
}