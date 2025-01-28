// Author : Peiyu Wang @ Daphatus
// 28 01 2025 01 26

using System;
using Narrative;
using TMPro;
using UnityEngine;

namespace UserInterface
{
    public class InteractIndicator : Singleton<InteractIndicator>, IUIHandler
    {
        [SerializeField] private GameObject interactIndicator;
        [SerializeField] private TextMeshProUGUI interactText;

        private void Start()
        {
            HideUI();
        }

        public void ShowUI()
        {
            interactIndicator.SetActive(true);
        }

        public void HideUI()
        {
            interactIndicator.SetActive(false);
        }

        public void ToggleUI()
        {
            interactIndicator.SetActive(!interactIndicator.activeSelf);
        }
    }
}