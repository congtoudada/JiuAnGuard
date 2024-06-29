/****************************************************
  文件：UIMainWindow.cs
  作者：聪头
  邮箱：1322080797@qq.com
  日期：2024年04月29日 14:17:20
  功能：
*****************************************************/
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using TEngine;
using UnityEngine;

namespace GameLogic
{
	[Window(UILayer.UI)]
	public partial class UIMainWindow : UIWindow
	{
		protected override void OnCreate()
		{
			OnToggleWanderChange(true);
			Log.Info("UIMainWindow OnCreate");
			CreateWidget<RuntimeWarnWidget>("Right/RuntimeWarnWidget");
		}

		protected override async void OnRefresh()
		{
			Log.Info("UIMainWindow OnRefresh");
			await UIGlobalDataInstance.Instance.SendCameraInfoReq();
		}

		protected override void OnSetVisible(bool visible)
		{
			Log.Info("UIMainWindow OnSetVisible");
		}
	}
}
