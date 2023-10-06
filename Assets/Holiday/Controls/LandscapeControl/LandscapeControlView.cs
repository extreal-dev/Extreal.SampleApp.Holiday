using System.Diagnostics.CodeAnalysis;
using System.Linq;
using UnityEngine;

namespace Extreal.SampleApp.Holiday.Controls.LandscapeControl
{
    public class LandscapeControlView : MonoBehaviour
    {
        [SerializeField] private GameObject stageRoot;
        [SerializeField] private MeshCollider moveLimit;

        private Mesh mesh;

        [SuppressMessage("Style", "IDE0051")]
        private void Awake()
        {
            mesh = Instantiate(moveLimit.sharedMesh);
            mesh.triangles = mesh.triangles.Reverse().ToArray();
            moveLimit.sharedMesh = mesh;
        }

        [SuppressMessage("Style", "IDE0051")]
        private void OnDestroy()
        {
            if (mesh != null)
            {
                Destroy(mesh);
                mesh = null;
            }
        }

        public void SetStageActive(bool value)
            => stageRoot.SetActive(value);
    }
}
