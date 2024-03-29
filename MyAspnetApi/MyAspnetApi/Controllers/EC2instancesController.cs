﻿using Amazon;
using Amazon.CostExplorer;
using Amazon.EC2;
using Amazon.EC2.Model;
using Amazon.Runtime;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyAspnetCore.Interfaces.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MyAspnetApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize]
    public class EC2instancesController : ControllerBase
    {
        private IAmazonEC2 _ec2client;
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;

        public EC2instancesController(IConfiguration configuration, IUserService userService)
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
                _ec2client = InitializeClient(user.AccessKey, user.SecretKey);
            }
        }
        private IAmazonEC2 InitializeClient(string accessKey, string secretKey)
        {
            if (!string.IsNullOrEmpty(accessKey) && !string.IsNullOrEmpty(secretKey))
            {
                var credentials = new BasicAWSCredentials(accessKey, secretKey);
                var regionEndpoint = RegionEndpoint.USEast1; // Thay đổi region endpoint tại đây
                return new AmazonEC2Client(credentials, regionEndpoint);
            }

            return null;
        }
        private bool ClientIsNull()
        {
            return _ec2client == null;
        }
        /// <summary>
        /// Lấy thông tin Ec2 bản thân người dùng đang sử dụng
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetInstances()
        {
            try
            {
                var describeInstancesRequest = new DescribeInstancesRequest();
                var describeInstancesResponse = await _ec2client.DescribeInstancesAsync(describeInstancesRequest);

                var instances = describeInstancesResponse.Reservations
                    .SelectMany(reservation => reservation.Instances)
                    .Select(async instance => new
                    {
                        Name = instance.KeyName,
                        InstanceId = instance.InstanceId,
                        InstanceType = instance.InstanceType.Value,
                        Zone = instance.Placement.AvailabilityZone,
                        LaunchTime = instance.LaunchTime,
                        State = instance.State.Name.Value,
                        vCpu = instance.CpuOptions?.CoreCount,
                        Platform = instance.Platform.Value,
                        PrivateIpAddress = instance.PrivateIpAddress, // Địa chỉ IPv4 nội bộ
                        PublicIpAddress = instance.PublicIpAddress,   // Địa chỉ IPv4 công khai (nếu có)
                        //z = instance.
                        //Ram = instance.GetMetadata()["instance-memory"].ToInt32(),
                        //Thêm các thông tin khác mà bạn quan tâm
                    });

                var results = await Task.WhenAll(instances);

                return Ok(results);
            }
            catch (AmazonEC2Exception ex)
            {
                return BadRequest($"Error: {ex.ErrorCode}, {ex.Message}");
            }
        }

        [HttpPost("start")]
        public async Task<IActionResult> StartInstance(string instanceId)
        {
            try
            {
                var startInstancesRequest = new StartInstancesRequest
                {
                    InstanceIds = new List<string> { instanceId }
                };

                var startInstancesResponse = await _ec2client.StartInstancesAsync(startInstancesRequest);

                // Kiểm tra trạng thái hoạt động của instance sau khi khởi động

                return Ok("Instance started successfully");
            }
            catch (AmazonEC2Exception ex)
            {
                return BadRequest($"Error: {ex.ErrorCode}, {ex.Message}");
            }
        }

        [HttpPost("stop")]
        public async Task<IActionResult> StopInstance(string instanceId)
        {
            try
            {
                var stopInstancesRequest = new StopInstancesRequest
                {
                    InstanceIds = new List<string> { instanceId }
                };

                var stopInstancesResponse = await _ec2client.StopInstancesAsync(stopInstancesRequest);

                // Kiểm tra trạng thái tắt của instance sau khi dừng

                return Ok("Instance stopped successfully");
            }
            catch (AmazonEC2Exception ex)
            {
                return BadRequest($"Error: {ex.ErrorCode}, {ex.Message}");
            }
        }

        [HttpPost("reboot")]
        public async Task<IActionResult> RebootInstance(string instanceId)
        {
            try
            {
                var rebootInstancesRequest = new RebootInstancesRequest
                {
                    InstanceIds = new List<string> { instanceId }
                };

                var rebootInstancesResponse = await _ec2client.RebootInstancesAsync(rebootInstancesRequest);

                // Kiểm tra trạng thái sau khi khởi động lại

                return Ok("Instance rebooted successfully");
            }
            catch (AmazonEC2Exception ex)
            {
                return BadRequest($"Error: {ex.ErrorCode}, {ex.Message}");
            }
        }

    }
}
