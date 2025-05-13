using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    public class SlowTape_Instructions_UI: MonoBehaviour
    {
        [SerializeField] private TutorialInstructionScreenManager tutorialInstructionScreenManager;

        private bool hasAccessedTape = false;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1) || 
                Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2) || 
                Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3) || 
                Input.GetMouseButtonDown(1)) hasAccessedTape = true;

            if (hasAccessedTape)
            {
                tutorialInstructionScreenManager.HideSmallSlowTapeScreen();

                // Deselect the currently selected UI element
                EventSystem.current.SetSelectedGameObject(null);
            }
        }
    }
}