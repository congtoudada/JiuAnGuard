using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TEngine;

namespace GameLogic
{
	public partial class UIMainWindow : UIWindow
	{
		#region 脚本工具生成的代码
		private Button m_btnReset;
		private Button m_btnWander;
		private Button m_btnEdit;
		private Button m_btnQuit;
		private Button m_btnCamera;
		private Button m_btnCount;
		private Button m_btnFace;
		private Button m_btnWarn;
		private Button m_btnSearch;
		private Button m_btnChangeScene;
		private ScrollRect m_scrollRectRuntimeInfo;
		private Button m_btnRightSwitch;
		private Button m_btnAllKnown;
		protected override void ScriptGenerator()
		{
			m_btnReset = FindChildComponent<Button>("Head/m_btnReset");
			m_btnWander = FindChildComponent<Button>("Head/m_btnWander");
			m_btnEdit = FindChildComponent<Button>("Head/m_btnEdit");
			m_btnQuit = FindChildComponent<Button>("Left/m_btnQuit");
			m_btnCamera = FindChildComponent<Button>("Left/VerticalUp/m_btnCamera");
			m_btnCount = FindChildComponent<Button>("Left/VerticalUp/m_btnCount");
			m_btnFace = FindChildComponent<Button>("Left/VerticalUp/m_btnFace");
			m_btnWarn = FindChildComponent<Button>("Left/VerticalUp/m_btnWarn");
			m_btnSearch = FindChildComponent<Button>("Left/VerticalUp/m_btnSearch");
			m_btnChangeScene = FindChildComponent<Button>("Left/VerticalUp/m_btnChangeScene");
			m_scrollRectRuntimeInfo = FindChildComponent<ScrollRect>("Right/InfoPanel/m_scrollRectRuntimeInfo");
			m_btnRightSwitch = FindChildComponent<Button>("Right/InfoPanel/m_btnRightSwitch");
			m_btnAllKnown = FindChildComponent<Button>("Right/InfoPanel/m_btnAllKnown");
			m_btnReset.onClick.AddListener(UniTask.UnityAction(OnClickResetBtn));
			m_btnWander.onClick.AddListener(UniTask.UnityAction(OnClickWanderBtn));
			m_btnEdit.onClick.AddListener(UniTask.UnityAction(OnClickEditBtn));
			m_btnQuit.onClick.AddListener(UniTask.UnityAction(OnClickQuitBtn));
			m_btnCamera.onClick.AddListener(UniTask.UnityAction(OnClickCameraBtn));
			m_btnCount.onClick.AddListener(UniTask.UnityAction(OnClickCountBtn));
			m_btnFace.onClick.AddListener(UniTask.UnityAction(OnClickFaceBtn));
			m_btnWarn.onClick.AddListener(UniTask.UnityAction(OnClickWarnBtn));
			m_btnSearch.onClick.AddListener(UniTask.UnityAction(OnClickSearchBtn));
			m_btnChangeScene.onClick.AddListener(UniTask.UnityAction(OnClickChangeSceneBtn));
			m_btnRightSwitch.onClick.AddListener(UniTask.UnityAction(OnClickRightSwitchBtn));
			m_btnAllKnown.onClick.AddListener(UniTask.UnityAction(OnClickAllKnownBtn));
		}
		#endregion

		#region 事件
		private async UniTaskVoid OnClickResetBtn()
		{
 await UniTask.Yield();
		}
		private async UniTaskVoid OnClickWanderBtn()
		{
 await UniTask.Yield();
		}
		private async UniTaskVoid OnClickEditBtn()
		{
 await UniTask.Yield();
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
 await UniTask.Yield();
		}
		private async UniTaskVoid OnClickChangeSceneBtn()
		{
 await UniTask.Yield();
		}
		private async UniTaskVoid OnClickRightSwitchBtn()
		{
 await UniTask.Yield();
		}
		private async UniTaskVoid OnClickAllKnownBtn()
		{
 await UniTask.Yield();
		}
		#endregion

	}
}
