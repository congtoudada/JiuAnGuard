/****************************************************
  文件：UICountGroupWidget.cs
  作者：聪头
  邮箱：1322080797@qq.com
  日期：2024年05月02日 16:31:23
  功能：
*****************************************************/

using System.Collections.Generic;
using Newtonsoft.Json;
using TEngine;

namespace GameLogic
{
    /// <summary>
    /// 统计组
    /// </summary>
    public class UICountGroupWidget : UIWidget
    {
        public const string KEY_CURRENT_GROUP_ID = nameof(KEY_CURRENT_GROUP_ID);
        public const string KEY_GROUP_DICT = nameof(KEY_GROUP_DICT);
        //统计组1表示默认所有
        public int CurrentGroupID = 1;
        //key: groupID value: camera_id list
        public Dictionary<int, List<int>> GroupDict = new Dictionary<int, List<int>>();

        protected override void OnCreate()
        {
            //从本地读取统计组配置
            CurrentGroupID = GameModule.Setting.GetInt(KEY_CURRENT_GROUP_ID, 1);
            string json = GameModule.Setting.GetString(KEY_GROUP_DICT, "");
            if (json != "")
            {
                GroupDict = JsonConvert.DeserializeObject<Dictionary<int, List<int>>>(json);
            }
        }

        public (int, Dictionary<int, List<int>>) GetInfo()
        {
            return (CurrentGroupID, GroupDict);
        }

        public void SaveInfo()
        {
            GameModule.Setting.SetInt(KEY_CURRENT_GROUP_ID, CurrentGroupID);
            string json = JsonConvert.SerializeObject(GroupDict);
            GameModule.Setting.SetString(KEY_GROUP_DICT, json);
            GameModule.Setting.Save();
        }
    }
}