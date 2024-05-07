/****************************************************
  文件：UIMainWindow.cs
  作者：聪头
  邮箱：1322080797@qq.com
  日期：2024年04月29日 14:17:20
  功能：
*****************************************************/
using System.Collections;
using System.Collections.Generic;
using TEngine;
using UnityEngine;

namespace GameLogic
{
	[Window(UILayer.UI)]
	public partial class UIMainWindow : UIWindow
	{
		protected override void OnCreate()
		{
			Log.Info("UIMainWindow OnCreate");
		}

		protected override void OnRefresh()
		{
			Log.Info("UIMainWindow OnRefresh");
			// var userData = new UITipWindow.UITipData();
			// userData.title = "你好";
			// // userData.main_text = "Hello World";
			// userData.img_url =
			// 	"file://E:\\Practice\\Unity\\Demo\\DUT\\JiuAnGuard\\JiuAnProject\\Assets\\AssetRaw\\Texture\\Snipaste_2024-04-17_22-56-45.png";
			// userData.sub_text = "入门";
			// GameModule.UI.ShowUI<UITipWindow>(userData);
		}

		protected override void OnSetVisible(bool visible)
		{
			Log.Info("UIMainWindow OnSetVisible");
		}
	}
}
