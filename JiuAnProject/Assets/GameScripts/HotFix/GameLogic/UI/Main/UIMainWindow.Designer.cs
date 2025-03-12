using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using TEngine;
using TMPro;
using UnityEditor;

namespace GameLogic
{
	public partial class UIMainWindow
	{
		private string sshResult;
		private bool sshOK;
		private bool rebootSucceed;
		private int waitCnt;

		#region 脚本工具生成的代码
		private Button m_btnReset;
		private Button m_btnCamIsOn;
		private Toggle m_toggleLock;
		private Toggle m_toggleWander;
		private Toggle m_toggleEdit;
		private Button m_btnQuit;
		private CanvasGroup m_cgVerticalUp;
		private Button m_btnCamera;
		private Button m_btnCount;
		private Button m_btnFace;
		private Button m_btnWarn;
		private Button m_btnSearch;
		private Button m_btnChangeScene;
		private Button m_btnReboot;
		private GameObject m_goUILoadingWidget;
		private Wander _wander;
		protected override void ScriptGenerator()
		{
			m_btnReset = FindChildComponent<Button>("Head/Left/m_btnReset");
			m_btnCamIsOn = FindChildComponent<Button>("Head/Left/m_btnCamIsOn");
			m_toggleLock = FindChildComponent<Toggle>("Head/Right/m_toggleLock");
			m_toggleWander = FindChildComponent<Toggle>("Head/Right/m_toggleWander");
			m_toggleEdit = FindChildComponent<Toggle>("Head/Right/m_toggleEdit");
			m_btnQuit = FindChildComponent<Button>("Left/m_btnQuit");
			m_cgVerticalUp = FindChildComponent<CanvasGroup>("Left/m_cgVerticalUp");
			m_btnCamera = FindChildComponent<Button>("Left/m_cgVerticalUp/m_btnCamera");
			m_btnCount = FindChildComponent<Button>("Left/m_cgVerticalUp/m_btnCount");
			m_btnFace = FindChildComponent<Button>("Left/m_cgVerticalUp/m_btnFace");
			m_btnWarn = FindChildComponent<Button>("Left/m_cgVerticalUp/m_btnWarn");
			m_btnSearch = FindChildComponent<Button>("Left/m_cgVerticalUp/m_btnSearch");
			m_btnChangeScene = FindChildComponent<Button>("Left/m_cgVerticalUp/m_btnChangeScene");
			m_btnReboot = FindChildComponent<Button>("Left/m_cgVerticalUp/m_btnReboot");
			m_goUILoadingWidget = FindChild("Right/m_goUILoadingWidget").gameObject;
			m_btnReset.onClick.AddListener(UniTask.UnityAction(OnClickResetBtn));
			m_btnCamIsOn.onClick.AddListener(UniTask.UnityAction(OnClickCamIsOnBtn));
			m_toggleLock.onValueChanged.AddListener(OnToggleLockChange);
			m_toggleWander.onValueChanged.AddListener(OnToggleWanderChange);
			m_toggleEdit.onValueChanged.AddListener(OnToggleEditChange);
			m_btnQuit.onClick.AddListener(UniTask.UnityAction(OnClickQuitBtn));
			m_btnCamera.onClick.AddListener(UniTask.UnityAction(OnClickCameraBtn));
			m_btnCount.onClick.AddListener(UniTask.UnityAction(OnClickCountBtn));
			m_btnFace.onClick.AddListener(UniTask.UnityAction(OnClickFaceBtn));
			m_btnWarn.onClick.AddListener(UniTask.UnityAction(OnClickWarnBtn));
			m_btnSearch.onClick.AddListener(UniTask.UnityAction(OnClickSearchBtn));
			m_btnChangeScene.onClick.AddListener(UniTask.UnityAction(OnClickChangeSceneBtn));
			m_btnReboot.onClick.AddListener(UniTask.UnityAction(OnClickRebootBtn));
			_wander = GameObject.FindWithTag("LevelManager").GetComponent<Wander>();
			// _wander = GameModule.Base.gameObject.GetOrAddComponent<Wander>();
			if (_wander == null)
			{
				Debug.LogWarning("找不到Wander脚本!");
			}
		}
		#endregion
		
		#region 事件
		private async UniTaskVoid OnClickResetBtn()
		{
			GameObject startPoint = GameObject.FindWithTag("StartPoint");
			if (startPoint == null)
			{
				Log.Info("在场景中没有找到原点");
				return;
			}
			Camera.main.transform.DOMove(startPoint.transform.position, 1f);
			Camera.main.transform.DORotate(startPoint.transform.rotation.eulerAngles, 1f);
			var levelMgr = GameObject.FindWithTag("LevelManager");
			if (levelMgr != null) levelMgr.GetComponent<Wander>().bScript = null;
		}
		private async UniTaskVoid OnClickCamIsOnBtn()
		{
			if (!UIGlobalDataInstance.Instance.IsPreview)
			{
				UITipWindow.Show("预览警告", main_text: "您是否要预览所有摄像头的画面，如果摄像头数量过多可能造成卡顿。", confirmCallback:
					() =>
					{
						UIGlobalDataInstance.Instance.IsPreview = true;
						Log.Info($"摄像头预览: {UIGlobalDataInstance.Instance.IsPreview}");
						UIGlobalDataInstance.Instance.OnPreviewChanged?.Invoke(true);
					});
			}
			else
			{
				UIGlobalDataInstance.Instance.IsPreview = false;
				Log.Info($"摄像头预览: {UIGlobalDataInstance.Instance.IsPreview}");
				UIGlobalDataInstance.Instance.OnPreviewChanged?.Invoke(false);
			}
		}
		private void OnToggleLockChange(bool isOn)
		{
			if (isOn)
			{
				_wander.enabled = false;
			}
		}
		private void OnToggleWanderChange(bool isOn)
		{
			if (isOn)
			{
				_wander.enabled = true;
			}
		}
		private void OnToggleEditChange(bool isOn)
		{
			if (isOn)
			{
				_wander.enabled = false;
			}
		}
		private async UniTaskVoid OnClickQuitBtn()
		{
			UITipWindow.Show(title: "退出", main_text: "您真的要退出吗？", confirmCallback: () =>
			{
#if UNITY_EDITOR
				if (EditorApplication.isPlaying)
				{
					EditorApplication.isPlaying = false;
				}
#else
				Application.Quit();
#endif
			});
		}
		private async UniTaskVoid OnClickCameraBtn()
		{
			GameModule.UI.ShowUI<UICameraWindow>();
		}
		private async UniTaskVoid OnClickCountBtn()
		{
			GameModule.UI.ShowUI<UICountWindow>();
		}
		private async UniTaskVoid OnClickFaceBtn()
		{
			GameModule.UI.ShowUI<UIFaceWindow>();
		}
		private async UniTaskVoid OnClickWarnBtn()
		{
			GameModule.UI.ShowUI<UIWarnWindow>();
		}
		private async UniTaskVoid OnClickSearchBtn()
		{
			GameModule.UI.ShowUI<UISearchWindow>();
		}
		private async UniTaskVoid OnClickChangeSceneBtn()
		{
            UISimpleTipWindow.Show("此功能暂未开放");
		}
		
		private async UniTaskVoid OnClickRebootBtn()
		{
			if (!m_goUILoadingWidget.activeSelf)
			{
				UITipWindow.Show("重启算法", main_text:"重启算法可能需要等待一小段时间(<5min)，请不要中途断开。您确定要重启吗？",
					confirmCallback: async () =>
					{
						m_goUILoadingWidget.SetActive(true);
						sshOK = false;
						m_btnReboot.GetComponent<Image>().color = new Color(230/255.0f, 147/255.0f, 40/255.0f);
						await Task.Run(() =>
						{
							foreach (var remote in WebURL.RemoteList)
							{
								//执行重启命令
								SSHTool.RunSSHCommands(remote.ServerIp, remote.Username, 
									remote.Password, $"source /home/{remote.Username}/dut/launcher.sh");
							}
							
						});
						// 查询是否重启成功
						waitCnt = 0;
						rebootSucceed = false;
						SSHTool.getSSHLog += SSHLogCallback;
						while (!rebootSucceed)
						{
							if (waitCnt > 300) //5min
							{
								break;
							}
							await Task.Run(() =>
							{
								foreach (var remote in WebURL.RemoteList)
								{
									//执行重启命令
									SSHTool.RunSSHCommands(remote.ServerIp, remote.Username, 
										remote.Password, $"source /home/{remote.Username}/dut/check_done.sh");
								}
							});
							await UniTask.Delay(1000); //每秒查询一次是否启动结束
							++waitCnt;
						}
						SSHTool.getSSHLog -= SSHLogCallback;
						m_btnReboot.GetComponent<Image>().color = new Color(46/255.0f,220/255.0f,50/255.0f);
						m_goUILoadingWidget.SetActive(false);
						if (sshOK)
						{
							await UniTask.Delay(10);  //延迟10s
							// 5s重启命令等待+3s网络延迟
							UISimpleTipWindow.Show("算法重启成功！耗时约: " + (waitCnt+18) + "秒", 5.0f);
						}
						else
						{
							// UISimpleTipWindow.Show("算法重启失败！");
							UITipWindow.Show("算法启动结果", main_text:"算法重启失败，请尝试重新启动或联系相关负责人。");
						}
					});
			}
			else
			{
				UISimpleTipWindow.Show("算法正在重启，请耐心等待...");
			}
		}

		private void SSHLogCallback(bool isOK, string info)
		{
			try
			{
				// UISimpleTipWindow.Show(info);
				// sshResult = info;
				sshOK = isOK;
				if (isOK)
				{
					Log.Info("Run Shell Succeed: " + info);
					rebootSucceed = Convert.ToInt32(info) == 1; //重启成功
				}
				else
				{
					Log.Warning("Shell Sheel Failed: " + info);
				}
			}
			catch (Exception e)
			{
				Log.Error("Shell Sheel Failed: " + info);
			}
		}
		#endregion

	}
}
