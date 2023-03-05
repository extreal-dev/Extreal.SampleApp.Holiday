using System;
using System.Collections.Generic;
using UnityEngine;

namespace Extreal.SampleApp.Holiday.App.Config
{
    [CreateAssetMenu(
        menuName = "Holiday/" + nameof(AvatarConfig),
        fileName = nameof(AvatarConfig))]
    public class AvatarConfig : ScriptableObject
    {
        [Serializable]
        public class Avatar
        {
            [SerializeField] private string name;
            [SerializeField] private string assetName;

            public string Name => name;
            public string AssetName => assetName;
        }

        [SerializeField] private List<Avatar> avatars;

        public List<Avatar> Avatars => avatars;
    }
}
