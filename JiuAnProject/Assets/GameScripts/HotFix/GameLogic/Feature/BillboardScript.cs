/****************************************************
  文件：BillboardScript.cs
  作者：聪头
  邮箱:  1322080797@qq.com
  日期：DateTime
  功能：
*****************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using GameLogic;
using TEngine;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(MeshRenderer))]
public class BillboardScript : MonoBehaviour
{
    [Header("缩放值修正")]
    public float Scale = 1f;
    [Header("描边GameObject")]
    public GameObject OutlineObj;
    [Header("视频GameObject")] 
    public GameObject PlayerObj;

    private bool _isHover = false;
    private Camera mainCam;
    public SubCameraMgr subMgr;
    private bool _isFirst = true;
    private void Start()
    {
        mainCam = Camera.main;
        if (GameApp.IsFramework)
        {
            UIGlobalDataInstance.Instance.OnPreviewChanged += OnPreviewChangedCallback;
        }
        subMgr = transform.parent.GetComponentInChildren<SubCameraMgr>();
    }

    private void Update()
    {
        // 已经将逻辑转移到Mgr类中，记得向场景添加BillboardMgr
        // mat.SetMatrix("_RotateMatrix", Matrix4x4.Rotate(mainCam.transform.rotation));
        transform.localScale = Vector3.one * Scale * GetAutoScale() * 0.01f;
    }

    private float GetAutoScale()
    {
        float depth = mainCam.WorldToViewportPoint(transform.position).z;
        //Debug.Log("depth: " + depth); //正数
        float k = (depth - mainCam.nearClipPlane) / (mainCam.farClipPlane - mainCam.nearClipPlane);
        float scale = Mathf.Lerp(1.0f, mainCam.farClipPlane - mainCam.nearClipPlane, k);
        return scale;
    }

    private void OnMouseEnter()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            _isHover = true;
            Show();
        }
    }

    private void OnMouseExit()
    {
        if (_isHover && !UIGlobalDataInstance.Instance.IsPreview)
        {
            _isHover = false;
            UnShow();
        }
    }

    private void OnDestroy()
    {
        if (GameApp.IsFramework)
        {
            UIGlobalDataInstance.Instance.OnPreviewChanged -= OnPreviewChangedCallback;
        }
    }

    private void OnPreviewChangedCallback(bool isPreview)
    {
        if (isPreview)
        {
            if (!PlayerObj.activeSelf)
            {
                Show();
            }
        }
        else
        {
            if (PlayerObj.activeSelf)
            {
                UnShow();
            }
        }
    }

    private void Show()
    {
        //选中被点击的物体
        OutlineObj.SetActive(true);
        PlayerObj.SetActive(true);
        if (_isFirst)
        {
            _isFirst = false;
            PlayerObj.GetComponent<VLCPlayerExample>().Open();
        }
        PlayerObj.GetComponent<VLCPlayerExample>().Play();
    }

    private void UnShow()
    {
        OutlineObj.SetActive(false);
        PlayerObj.SetActive(false);
        PlayerObj.GetComponent<VLCPlayerExample>().Pause();
    }
}