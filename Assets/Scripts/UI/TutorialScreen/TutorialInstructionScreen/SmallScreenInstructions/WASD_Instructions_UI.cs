using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    public class WASD_Instructions: MonoBehaviour
    {
        [SerializeField] private TutorialInstructionScreenManager tutorialInstructionScreenManager;

        private bool hasPressedUp = false;
        private bool hasPressedDown = false;
        private bool hasPressedLeft = false;
        private bool hasPressedRight = false;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) hasPressedUp = true;
            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) hasPressedLeft = true;
            if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) hasPressedDown = true;
            if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) hasPressedRight = true;

            if (hasPressedUp && hasPressedLeft && hasPressedDown && hasPressedRight)
            {
                tutorialInstructionScreenManager.HideSmallMoveScreen();

                // Deselect the currently selected UI element
                EventSystem.current.SetSelectedGameObject(null);
            }
        }
    }
}