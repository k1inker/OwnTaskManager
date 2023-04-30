using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms.DataVisualization.Charting;

namespace TaskManager
{
    internal class ProcessManager
    {
        public ProcessList ProcessList { get { return _processList; } }
        private ProcessList _processList = new ProcessList();

        public GraphBuilder GraphBuilder { get { return _graphBuilder; } }
        private GraphBuilder _graphBuilder;

        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private Task _updateTask;
        public ProcessManager(Chart chart)
        {
            _graphBuilder = new GraphBuilder(chart);
        }
        public void UpdateProcessList()
        {
            // сделать паралельний поток отримання інформації процессів
            _processList.UpdateListProcesses();
        }
        public void StartUpdateInfo()
        {
            _updateTask = Task.Run(async () =>
            {
                while (!_cancellationTokenSource.Token.IsCancellationRequested)
                {
                    // Викликаємо метод оновлення інформації про процеси
                    await UpdateProcessesAsync();

                    // Почекати 2 секунд перед наступним оновленням
                    await Task.Delay(2000);
                }
            });
        }
        private async Task UpdateProcessesAsync()
        {
            // Тут виконуємо код для оновлення інформації про процеси
            _processList.UpdateInfoProcesses();
            await Task.Yield(); // додаємо цей рядок, щоб виконання методу було асинхронним
        }
        private void StopUpdate()
        {
            _cancellationTokenSource.Cancel();
            _updateTask.Wait();
        }
    }
}
