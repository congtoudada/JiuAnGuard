/****************************************************
  文件：UICameraWidget.cs
  作者：聪头
  邮箱：1322080797@qq.com
  日期：2024年04月29日 15:28:58
  功能：
*****************************************************/

using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using TEngine;
using TMPro;
using UnityEngine.Video;

namespace GameLogic
{
    public class UICameraWidget : UIWidget
    {
        // private VideoPlayer _videoPlayer;
        private VLCPlayerExample _VLCPlayer;
        private GameObject subCamGroup = null;
        private Wander wander = null;
        public int currentID = 0;
        
        #region 脚本工具生成的代码
        private RawImage m_rimgVideoRT;
        private Button m_btnPlay;
        private TextMeshProUGUI m_textCamType;
        private TextMeshProUGUI m_textCamIp;
        private Button m_btnMove;
        private Button m_btnCloudControl;
        protected override void ScriptGenerator()
        {
            m_rimgVideoRT = FindChildComponent<RawImage>("LeftGroup/Video/Show/m_rimgVideoRT");
            m_btnPlay = FindChildComponent<Button>("LeftGroup/Video/ToolMenu/m_btnPlay");
            m_textCamType = FindChildComponent<TextMeshProUGUI>("LeftGroup/m_textCamType");
            m_textCamIp = FindChildComponent<TextMeshProUGUI>("LeftGroup/m_textCamIp");
            m_btnMove = FindChildComponent<Button>("RightGroup/InteractGroup/VerticalGroup/m_btnMove");
            m_btnCloudControl = FindChildComponent<Button>("RightGroup/InteractGroup/VerticalGroup/m_btnCloudControl");
            m_btnPlay.onClick.AddListener(UniTask.UnityAction(OnClickPlayBtn));
            m_btnMove.onClick.AddListener(UniTask.UnityAction(OnClickMoveBtn));
            m_btnCloudControl.onClick.AddListener(UniTask.UnityAction(OnClickCloudControlBtn));
        }
        #endregion

        protected override void BindMemberProperty()
        {
            // _videoPlayer = m_rimgVideoRT.GetComponent<VideoPlayer>();
            _VLCPlayer = m_rimgVideoRT.GetComponent<VLCPlayerExample>();
        }

        #region 事件
        private async UniTaskVoid OnClickPlayBtn()
        {
            Log.Debug("UICameraWidget IsPlaying: " + _VLCPlayer.IsPlaying);
            if (!_VLCPlayer.IsPlaying)
            {
                Log.Debug("UICameraWidget Play: " + _VLCPlayer.path);
                _VLCPlayer.Play();
                m_btnPlay.GetComponent<Image>().SetSprite("camera_pause_btn2");
            }
            else
            {
                Log.Debug("UICameraWidget Pause: " + _VLCPlayer.path);
                _VLCPlayer.Pause();
                m_btnPlay.GetComponent<Image>().SetSprite("camera_play_btn2");
            }
        }
        private async UniTaskVoid OnClickMoveBtn()
        {
            if (subCamGroup == null)
            {
                Log.Warning("场景内找不到SubCameraGroup对象！");
                return;
            }
            if (wander == null)
            {
                Log.Warning("场景内找不到Wander脚本！");
                return;
            }
            int childIdx = currentID - 1;
            if (childIdx < 0 || childIdx >= subCamGroup.transform.childCount)
                return;
            var child = subCamGroup.transform.GetChild(childIdx); //获取次相机prefab
            var bScript = child.GetComponentInChildren<BillboardScript>();
            (parent as UICameraWindow)?.OnClickCloseBtn().Forget();
            wander.ProcessBillboard(bScript);
        }
        private async UniTaskVoid OnClickCloudControlBtn()
        {
            await UniTask.Yield();
        }
        #endregion

        protected override void OnCreate()
        {
            base.OnCreate();
            subCamGroup = GameObject.FindWithTag("SubCameraGroup");
            wander = GameObject.FindWithTag("LevelManager").GetComponent<Wander>();
        }

        public void Refresh(RspCameraInfoDTO info)
        {
            Log.Info("刷新UICameraWidget: " + info.streamUrl);
            _VLCPlayer.Open(info.streamUrl);
            currentID = info.id;
            m_textCamType.text = "型号：" + info.cameraType;
            m_textCamIp.text = "IP：" + info.address;
            m_btnPlay.GetComponent<Image>().SetSprite("camera_pause_btn2");
        }

        protected override void OnDestroy()
        {
            _VLCPlayer.Stop();
        }
    }
}
