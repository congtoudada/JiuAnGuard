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
        public class WidgetData
        {
            public int groupID;
            //7200~93600 2:00~次日2:00
            public List<int> inStatus; //随时间降序（从7200开始的秒数） 7200<->2:00
            public List<int> outStatus; //随时间降序（从7200开始的秒数）
        }
        
        private LineChart _lineChart;
        private int curInCount = 0;
        private int curOutCount = 0;
        private int stockCount => curInCount - curOutCount;
        public void Init(WidgetData data)
        {
            _lineChart = transform.GetComponent<LineChart>();
            if (data == null)
            {
                Log.Warning("LineChartData为null");
                return;
            }
            _lineChart.EnsureChartComponent<Title>().text = $"统计组{data.groupID}";
            _lineChart.ClearData(); //清空所有图表数据
            curInCount = data.inStatus.Count;
            curOutCount = data.outStatus.Count;

            while (data.inStatus.Count > 0 && data.outStatus.Count > 0)
            {
                int lastIn = data.inStatus[^1];
                int lastOut = data.outStatus[^1];
                if (lastIn < lastOut)
                {
                    DrawPoint(1, lastIn);
                    data.inStatus.RemoveAt(data.inStatus.Count-1);
                }
                else
                {
                    DrawPoint(2, lastOut);
                    data.outStatus.RemoveAt(data.outStatus.Count-1);
                }
            }

            if (data.inStatus.Count > 0)
            {
                for (int i = data.inStatus.Count - 1; i >= 0; i--)
                {
                    DrawPoint(1, data.inStatus[i]);
                    data.inStatus.RemoveAt(i);
                }
            }
            if (data.outStatus.Count > 0)
            {
                for (int i = data.outStatus.Count - 1; i >= 0; i--)
                {
                    DrawPoint(2, data.outStatus[i]);
                    data.outStatus.RemoveAt(i);
                }
            }
        }
        
        //status=1表示进, status=2表示出
        public void DrawPoint(int status, int timeStamp)
        {
            string timeStr = CustomDataFormatter(timeStamp);
            //进
            if (status == 1)
            {
                curInCount++;
                //series0是进的曲线
                _lineChart.AddData(0, 
                    new List<double>(){timeStamp, curInCount},
                    timeStr);
            }
            //出
            else
            {
                curOutCount++;
                //series1是进的曲线
                _lineChart.AddData(1, 
                    new List<double>(){timeStamp, curOutCount}, 
                    timeStr);
            }
            _lineChart.AddData(2, new List<double>(){timeStamp, stockCount}, timeStr);
        }
        
        private string CustomDataFormatter(long timestamp)
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(timestamp);
            string formattedTime = timeSpan.ToString(@"hh\:mm");
            return formattedTime;
        }
    }
}
