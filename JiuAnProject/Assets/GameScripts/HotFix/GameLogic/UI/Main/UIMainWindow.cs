/****************************************************
  文件：UIMainWindow.cs
  作者：聪头
  邮箱：1322080797@qq.com
  日期：2024年04月29日 14:17:20
  功能：
*****************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Newtonsoft.Json;
using TEngine;
using UnityEngine;
using UnityEngine.UI;

namespace GameLogic
{
	[Window(UILayer.UI)]
	public partial class UIMainWindow : UIWindow
	{
		protected override void OnCreate()
		{
			OnToggleWanderChange(true);
			m_goUILoadingWidget.SetActive(false);
			m_btnReboot.GetComponent<Image>().color = Color.white;
			Log.Info("UIMainWindow OnCreate");
			CreateWidget<RuntimeWarnWidget>("Right/RuntimeWarnWidget");
			CreateWidget<RuntimeCountInfoWidget>("Right/RuntimeCountInfoWidget");
		}

		protected override async void OnRefresh()
		{
			Log.Info("UIMainWindow OnRefresh");
			m_cgVerticalUp.interactable = false;
			//验证后端是否正常
			bool ret = await UIGlobalDataInstance.Instance.SendCameraInfoReq();
			if (ret)
			{
				m_cgVerticalUp.interactable = true;
				GameModule.UI.ShowUIAsync<UICountWindow>();
			}
			//验证算法端是否正常
			SSHTool.getSSHLog += CheckAlgorithmCallback;
			await Task.Run(() =>
			{
				foreach (var remote in WebURL.RemoteList)
				{
					SSHTool.RunSSHCommands(remote.ServerIp, remote.Username, 
						remote.Password, $"source /home/{remote.Username}/dut/check_done.sh");
				}
			});
			SSHTool.getSSHLog -= CheckAlgorithmCallback;
		}

		private void CheckAlgorithmCallback(bool isOK, string info)
		{
			try
			{
				// UISimpleTipWindow.Show(info);
				// sshResult = info;
				sshOK = isOK;
				if (isOK)
				{
					Log.Info("Run Shell Succeed: " + info);
					bool isRunning = Convert.ToInt32(info) == 1; //重启成功
					if (isRunning)
					{
						m_btnReboot.GetComponent<Image>().color = new Color(46/255.0f,220/255.0f,50/255.0f);
					}
					else
					{
						m_btnReboot.GetComponent<Image>().color = Color.red;
					}
				}
				else
				{
					Log.Warning("Shell Sheel Failed: " + info);
				}
			}
			catch (Exception e)
			{
				Log.Warning("Shell Sheel Failed: " + info);
			}
		}
		// protected override void OnSetVisible(bool visible)
		// {
		// 	Log.Info("UIMainWindow OnSetVisible");
		// }
	}
}
