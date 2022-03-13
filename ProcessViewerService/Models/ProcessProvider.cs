using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Management;
using System.Threading.Tasks;

namespace SimpleProcessViewer.Models
{
    public class ProcessProvider : IProcessProvider
    {
        
        public List<ProcessDescription> GetAllProcessList()
        {
            var processList = Process.GetProcesses();
            List<ProcessDescription> processBriefList = new List<ProcessDescription>();
            foreach (var process in processList)
            {
                ProcessDescription briefDescription = new ProcessDescription()
                {
                    Id = process.Id,
                    Name = process.ProcessName,
                    Responding = process.Responding,
                    Memory = BytesToReadableValue(process.PrivateMemorySize64)

                };
                processBriefList.Add(briefDescription);
            }


            return processBriefList;
        }

        public async Task<List<ProcessDescription>> GetAllProcessListAsync()
        {
            return await Task.Run(() => GetAllProcessList());
            
        }

        /// <summary>
        /// Get owner property and executable path selected process. WMI used
        /// </summary>
        /// <param name="processId"></param>
        /// <returns></returns>
        public ProcessDetails GetProcessDetail(int processId)
        {
            //string query = $"SELECT ProcessId, ExecutablePath,  FROM Win32_Process WHERE ProcessId={processId}";
            //SELECT PercentProcessorTime FROM Win32_PerfFormattedData_PerfOS_Processor WHERE Name = '_Total'

            string query = $"SELECT * FROM Win32_Process WHERE ProcessId={processId}";
            ProcessDetails processDetails = new ProcessDetails();
            using (var search = new ManagementObjectSearcher(query))
            {
                using (var results = search.Get())
                {

                    var processModuleDetails = results.Cast<ManagementObject>().FirstOrDefault();

                    if (processModuleDetails != null)
                    {
                        //username 
                        string[] argList = new string[] { string.Empty, string.Empty };
                        int returnVal = Convert.ToInt32(processModuleDetails.InvokeMethod("GetOwner", argList));
                        if (returnVal == 0)
                        {
                            processDetails.Username = argList[0];
                        }

                        if (processModuleDetails["ExecutablePath"] != null)
                        {
                            processDetails.ExecutablePath = processModuleDetails["ExecutablePath"].ToString();
                        }

                    }
                }
            }
            return processDetails;
        }

        public async Task<ProcessDetails> GetProcessDetailAsync(int processId)
        {
            return await Task.Run(() => GetProcessDetail(processId));
        }

        public async Task<List<FullProcessInfo>> GetAllDetailedProcessListAsync()
        {
            return await Task.Run(() => GetAllDetailedProcessList());
        }
    
        /// <summary>
        /// Only for internal test in wpf solution. Very slow
        /// </summary>
        /// <returns></returns>
        public List<FullProcessInfo> GetAllDetailedProcessList()
        {
            var processList = GetAllProcessList();
            List<FullProcessInfo> result = new List<FullProcessInfo>();
            
                foreach (ProcessDescription process in processList)
                {
                    var desc = GetProcessDetail(process.Id);
               
                    FullProcessInfo fullInfo = new FullProcessInfo
                    {
                        Id = process.Id,
                        Memory = process.Memory,
                        Name = process.Name,
                        Responding = process.Responding,
                        Username = desc.Username,
                        ExecutablePath = desc.ExecutablePath,
                    };
                    result.Add(fullInfo);
                }
            

            return result;
        }

        private string BytesToReadableValue(long number)
        {
            List<string> suffixes = new List<string> { " B", " KB", " MB", " GB", " TB" };

            for (int i = 0; i < suffixes.Count; i++)
            {
                long temp = number / (int)Math.Pow(1024, i + 1);

                if (temp == 0)
                {
                    return (number / (int)Math.Pow(1024, i)) + suffixes[i];
                }
            }

            return number.ToString();
        }


        

        



    }
}
