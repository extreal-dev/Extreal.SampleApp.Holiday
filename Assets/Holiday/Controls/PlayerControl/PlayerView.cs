namespace Extreal.SampleApp.Holiday.Holiday.Controls.PlayerControl
{
    using Cinemachine;
    using UnityEngine;

    public class PlayerView : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera playerFollowCamera;

        private void Start() => playerFollowCamera.gameObject.SetActive(false);

        public void FollowPlayer(Transform player)
        {
            playerFollowCamera.Follow = player;
            playerFollowCamera.gameObject.SetActive(true);
        }

        public void UnfollowPlayer()
        {
            playerFollowCamera.Follow = null;
            playerFollowCamera.gameObject.SetActive(false);
        }
    }
}
