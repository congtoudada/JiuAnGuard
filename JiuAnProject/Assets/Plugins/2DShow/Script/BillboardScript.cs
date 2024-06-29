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
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class BillboardScript : MonoBehaviour
{
    [Header("缩放值修正")]
    public float Scale = 1f;
    [Header("描边GameObject")]
    public GameObject OutlineObj;
    [Header("视频GameObject")] 
    public GameObject PlayerObj;
    
    private Camera mainCam;
    private void Start()
    {
        mainCam = Camera.main;
        UIGlobalDataInstance.Instance.OnPreviewChanged += OnPreviewChangedCallback;
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
        OutlineObj.SetActive(true);
        PlayerObj.SetActive(true);
        PlayerObj.GetComponent<VLCPlayerExample>().Play();
    }

    private void OnMouseExit()
    {
        if (!UIGlobalDataInstance.Instance.IsPreview)
        {
            OutlineObj.SetActive(false);
            PlayerObj.SetActive(false);
            PlayerObj.GetComponent<VLCPlayerExample>().Pause();
        }
    }

    private void OnDestroy()
    {
        UIGlobalDataInstance.Instance.OnPreviewChanged -= OnPreviewChangedCallback;
    }

    private void OnPreviewChangedCallback(bool isPreview)
    {
        if (isPreview)
        {
            if (!PlayerObj.activeSelf)
            {
                OnMouseEnter();
            }
        }
        else
        {
            if (PlayerObj.activeSelf)
            {
                OnMouseExit();
            }
        }
    }
}