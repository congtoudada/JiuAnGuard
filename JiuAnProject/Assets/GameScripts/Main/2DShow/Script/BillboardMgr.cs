/****************************************************
  文件：BillboardMgr.cs
  作者：聪头
  邮箱：1322080797@qq.com
  日期：2024年06月17日 00:32:19
  功能：
*****************************************************/

using System;
using System.Collections.Generic;
using UnityEngine;

public class BillboardMgr : MonoBehaviour
{
  public List<Material> billboardMatList;
  private Camera mainCamera;
  private static readonly int RotateMatrix = Shader.PropertyToID("_RotateMatrix");

  private void Start()
  {
    mainCamera = Camera.main;
    if (mainCamera == null)
    {
      Debug.LogError("没有找到MainCamera！");
    }
  }

  private void Update()
  {
    var matrix = Matrix4x4.Rotate(mainCamera.transform.rotation);
    for (int i = 0; i < billboardMatList.Count; i++)
    {
      billboardMatList[i].SetMatrix(RotateMatrix, matrix);
    }
  }
}