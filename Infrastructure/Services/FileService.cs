using Application.Interface;
using Common.Configurations;
using Common.Exceptions;
using Microsoft.Extensions.Options;
using System.Net;

namespace Infrastructure.Services
{
    public class FileService : IFileService
    {
        private readonly FtpConfiguration _ftpConfig;
        public FileService(IOptions<FtpConfiguration> ftpOptions)
        {
            _ftpConfig = ftpOptions.Value;
        }

        public async Task<string> SaveFileAndGetUrlAsync(string fileName, Stream fileStream)
        {
            var filePath = $"{_ftpConfig.Hostname}/{fileName}";

            var request = WebRequest.Create(filePath) as FtpWebRequest;
            request.Credentials = new NetworkCredential(_ftpConfig.Username, _ftpConfig.Password);
            request.Method = WebRequestMethods.Ftp.UploadFile;

            using var ftpStream = await request.GetRequestStreamAsync();
            await fileStream.CopyToAsync(ftpStream);

            return filePath; 
        }

        public async Task SaveFile(string fileName, Stream fileStream)
        {
            // Ensure proper FTP URL format
            var ftpUrl = FormatFtpUrl(fileName);
            var request = (FtpWebRequest)WebRequest.Create(ftpUrl);
            request.Credentials = new NetworkCredential(_ftpConfig.Username, _ftpConfig.Password);
            request.Method = WebRequestMethods.Ftp.UploadFile;

            try
            {
                using var ftpStream = await request.GetRequestStreamAsync();
                await fileStream.CopyToAsync(ftpStream);
                using FtpWebResponse response = (FtpWebResponse)await request.GetResponseAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"FTP Error: {ex.Message}");
                throw; 
            }
        }

        private string FormatFtpUrl(string fileName)
        {
            string ftpHost = _ftpConfig.Hostname.TrimEnd('/');
            if (!ftpHost.StartsWith("ftp://", StringComparison.OrdinalIgnoreCase))
            {
                ftpHost = $"ftp://{ftpHost}";
            }
            return $"{ftpHost}/{fileName.TrimStart('/')}";
        }

        public async Task<byte[]> GetFile(string fileName)
        {
            var request = WebRequest.Create($"{_ftpConfig.Hostname}/{fileName}") as FtpWebRequest;
            request.Credentials = new NetworkCredential(_ftpConfig.Username, _ftpConfig.Password);
            request.Method = WebRequestMethods.Ftp.DownloadFile;

            try
            {
                using var response = (FtpWebResponse)await request.GetResponseAsync();
                using var responseStream = response.GetResponseStream();

                using var memoryStream = new MemoryStream();
                responseStream.CopyTo(memoryStream);
                var bytes = memoryStream.ToArray();
                return bytes;
            }
            catch (Exception ex)
            {
                throw new NotFoundException(ex.Message);
            }
        }

        public async Task DeleteFile(string fileName)
        {
            var request = WebRequest.Create($"{_ftpConfig.Hostname}/{fileName}") as FtpWebRequest;
            request.Credentials = new NetworkCredential(_ftpConfig.Username, _ftpConfig.Password);
            request.Method = WebRequestMethods.Ftp.DeleteFile;

            using FtpWebResponse response = (FtpWebResponse)await request.GetResponseAsync();
        }
    }
}
