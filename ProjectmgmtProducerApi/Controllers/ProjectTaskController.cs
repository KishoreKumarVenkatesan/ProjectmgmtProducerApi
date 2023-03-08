using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectmgmtProducerApi.Model;
using ProjectmgmtProducerApi.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Confluent.Kafka;
using System.Diagnostics;
using System.Net;
using System.Text.Json;
using System.Net.Http;
using Microsoft.AspNetCore.Authorization;

namespace ProjectmgmtProducerApi.Controllers
{
    [Authorize]
    [Route("[controller]/api/v1/manager")]
    [ApiController]
    public class ProjectTaskController : ControllerBase
    {
        private readonly string bootstrapServers = "localhost:9092";
        private readonly string topic = "ProjectMgmtAPITopic";
        private readonly ICosmosDBService _cosmosDBService;
        public ProjectTaskController(ICosmosDBService cosmosDBService)
        {
            _cosmosDBService = cosmosDBService ?? throw new ArgumentNullException(nameof(cosmosDBService));
        }
 

        [HttpGet]

        public async Task<IActionResult> List([FromQuery] PagingParameters paging)
        {
            return Ok(await _cosmosDBService.GetProjectTaskMultipleAsync("select * from c", paging));
        }

        [HttpGet("{id}/{partitionKey}")]

        public async Task<IActionResult> Get(string id, string partitionKey)
        {
            return Ok(await _cosmosDBService.GetProjectTaskAsync(id, partitionKey));
        }

        [HttpPut("{id}")]

        public async Task<IActionResult> Edit([FromBody] ProjectTaskDetails taskDetails)
        {
            await _cosmosDBService.UpdateTaskAsync(taskDetails.Id,taskDetails);
            return CreatedAtAction(nameof(List), new { taskDetails.PartitionKey }, taskDetails);
        }

        [HttpPost, Route("Producer")]
        public async Task<IActionResult> PostProducer([FromBody] ProjectTaskDetails taskDetails)
        {
            string message = JsonSerializer.Serialize(taskDetails);
            return Ok(await SendTaskDetails(topic, message));
        }

        private async Task<bool> SendTaskDetails(string topic, string message)
        {
            ProducerConfig config = new ProducerConfig
            {
                BootstrapServers = bootstrapServers,
                ClientId = Dns.GetHostName()
            };

            try
            {
                using (var producer = new ProducerBuilder<Null, string>(config).Build())
                {
                    var result = await producer.ProduceAsync
                        (topic, new Message<Null, string>
                        {
                            Value = message
                        });
                    Debug.WriteLine($"Delivered TimeStamp: {result.Timestamp.UtcDateTime}");
                    return await Task.FromResult(true);

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error Occurred: {ex.Message}");

            }
            return await Task.FromResult(false);

        }
    }
}
