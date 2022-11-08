namespace Extreal.SampleApp.Holiday.Models
{
    using System;
    using UnityEngine;

    [Serializable]
    public class Avatar
    {
        [SerializeField] private AvatarName avatarName;
        [SerializeField] private string assetName;

        public AvatarName AvatarName => avatarName;
        public string AssetName => assetName;
    }
}
