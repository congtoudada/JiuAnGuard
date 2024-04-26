using System.Collections;
using System.Collections.Generic;
using GameMain;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace GameLogic
{
    public class DepthPass : CustomPostProcessPassBase
    {
        public DepthPass(RenderPassEvent evt, Shader shader, string profileTag = "") : base(evt, shader, profileTag)
        {
            volume = VolumeManager.instance.stack.GetComponent<DepthVolume>();
        }

        public override CustomVolumeBase OnTryGetVolume()
        {
            return VolumeManager.instance.stack.GetComponent<DepthVolume>();
        }

        protected override void OnRender(CommandBuffer cmd, ref RenderingData renderingData)
        {
            Blitter.BlitCameraTexture(cmd, m_CameraColorHandle, m_CameraColorHandle, m_Material, 0);
        }
    }
}
