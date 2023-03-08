using Confluent.Kafka;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Text.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
using ProjectmgmtProducerApi.Model;
using ProjectmgmtProducerApi.Data;
using Microsoft.AspNetCore.Authorization;
//using System.Web.Http;
//using System.Web.Http;

namespace ProjectmgmtProducerApi.Controllers
{
    [Authorize]
    [Route("[controller]/api/v1/manager")]
    [ApiController]
    
    public class ProjectMemberController : ControllerBase
    {
        private readonly string bootstrapServers = "localhost:9092";
        private readonly string topic = "PrjMgmtAPITest";
        private readonly ICosmosDBService _cosmosDBService;

        public ProjectMemberController(ICosmosDBService cosmosDBService)
        {
            _cosmosDBService = cosmosDBService ?? throw new ArgumentNullException(nameof(cosmosDBService));
        }

        [HttpGet]
     
        public async Task<IActionResult> List([FromQuery] PagingParameters paging)
        {
            return Ok(await _cosmosDBService.GetMultipleAsync("select * from c",paging));
        }

        [HttpGet("{id}/{partitionKey}")]

        public async Task<IActionResult> Get(string id,string partitionKey)
        {
            return Ok(await _cosmosDBService.GetAsync(id,partitionKey));
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Edit([FromBody] ProjectMemberDetails memberDetails)
        {
            if (memberDetails.ProjectEndDate < Convert.ToDateTime(DateTime.Now.ToShortDateString()))
            {
               memberDetails.AllocationPercentage = 0;
            }
            else
            {
                memberDetails.AllocationPercentage = 100;
            }

            await _cosmosDBService.UpdateAsync(memberDetails.Id, memberDetails);
            return CreatedAtAction(nameof(List), new { memberDetails.PartitionKey }, memberDetails);

        }
        [HttpDelete("{id}/{partitionKey}")]
        public async Task<IActionResult> Delete(string id,string partitionKey)
        {
            await _cosmosDBService.DeleteAsync(id,partitionKey);
           
            return NoContent();

        }

        [HttpPost, Route("Create")]
        
        public async Task<IActionResult> Create([FromBody] ProjectMemberDetails memberDetails)
        {
            memberDetails.Id = Guid.NewGuid().ToString();
            await _cosmosDBService.AddAsync(memberDetails);
            return CreatedAtAction(nameof(List), new { memberDetails.PartitionKey }, memberDetails);
        }
       
    }

}

