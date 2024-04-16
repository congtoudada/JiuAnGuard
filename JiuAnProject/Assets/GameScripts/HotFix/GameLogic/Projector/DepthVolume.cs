using System.Collections;
using System.Collections.Generic;
using GameMain;
using UnityEngine;
using UnityEngine.Rendering;

namespace GameLogic
{
    [System.Serializable, VolumeComponentMenu("Custom/Depth")]
    public class DepthVolume : CustomVolumeBase
    {
        public BoolParameter isOn = new BoolParameter(true);
        
        public override bool IsActive()
        {
            return AnyPropertiesIsOverridden() && isOn.value;
        }
    }
}
