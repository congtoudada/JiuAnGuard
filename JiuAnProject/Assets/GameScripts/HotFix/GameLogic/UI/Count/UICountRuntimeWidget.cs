/****************************************************
  文件：UICountRuntimeWidget.cs
  作者：聪头
  邮箱：1322080797@qq.com
  日期：2024年04月29日 18:27:52
  功能：
*****************************************************/

using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;
using TEngine;
using TMPro;

namespace GameLogic
{
  /// <summary>
  /// 实时计数
  /// </summary>
  class UICountRuntimeWidget : UIWidget
  {
    private CountLineChartWidget _chartWidget;
    private static int _runtimeTaskId = -1;
    private float _reqRuntimeCountInterval = 1f;
    private int _inCount = -1;
    private int _outCount = -1;
    private string candidate;
    private Button m_btnPause;
    private bool isPause = false;
    protected override void BindMemberProperty()
    {
      isPause = false;
      m_btnPause = FindChildComponent<Button>("m_btnPause");
      m_btnPause.onClick.AddListener(() =>
      {
        if (isPause)
        {
          BeginReq();
          m_btnPause.transform.GetComponentInChildren<TextMeshProUGUI>().text = "暂停";
        }
        else
        {
          CloseReq();
          m_btnPause.transform.GetComponentInChildren<TextMeshProUGUI>().text = "继续";
        }
        isPause = !isPause;
      });
#if UNITY_EDITOR
      GameModule.Setting.RemoveSetting(nameof(_reqRuntimeCountInterval));
#endif
      _reqRuntimeCountInterval = GameModule.Setting.GetFloat(nameof(_reqRuntimeCountInterval), _reqRuntimeCountInterval);
      List<int> camList = UIGlobalDataInstance.Instance.GroupDict[UIGlobalDataInstance.Instance.CurrentGroupID];
      candidate = string.Join(",", camList);
      _chartWidget = CreateWidget<CountLineChartWidget>("CountLineChartWidget");
      // _chartWidget.Init(data);
      // MockData().Forget();
      _chartWidget.LineChart.ClearData();
      if (_runtimeTaskId == -1)
      {
        BeginReq();
      }
      ReqRuntimeCount(null); //立即请求一次数据
    }

    private void BeginReq()
    {
      _runtimeTaskId = GameModule.Timer.AddTimer(ReqRuntimeCount,
        _reqRuntimeCountInterval, true, true);
    }

    private void CloseReq()
    {
      GameModule.Timer.RemoveTimer(_runtimeTaskId);
    }

    private async void ReqRuntimeCount(object[] args)
    {
      string rsp = await Utility.Http.Get(WebURL.GetFullURL($"runtime_count?inCount={_inCount}&outCount={_outCount}&candidate={candidate}"));
      if (string.IsNullOrEmpty(rsp))
      {
        return;
      }
      var dto = JsonConvert.DeserializeObject<RspRuntimeCountDTO>(rsp);
      if (dto != null)
      {
        Log.Debug($"dto in: {dto.inCount} out: {dto.outCount}");
        if (_inCount != dto.inCount)
        {
          _chartWidget.DrawPoint(CountLineChartWidget.CountLineType.In, dto.timestamp, dto.inCount);
          _inCount = dto.inCount;
        }
        if (_outCount != dto.outCount)
        {
          _chartWidget.DrawPoint(CountLineChartWidget.CountLineType.Out, dto.timestamp, dto.outCount);
          _outCount = dto.outCount;
        }
        _chartWidget.DrawPoint(CountLineChartWidget.CountLineType.Stock, dto.timestamp, _inCount - _outCount);
      }
      else
      {
        Log.Warning("RspRuntimeCount is null!");
      }
    }

    protected override void OnDestroy()
    {
      // CloseReq();
      GameModule.Setting.SetFloat(nameof(_reqRuntimeCountInterval), _reqRuntimeCountInterval);
    }

    // private async UniTaskVoid MockData()
    // {
    //   _chartWidget.DrawPoint(1, 8000);
    //   await UniTask.WaitForSeconds(1f);
    //   _chartWidget.DrawPoint(1, 16000);
    //   await UniTask.WaitForSeconds(1f);
    //   _chartWidget.DrawPoint(2, 24000);
    //   await UniTask.WaitForSeconds(1f);
    //   _chartWidget.DrawPoint(1, 36000);
    //   await UniTask.WaitForSeconds(1f);
    //   _chartWidget.DrawPoint(2, 88000);
    // }
  }
}
