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

        // private static Material lineMat = null;
        // private static Color lineColor = Color.yellow;
        // private GameObject lineGroup = null;
        // private static readonly int BaseColor = Shader.PropertyToID("_BaseColor");

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
            // if (transform.childCount > 0)
            // {
            //     lineGroup = transform.GetChild(0).gameObject;
            // }
            Hide();
        }
        
        [Button]
        public void Show()
        {
            _vlcPlayer.Open();
            // lineGroup?.SetActive(true);
            _renderer.enabled = true;
        }
        
        [Button]
        public void Hide()
        {
            _vlcPlayer.Stop();
            // lineGroup?.SetActive(false);
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
            while (_wander.bScript != null && _wander.bScript.subMgr == this)
            {
                // if (lineMat == null)
                // {
                //     lineMat = GameModule.Resource.LoadAsset<Material>("LineMat");
                //     lineColor = lineMat.GetColor(BaseColor);
                // }
                float sqrLength = Vector3.SqrMagnitude(transform.position - Camera.main.transform.position);
                float k = 1 - sqrLength / (visibleRange * visibleRange); //越近越大，越远越小
                k = Mathf.Clamp01(k);
                // Debug.Log("k: " + k);
                float lenLerp = Mathf.SmoothStep(0f, 1f, k);
                if (lenLerp > visibleThresh) lenLerp = 1.0f;
                _projector.alpha = Mathf.Clamp01(lenLerp);
                
                // float lineAlpha = Mathf.Lerp(1.0f, 0.2f, k);
                // Color nowCol = lineColor;
                // nowCol.a = lineAlpha;
                // lineMat.SetColor(BaseColor, nowCol);
                
                yield return null;
            }
            Hide();
        }

        // private void OnDestroy()
        // {
        //     if (lineMat != null)
        //     {
        //         Color col = lineColor;
        //         col.a = 0.8f;
        //         lineMat.SetColor(BaseColor, col);
        //         lineMat = null;
        //     }
        // }
    }
}
