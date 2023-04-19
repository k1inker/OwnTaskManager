using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Management;

namespace TaskManager
{
    internal class ProcessList
    {
        public List<ProcessItem> CurrentProcesses { get { return _currentProcesses; } }
        private List<ProcessItem> _currentProcesses = new List<ProcessItem>();

        public List<ProcessItem> NewProcesses{ get { return _newProcesses; } }
        private List<ProcessItem> _newProcesses = new List<ProcessItem>();

        public List<int> FinishedIdProcesses{ get { return _finishedIdProcesses; } }
        private List<int> _finishedIdProcesses = new List<int>();

        public void UpdateListProcesses()
        {
            _newProcesses.Clear();
            _finishedIdProcesses.Clear();

            var currentIds = _currentProcesses.Select(p => p.ProcessId).ToList();

            foreach (var p in Process.GetProcesses())
            {
                if (!currentIds.Remove(p.Id)) // it's a new process id
                {
                    ProcessItem newProcces = new ProcessItem(p);
                    _newProcesses.Add(newProcces);
                    _currentProcesses.Add(newProcces);
                }
            }

            foreach (var id in currentIds) // these do not exist any more
            {
                var process = _currentProcesses.First(p => p.ProcessId == id);

                _finishedIdProcesses.Add(process.ProcessId);
                _currentProcesses.Remove(process);
            }
        }
        public void UpdateInfoProcesses()
        {
            foreach(var process in _currentProcesses)
            {
                process.UpdateInfo();
            }
        }
        public int GetParentProcessId(ProcessItem p)
        {
            int parentId = 0;

            try
            {
                ManagementObject managementObject = new ManagementObject("win32_process.handle='" + p.ProcessId + "'");

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
                ProcessItem proc = CurrentProcesses.First((x) => x.ProcessId == idProcess);
                KillProcess(proc);
            }
            catch (ArgumentException) { }
        }
        public void KillProcess(ProcessItem process)
        {
            if(process != null)
            {
                process.TerminateProcess();
            }
        }
    }
}
