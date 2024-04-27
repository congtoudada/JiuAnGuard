using System;
using System.Collections;
using System.Collections.Generic;
using TEngine;
using UnityEngine;
using UnityEngine.UI;

namespace GameLogic
{
    [ExecuteInEditMode]
    public class GridAutoResize : MonoBehaviour
    {
        private int old_screen_width;
        private int old_screen_height;
        [SerializeField] private float defaultSellWidth = 300; //1920x1080下cellSize.x

        private void Start()
        {
            old_screen_width = Screen.width;
            old_screen_height = Screen.height;
        }

        // Start is called before the first frame update
        void OnEnable()
        {
            ChangeCellSize();
        }

        // private void Update()
        // {
        //     if (old_screen_width != Screen.width || old_screen_height != Screen.height)
        //     {
        //         ChangeCellSize();
        //     }
        // }

        private void ChangeCellSize()
        {
            var grid = GetComponent<GridLayoutGroup>();
            var canvasScaler = GetComponentInParent<CanvasScaler>();
            if (canvasScaler == null) return;
            var ref_width = canvasScaler.referenceResolution.x;
#if UNITY_EDITOR
            float windowWidth = GetGameViewWidth();
#else
            float windowWidth = Screen.width;
#endif
            float scale_width = windowWidth / ref_width;
            float cur_width = defaultSellWidth * ((scale_width - 1) * 0.667f + 1);
            grid.cellSize = new Vector2(cur_width, grid.cellSize.y);
        }

        private float GetGameViewWidth()
        {
            var mouseOverWindow = UnityEditor.EditorWindow.mouseOverWindow;
            System.Reflection.Assembly assembly = typeof(UnityEditor.EditorWindow).Assembly;
            System.Type type = assembly.GetType("UnityEditor.PlayModeView");

            Vector2 size = (Vector2) type.GetMethod(
                "GetMainPlayModeViewTargetSize",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Static
            ).Invoke(mouseOverWindow, null);

            return size.x;
        }

    }
}
