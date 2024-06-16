using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

namespace GameLogic
{
    public class AddOverlayCamera : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            SceneManager.sceneLoaded += (scene, mode) =>
            {
                var mainCamera = Camera.main;
                if (mainCamera != null)
                {
                    var stack = Camera.main.GetComponent<UniversalAdditionalCameraData>().cameraStack;
                    if (stack.Count == 0)
                        stack.Add(GetComponent<Camera>());
                }
                else
                {
                    Debug.LogWarning("没有找到主相机！");
                }
            };
        }
    }
}
