/****************************************************
  文件：UICameraWidget.cs
  作者：聪头
  邮箱：1322080797@qq.com
  日期：2024年04月29日 15:28:58
  功能：
*****************************************************/

using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TEngine;
using UnityEngine.Video;

namespace GameLogic
{
    public class UICameraWidget : UIWidget
    {
        #region 脚本工具生成的代码
        private RawImage m_rimgVideoRT;
        private VideoPlayer _videoPlayer;
        private Button m_btnPlay;
        private ScrollRect m_scrollRectCamInfo;
        private Button m_btnMove;
        private Button m_btnCloudControl;
        protected override void ScriptGenerator()
        {
            m_rimgVideoRT = FindChildComponent<RawImage>("LeftGroup/Video/Show/m_rimgVideoRT");
            _videoPlayer = m_rimgVideoRT.GetComponent<VideoPlayer>();
            m_btnPlay = FindChildComponent<Button>("LeftGroup/Video/ToolMenu/m_btnPlay");
            m_scrollRectCamInfo = FindChildComponent<ScrollRect>("LeftGroup/m_scrollRectCamInfo");
            m_btnMove = FindChildComponent<Button>("RightGroup/InteractGroup/VerticalGroup/m_btnMove");
            m_btnCloudControl = FindChildComponent<Button>("RightGroup/InteractGroup/VerticalGroup/m_btnCloudControl");
            m_btnPlay.onClick.AddListener(UniTask.UnityAction(OnClickPlayBtn));
            m_btnMove.onClick.AddListener(UniTask.UnityAction(OnClickMoveBtn));
            m_btnCloudControl.onClick.AddListener(UniTask.UnityAction(OnClickCloudControlBtn));
        }
        #endregion

        #region 事件
        private async UniTaskVoid OnClickPlayBtn()
        {
            if (!_videoPlayer.isPlaying)
            {
                _videoPlayer.Play();
                m_btnPlay.GetComponent<Image>().SetSprite("camera_pause_btn2");
            }
            else
            {
                _videoPlayer.Pause();
                m_btnPlay.GetComponent<Image>().SetSprite("camera_play_btn2");
            }
        }
        private async UniTaskVoid OnClickMoveBtn()
        {
            await UniTask.Yield();
        }
        private async UniTaskVoid OnClickCloudControlBtn()
        {
            await UniTask.Yield();
        }
        #endregion

    }
}
