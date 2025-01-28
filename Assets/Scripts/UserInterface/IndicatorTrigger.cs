// Author : Peiyu Wang @ Daphatus
// 28 01 2025 01 17

using UnityEngine;

namespace UserInterface
{
    public class IndicatorTrigger : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            InteractIndicator.Instance.ShowUI();
        }

        private void OnTriggerExit(Collider other)
        {
            InteractIndicator.Instance.HideUI();
        }
    }
}