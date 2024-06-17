using Amazon;
using Amazon.EC2;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyAspnetCore.Interfaces.Services;

namespace MyAspnetApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    //[Authorize]
    public class S3Controller : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;
        private IAmazonS3 _s3client;
        public S3Controller(IConfiguration configuration, IUserService userService)
        {
            _configuration = configuration;
            _userService = userService;
            Initialize().Wait();
        }
        private async Task Initialize()
        {
            var user = await _userService.GetUserInfo(); ;
            if (user != null)
            {
                _s3client = InitializeClient(user.AccessKey, user.SecretKey);
            }
        }
        private IAmazonS3 InitializeClient(string accessKey, string secretKey)
        {
            if (!string.IsNullOrEmpty(accessKey) && !string.IsNullOrEmpty(secretKey))
            {
                var credentials = new BasicAWSCredentials(accessKey, secretKey);
                var regionEndpoint = RegionEndpoint.USEast1; // Thay đổi region endpoint tại đây
                return new AmazonS3Client(credentials, regionEndpoint);
            }

            return null;
        }
        private bool ClientIsNull()
        {
            return _s3client == null;
        }
        [HttpGet("buckets")]
        public async Task<IActionResult> ListBuckets()
        {
            //if (ClientIsNull())
            //    return BadRequest("S3 client is not initialized.");

            try
            {
                var listBucketsResponse = await _s3client.ListBucketsAsync();
                var buckets = listBucketsResponse.Buckets.Select(bucket => new
                {
                    BucketName = bucket.BucketName,
                    CreationDate = bucket.CreationDate
                });

                return Ok(buckets);
            }
            catch (AmazonS3Exception ex)
            {
                return BadRequest($"Error: {ex.ErrorCode}, {ex.Message}");
            }
        }

        [HttpPost("create-bucket")]
        public async Task<IActionResult> CreateBucket(string bucketName)
        {
            //if (ClientIsNull())
            //    return BadRequest("S3 client is not initialized.");

            try
            {
                var putBucketRequest = new PutBucketRequest
                {
                    BucketName = bucketName
                };

                var putBucketResponse = await _s3client.PutBucketAsync(putBucketRequest);
                return Ok($"Bucket '{bucketName}' created successfully.");
            }
            catch (AmazonS3Exception ex)
            {
                return BadRequest($"Error: {ex.ErrorCode}, {ex.Message}");
            }
        }

        [HttpPost("delete-bucket")]
        public async Task<IActionResult> DeleteBucket(string bucketName)
        {
            //if (ClientIsNull())
            //    return BadRequest("S3 client is not initialized.");

            try
            {
                var deleteBucketRequest = new DeleteBucketRequest
                {
                    BucketName = bucketName
                };

                var deleteBucketResponse = await _s3client.DeleteBucketAsync(deleteBucketRequest);
                return Ok($"Bucket '{bucketName}' deleted successfully.");
            }
            catch (AmazonS3Exception ex)
            {
                return BadRequest($"Error: {ex.ErrorCode}, {ex.Message}");
            }
        }

        [HttpPost("upload-file")]
        public async Task<IActionResult> UploadFile(string bucketName, IFormFile file)
        {
            //if (ClientIsNull())
            //    return BadRequest("S3 client is not initialized.");

            try
            {
                using (var stream = file.OpenReadStream())
                {
                    var putObjectRequest = new PutObjectRequest
                    {
                        BucketName = bucketName,
                        Key = file.FileName,
                        InputStream = stream,
                        ContentType = file.ContentType
                    };

                    var putObjectResponse = await _s3client.PutObjectAsync(putObjectRequest);
                    return Ok($"File '{file.FileName}' uploaded successfully to bucket '{bucketName}'.");
                }
            }
            catch (AmazonS3Exception ex)
            {
                return BadRequest($"Error: {ex.ErrorCode}, {ex.Message}");
            }
        }

        [HttpPost("delete-file")]
        public async Task<IActionResult> DeleteFile(string bucketName, string key)
        {
            //if (ClientIsNull())
            //    return BadRequest("S3 client is not initialized.");

            try
            {
                var deleteObjectRequest = new DeleteObjectRequest
                {
                    BucketName = bucketName,
                    Key = key
                };

                var deleteObjectResponse = await _s3client.DeleteObjectAsync(deleteObjectRequest);
                return Ok($"File '{key}' deleted successfully from bucket '{bucketName}'.");
            }
            catch (AmazonS3Exception ex)
            {
                return BadRequest($"Error: {ex.ErrorCode}, {ex.Message}");
            }
        }
    }
}
