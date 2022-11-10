namespace Extreal.SampleApp.Holiday.Models
{
    using System;
    using UnityEngine;

    [Serializable]
    public class Avatar
    {
        [SerializeField] private string name;
        [SerializeField] private string assetName;

        public string Name => name;
        public string AssetName => assetName;
    }
}
