using System.Collections.Generic;
using System.Threading.Tasks;

namespace SimpleProcessViewer.Models
{
    public interface IProcessProvider
    {
        List<ProcessDescription> GetAllProcessList();
        Task<List<ProcessDescription>> GetAllProcessListAsync();
        ProcessDetails GetProcessDetail(int processId);
        Task<ProcessDetails> GetProcessDetailAsync(int processId);
        List<FullProcessInfo> GetAllDetailedProcessList();
        Task<List<FullProcessInfo>> GetAllDetailedProcessListAsync();
    }
}
