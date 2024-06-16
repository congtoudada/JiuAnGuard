/****************************************************
  文件：UICountGroupWidget.cs
  作者：聪头
  邮箱：1322080797@qq.com
  日期：2024年05月02日 16:31:23
  功能：
*****************************************************/

using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using TEngine;

namespace GameLogic
{
    /// <summary>
    /// 统计组
    /// </summary>
    public partial class UICountGroupWidget : UIWidget
    {
        private List<GroupItemWidget> curWidgets = new List<GroupItemWidget>();
        private List<GroupItemWidget> noneWidgets = new List<GroupItemWidget>();
        
        protected override void OnCreate()
        {
            RefeshTitle();
        }

        private void RefeshTitle()
        {
            m_dpGroups.ClearOptions();
            List<string> groups = new List<string>()
            {
                //确保默认统计组始终处于第一个位置
                UIGlobalDataInstance.DEFAULT_GROUP_ID
            };
            List<string> keys = UIGlobalDataInstance.Instance.GroupDict.Keys
                .Where(key => key != UIGlobalDataInstance.DEFAULT_GROUP_ID).ToList();
            groups.AddRange(keys);
            m_dpGroups.AddOptions(groups);
            m_dpGroups.value = groups.FindIndex(key => key == UIGlobalDataInstance.Instance.CurrentGroupID);
            //修改不同统计组
            m_dpGroups.onValueChanged.AddListener(val =>
            {
                UIGlobalDataInstance.Instance.CurrentGroupID = m_dpGroups.options[val].text;
                RefreshContent();
            });
            m_dpGroups.onValueChanged.Invoke(m_dpGroups.value); //手动触发
        }
        
        private void RefreshContent()
        {
            m_scrollRectLeft.Clear();
            m_scrollRectRight.Clear();
            curWidgets.Clear();
            noneWidgets.Clear();
            List<RspCameraInfoDTO> curList = UIGlobalDataInstance.Instance.GetCurrentGroups();
            List<RspCameraInfoDTO> noneList = UIGlobalDataInstance.Instance.GetNoneCurrentGroups();
            foreach (var item in curList)
            {
                var widget = CreateWidgetByPath<GroupItemWidget>(m_scrollRectRight.content, "GroupItemWidget");
                widget.Refresh(item);
                curWidgets.Add(widget);
            }
            foreach (var item in noneList)
            {
                var widget = CreateWidgetByPath<GroupItemWidget>(m_scrollRectLeft.content, "GroupItemWidget");
                widget.Refresh(item);
                noneWidgets.Add(widget);
            }
        }
    }
}