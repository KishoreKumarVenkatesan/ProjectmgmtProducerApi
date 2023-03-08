using ProjectmgmtProducerApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectmgmtProducerApi.Data
{
    public interface ICosmosDBService
    {
        Task<IEnumerable<ProjectMemberDetails>> GetMultipleAsync(string query, PagingParameters paging);
        Task<IEnumerable<ProjectTaskDetails>> GetProjectTaskMultipleAsync(string queryString, PagingParameters paging);
        Task<ProjectMemberDetails> GetAsync(string id,string partitionKey);
        Task<ProjectTaskDetails> GetProjectTaskAsync(string id, string partitionKey);
        Task AddAsync(ProjectMemberDetails memberDetails);
        Task UpdateTaskAsync(string id, ProjectTaskDetails taskDetails);
        Task UpdateAsync(string id, ProjectMemberDetails memberDetails);
        Task DeleteAsync(string id,string partitionKey);
    }
}
