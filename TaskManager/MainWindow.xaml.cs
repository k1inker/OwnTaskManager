using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Windows.Threading;
using System.Data;

namespace TaskManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ProcessManager _processManager = new ProcessManager();
        public MainWindow()
        {
            InitializeComponent();

            // ініціалізація таймера
            DispatcherTimer _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(2) };
            _timer.Tick += UpdateProcesses;
            _timer.Start();
        }
        private void UpdateProcesses(object sender, EventArgs e)
        {
            _processManager.StartUpdateInfo();
            UpdateListProcesses();
        }
        private void UpdateListProcesses()
        {
            _processManager.UpdateProcessList();
            AddNewProcesses();
            DeleteFinishedProcessItem();
            UpdateInfoProcessList();
        }
        private void UpdateInfoProcessList()
        {
            var rowsToUpdate = new Dictionary<int,dynamic>();
            foreach (dynamic item in gridPocess.Items)
            {
                if (item != null)
                {
                    ProcessItem updatedItem = _processManager.ProcessList.CurrentProcesses.First(x => x.ProcessId == item.ID);
                    if (updatedItem != null)
                    {
                        var newItem = new { 
                            ID = updatedItem.ProcessId, 
                            Name = updatedItem.ProcessName, 
                            CPU = updatedItem.ProcessUsedCPU, 
                            Memory = updatedItem.ProcessMemoryUsed, 
                            Network = updatedItem.ProcessNetworkUsed };
                        rowsToUpdate.Add(gridPocess.Items.IndexOf(item), newItem);
                    }
                }
            }
            foreach(var item in rowsToUpdate)
            {
                gridPocess.Items[item.Key] = item.Value; 
            }    
            gridPocess.Items.Refresh();
        }
        private void DeleteFinishedProcessItem()
        {
            var itemsToDelete = new List<dynamic>();
            foreach (dynamic item in gridPocess.Items)
            {
                if (item != null)
                {
                    int id = item.ID;
                    if (_processManager.ProcessList.FinishedIdProcesses.Contains(id))
                    {
                        itemsToDelete.Add(item);
                    }
                }
            }

            foreach (dynamic item in itemsToDelete)
            {
                gridPocess.Items.Remove(item);
            }
        }
        private void AddNewProcesses()
        {
            foreach (var item in _processManager.ProcessList.NewProcesses)
            {
                gridPocess.Items.Add(new
                {
                    ID = item.ProcessId,
                    //Name = item.ProcessName,
                    //CPU = item.ProcessUsedCPU,
                    //Memory = item.ProcessMemoryUsed,
                    //Network = item.ProcessNetworkUsed
                });
            }
        }
        private void GridRowMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            DataGridRow row = sender as DataGridRow;
            if (row != null && row.Item != null)
            {
                // Створити нове контекстне меню
                ContextMenu contextMenu = new ContextMenu();

                // Додати пункти меню
                MenuItem menuItem1 = new MenuItem();
                menuItem1.Header = "Завершити процесс";
                menuItem1.Click += MenuItem1_Click;
                contextMenu.Items.Add(menuItem1);

                MenuItem menuItem2 = new MenuItem();
                menuItem2.Header = "Завершити дерево процессів";
                menuItem2.Click += MenuItem2_Click;
                contextMenu.Items.Add(menuItem2);

                MenuItem menuItem3 = new MenuItem();
                menuItem3.Header = "Відкрити розташування файлу";
                menuItem3.Click += MenuItem3_Click;
                contextMenu.Items.Add(menuItem3);

                // Відобразити контекстне меню
                row.ContextMenu = contextMenu;
            }
        }
        private void GridRowMouseDoubleButtonDown(object sender, MouseButtonEventArgs e)
        {

        }
        private void MenuItem1_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (gridPocess.SelectedItems[0] != null)
                {
                    dynamic selectedItem = gridPocess.SelectedItems[0];
                    ProcessItem processToKill = _processManager.ProcessList.CurrentProcesses.FirstOrDefault((x) => x.ProcessId == selectedItem.ID);
                    _processManager.ProcessList.KillProcess(processToKill);
                    
                    UpdateListProcesses();
                }
            }
            catch (Exception) { }
        }
        private void MenuItem2_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (gridPocess.SelectedItems[0] != null)
                {
                    dynamic selectedItem = gridPocess.SelectedItems[0];
                    ProcessItem processToKill = _processManager.ProcessList.CurrentProcesses.FirstOrDefault((x) => x.ProcessId == selectedItem.ID);
                    if (processToKill != null)
                    {
                        _processManager.ProcessList.KillTreeProcesses(_processManager.ProcessList.GetParentProcessId(processToKill));

                        UpdateListProcesses();
                    }
                }
            }
            catch (Exception) { }
        }
        private void MenuItem3_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (gridPocess.SelectedItems[0] != null)
                {
                    dynamic selectedItem = gridPocess.SelectedItems[0];
                    ProcessItem processToOpen = _processManager.ProcessList.CurrentProcesses.FirstOrDefault((x) => x.ProcessId == selectedItem.ID);
                    processToOpen.OpenFileLocation();
                }
            }
            catch (Exception) { }
        }
    }
}
