/****************************************************
  文件：UIGlobalDataInstance.cs
  作者：聪头
  邮箱：1322080797@qq.com
  日期：2024年05月09日 10:47:22
  功能：
*****************************************************/

using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using GameBase;
using Newtonsoft.Json;
using TEngine;
using UnityEngine;

namespace GameLogic
{
    public class UIGlobalDataInstance : Singleton<UIGlobalDataInstance>
    {
        private const string KEY_CURRENT_GROUP_ID = nameof(KEY_CURRENT_GROUP_ID);
        private const string KEY_GROUP_DICT = nameof(KEY_GROUP_DICT);
        
        /// <summary>
        /// 摄像头参数列表
        /// Key: camera_id Value:Camera Info
        /// </summary>
        private Dictionary<int, RspCameraInfoDTO> _CameraInfoDict = new Dictionary<int, RspCameraInfoDTO>();
        
        /// <summary>
        /// 当前统计组
        /// </summary>
        public const string DEFAULT_GROUP_ID = "默认统计组";
        private string _CurrentGroupID = DEFAULT_GROUP_ID;
        /// <summary>
        /// 统计组规则（默认统计组会在收到camera_list请求后自动更新）
        /// key: groupID value: camera_id list
        /// </summary>
        private Dictionary<string, List<int>> _GroupDict = new Dictionary<string, List<int>>();

        public Dictionary<int, RspCameraInfoDTO> CameraInfoDict => _CameraInfoDict;

        public string CurrentGroupID
        {
            get => _CurrentGroupID;
            set => _CurrentGroupID = value;
        }
        public Dictionary<string, List<int>> GroupDict => _GroupDict;

        public UIGlobalDataInstance()
        {
            Load();
        }

        private void Load()
        {
            //从本地读取统计组配置
            _CurrentGroupID = GameModule.Setting.GetString(KEY_CURRENT_GROUP_ID, DEFAULT_GROUP_ID);
            string json = GameModule.Setting.GetString(KEY_GROUP_DICT, "");
            if (json != "")
            {
                //本地旧缓存
                _GroupDict = JsonConvert.DeserializeObject<Dictionary<string, List<int>>>(json);
            }
            
            
        }

        public async UniTask<bool> SendCameraInfoReq()
        {
            string rsp = await Utility.Http.Get(WebURL.GetFullURL("camera_list"));
            //WebURL.Get(WebURL.GetFullURL("camera_list"), content => {Debug.Log("rsp: " + content);}).Forget();
            if (string.IsNullOrEmpty(rsp))
            {
                UISimpleTipWindow.Show("无法连接服务器，请检查网络");
                return false;
            }
            var ret = JsonConvert.DeserializeObject<List<RspCameraInfoDTO>>(rsp);
            Instance.UpdateCameraInfoDict(ret);
            return true;
        }

        public void UpdateCameraInfoDict(List<RspCameraInfoDTO> list)
        {
            _CameraInfoDict.Clear();
            foreach (var item in list)
            {
                _CameraInfoDict.Add(item.id, item);
            }
            UpdateGroup(DEFAULT_GROUP_ID, list.Select(x=>x.id).ToList());
        }

        public List<RspCameraInfoDTO> GetCurrentGroups()
        {
            return _CameraInfoDict
                .Where(pair => _GroupDict[_CurrentGroupID].Contains(pair.Key))
                .Select(pair => pair.Value).ToList();
        }

        public List<RspCameraInfoDTO> GetNoneCurrentGroups()
        {
            return _CameraInfoDict
                .Where(pair => !_GroupDict[_CurrentGroupID].Contains(pair.Key))
                .Select(pair => pair.Value).ToList();
        }

        public void UpdateGroup(string groupName, List<int> newGroup)
        {
            _GroupDict[groupName] = newGroup;
        }

        public bool RemoveGroup(string groupName)
        {
            if (_GroupDict.ContainsKey(groupName))
            {
                _GroupDict.Remove(groupName);
                return true;
            }

            return false;
        }
        
        public void Save()
        {
            GameModule.Setting.SetString(KEY_CURRENT_GROUP_ID, _CurrentGroupID);
            string json = JsonConvert.SerializeObject(_GroupDict);
            GameModule.Setting.SetString(KEY_GROUP_DICT, json);
            GameModule.Setting.Save();
        }
    }
}