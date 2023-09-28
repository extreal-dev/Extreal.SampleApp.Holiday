using System;
using System.Collections.Generic;
using UnityEngine;

namespace Extreal.SampleApp.Holiday.App.Config
{
    [CreateAssetMenu(
        menuName = "Holiday/" + nameof(SpaceConfig),
        fileName = nameof(SpaceConfig))]
    public class SpaceConfig : ScriptableObject
    {
        [Serializable]
        public class Space
        {
            [SerializeField] private string spaceName;
            [SerializeField] private StageName stageName;
            [SerializeField] private LandscapeType landscapeType;

            public string SpaceName => spaceName;
            public StageName StageName => stageName;
            public LandscapeType LandscapeType => landscapeType;
        }

        [SerializeField] private List<Space> spaces;

        public List<Space> Spaces => spaces;
    }
}
