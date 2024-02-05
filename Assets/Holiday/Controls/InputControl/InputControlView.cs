using UnityEngine;

namespace Extreal.SampleApp.Holiday.Controls.InputControl
{
    public class InputControlView : MonoBehaviour
    {
        [SerializeField] private GameObject joystickCanvas;

        public void SwitchJoystickVisibility(bool isVisible)
            => joystickCanvas.SetActive(isVisible);
    }
}
