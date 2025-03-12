/****************************************************
  文件：UIRecordWidget.cs
  作者：聪头
  邮箱：1322080797@qq.com
  日期：2024年05月09日 13:34:59
  功能：
*****************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using TEngine;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Application = UnityEngine.Device.Application;

namespace GameLogic
{
    public abstract partial class UIRecordWidget : UIWidget
    {
      private RecordItemWidgetMgr _recordMgr;
      private int page = 1; //当前页数
      private bool isRequest = false;
      private const string EXPORT_DIR = "outputs";

      protected virtual PageType pageType { get; } = PageType.Count;
      
      protected override void OnCreate()
      {
        //为自身绑定RecordItemWidgetMgr脚本
        _recordMgr = CreateWidget<RecordItemWidgetMgr>(gameObject);
        _recordMgr.Init(pageType);
        _recordMgr.RegisterTurnPageEvent(SendReq); //注册翻页事件
        
        //填充抓拍点数据
        List<string> posList = new List<string>()
        {
          "全部抓拍点"
        };
        posList.AddRange(UIGlobalDataInstance.Instance.CameraInfoDict.Values.Select(x => x.pos).ToList());
        m_dpPos.ClearOptions();
        m_dpPos.AddOptions(posList);
        
        //填充统计组数据
        List<string> groupList = new List<string>();
        // {
        //   UIGlobalDataInstance.DEFAULT_GROUP_ID //默认统计组
        // };
        groupList.AddRange(UIGlobalDataInstance.Instance.GroupDict.Keys);
        m_dpGroup.ClearOptions();
        m_dpGroup.AddOptions(groupList);
        m_dpGroup.value = groupList.FindIndex(key => key == UIGlobalDataInstance.Instance.CurrentGroupID);
        
        // OnClickResetBtn().Forget();
        m_inputKey.text = "";
        m_textBeginTime.text = "";
        m_textEndTime.text = "";
        m_dpPos.value = 0;
        m_dpGroup.value = m_dpGroup.options.FindIndex(data => data.text == UIGlobalDataInstance.Instance.CurrentGroupID);
        m_dpStatus.value = 0;
        m_inputName.text = "";
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
        // Debug.Log("Req CurrentPage: " + page);
        Post(page).Forget();
      }

      private async UniTask<bool> Post(int reqPage, bool bIsExport=false)
      {
        if (isRequest)
          return false;
        //准备请求体
        isRequest = true;
        Log.Info("发送异步CountList请求");
        WWWForm form = new WWWForm();
        ReqRecordListDTO query = new ReqRecordListDTO();
        query.page = reqPage;
        query.limit = RecordItemWidgetMgr.PAGE_MAX;
        query.pageType = (int) pageType;
        if (!string.IsNullOrEmpty(m_inputKey.text))
        {
          query.recordId = Convert.ToInt64(m_inputKey.text);
        }
        if (m_textBeginTime.text.Length > 2) //处理ZWSP情况（空字符，但实际会跑到非空判断内）
        {
          query.beginTime = m_textBeginTime.text;
        }
        else
        {
          query.beginTime = "1970-01-01";
          // 转换成 "yyyy-MM-dd" 格式的字符串
          // query.beginTime = DateTime.Today.ToString("yyyy-MM-dd");
        }
        if (m_textEndTime.text.Length > 2)
        {
          query.endTime = m_textEndTime.text;
        }
        else
        {
          query.endTime = "2999-01-01";
        }
        string pos = m_dpPos.options[m_dpPos.value].text;
        if (!string.IsNullOrEmpty(pos))
        {
          query.pos = pos;
        }
        query.status = m_dpStatus.value;
        if (!string.IsNullOrEmpty(m_inputName.text))
        {
          query.name = m_inputName.text;
        }
        query.candidate = UIGlobalDataInstance.Instance.GroupDict[m_dpGroup.options[m_dpGroup.value].text];
        query.sort = 2; //默认按编号升序
        if (bIsExport)
        {
          query.directory = Path.Combine(Application.streamingAssetsPath, EXPORT_DIR)
            .Replace("\\", "/");
        }
        Log.Info(query);
        string json = JsonConvert.SerializeObject(query);
        form.AddField("DTO", json);
        //请求服务器
        if (bIsExport)
        {
          string rsp = await Utility.Http.Post(WebURL.GetFullURL("export"), form);
          isRequest = false;
          if (string.IsNullOrEmpty(rsp))
          {
            UISimpleTipWindow.Show("导出失败，没有收到服务器响应！");
            return false;
          }
          UISimpleTipWindow.Show("导出成功！文件名: " + Path.GetFileName(rsp), 3.0f);
        }
        else
        {
          string rsp = await Utility.Http.Post(WebURL.GetFullURL("record"), form);
          isRequest = false;
          if (string.IsNullOrEmpty(rsp))
          {
            UISimpleTipWindow.Show("没有收到服务器响应！");
            return false;
          }
          //获取响应
          RspRecordDTO resultDTO = JsonConvert.DeserializeObject<RspRecordDTO>(rsp);
          page = resultDTO.page;
          _recordMgr.Clear(); //清空旧数据和选中信息
          for (int i = 0; i < resultDTO.list.Count; i++)
          {
            var item = resultDTO.list[i];
            _recordMgr.SetRecordItem(item.recordId, item.recordTime, item.name, 
              item.pos, GetStatusStr(item.status), item.shotImg, item.warnScore,
              GetRenderType(item));
          }
          _recordMgr.RefreshPageWidget(resultDTO.totalCount, resultDTO.page);
        }
        return true;
      }

      protected abstract string GetStatusStr(int status);

      protected virtual RecordItemWidget.RenderTypeEnum GetRenderType(RspRecordItemDTO item)
      {
        if (item.warnScore > UIConstant.WarnThreshold || item.warnScore < 0.01)
        {
          return RecordItemWidget.RenderTypeEnum.Red;
        }
        else
        {
          return RecordItemWidget.RenderTypeEnum.Green;
        }
      }
    }
}