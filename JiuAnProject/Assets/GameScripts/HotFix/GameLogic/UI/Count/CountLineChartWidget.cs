using System;
using System.Collections;
using System.Collections.Generic;
using TEngine;
using UnityEngine;
using XCharts.Runtime;

namespace GameLogic
{
    public class CountLineChartWidget : UIWidget
    {
        public enum CountLineType
        {
            In = 0,
            Out,
            Stock
        }
        
        private LineChart _lineChart;
        public LineChart LineChart => _lineChart;
        protected override void OnCreate()
        {
            _lineChart = transform.GetComponent<LineChart>();
            _lineChart.EnsureChartComponent<Title>().text = $"{UIGlobalDataInstance.Instance.CurrentGroupID}";
        }
        
        public void DrawPoint(CountLineType lineType, int timeStamp, int count)
        {
            string timeStr = CustomDataFormatter(timeStamp);
            _lineChart.AddData((int)lineType, 
                new List<double>(){timeStamp, count},
                timeStr);
        }
        
        private string CustomDataFormatter(long timestamp)
        {
            DateTime dateTime = DateTimeOffset.FromUnixTimeSeconds(timestamp).LocalDateTime;
            return dateTime.ToString("HH:mm");
        }
    }
}
