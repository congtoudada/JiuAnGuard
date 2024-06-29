using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using TEngine;

namespace GameLogic
{
	public partial class UIMainWindow
	{
		#region 脚本工具生成的代码
		private Button m_btnReset;
		private Button m_btnCamIsOn;
		private Toggle m_toggleLock;
		private Toggle m_toggleWander;
		private Toggle m_toggleEdit;
		private Button m_btnQuit;
		private Button m_btnCamera;
		private Button m_btnCount;
		private Button m_btnFace;
		private Button m_btnWarn;
		private Button m_btnSearch;
		private Button m_btnChangeScene;
		protected override void ScriptGenerator()
		{
			m_btnReset = FindChildComponent<Button>("Head/Left/m_btnReset");
			m_btnCamIsOn = FindChildComponent<Button>("Head/Left/m_btnCamIsOn");
			m_toggleLock = FindChildComponent<Toggle>("Head/Right/m_toggleLock");
			m_toggleWander = FindChildComponent<Toggle>("Head/Right/m_toggleWander");
			m_toggleEdit = FindChildComponent<Toggle>("Head/Right/m_toggleEdit");
			m_btnQuit = FindChildComponent<Button>("Left/m_btnQuit");
			m_btnCamera = FindChildComponent<Button>("Left/VerticalUp/m_btnCamera");
			m_btnCount = FindChildComponent<Button>("Left/VerticalUp/m_btnCount");
			m_btnFace = FindChildComponent<Button>("Left/VerticalUp/m_btnFace");
			m_btnWarn = FindChildComponent<Button>("Left/VerticalUp/m_btnWarn");
			m_btnSearch = FindChildComponent<Button>("Left/VerticalUp/m_btnSearch");
			m_btnChangeScene = FindChildComponent<Button>("Left/VerticalUp/m_btnChangeScene");
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
				GameModule.Base.gameObject.GetOrAddComponent<Wander>().enabled = false;
			}
		}
		private void OnToggleWanderChange(bool isOn)
		{
			if (isOn)
			{
				GameModule.Base.gameObject.GetOrAddComponent<Wander>().enabled = true;
			}
		}
		private void OnToggleEditChange(bool isOn)
		{
			if (isOn)
			{
				GameModule.Base.gameObject.GetOrAddComponent<Wander>().enabled = false;
			}
		}
		private async UniTaskVoid OnClickQuitBtn()
		{
			await UniTask.Yield();
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
 await UniTask.Yield();
		}
		#endregion

	}
}
