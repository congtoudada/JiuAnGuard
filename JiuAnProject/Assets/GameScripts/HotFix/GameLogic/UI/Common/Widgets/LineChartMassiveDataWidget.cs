using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using E2C;
using UnityEngine;
using UnityEngine.UI;
using TEngine;
using TMPro;

namespace GameLogic
{
    public class LineChartMassiveDataWidget : UIWidget
    {
        #region 脚本工具生成的代码
        private GameObject m_goE2Chart;
        private TextMeshProUGUI m_textCurrentGroup;
        private TextMeshProUGUI m_textStock;
        private TextMeshProUGUI m_textIn;
        private TextMeshProUGUI m_textOut;
        protected override void ScriptGenerator()
        {
            m_goE2Chart = FindChild("Scroll View/Viewport/Content/Chart Area/m_goE2Chart").gameObject;
            m_textCurrentGroup = FindChildComponent<TextMeshProUGUI>("Scroll View/Viewport/Content/Description/Elem/m_textCurrentGroup");
            m_textStock = FindChildComponent<TextMeshProUGUI>("Scroll View/Viewport/Content/Description/Elem (1)/m_textStock");
            m_textIn = FindChildComponent<TextMeshProUGUI>("Scroll View/Viewport/Content/Description/Elem (2)/m_textIn");
            m_textOut = FindChildComponent<TextMeshProUGUI>("Scroll View/Viewport/Content/Description/Elem (3)/m_textOut");
        }
        #endregion

        private E2Chart _chart;
        private int currentGroup;
        private int inCount;
        private int outCount;
        private int stockCount => inCount - outCount;
        protected override void BindMemberProperty()
        {
            _chart = m_goE2Chart.GetComponent<E2Chart>();
            var stockSeries = new E2ChartData.Series();
            var inSeries = new E2ChartData.Series();
            var outSeries = new E2ChartData.Series();

            stockSeries.name = "保有量";
            inSeries.name = "进入";
            outSeries.name = "离开";
            
            _chart.chartData.series.Clear();
            _chart.chartData.series.Add(stockSeries);
            _chart.chartData.series.Add(inSeries);
            _chart.chartData.series.Add(outSeries);

            foreach (var item in _chart.chartData.series)
            {
                item.dateTimeString = new List<string>() { "02:00" };
                item.dataY = new List<float>() { 0 };
            }
        }
        
        public void Refresh(int currentGroup, int stockCount, int inCount, int outCount)
        {
            this.currentGroup = currentGroup;
            this.inCount = inCount;
            this.outCount = outCount;
            InternalRefresh();
        }

        private void InternalRefresh()
        {
            m_textCurrentGroup.text = currentGroup.ToString();
            m_textStock.text = stockCount.ToString();
            m_textIn.text = inCount.ToString();
            m_textOut.text = outCount.ToString();
        }
        
        //1:进 2:出
        public void DrawPoint(int status, string timeStamp, int count)
        {
            //进
            if (status == 1)
            {
                inCount = count;
                _chart.chartData.series[1].dateTimeString.Add(timeStamp);
                _chart.chartData.series[1].dataY.Add(count);
            }
            //出
            else
            {
                outCount = count;
                _chart.chartData.series[2].dateTimeString.Add(timeStamp);
                _chart.chartData.series[2].dataY.Add(count);
            }
            _chart.chartData.series[0].dateTimeString.Add(timeStamp);
            _chart.chartData.series[0].dataY.Add(stockCount);
            InternalRefresh();
            _chart.UpdateChart();
        }
    }
}