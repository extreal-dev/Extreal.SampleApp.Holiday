using UnityEngine;

namespace Extreal.SampleApp.Holiday.App.Config
{
    [CreateAssetMenu(
        menuName = nameof(Holiday) + "/" + nameof(LandscapeConfig),
        fileName = nameof(LandscapeConfig))]
    public class LandscapeConfig : ScriptableObject
    {
        [SerializeField] private string baseUrl;

        public string BaseUrl => baseUrl;
    }
}
