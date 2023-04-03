using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager
{
    internal class ProcessManager
    {
        private ProcessList _processList = new ProcessList();
        public ProcessList ProcessList { get { return _processList; } } 
        public List<ProcessItem> GetProcessList()
        {
            return _processList.GetNewListProcesses();
        }
    }
}
