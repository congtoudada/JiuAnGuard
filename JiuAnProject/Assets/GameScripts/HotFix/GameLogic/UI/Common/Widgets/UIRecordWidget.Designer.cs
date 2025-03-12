/****************************************************
  文件：UIRecordWidget_Designer.cs
  作者：聪头
  邮箱：1322080797@qq.com
  日期：2024年05月09日 13:36:42
  功能：
*****************************************************/

using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using Cysharp.Threading.Tasks;
using TEngine;
using TMPro;
using UnityEngine.Device;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

namespace GameLogic
{
    public abstract partial class UIRecordWidget
    {
        #region 脚本工具生成的代码
        protected TMP_InputField m_inputKey;
        protected TextMeshProUGUI m_textBeginTime;
        protected TextMeshProUGUI m_textEndTime;
        protected TMP_Dropdown m_dpPos;
        protected TMP_Dropdown m_dpGroup;
        protected TMP_Dropdown m_dpStatus;
        protected TMP_InputField m_inputName;
        protected Button m_btnReset;
        protected Button m_btnSearch;
        protected TextMeshProUGUI m_textNameTitle;
        protected ScrollRect m_scrollRectRecord;
        protected override void ScriptGenerator()
        {
            m_inputKey = FindChildComponent<TMP_InputField>("Top/HorizontalGroup/m_inputKey");
            m_textBeginTime = FindChildComponent<TextMeshProUGUI>("Top/HorizontalGroup/DatePicker - Popup (Shared Calendar)/Input Field Container/Row/Cell/InputField/Text Area/m_textBeginTime");
            m_textEndTime = FindChildComponent<TextMeshProUGUI>("Top/HorizontalGroup/DatePicker - Popup (Shared Calendar) (1)/Input Field Container/Row/Cell/InputField/Text Area/m_textEndTime");
            m_dpPos = FindChildComponent<TMP_Dropdown>("Top/HorizontalGroup/m_dpPos");
            m_dpGroup = FindChildComponent<TMP_Dropdown>("Top/HorizontalGroup/m_dpGroup");
            m_dpStatus = FindChildComponent<TMP_Dropdown>("Top/HorizontalGroup/m_dpStatus");
            m_inputName = FindChildComponent<TMP_InputField>("Top/HorizontalGroup/m_inputName");
            m_btnReset = FindChildComponent<Button>("Top/HorizontalGroup/m_btnReset");
            m_btnSearch = FindChildComponent<Button>("Top/HorizontalGroup/m_btnSearch");
            m_textNameTitle = FindChildComponent<TextMeshProUGUI>("Main/HorizontalTitle/m_textNameTitle");
            m_scrollRectRecord = FindChildComponent<ScrollRect>("Main/m_scrollRectRecord");
            m_btnReset.onClick.AddListener(UniTask.UnityAction(OnClickResetBtn));
            m_btnSearch.onClick.AddListener(UniTask.UnityAction(OnClickSearchBtn));
        }
        #endregion

        #region 事件
        //导出功能
        private async UniTaskVoid OnClickResetBtn()
        {
            // m_inputKey.text = "";
            // m_textBeginTime.text = "";
            // m_textEndTime.text = "";
            // m_dpPos.value = 0;
            // m_dpGroup.value = m_dpGroup.options.FindIndex(data => data.text == UIGlobalDataInstance.Instance.CurrentGroupID);
            // m_dpStatus.value = 0;
            // m_inputName.text = "";
            string folderPath = Path.Join(Application.streamingAssetsPath, EXPORT_DIR)
                .Replace("\\", "/");
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            UITipWindow.Show("导出记录", main_text: "导出目录: " + folderPath + "\n是否确认导出？", confirmCallback:
                async () =>
                {
                    bool ret = await Post(1, true); //搜索只搜索首页
                    if (ret)
                    {
                        //打开目录
                        Process.Start("explorer.exe", folderPath);
                    }
                });
        }
        private async UniTaskVoid OnClickSearchBtn()
        {
            //合法性校验
            if (CheckIsValid())
            {
                bool ret = await Post(1); //搜索只搜索首页
                if (ret)
                {
                    UISimpleTipWindow.Show("搜索成功");
                }
            }
        }

        private bool CheckIsValid()
        {
            //合法性校验
            if (m_textBeginTime.text.Length > 5 && m_textEndTime.text.Length > 5)
            {
                string beginTime = m_textBeginTime.text.Substring(0, 10);
                string endTime = m_textEndTime.text.Substring(0, 10);
                DateTime date1 = DateTime.ParseExact(beginTime, "yyyy-MM-dd", null);
                DateTime date2 = DateTime.ParseExact(endTime, "yyyy-MM-dd", null);
                if (date2 < date1)
                {
                    UISimpleTipWindow.Show("不合法的日期范围");
                    return false;
                }
            }
            return true;
        }
        #endregion
    }
}