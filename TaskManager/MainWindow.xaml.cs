using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.DataVisualization.Charting;
using System.Windows.Input;
using System.Windows.Threading;

using DataVis = System.Windows.Forms.DataVisualization;

namespace TaskManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ProcessManager _processManager;
        private bool isGraphOpen = false;
        public MainWindow()
        {
            InitializeComponent();

            _processManager = new ProcessManager(chart);
            Task.Run(() => UpdateProcessesAsync());
        }
        private async Task UpdateProcessesAsync()
        {
            while (true)
            {
                _processManager.StartUpdateInfo();
                // оновлюємо список процесів у головному потоці
                await Dispatcher.InvokeAsync(UpdateListProcesses);
                await Task.Delay(TimeSpan.FromSeconds(2));
            }
        }
        private void UpdateListProcesses()
        {
            _processManager.UpdateProcessList();
            AddNewProcesses();
            DeleteFinishedProcessItem();
            UpdateInfoProcessList();
            if(isGraphOpen)
            {
                UpdateViewGrapher();
            }
        }
        private void UpdateViewGrapher()
        {
            _processManager.GraphBuilder.AddPoint();
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
                            CPU = updatedItem.ProcessUsedCPU.ToString("F2"), 
                            Memory = updatedItem.ProcessMemoryUsed.ToString("F2"), 
                            Network = updatedItem.ProcessNetworkUsed.ToString("F2") };
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
                menuItem1.Click += MenuItemTerminate_Click;
                menuItem1.Tag = row.Item; // передача об'єкту row.Item
                contextMenu.Items.Add(menuItem1);

                MenuItem menuItem2 = new MenuItem();
                menuItem2.Header = "Завершити дерево процессів";
                menuItem2.Click += MenuItemParentTerminate_Click;
                menuItem2.Tag = row.Item;
                contextMenu.Items.Add(menuItem2);

                MenuItem menuItem3 = new MenuItem();
                menuItem3.Header = "Відкрити розташування файлу";
                menuItem3.Click += MenuItemFileLocation_Click;
                menuItem3.Tag = row.Item;
                contextMenu.Items.Add(menuItem3);
                
                MenuItem menuItem4 = new MenuItem();
                menuItem4.Header = "Відкрити властивості процессу";
                menuItem4.Click += MenuItemOpenProperties_Click;
                menuItem4.Tag = row.Item;
                contextMenu.Items.Add(menuItem4);

                // Відобразити контекстне меню
                row.ContextMenu = contextMenu;
            }
        }
        private void GridRowMouseDoubleButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                DataGridRow row = sender as DataGridRow;
                if (row.Item != null)
                {
                    rightPanel.Width = new GridLength(1, GridUnitType.Star);
                    isGraphOpen = true;
                    dynamic selectedItem = row.Item;
                    ProcessItem process = _processManager.ProcessList.CurrentProcesses.FirstOrDefault((x) => x.ProcessId == selectedItem.ID);
                    _processManager.GraphBuilder.ClearPoint();
                    _processManager.GraphBuilder.SetGraphSettings(process);
                    UpdateViewGrapher();
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
        private void MenuItemTerminate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                dynamic selectedItem = (sender as MenuItem).Tag;
                ProcessItem processToKill = _processManager.ProcessList.CurrentProcesses.FirstOrDefault((x) => x.ProcessId == selectedItem.ID);
                _processManager.ProcessList.KillProcess(processToKill);
                    
                UpdateListProcesses();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
        private void MenuItemParentTerminate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                dynamic selectedItem = (sender as MenuItem).Tag;
                ProcessItem processToKill = _processManager.ProcessList.CurrentProcesses.FirstOrDefault((x) => x.ProcessId == selectedItem.ID);
                if (processToKill != null)
                {
                    _processManager.ProcessList.KillTreeProcesses(_processManager.ProcessList.GetParentProcessId(processToKill));

                    UpdateListProcesses();
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
        private void MenuItemFileLocation_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                dynamic selectedItem = (sender as MenuItem).Tag;
                ProcessItem processToOpen = _processManager.ProcessList.CurrentProcesses.FirstOrDefault((x) => x.ProcessId == selectedItem.ID);
                processToOpen.OpenFileLocation();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
        private void MenuItemOpenProperties_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                dynamic selectedItem = (sender as MenuItem).Tag;
                ProcessItem processToOpen = _processManager.ProcessList.CurrentProcesses.FirstOrDefault((x) => x.ProcessId == selectedItem.ID);
                processToOpen.OpenPropertiesProcess();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
        private void MenuItemMetric_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;
            string header = menuItem.Header.ToString();

            if(header == "Memory Usage")
            {
                _processManager.GraphBuilder.ChangeMetric(GraphBuilder.TypeMetric.MemoryUsage, header);
                CPUMenuItem.IsEnabled = true;
                networkMenuItem.IsEnabled = true;
            }
            else if(header == "CPU Usage")
            {
                _processManager.GraphBuilder.ChangeMetric(GraphBuilder.TypeMetric.CPU, header);
                memoryMenuItem.IsEnabled = true;
                networkMenuItem.IsEnabled = true;
            }
            else if(header == "Network Usage")
            {
                _processManager.GraphBuilder.ChangeMetric(GraphBuilder.TypeMetric.Network, header);
                memoryMenuItem.IsEnabled = true;
                CPUMenuItem.IsEnabled = true;
            }
            menuItem.IsEnabled = false;
        }
        private void MenuItemHideGraph(object sender, RoutedEventArgs e)
        {
            rightPanel.Width = new GridLength(0);
            isGraphOpen = false;
            _processManager.GraphBuilder.ClearPoint();
        }
    }
}
