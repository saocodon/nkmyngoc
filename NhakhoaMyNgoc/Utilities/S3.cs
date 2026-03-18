using Amazon.S3;
using Amazon.S3.Model;
using System.IO;
using System.Net.Http;
using System.Runtime.CompilerServices;

namespace NhakhoaMyNgoc.Utilities
{
    public class S3
    {
        private readonly IAmazonS3 _client;
        private readonly string _bucket;
        private readonly string _baseUrl;

        public S3(string endpoint, string accessKey, string secretKey, string bucket)
        {
            var config = new AmazonS3Config
            {
                ServiceURL = endpoint,
                ForcePathStyle = true, // BẮT BUỘC cho MinIO
            };

            _client = new AmazonS3Client(accessKey, secretKey, config);
            _bucket = bucket;
            _baseUrl = endpoint.TrimEnd('/');
        }

        // Upload file
        public async Task<string> UploadAsync(Stream stream, string fileName, string contentType = "application/octet-stream")
        {
            var request = new PutObjectRequest
            {
                BucketName = _bucket,
                Key = fileName,
                InputStream = stream,
                ContentType = contentType
            };

            await _client.PutObjectAsync(request);

            return GetUrl(fileName);
        }

        // Delete file
        public async Task DeleteAsync(string fileName)
        {
            var request = new DeleteObjectRequest
            {
                BucketName = _bucket,
                Key = fileName
            };

            await _client.DeleteObjectAsync(request);
        }

        // Lấy URL public (nếu bucket public)
        public string GetUrl(string fileName)
        {
            return $"{_baseUrl}/{_bucket}/{fileName}";
        }

        // Presigned URL (nếu private)
        public string GetPresignedUrl(string fileName, int expireMinutes = 60)
        {
            return _client.GetPreSignedURL(new GetPreSignedUrlRequest
            {
                BucketName = _bucket,
                Key = fileName,
                Expires = DateTime.UtcNow.AddMinutes(expireMinutes)
            });
        }

        public async Task Cache(string key, string localPath)
        {
            // TODO: bỏ http
            var url = GetPresignedUrl(key, 1).Replace("https", "http");

            using var http = new HttpClient();
            var bytes = await http.GetByteArrayAsync(url);

            await File.WriteAllBytesAsync(localPath, bytes);
        }
    }
}
