using System.Collections.Generic;
using Extreal.Integration.Multiplay.Messaging;
using UnityEngine;

namespace Extreal.SampleApp.Holiday
{
    [CreateAssetMenu(
        menuName = nameof(Holiday) + "/" + nameof(NetworkObjectsProvider),
        fileName = nameof(NetworkObjectsProvider))]
    public class NetworkObjectsProvider : ScriptableObject, INetworkObjectsProvider
    {
        [SerializeField] private GameObject playerPrefab;

        public List<GameObject> Provide()
            => new List<GameObject> { playerPrefab };
    }
}
