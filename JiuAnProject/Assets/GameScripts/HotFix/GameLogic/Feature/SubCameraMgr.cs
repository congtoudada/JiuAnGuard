using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using TEngine;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameLogic
{
    public class SubCameraMgr : MonoBehaviour
    {
        [Serializable]
        public class SubConfig
        {
            public List<string> IPList;

            public SubConfig()
            {
                IPList = new List<string>();
            }
        }
        
        private VLCPlayerExample _vlcPlayer;
        private MeshRenderer _renderer;
        private ProjectorURP _projector;
        private Wander _wander;
        private Coroutine _coroutine;
        //摄像头路径相关
        private static string CONFIG_FILENAME = "SubCamera.json";
        private static SubConfig _config;
        
        [Header("投影可视范围")]
        public float visibleRange = 5f;
        [Header("完全显示阈值")] public float visibleThresh = 0.7f;
        [ReadOnly]
        [Header("配置项索引 (0为测试路径)"), Tooltip("如果存在配置文件，会解析")] 
        public int configIdx = 0;

        private void Awake()
        {
            CONFIG_FILENAME = SceneManager.GetActiveScene().name + ".json";
            if (_config == null)
            {
                string path = Path.Combine(Application.streamingAssetsPath, CONFIG_FILENAME);
                //存在配置
                if (File.Exists(path))
                {
                    string content = File.ReadAllText(path);
                    // _config = JsonConvert.DeserializeObject<SubConfig>(content);
                    _config = JsonUtility.FromJson<SubConfig>(content);
                }
                //不存在配置
                else
                {
                    _config = new SubConfig();
                    _config.IPList.Add("file://E:/Practice/Unity/Demo/DUT/JiuAnGuard/JiuAnProject/Assets/AssetRaw/Texture/bandicam 2023-03-30 14-28-39-697.mp4");
                    _config.IPList.Add("rtsp://admin:zwjs1357@192.168.1.111:554/h264/ch1/main/av_stream_count1");
                    _config.IPList.Add("rtsp://admin:zwjs1357@192.168.1.112:554/h264/ch1/main/av_stream_count2");
                    _config.IPList.Add("rtsp://admin:zwjs123456@192.168.1.113:554/h264/ch1/main/av_stream_face1");
                    _config.IPList.Add("rtsp://admin:zwjs1357@192.168.1.114:554/h264/ch1/main/av_stream_face2");
                    _config.IPList.Add("rtsp://admin:zwjs1357@192.168.1.115:554/h264/ch1/main/av_stream_face3");
                    _config.IPList.Add("rtsp://admin:zwjs1357@192.168.1.116:554/h264/ch1/main/av_stream_face4");
                    _config.IPList.Add("rtsp://admin:zwjs1357@192.168.1.117:554/h264/ch1/main/av_stream_intrude1");
                    _config.IPList.Add("rtsp://admin:zwjs1357@192.168.1.118:554/h264/ch1/main/av_stream_card1");
                    _config.IPList.Add("rtsp://admin:zwjs1357@192.168.1.119:554/h264/ch1/main/av_stream_right1");
                    _config.IPList.Add("rtsp://admin:zwjs1357@192.168.1.120:554/h264/ch1/main/av_stream_left1");
                    // string content = JsonConvert.SerializeObject(_config, Formatting.None);
                    string content = JsonUtility.ToJson(_config, true);
                    File.WriteAllText(path, content);
                }
            }

            if (_config != null)
            {
                var parent = transform.parent;
                configIdx = parent.GetSiblingIndex() + 1;  //获取实际下标
// #if !UNITY_EDITOR
//                 configIdx = transform.parent.GetSiblingIndex() + 1;
// #else
//                 configIdx = 0;
//                 // Debug.Log($"{transform.parent.name} SiblingIndex + 1: {transform.parent.GetSiblingIndex() + 1}");
// #endif
                VLCPlayerExample vlc = GetComponent<VLCPlayerExample>();
                VLCPlayerExample vlcPreview = parent.Find("Billboard/Billboard_VPlayer").GetComponent<VLCPlayerExample>();
                if (vlc)
                {
                    if (_config.IPList.Count == 0)
                    {
                        Debug.LogWarning($"配置不存在，请检查{CONFIG_FILENAME}配置");
                        return;
                    }
                    if (configIdx < 0 || configIdx >= _config.IPList.Count)
                    {
                        //调试模式运行次相机
                        Debug.Log($"只有一项配置，默认以调试模式运行！");
                        configIdx = 0;
                    }
                    vlc.path = _config.IPList[configIdx]; //vlc self
                    if (vlcPreview)
                    {
                        vlcPreview.path = _config.IPList[configIdx]; //vlc preview
                    }
                    Log.Info("次相机配置读取成功: " + _config.IPList[configIdx]);
                }
            }
            else
            {
                Debug.LogWarning("获取摄像头本地配置失败，次相机无法通过配置文件投屏");
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            _vlcPlayer = GetComponent<VLCPlayerExample>();
            if (_vlcPlayer == null)
            {
                Debug.LogError("请先挂载VLCPlayerExample!");
                return;
            }
            _renderer = GetComponent<MeshRenderer>();
            _projector = GetComponent<ProjectorURP>();
            _wander = GameObject.FindWithTag("LevelManager").GetComponent<Wander>();
            // _wander = GameModule.Base.gameObject.GetOrAddComponent<Wander>();
            Hide();
        }
        
        [Button]
        public void Show()
        {
            _vlcPlayer.Open();
            _renderer.enabled = true;
        }
        
        [Button]
        public void Hide()
        {
            _vlcPlayer.Stop();
            _renderer.enabled = false;
        }

        public void LaunchCheckHide()
        {
            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
            }
            _coroutine = StartCoroutine(CheckHideEnumerator());
        }
        
        private IEnumerator CheckHideEnumerator()
        {
            while (_wander.bScript.subMgr == this)
            {
                float sqrLength = Vector3.SqrMagnitude(transform.position - Camera.main.transform.position);
                float k = 1 - sqrLength / (visibleRange * visibleRange);
                float lenLerp = Mathf.SmoothStep(0f, 1f, k);
                if (lenLerp > visibleThresh) lenLerp = 1.0f;
                _projector.alpha = Mathf.Clamp01(lenLerp);
                // Debug.Log(lenLerp);
                // if (sqrLength > visibleRange * visibleRange * 0.5)
                // {
                //     float lenLerp = Mathf.Lerp(0.75f, 0, sqrLength / visibleRange * visibleRange);
                //     _projector.alpha = lenLerp;
                // }
                // else
                // {
                //     _projector.alpha = 1.0f;
                // }
                yield return null;
            }
            Hide();
        }
    }
}
