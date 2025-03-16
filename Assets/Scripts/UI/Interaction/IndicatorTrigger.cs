// Author : Peiyu Wang @ Daphatus
// 28 01 2025 01 17

using UnityEngine;

namespace UserInterface
{
    public class IndicatorTrigger : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {

            // Should only be the player that can trigger this! -Ryan
            if (other.gameObject.GetComponent<Player.PlayerController>() != null && !PlayerStateManager.Instance.IsCombat()) 
            {
                InteractIndicator.Instance.ShowUI();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.GetComponent<Player.PlayerController>() != null) 
            {
                InteractIndicator.Instance.HideUI();
            }
        }
    }
}