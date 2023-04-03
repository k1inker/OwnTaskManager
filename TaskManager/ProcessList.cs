using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Threading;
using System.Threading.Tasks;

namespace TaskManager
{
    internal class ProcessList
    {
        private List<ProcessItem> _processes = new List<ProcessItem>();
        public List<ProcessItem> Processes { get { return _processes; } }
        public List<ProcessItem> GetNewListProcesses()
        {
            var processes = Process.GetProcesses();
            var threads = new List<Thread>();
            var processItems = new List<ProcessItem>();

            foreach (Process process in processes)
            {
                Thread thread = new Thread(() =>
                {
                    ProcessItem processItem = new ProcessItem(process);
                    processItems.Add(processItem);
                });
                thread.Start();
                threads.Add(thread);
            }

            foreach (Thread thread in threads)
            {
                thread.Join();
            }

            return processItems;
            //_processes.Clear();

            //foreach (Process process in Process.GetProcesses())
            //{
            //    _processes.Add(new ProcessItem(process));
            //}
            //return _processes;
        }
        private int GetParentProcessId(Process p)
        {
            int parentId = 0;

            try
            {
                ManagementObject managementObject = new ManagementObject("win32_process.handle='" + p.Id + "'");

                managementObject.Get();

                parentId = Convert.ToInt32(managementObject["ParentProcessId"]);
            }
            catch (Exception) { }
            return parentId;
        }
        public void KillTreeProcesses(int idProcess)
        {
            if (idProcess == 0)
                return;

            // ManagementObjectSearcher з параметром-запитом на виконання
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(
                "Select * From Win32_Process Where ParentProcessID=" + idProcess);

            // повертається колекція об'єктів, які задовольняють запит
            ManagementObjectCollection objectCollection = searcher.Get();

            foreach(ManagementObject obj in objectCollection)
            {
                KillTreeProcesses(Convert.ToInt32(obj["ProcessID"]));
            }
            try
            {
                //var proc = 
                //KillProcess();
            }
            catch (ArgumentException) { }
        }
        public void KillProcess(ProcessItem process)
        {
            if(process != null)
            {
                Console.WriteLine(1);
                _processes.Remove(process);
                process.TerminateProcess();
            }
        }
    }
}
