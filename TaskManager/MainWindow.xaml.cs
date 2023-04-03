﻿using System;
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


            gridPocess.Items.Clear();
            foreach (var item in _processManager.GetProcessList())
            {
                gridPocess.Items.Add(new
                {
                    ID = item.ProcessId,
                    Name = item.ProcessName,
                    CPU = item.ProcessUsedCPU,
                    Memory = item.ProcessMemoryUsed,
                    Network = item.ProcessNetworkUsed
                });
            }

            // ініціалізація таймера
            DispatcherTimer _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(2) };
            _timer.Tick += UpdateProcesses;
            //_timer.Start();
        }
        private void UpdateProcesses(object sender, EventArgs e)
        {
            UpdateListProcesses();
        }
        private void UpdateListProcesses()
        {
            gridPocess.Items.Clear();
            foreach (var item in _processManager.GetProcessList())
            {
                gridPocess.Items.Add(new
                {
                    Name = item.ProcessName,
                    CPU = item.ProcessUsedCPU,
                    Memory = item.ProcessMemoryUsed,
                    Network = item.ProcessNetworkUsed
                });
            }
        }
        private void GridRow_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
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
        private void MenuItem1_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (gridPocess.SelectedItems[0] != null)
                {
                    dynamic selectedItem = gridPocess.SelectedItems[0];
                    ProcessItem processToKill = _processManager.ProcessList.Processes.FirstOrDefault((x) => x.ProcessId == selectedItem.ID);
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
                    ProcessItem processToKill = _processManager.ProcessList.Processes.FirstOrDefault((x) => x.ProcessId == selectedItem.ID);
                    _processManager.ProcessList.KillTreeProcesses(_processManager.ProcessList.GetParentProcessId(processToKill));
                    
                    UpdateListProcesses();
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
                    ProcessItem processToOpen = _processManager.ProcessList.Processes.FirstOrDefault((x) => x.ProcessId == selectedItem.ID);
                    processToOpen.OpenFileLocation();
                }
            }
            catch (Exception) { }
        }
    }
}
