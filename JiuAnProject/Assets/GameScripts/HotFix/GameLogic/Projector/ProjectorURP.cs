using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

namespace GameLogic
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Camera))]
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class ProjectorURP : MonoBehaviour
    {
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
        
        private Camera camera;
        private string depthSavePath = "Create DepthRT Offline (Step1.Render Depth)";
        private Material _projMat;
        [SerializeField, ReadOnly] private RenderTexture _depthRT;
        private RenderTextureDescriptor _descriptor;
#if UNITY_EDITOR
        private bool _offlineFlag;
        private Camera _mainCamera;
        private string _depthBtnInfo;
#endif

        private void OnEnable()
        {
            camera = GetComponent<Camera>();
            camera.enabled = false;
            _projMat = GetComponent<MeshRenderer>().sharedMaterials[0];
            if (AllocateRTIfNeeded())
            {
                CreateDepthRT_Runtime().Forget();
            }
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
            _projMat.SetFloat("_uMin", uMin);
            _projMat.SetFloat("_uMax", uMax);
            _projMat.SetFloat("_vMin", vMin);
            _projMat.SetFloat("_vMax", vMax);
            _projMat.SetFloat("_Alpha", alpha);
            _projMat.SetFloat("_DepthThreshold", depthThreshold);
            _projMat.SetFloat("_FarDistance", camera.farClipPlane);
            _projMat.SetFloat("_NearDistance", camera.nearClipPlane);
            _projMat.SetTexture("_SubCameraDepthTexture", _depthRT);
            _projMat.SetMatrix("_VP", GL.GetGPUProjectionMatrix(camera.projectionMatrix, false) * camera.worldToCameraMatrix);
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
            
            Vector3[] VBO = {
                //近平面
                new Vector3(-halfWidth_Near, -halfHeight_Near, camera.nearClipPlane + 0.01f), //左下角 0
                new Vector3(-halfWidth_Near, halfHeight_Near, camera.nearClipPlane + 0.01f), //左上角 1
                new Vector3(halfWidth_Near, halfHeight_Near, camera.nearClipPlane + 0.01f), //右上角 2
                new Vector3(halfWidth_Near, -halfHeight_Near, camera.nearClipPlane + 0.01f), //右下角 3
                //远平面
                new Vector3(-halfWidth_Far, -halfHeight_Far, camera.farClipPlane), //左下角 4
                new Vector3(-halfWidth_Far, halfHeight_Far, camera.farClipPlane), //左上角 5 
                new Vector3(halfWidth_Far, halfHeight_Far, camera.farClipPlane), //右上角 6
                new Vector3(halfWidth_Far, -halfHeight_Far, camera.farClipPlane), //右下角 7
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
            
            camera.targetTexture = null;
            camera.enabled = false;
            // _mainCamera.enabled = true;
            // _mainCamera = null;
        }
        
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
