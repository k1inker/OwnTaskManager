using System.IO;
using System.Diagnostics;
using System;

namespace TaskManager
{
    internal class ProcessItem
    {
        public int ProcessId { get { return _processId; } }
        private int _processId;
        public string ProcessName { get { return _processName; } }
        private string _processName;
        public double ProcessMemoryUsed { get { return _processMemoryUsed; } }
        private double _processMemoryUsed;
        public float ProcessUsedCPU { get{ return _processUsedCPU; } }
        private float _processUsedCPU;
        public float ProcessNetworkUsed { get { return _processNetworkUsed; } }
        private float _processNetworkUsed;

        private Process _process;
        private PerformanceCounter _networkPerformanceCounter;
        private PerformanceCounter _cpuPerformanceCounter;
        public ProcessItem(Process process)
        {
            _process = process;
            _processId = _process.Id;
            _processName = _process.ProcessName;
            _processMemoryUsed = (double)_process.WorkingSet64 / (1000 * 1000);

            _networkPerformanceCounter = new PerformanceCounter("Process", "IO Data Bytes/sec", _processName);
            _cpuPerformanceCounter = new PerformanceCounter("Process", "% Processor Time", _processName);
            _networkPerformanceCounter.NextValue();
            _cpuPerformanceCounter.NextValue();
        }
        public void UpdateProcessUsedCPU()
        {
            if(_cpuPerformanceCounter != null)
                _processUsedCPU = _cpuPerformanceCounter.NextValue();
            else
                _processUsedCPU = 0;
        }
        public void UpdateNetworkUsed()
        {
            if (_cpuPerformanceCounter != null)
                _processNetworkUsed = _networkPerformanceCounter.NextValue() * 0.000008f;
            else
                _processNetworkUsed = 0;
        }
        public void UpdateInfo()
        {
            _processMemoryUsed = (double)_process.WorkingSet64 / (1000 * 1000);
            UpdateProcessUsedCPU();
            UpdateNetworkUsed();
        }
        public void OpenFileLocation()
        {
            string processFilePath = _process.MainModule.FileName;
            Console.WriteLine(processFilePath);
            string processFolder = Path.GetDirectoryName(processFilePath);
            Console.WriteLine(processFolder);
            _process.StartInfo.Arguments = "/select," + processFolder;

            _process.Start();
        }
        public void TerminateProcess()
        {
            _process.Kill();
            _process.WaitForExit();
        }
    }
}
