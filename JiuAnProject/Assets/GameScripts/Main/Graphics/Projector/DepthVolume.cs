using System;
using System.Collections;
using System.Collections.Generic;
using GameMain;
using UnityEngine;
using UnityEngine.Rendering;

namespace GameLogic
{
#if UNITY_EDITOR
    [VolumeComponentMenu("Custom/Depth")]
#endif
    [Serializable]
    public class DepthVolume : CustomVolumeBase
    {
        public BoolParameter isOn = new BoolParameter(true);
        
        public override bool IsActive()
        {
            return AnyPropertiesIsOverridden() && isOn.value;
        }
    }
}
