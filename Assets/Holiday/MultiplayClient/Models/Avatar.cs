using System;
using UnityEngine;

namespace Extreal.SampleApp.Holiday.MultiplayClient.Models
{
    [Serializable]
    public class Avatar
    {
        [SerializeField] private string name;
        [SerializeField] private string assetName;

        public string Name => name;
        public string AssetName => assetName;
    }
}
