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
using TEngine;

namespace GameLogic
{
  public partial class UICountRecordWidget : UIRecordWidget
  {
    protected override PageType pageType => PageType.Count;

    protected override void OnCreate()
    {
      base.OnCreate();
      m_inputName.gameObject.SetActive(false); //关闭姓名查询
      m_textNameTitle.gameObject.SetActive(false); //关闭姓名标题
      
      //填充状态
      List<string> statusList = new List<string>()
      {
        "全部方向",
        "进",
        "出"
      };
      m_dpStatus.ClearOptions();
      m_dpStatus.AddOptions(statusList);
      m_dpStatus.value = 0;
    }

    protected override string GetStatusStr(int status)
    {
      switch (status)
      {
        case 1:
          return "进";
        case 2:
          return "出";
      }
      return "非法";
    }

    protected override RecordItemWidget.RenderTypeEnum GetRenderType(RspRecordItemDTO item)
    {
      if (item.status == 1) return RecordItemWidget.RenderTypeEnum.Red;
      else return RecordItemWidget.RenderTypeEnum.Green;
    }
  }
}
