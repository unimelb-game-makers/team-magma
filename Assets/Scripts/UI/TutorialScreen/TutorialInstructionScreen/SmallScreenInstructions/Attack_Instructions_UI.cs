using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    public class Attack_Instructions_UI: MonoBehaviour
    {
        [SerializeField] private TutorialInstructionScreenManager tutorialInstructionScreenManager;

        private bool hasAttacked = false;

        private void Update()
        {
            if (Input.GetMouseButtonDown(0)) hasAttacked = true;

            if (hasAttacked)
            {
                tutorialInstructionScreenManager.HideSmallBasicAttack1Screen();

                // Deselect the currently selected UI element
                EventSystem.current.SetSelectedGameObject(null);
            }
        }
    }
}