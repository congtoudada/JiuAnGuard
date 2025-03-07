using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using GameMain;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

namespace GameLogic
{
#if UNITY_EDITOR
    [ExecuteInEditMode]
#endif
    [RequireComponent(typeof(Camera))]
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class ProjectorURP : MonoBehaviour
    {
        [Title("Basic Params")]
        //U向剔除的最小和最大值
        [Range(0,1f)]
        [Tooltip("默认0。将U方向小于该值的像素剔除")]
        public float uMin = 0f;
        [Range(0, 1f)]
        [Tooltip("默认1。将U方向大于该值的像素剔除")]
        public float uMax = 1f;
        //V向剔除的最小和最大值
        [Range(0, 1f)]
        [Tooltip("默认0。将V方向小于该值的像素剔除")]
        public float vMin = 0f;
        [Range(0, 1f)]
        [Tooltip("默认1。将V方向大于该值的像素剔除")]
        public float vMax = 1f;
        [Range(0, 1f)]
        [Tooltip("默认1。1代表完全不透明，0代表完全透明")]
        public float alpha = 1.0f; //投影Alpha值
        [Tooltip("默认None(纯白色)。遮罩纹理中白色代表完全不透明，黑色代表完全透明")]
        public Texture maskTexture = null; //遮罩贴图
        [Range(0.01f, 1f)]
        [Tooltip("默认0.01。该值越小，前后遮挡关系越清楚")]
        public float depthThreshold = 0.01f; //投影深度容差，值越小，遮挡关系越清楚
        
        [Title("2D - 3D Projector")]
        [Range(0, 1f)]
        [Tooltip("2D投影和3D投影的插值系数")] 
        public float lerp2Dto3D = 1;
        [Tooltip("主相机与当前次相机的相对距离系数")] 
        public float distanceScale = 1f;
        [Range(-1, 1)]
        [Tooltip("初始U偏移")] 
        public float adjOffsetU = 0;
        [Range(-1, 1)]
        [Tooltip("初始V偏移")] 
        public float adjOffsetV = 0;
        [Range(-2, 2)]
        [Tooltip("X最小hide值，视口X小于该值会隐藏投影")] 
        public float hideXMin = 0;
        [Range(-2, 2)]
        [Tooltip("X最大hide值，视口X大于该值会隐藏投影")] 
        public float hideXMax = 1;
        
        [ReadOnly] public float curViewportX = 0;
        [Tooltip("描边可视化H修正值")] 
        public float correctH = 0.17f;
        
        private Camera mainCamera = null;
        private Camera camera;
        private string depthSavePath = "Create DepthRT Offline (Step1.Render Depth)";
        private Material _projMat;
        [SerializeField, ReadOnly] private RenderTexture _depthRT;
        private RenderTextureDescriptor _descriptor;
        private Vector2 _viewportPoint = Vector2.one * 0.5f;
        private float _defaultDis = 0;
        private MeshRenderer _renderer;
#if UNITY_EDITOR
        private bool _offlineFlag;
        private Camera _mainCamera;
        private string _depthBtnInfo;
#endif
        private static readonly int UMin = Shader.PropertyToID("_uMin");
        private static readonly int UMax = Shader.PropertyToID("_uMax");
        private static readonly int VMin = Shader.PropertyToID("_vMin");
        private static readonly int VMax = Shader.PropertyToID("_vMax");
        private static readonly int Alpha = Shader.PropertyToID("_Alpha");
        private static readonly int DepthThreshold = Shader.PropertyToID("_DepthThreshold");
        private static readonly int FarDistance = Shader.PropertyToID("_FarDistance");
        private static readonly int NearDistance = Shader.PropertyToID("_NearDistance");
        private static readonly int SubCameraDepthTexture = Shader.PropertyToID("_SubCameraDepthTexture");
        private static readonly int Vp = Shader.PropertyToID("_VP");
        private static readonly int Distance = Shader.PropertyToID("_Distance");
        private static readonly int UVLerp = Shader.PropertyToID("_UVLerp");
        private static readonly int OffsetX = Shader.PropertyToID("_OffsetX");
        private static readonly int OffsetY = Shader.PropertyToID("_OffsetY");

        // private void Start()
        // {
        //     //相机辅助线
        //     if (transform.childCount > 0)
        //     {
        //         transform.GetChild(0).gameObject.SetActive(false);
        //     }
        // }

        private void OnEnable()
        {
            mainCamera = Camera.main;
            if (mainCamera == null)
            {
                Debug.LogError("找不到Main Camera!");
                return;
            }
            _renderer = GetComponent<MeshRenderer>();
            camera = GetComponent<Camera>();
            camera.enabled = false;
            _projMat = Application.isPlaying ? GetComponent<MeshRenderer>().materials[0] : GetComponent<MeshRenderer>().sharedMaterials[0];
            if (AllocateRTIfNeeded())
            {
                CreateDepthRT_Runtime().Forget();
            }
            //预计算初始次相机位置与主相机的视口坐标
            var position = transform.position;
            _viewportPoint = mainCamera.WorldToScreenPoint(position);
            //初始次相机与主相机距离
            _defaultDis = Vector3.Distance(position, mainCamera.transform.position);
#if UNITY_EDITOR
            _offlineFlag = false;
#endif
        }

        private void OnDisable()
        {
            if (_depthRT != null)
            {
                if (camera != null) camera.targetTexture = null;
                _depthRT.Release();
                _depthRT = null;
            }
        }

        private void Update()
        {
            //获取相关信息
            _projMat.SetFloat(UMin, uMin);
            _projMat.SetFloat(UMax, uMax);
            _projMat.SetFloat(VMin, vMin);
            _projMat.SetFloat(VMax, vMax);
            _projMat.SetFloat(Alpha, alpha);
            _projMat.SetFloat(DepthThreshold, depthThreshold);
            _projMat.SetFloat(FarDistance, camera.farClipPlane);
            _projMat.SetFloat(NearDistance, camera.nearClipPlane);
            _projMat.SetTexture(SubCameraDepthTexture, _depthRT);
            _projMat.SetMatrix(Vp, GL.GetGPUProjectionMatrix(camera.projectionMatrix, false) * camera.worldToCameraMatrix);
            if (lerp2Dto3D < 1)
            {
                Vector3 position = transform.position;
                float dis = Vector3.Distance(position, mainCamera.transform.position);
                _projMat.SetFloat(Distance, dis / _defaultDis * distanceScale);
                _projMat.SetFloat(UVLerp, lerp2Dto3D);
                Vector3 curViewport = mainCamera.WorldToScreenPoint(position);
                float offsetX = (curViewport.x - _viewportPoint.x) / Screen.width + adjOffsetU;
                float offsetY = (curViewport.y - _viewportPoint.y) / Screen.height + adjOffsetV;
                _projMat.SetFloat(OffsetX, offsetX);
                _projMat.SetFloat(OffsetY, offsetY);
            
                //处于X边缘就隐藏
                curViewportX = curViewport.x / Screen.width;
                if (curViewportX < hideXMin || curViewportX > hideXMax)
                {
                    _renderer.enabled = false;
                }
                else
                {
                    _renderer.enabled = true;
                }
            }
            else
            {
                _projMat.SetFloat(Distance, 1);
                _projMat.SetFloat(UVLerp, lerp2Dto3D);
                _projMat.SetFloat(OffsetX, 0.5f);
                _projMat.SetFloat(OffsetY, 0.5f);
            }
            
        }

        [Button("Create Mesh")]
        public void CreateMesh()
        {
            //获取相关信息
            camera = GetComponent<Camera>();
            float halfFOV = (camera.fieldOfView * 0.5f) * Mathf.Deg2Rad;
            float m_aspect = camera.aspect;
            float halfHeight_Near = camera.nearClipPlane * Mathf.Tan(halfFOV);
            float halfWidth_Near = halfHeight_Near * m_aspect;
            float halfHeight_Far = camera.farClipPlane * Mathf.Tan(halfFOV);
            float halfWidth_Far = halfHeight_Far * m_aspect;

            float epsilon = 0f;
            var nearClipPlane = camera.nearClipPlane;
            var farClipPlane = camera.farClipPlane;
            Vector3[] VBO = {
                //近平面
                new (-halfWidth_Near, -halfHeight_Near, nearClipPlane + epsilon), //左下角 0
                new (-halfWidth_Near, halfHeight_Near, nearClipPlane + epsilon), //左上角 1
                new (halfWidth_Near, halfHeight_Near, nearClipPlane + epsilon), //右上角 2
                new (halfWidth_Near, -halfHeight_Near, nearClipPlane + epsilon), //右下角 3
                //远平面
                new (-halfWidth_Far, -halfHeight_Far, farClipPlane), //左下角 4
                new (-halfWidth_Far, halfHeight_Far, farClipPlane), //左上角 5 
                new (halfWidth_Far, halfHeight_Far, farClipPlane), //右上角 6
                new (halfWidth_Far, -halfHeight_Far, farClipPlane), //右下角 7
            };
            
            int[] EBO = {
                //近平面(顺时针为正方向)
                0, 1, 2,
                0, 2, 3,
                0, 4, 1,
                1, 4, 5,
                1, 5, 2,
                2, 5, 6,
                2, 7, 3,
                2, 6, 7,
                0, 7, 4,
                0, 3, 7,
                4, 7, 5,
                5, 7, 6,
            };
            
            Mesh mesh = new Mesh();
            mesh.vertices = VBO;
            mesh.triangles = EBO;
            mesh.name = "ProjectorMesh";
            var meshFilter = GetComponent<MeshFilter>();
            if (meshFilter.sharedMesh != null) meshFilter.sharedMesh.Clear();
            meshFilter.sharedMesh = mesh;
            Debug.Log("生成Mesh成功!");

            if (transform.childCount > 0)
            {
                DestroyImmediate(transform.GetChild(0).gameObject);
            }
            GameObject outlineGroup = new GameObject("OutlineGroup");
            outlineGroup.transform.parent = transform;
            outlineGroup.transform.localPosition = Vector3.zero;
            outlineGroup.transform.localRotation = Quaternion.identity;
            outlineGroup.transform.localScale = Vector3.one;
            var lineTemplate = Resources.Load<GameObject>("OutlineLine");
            Transform outlineTrans = outlineGroup.transform;
            LineScript line;
            // for (int i = 0; i < VBO.Length / 2; i++)
            // {
            //     line = Instantiate(lineTemplate, outlineTrans.position, outlineTrans.rotation, outlineTrans)
            //            .GetComponent<LineScript>();
            //     line.SetLine(VBO[i], VBO[i+4]);
            // }
            float minorNearH = halfHeight_Near * 2 * correctH;
            float minorFarH = halfHeight_Far * 2 * correctH;
            // float minorNearH = 0;
            // float minorFarH = 0;
            Vector3[] VBOEx =
            {
                // 近远平面
                //近上
                new (-halfWidth_Near, -halfHeight_Near, nearClipPlane + epsilon),
                //近右
                new (halfWidth_Near, -halfHeight_Near, nearClipPlane + epsilon),
                //近下
                new (halfWidth_Near, halfHeight_Near - minorNearH, nearClipPlane + epsilon),
                //近左
                new (-halfWidth_Near, halfHeight_Near - minorNearH, nearClipPlane + epsilon),
                //远上
                new (-halfWidth_Far, -halfHeight_Far, farClipPlane),
                //远右
                new (halfWidth_Far, -halfHeight_Far, farClipPlane),
                //远下
                new (halfWidth_Far, halfHeight_Far - minorFarH, farClipPlane),
                //远左
                new (-halfWidth_Far, halfHeight_Far - minorFarH, farClipPlane),
                //视椎体边长
                //近平面
                new (-halfWidth_Near, -halfHeight_Near, nearClipPlane + epsilon), //左下角 0
                new (-halfWidth_Near, halfHeight_Near - minorNearH, nearClipPlane + epsilon), //左上角 1
                new (halfWidth_Near, halfHeight_Near - minorNearH, nearClipPlane + epsilon), //右上角 2
                new (halfWidth_Near, -halfHeight_Near, nearClipPlane + epsilon), //右下角 3
                // ------------------------------------------------------------
                new (halfWidth_Near, -halfHeight_Near, nearClipPlane + epsilon),
                new (halfWidth_Near, halfHeight_Near - minorNearH, nearClipPlane + epsilon),
                new (-halfWidth_Near, halfHeight_Near - minorNearH, nearClipPlane + epsilon),
                new (-halfWidth_Near, -halfHeight_Near, nearClipPlane + epsilon),
                new (halfWidth_Far, -halfHeight_Far, farClipPlane),
                new (halfWidth_Far, halfHeight_Far - minorFarH, farClipPlane),
                new (-halfWidth_Far, halfHeight_Far - minorFarH, farClipPlane),
                new (-halfWidth_Far, -halfHeight_Far, farClipPlane),
                new (-halfWidth_Far, -halfHeight_Far, farClipPlane), //左下角 4
                new (-halfWidth_Far, halfHeight_Far - minorFarH, farClipPlane), //左上角 5 
                new (halfWidth_Far, halfHeight_Far - minorFarH, farClipPlane), //右上角 6
                new (halfWidth_Far, -halfHeight_Far, farClipPlane), //右下角 7
            };
            int halfLen = VBOEx.Length / 2;
            for (int i = 0; i < halfLen; i++)
            {
                line = Instantiate(lineTemplate, outlineTrans.position, outlineTrans.rotation, outlineTrans)
                    .GetComponent<LineScript>();
                line.SetLine(VBOEx[i], VBOEx[i+halfLen]);
            }
            
        }

        private bool AllocateRTIfNeeded()
        {
            if (_depthRT == null || !_depthRT.IsCreated())
            {
                Vector2 screenSize = GetRealScreenSize();
                int screen_width = (int)screenSize.x;
                int screen_height = (int)screenSize.y;
                if (_descriptor.width != screen_width || _descriptor.height != screen_height)
                {
                    _descriptor = new RenderTextureDescriptor(screen_width, screen_height, GraphicsFormat.R32_SFloat, 0);
                }
                if (_depthRT != null)
                {
                    _depthRT.Release();
                    _depthRT = null;
                }
                _depthRT = new RenderTexture(_descriptor);
                _depthRT.name = "SubCamera_DepthRT";
                return true;
            }

            return false;
        }

        public async UniTaskVoid CreateDepthRT_Runtime()
        {
            Debug.Log("CreateDepthRT_Runtime");
            // _mainCamera = Camera.main;
            // if (_mainCamera == null)
            // {
            //     Debug.LogError("没有找到MainCamera！");
            //     return;
            // }
            // _mainCamera.enabled = false;
            
            camera = GetComponent<Camera>();
            AllocateRTIfNeeded();
            camera.targetTexture = _depthRT;
            camera.enabled = true;

            await UniTask.DelayFrame(2);

            if (camera != null)
            {
                camera.targetTexture = null;
                camera.enabled = false;
                // _mainCamera.enabled = true;
                // _mainCamera = null;
            }
        }

#if UNITY_EDITOR
        [Button("$depthSavePath")]
        public void CreateDepthRT_Offline()
        {
            if (!_offlineFlag)
            {
                // _mainCamera = Camera.main;
                // if (_mainCamera == null)
                // {
                //     Debug.LogError("没有找到MainCamera！");
                //     return;
                // }
                // _mainCamera.enabled = false;
                camera = GetComponent<Camera>();
                AllocateRTIfNeeded();
                camera.targetTexture = _depthRT;
                camera.enabled = true;
                depthSavePath = "Create DepthRT Offline (Step2.Export Result and Restore)";
            }
            else
            {
                // 保存截图为PNG格式的文件
                // Texture2D texture = new Texture2D(_depthRT.width, _depthRT.height, TextureFormat.RFloat, false);
                // texture.ReadPixels(new Rect(0, 0, _depthRT.width, _depthRT.height), 0, 0);
                // texture.Apply();
                // byte[] bytes = texture.EncodeToPNG();
                // File.WriteAllBytes(Path.Combine(Application.dataPath, "depthShot.png"), bytes);
                
                camera.targetTexture = null;
                camera.enabled = false;
                // _mainCamera.enabled = true;
                // _mainCamera = null;
                depthSavePath = "Create DepthRT Offline (Step1.Render Depth)";
            }
            _offlineFlag = !_offlineFlag;
        }
#endif
        
        private Vector2 GetRealScreenSize()
        {
#if UNITY_EDITOR
            var mouseOverWindow = UnityEditor.EditorWindow.mouseOverWindow;
            System.Reflection.Assembly assembly = typeof(UnityEditor.EditorWindow).Assembly;
            System.Type type = assembly.GetType("UnityEditor.PlayModeView");
            Vector2 size = (Vector2) type.GetMethod("GetMainPlayModeViewTargetSize",
                    System.Reflection.BindingFlags.NonPublic |
                    System.Reflection.BindingFlags.Static)
                ?.Invoke(mouseOverWindow, null)!;
            return size;
#else
            return new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);
#endif
        }
    }
}
