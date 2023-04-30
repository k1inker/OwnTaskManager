using System;
using System.Windows.Forms.DataVisualization.Charting;

namespace TaskManager
{
    internal class GraphBuilder
    {
        public enum TypeMetric
        {
            MemoryUsage,
            CPU,
            Network
        }
        private ProcessItem _processItem;
        private Chart _chart;
        
        private TypeMetric _typeMetric = TypeMetric.MemoryUsage;
        
        private int _maxDataPoints = 20;
        public GraphBuilder(Chart chart)
        {
            _chart = chart;
        }
        public void SetGraphSettings(ProcessItem selectedProcessItem)
        {
            _processItem = selectedProcessItem;
            _chart.Series[0].Points.AddXY(0, 0);
            _chart.ChartAreas[0].AxisX.LabelStyle.Enabled = false;
            _chart.Titles[0].Text = "Graph process " + selectedProcessItem.ProcessName + " by " + _typeMetric.ToString();
            _chart.ChartAreas[0].AxisY.Title = GetAxisTitleMetricType();
        }
        public void AddPoint()
        {
            if (_chart.Series[0].Points.Count - 1 >= _maxDataPoints)
            {
                _chart.Series[0].Points.RemoveAt(0);
            }

            if (_typeMetric == TypeMetric.MemoryUsage)
            {
                _chart.Series[0].Points.Add((float)_processItem.ProcessMemoryUsed);
            }
            else if (_typeMetric == TypeMetric.CPU)
            {
                _chart.Series[0].Points.Add((float)_processItem.ProcessUsedCPU);
            }
            else if (_typeMetric == TypeMetric.Network)
            {
                _chart.Series[0].Points.Add((float)_processItem.ProcessNetworkUsed);
            }
        }
        public void ChangeMetric(TypeMetric typeMetric, string nameMetric)
        {
            _typeMetric = typeMetric;
            _chart.Titles[0].Text = "Graph process " + _processItem.ProcessName + " by " + _typeMetric.ToString();
            _chart.ChartAreas[0].AxisY.Title = GetAxisTitleMetricType();
            ClearPoint();
        }
        public void ClearPoint()
        {
            _chart.Series[0].Points.Clear();
        }
        private string GetAxisTitleMetricType()
        {
            if (_typeMetric == TypeMetric.MemoryUsage)
            {
                return "Memory Usage МB";
            }
            else if (_typeMetric == TypeMetric.CPU)
            {
                return "CPU Usage (%)";
            }
            else if (_typeMetric == TypeMetric.Network)
            {
                return "Network Usage Мb";
            }
            else
                return "";
        }
    }
}
