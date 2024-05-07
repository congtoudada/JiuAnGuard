/****************************************************
  文件：UICountRecordWidget.cs
  作者：聪头
  邮箱：1322080797@qq.com
  日期：2024年05月02日 16:29:43
  功能：
*****************************************************/

using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;
using TEngine;
using TMPro;

namespace GameLogic
{
  class UICountRecordWidget : UIWidget
  {
    #region 脚本工具生成的代码
    private TMP_InputField m_inputKey;
    private TextMeshProUGUI m_textBeginTime;
    private TextMeshProUGUI m_textEndTime;
    private TMP_Dropdown m_dpPos;
    private TMP_Dropdown m_dpDirection;
    private Button m_btnReset;
    private Button m_btnSearch;
    private ScrollRect m_scrollRectRecord;
    protected override void ScriptGenerator()
    {
      m_inputKey = FindChildComponent<TMP_InputField>("Top/HorizontalGroup/m_inputKey");
      m_textBeginTime = FindChildComponent<TextMeshProUGUI>("Top/HorizontalGroup/DatePicker - Popup (Shared Calendar)/Input Field Container/Row/Cell/InputField/Text Area/m_textBeginTime");
      m_textEndTime = FindChildComponent<TextMeshProUGUI>("Top/HorizontalGroup/DatePicker - Popup (Shared Calendar) (1)/Input Field Container/Row/Cell/InputField/Text Area/m_textEndTime");
      m_dpPos = FindChildComponent<TMP_Dropdown>("Top/HorizontalGroup/m_dpPos");
      m_dpDirection = FindChildComponent<TMP_Dropdown>("Top/HorizontalGroup/m_dpDirection");
      m_btnReset = FindChildComponent<Button>("Top/HorizontalGroup/m_btnReset");
      m_btnSearch = FindChildComponent<Button>("Top/HorizontalGroup/m_btnSearch");
      m_scrollRectRecord = FindChildComponent<ScrollRect>("Main/m_scrollRectRecord");
      m_btnReset.onClick.AddListener(UniTask.UnityAction(OnClickResetBtn));
      m_btnSearch.onClick.AddListener(UniTask.UnityAction(OnClickSearchBtn));
    }
    #endregion

    #region 事件
    private async UniTaskVoid OnClickResetBtn()
    {
      m_inputKey.text = "";
      m_textBeginTime.text = "";
      m_textEndTime.text = "";
      m_dpPos.value = 0;
      m_dpDirection.value = 0;

    }
    private async UniTaskVoid OnClickSearchBtn()
    {
      Post(page).Forget();
    }
    #endregion
    
    private RecordItemWidgetMgr _recordMgr;
    private int page = 1; //当前页数
    private bool isRequest = false;
    protected override void BindMemberProperty()
    {
      //为自身绑定RecordItemWidgetMgr脚本
      _recordMgr = CreateWidget<RecordItemWidgetMgr>(gameObject);
      _recordMgr.Init(PageType.Count);
    }

    protected override void OnSetVisible(bool visible)
    {
      //请求数据
      if (visible)
      {
        SendReq(page);
      }
    }

    public void SendReq(int page)
    {
      Debug.Log("Req CurrentPage: " + page);
      Post(page).Forget();
    }

    private async UniTaskVoid Post(int reqPage)
    {
      if (isRequest) return;
      _recordMgr.Clear(); //清空旧数据和选中信息
      isRequest = true;
      Log.Info("发送异步CountList请求");
      WWWForm form = new WWWForm();
      ReqRecordList query = new ReqRecordList();
      query.page = reqPage;
      query.limit = RecordItemWidgetMgr.PAGE_MAX;
      query.type = PageType.Count;
      if (!string.IsNullOrEmpty(m_inputKey.text))
      {
        query.recordId = Convert.ToInt64(m_inputKey.text);
      }
      if (!string.IsNullOrEmpty(m_textBeginTime.text))
      {
        query.beginTime = m_textBeginTime.text;
      }
      if (!string.IsNullOrEmpty(m_textEndTime.text))
      {
        query.endTime = m_textEndTime.text;
      }
      string pos = m_dpPos.options[m_dpPos.value].text;
      if (!string.IsNullOrEmpty(pos))
      {
        query.pos = pos;
      }
      string dir = m_dpDirection.options[m_dpDirection.value].text;
      if (!string.IsNullOrEmpty(dir))
      {
        query.direction = dir=="进"? 1:2;
      }
      query.sort = 0; //默认按编号升序
      string json = JsonConvert.SerializeObject(query);
      form.AddField("reqQuery", json);
      
      string rsp = await Utility.Http.Post(WebURL.GetFullURL("record_list"), form);
      if (string.IsNullOrEmpty(rsp))
      {
        Log.Warning("没有收到record响应！");
        return;
      }
      
      RspRecordDTO resultDTO = JsonConvert.DeserializeObject<RspRecordDTO>(rsp);
      page = resultDTO.page;
      for (int i = 0; i < resultDTO.list.Count; i++)
      {
        var item = resultDTO.list[i];
        _recordMgr.SetRecordItem(item.key, item.recordTime, item.name, item.pos, item.status==1?"进":"出", item.shot_img);
      }
      _recordMgr.RefreshPageWidget(resultDTO.totalCount, resultDTO.page);
      _recordMgr.RegisterTurnPageEvent(SendReq); //注册翻页事件
      isRequest = false;
    }
  }
}
