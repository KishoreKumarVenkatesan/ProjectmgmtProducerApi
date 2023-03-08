using Microsoft.Azure.Cosmos;
using ProjectmgmtProducerApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectmgmtProducerApi.Data
{
    public class CosmosDBService:ICosmosDBService
    {
        private Container _container;
        public CosmosDBService(
            CosmosClient cosmosDbClient,
            string databaseName,
            string containerName)
        {
            _container = cosmosDbClient.GetContainer(databaseName, containerName);
        }
        public async Task AddAsync(ProjectMemberDetails memberdetails)
        {
            await _container.CreateItemAsync(memberdetails, new PartitionKey(memberdetails.PartitionKey));
        }

        public async Task DeleteAsync(string id,string partitionKey)
        {
            await _container.DeleteItemAsync<ProjectMemberDetails>(id, new PartitionKey(partitionKey));
        }

        public async Task<ProjectMemberDetails> GetAsync(string id,string partitionKey)
        {
            try
            {
                var response = await _container.ReadItemAsync<ProjectMemberDetails>(id, new PartitionKey(partitionKey));               
                return response.Resource;
            }
            catch (CosmosException ex)
            {
                return null;
            }
        }

        public async Task<ProjectTaskDetails> GetProjectTaskAsync(string id, string partitionKey)
        {
            try
            {
                var response = await _container.ReadItemAsync<ProjectTaskDetails>(id, new PartitionKey(partitionKey));
                return response.Resource;
            }
            catch (CosmosException ex)
            {
                return null;
            }
        }

        public async Task<IEnumerable<ProjectMemberDetails>> GetMultipleAsync(string queryString,PagingParameters paging)
        {
            var query = _container.GetItemQueryIterator<ProjectMemberDetails>(new QueryDefinition(queryString));
            var results = new List<ProjectMemberDetails>();
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                //Sorting,Filtering & Paging Implementation
                results.AddRange(response.ToList().OrderByDescending(x => x.MemberName)
                    .Skip((paging.PageNumber - 1) * paging.PageSize)
                    .Take(paging.PageSize)
                    .ToList());
            }

            return results;
        }

        public async Task<IEnumerable<ProjectTaskDetails>> GetProjectTaskMultipleAsync(string queryString, PagingParameters paging)
        {
            var query = _container.GetItemQueryIterator<ProjectTaskDetails>(new QueryDefinition(queryString));
            var results = new List<ProjectTaskDetails>();
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                //Sorting,Filtering & Paging Implementation
                results.AddRange(response.ToList().OrderByDescending(x => x.Id)
                    .Skip((paging.PageNumber - 1) * paging.PageSize)
                    .Take(paging.PageSize)
                    .ToList());
            }

            return results;
        }
        public async Task UpdateAsync(string id, ProjectMemberDetails memberdetails)
        {
            await _container.UpsertItemAsync(memberdetails, new PartitionKey(memberdetails.PartitionKey));
        }

        public async Task UpdateTaskAsync(string id, ProjectTaskDetails taskDetails)
        {
            await _container.UpsertItemAsync(taskDetails, new PartitionKey(taskDetails.PartitionKey));
        }

         
    }
}
