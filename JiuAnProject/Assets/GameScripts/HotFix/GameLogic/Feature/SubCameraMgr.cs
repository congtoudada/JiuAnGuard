using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GameLogic
{
    public class SubCameraMgr : MonoBehaviour
    {
        private VLCPlayerExample _vlcPlayer;
        private MeshRenderer _renderer;
        private ProjectorURP _projector;
        private Wander _wander;
        private Coroutine _coroutine;
        
        [Header("投影可视范围")]
        public float visibleRange = 5f;
        [Header("完全显示阈值")] public float visibleThresh = 0.7f;
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
            Hide();
        }
        
        [Button]
        public void Show()
        {
            _vlcPlayer.Play();
            _renderer.enabled = true;
        }
        
        [Button]
        public void Hide()
        {
            _vlcPlayer.Pause();
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
            while (_wander.curSubMgr == this)
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
