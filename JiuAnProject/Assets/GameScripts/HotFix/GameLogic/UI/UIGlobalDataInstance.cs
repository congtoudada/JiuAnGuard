/****************************************************
  文件：UIGlobalDataInstance.cs
  作者：聪头
  邮箱：1322080797@qq.com
  日期：2024年05月09日 10:47:22
  功能：
*****************************************************/

using System;
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
        private const string KEY_IS_PREVIEW = nameof(KEY_IS_PREVIEW);
        
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
        
        //-------------------- 非序列化 --------------------
        public bool IsPreview = false; //是否预览所有摄像头
        public Action<bool> OnPreviewChanged;
        //当前统计组下进入、离开和保有量
        public int InCount = 0;
        public int OutCount = 0;
        public int RemainCount = 0;
        //①:进入 ②:离开
        public Action<int, int, int> OnCountChanged; //计数结果改变
        public Action<string> OnGroupIDChanged; //统计组改变
        //-------------------- 序列化 --------------------
        public string CurrentGroupID
        {
            get => _CurrentGroupID;
            set
            {
                if (_CurrentGroupID != value)
                {
                    _CurrentGroupID = value;
                    OnGroupIDChanged?.Invoke(value);
                }
            }
        }
        public Dictionary<string, List<int>> GroupDict => _GroupDict;

        public UIGlobalDataInstance()
        {
            Load();
            OnCountChanged = null;
            OnCountChanged += (inCount, outCount, remainCount) =>
            {
                InCount = inCount;
                OutCount = outCount;
                RemainCount = remainCount;
            };
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
                UITipWindow.Show("网络异常", main_text: "无法连接服务器，请检查网络或后端服务是否正常！");
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
            return GetGroups(_CurrentGroupID);
        }

        public List<RspCameraInfoDTO> GetNoneCurrentGroups()
        {
            return GetNoneGroups(_CurrentGroupID);
        }
        
        public List<RspCameraInfoDTO> GetGroups(string groupID)
        {
            if (_GroupDict.ContainsKey(groupID))
            {
                return _CameraInfoDict
                    .Where(pair => _GroupDict[groupID].Contains(pair.Key))
                    .Select(pair => pair.Value).ToList();
            }
            else
            {
                Debug.LogError("无效统计组: " + groupID);
                return new List<RspCameraInfoDTO>();
            }
        }

        public List<RspCameraInfoDTO> GetNoneGroups(string groupID)
        {
            if (_GroupDict.ContainsKey(groupID))
            {
                return _CameraInfoDict
                    .Where(pair => !_GroupDict[groupID].Contains(pair.Key))
                    .Select(pair => pair.Value).ToList();
            }
            else
            {
                Debug.LogError("无效统计组: " + groupID);
                return new List<RspCameraInfoDTO>();
            }
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