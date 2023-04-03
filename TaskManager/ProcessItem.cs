using System.IO;
using System.Diagnostics;
using System;

namespace TaskManager
{
    internal class ProcessItem
    {
        private int _processId;
        public int ProcessId { get { return _processId; } }
        private string _processName;
        public string ProcessName { get { return _processName; } }
        private double _processMemoryUsed;
        public double ProcessMemoryUsed { get { return _processMemoryUsed; } }
        public float ProcessUsedCPU { get { return _cpuPerformanceCounter.NextValue(); } }
        public float ProcessNetworkUsed { get { return _networkPerformanceCounter.NextValue() * 0.000008f; } }

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
