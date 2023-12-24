using Amazon;
using Amazon.Pricing;
using Amazon.Pricing.Model;
using Amazon.Runtime;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using MyAspnetCore.DTO.Service;
using MyAspnetCore.Entities;
using MyAspnetCore.Interfaces.Infrastructure;
using MyAspnetCore.Interfaces.Services;
using MyAspnetCore.Resources;
using Newtonsoft.Json.Linq;
using MyAspnetCore.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MyAspnetCore.DTO.User;

namespace MyAspnetCore.Services
{
    public class ServiceService : BaseService<Entities.Service, ServiceDto, ServiceCreateDto, ServiceUpdateDto>, IServiceService
    {
        private readonly IServiceRepository _serviceRepository;
        private readonly IAmazonPricing _pricingClient;
        private readonly IConfiguration _configuration;
        public ServiceService(IServiceRepository serviceRepository, IMapper mapper, IConfiguration configuration) : base(serviceRepository, mapper)
        {
            _serviceRepository = serviceRepository;
            _configuration = configuration;
            var accessKey = _configuration.GetValue<string>("AWS:AccessKey");
            var secretKey = _configuration.GetValue<string>("AWS:SecretKey");
            var credentials = new BasicAWSCredentials(accessKey, secretKey); // Sử dụng IAM Role
            var regionEndpoint = RegionEndpoint.USEast1; // Thay đổi region endpoint tại đây
            _pricingClient = new AmazonPricingClient(credentials, regionEndpoint);
        }

        public async Task<(int, IEnumerable<ServiceDto>)> GetListAsync(string? queryName, int? recordsPerPage, int? page)
        {
            IEnumerable<Entities.Service> services = new List<Entities.Service>();
            var totalRecord = 0;
            (totalRecord, services) = await _serviceRepository.GetListAsync(queryName, recordsPerPage, page);
            IEnumerable<ServiceDto> userDtos = _mapper.Map<IEnumerable<ServiceDto>>(services);
            return (totalRecord, userDtos);
        }

        public async Task<ServiceCreateDto> getServiceDetails(string serviceCode, Guid providerId)
        {
            var serviceRequest = new GetProductsRequest
            {
                ServiceCode = serviceCode,
                FormatVersion = "aws_v1",
                MaxResults = 1
            };

            var serviceResponse = await _pricingClient.GetProductsAsync(serviceRequest);

            if (serviceResponse.PriceList.Count > 0)
            {

                var productJson = serviceResponse.PriceList[0];
                var product = JObject.Parse(productJson)["product"];
                var attributes = product["attributes"];

                var res = new ServiceCreateDto();
                res.ProviderId = providerId;
                res.ServiceCode = attributes["servicecode"]?.Value<string>();
                res.ProductFamily = product["productFamily"]?.Value<string>();
                res.EngineCode = attributes?["engineCode"]?.Value<string>();
                //regionCode = attributes?["regionCode"]?.Value<string>(),
                res.UsageType = attributes?["usagetype"]?.Value<string>();
                res.LocationType = attributes?["locationType"]?.Value<string>();
                //location = attributes?["location"]?.Value<string>(),
                res.ServiceName = attributes?["servicename"]?.Value<string>();
                res.InstanceFamily = attributes?["instanceFamily"]?.Value<string>();
                res.Operation = attributes?["operation"]?.Value<string>();
                res.DatabaseEngine = attributes?["databaseEngine"]?.Value<string>();

                return res;
            }
            else
            {
                throw new Exceptions.NotFoundException(new List<string> { ResourceVN.Err_NotFound });
            }
        }

        public async Task<int> InsertAsyncGetDetail(string serviceCode, Guid providerId)
        {
            var details = await getServiceDetails(serviceCode, providerId);
            var entity = _mapper.Map<Entities.Service>(details);
            var properties = entity.GetType().GetProperties();
            foreach (var property in properties)
            {
                var name = property.Name;
                if (name == $"ServiceId")
                {
                    property.SetValue(entity, Guid.NewGuid());
                }
                else if (name == $"CreatedDate")
                {
                    property.SetValue(entity, DateTime.Now);
                }
                else if (name == $"CreatedBy")
                {
                    property.SetValue(entity, null);
                }
            }

            var result = await _baseRepository.InsertAsync(entity);

            //if (result == 0)
            //{
            //    throw new NotFoundException();
            //}

            return result; ;
        }
    }
}
