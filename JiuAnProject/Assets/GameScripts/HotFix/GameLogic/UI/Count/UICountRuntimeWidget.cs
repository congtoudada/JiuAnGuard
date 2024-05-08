/****************************************************
  文件：UICountRuntimeWidget.cs
  作者：聪头
  邮箱：1322080797@qq.com
  日期：2024年04月29日 18:27:52
  功能：
*****************************************************/

using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TEngine;

namespace GameLogic
{
  /// <summary>
  /// 实时计数
  /// </summary>
  class UICountRuntimeWidget : UIWidget
  {
    private CountLineChartWidget _chartWidget;
    protected override void BindMemberProperty()
    {
      // CountLineChartWidget.WidgetData data = new CountLineChartWidget.WidgetData();
      // data.groupID = 1;
      // data.inStatus = new List<int>();
      // data.outStatus = new List<int>();
      _chartWidget = CreateWidget<CountLineChartWidget>("CountLineChartWidget");
      // _chartWidget.Init(data);
      // MockData().Forget();
    }

    
    
    private async UniTaskVoid MockData()
    {
      _chartWidget.DrawPoint(1, 8000);
      await UniTask.WaitForSeconds(1f);
      _chartWidget.DrawPoint(1, 16000);
      await UniTask.WaitForSeconds(1f);
      _chartWidget.DrawPoint(2, 24000);
      await UniTask.WaitForSeconds(1f);
      _chartWidget.DrawPoint(1, 36000);
      await UniTask.WaitForSeconds(1f);
      _chartWidget.DrawPoint(2, 88000);
    }
  }
}
