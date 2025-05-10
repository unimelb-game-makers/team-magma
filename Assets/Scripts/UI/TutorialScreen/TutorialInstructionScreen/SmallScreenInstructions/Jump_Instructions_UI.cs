using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    public class Jump_Instructions_UI: MonoBehaviour
    {
        [SerializeField] private TutorialInstructionScreenManager tutorialInstructionScreenManager;

        private bool hasJumped = false;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space)) hasJumped = true;

            if (hasJumped)
            {
                tutorialInstructionScreenManager.HideSmallJumpScreen();

                // Deselect the currently selected UI element
                EventSystem.current.SetSelectedGameObject(null);
            }
        }
    }
}