using System.Globalization;
using InventoryManagementSystem.Dtos;
using Microsoft.AspNetCore.Hosting;

namespace InventoryManagementSystem.Services.FileHandling;

public class FileService : IFileService
{
    private readonly IWebHostEnvironment _environment;
    private readonly IConfiguration _configuration;
    private readonly string _uploadPath;

    public FileService(IWebHostEnvironment environment, IConfiguration configuration)
    {
        _environment = environment;
        _configuration = configuration;
        _uploadPath = Path.Combine(_environment.WebRootPath, "uploads");
        
        // Ensure upload directory exists
        if (!Directory.Exists(_uploadPath))
        {
            Directory.CreateDirectory(_uploadPath);
        }
    }

    public async Task<OperationResult> Delete(List<string> fileNames)
    {
        try
        {
            foreach (var fileName in fileNames)
            {
                var filePath = Path.Combine(_uploadPath, fileName);
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
            return new OperationResult(true, "Files deleted successfully", null);
        }
        catch (Exception ex)
        {
            return new OperationResult(false, $"Error deleting files: {ex.Message}", null);
        }
    }

    public OperationResult GetUrl(string fileName)
    {
        try
        {
            var filePath = Path.Combine(_uploadPath, fileName);
            if (!File.Exists(filePath))
            {
                return new OperationResult(false, "File not found", null);
            }

            // For local development, create a relative URL
            var baseUrl = _configuration["BaseUrl"] ?? "https://localhost:7001";
            var publicUrl = $"{baseUrl}/uploads/{fileName}";
            
            return new OperationResult(true, publicUrl, null);
        }
        catch (Exception ex)
        {
            return new OperationResult(false, $"Error getting file URL: {ex.Message}", null);
        }
    }

    public async Task<OperationResult> Upload(IFormFile file)
    {
        if (file == null && file.Length == 0)
            return new OperationResult(false, "File is null or empty", null);

        try
        {
            // Validate file size (e.g., 10MB limit)
            if (file.Length > 10 * 1024 * 1024)
                return new OperationResult(false, "File size exceeds 10MB limit", null);

            // Validate file extensions
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".pdf", ".doc", ".docx", ".txt" };
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(fileExtension))
                return new OperationResult(false, "Invalid file type", null);

            // Generate unique filename
            var newFileName = $"{Guid.NewGuid()}{fileExtension}";
            var filePath = Path.Combine(_uploadPath, newFileName);

            // Save file to disk
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return new OperationResult(true, "File uploaded successfully", newFileName);
        }
        catch (Exception ex)
        {
            return new OperationResult(false, $"Error uploading file: {ex.Message}", null);
        }
    }

    // Additional helper method to get file info
    public async Task<FileInfo> GetFileInfo(string fileName)
    {
        var filePath = Path.Combine(_uploadPath, fileName);
        return new FileInfo(filePath);
    }

    // Method to check if file exists
    public bool FileExists(string fileName)
    {
        var filePath = Path.Combine(_uploadPath, fileName);
        return File.Exists(filePath);
    }
}