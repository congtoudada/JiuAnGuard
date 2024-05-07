/****************************************************
  文件：UICountRuntimeWidget.cs
  作者：聪头
  邮箱：1322080797@qq.com
  日期：2024年04月29日 18:27:52
  功能：
*****************************************************/
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
    private LineChartMassiveDataWidget _chartWidget;
    protected override void BindMemberProperty()
    {
      _chartWidget = CreateWidget<LineChartMassiveDataWidget>("LineChartMassiveDataWidget");
      _chartWidget.Refresh(1, 0, 0, 0);
      
      MockData().Forget();
    }

    private async UniTaskVoid MockData()
    {
      _chartWidget.DrawPoint(1, "16:05", 1);
      // await UniTask.WaitForSeconds(1f);
      _chartWidget.DrawPoint(1, "17:05", 2);
      // await UniTask.WaitForSeconds(1f);
      _chartWidget.DrawPoint(2, "18:05", 1);
      // await UniTask.WaitForSeconds(1f);
      _chartWidget.DrawPoint(1, "18:35", 30);
      // await UniTask.WaitForSeconds(1f);
      _chartWidget.DrawPoint(2, "20:05", 22);
    }
  }
}
