// Author : Peiyu Wang @ Daphatus
// 07 01 2025 01 08

using System;
using TMPro;

namespace Narrative
{
    using UnityEngine;
    using UnityEngine.UI;
    using Ink.Runtime;

    public class DialogueController : MonoBehaviour, IUIHandler
    {
        [Header("Ink Story JSON")] [SerializeField]
        private TextAsset inkJSONAsset; // Assign the compiled LvlPocLeaderStory.json here

        [Header("UI Elements")] [SerializeField]
        private TextMeshProUGUI dialogueText; // The main text field for displaying dialogue

        [SerializeField] private GameObject choiceButtonPrefab; // A prefab for a button that represents a choice
        [SerializeField] private Transform choicePanel; // A container (e.g., VerticalLayoutGroup) for choice buttons

        private Story _story;

        private void Start()
        {
            // Load the Ink Story
            _story = new Story(inkJSONAsset.text);

            // Begin the story from the first line
            RefreshView();
        }
        
        private void RefreshView()
        {
            // Clear out previous choices
            RemoveChoiceButtons();

            dialogueText.text = ""; // Clear the text field before adding new content

            while (_story.canContinue)
            {
                var nextLine = _story.Continue().Trim();
                // Append instead of overwriting
                dialogueText.text += nextLine + "\n";
            }

            // If there are choices, create choice buttons
            if (_story.currentChoices.Count > 0)
            {
                for (int i = 0; i < _story.currentChoices.Count; i++)
                {
                    CreateChoiceButton(_story.currentChoices[i]);
                }
            }
            else
            {
                // No more choices? Could mean the conversation is over, or story ended.
                // You could hide the UI or trigger something else here.
            }
        }

        private void CreateChoiceButton(Choice choice)
        {
            // Instantiate a new button
            GameObject buttonGO = Instantiate(choiceButtonPrefab, choicePanel);
            Button buttonComponent = buttonGO.GetComponent<Button>();

            // Set button text
            TextMeshProUGUI buttonText = buttonGO.GetComponentInChildren<TextMeshProUGUI>();
            buttonText.text = choice.text;

            // When the button is clicked, choose this choice
            buttonComponent.onClick.AddListener(delegate { OnChoiceSelected(choice); });
        }

        private void OnChoiceSelected(Choice choice)
        {
            // Tell the Ink story which choice was selected
            _story.ChooseChoiceIndex(choice.index);
            RefreshView();
        }

        private void RemoveChoiceButtons()
        {
            // Clear out old choice buttons
            for (int i = choicePanel.childCount - 1; i >= 0; i--)
            {
                Destroy(choicePanel.GetChild(i).gameObject);
            }
        }

        public void ShowUI()
        {
            gameObject.SetActive(true);
        }

        public void HideUI()
        {
            gameObject.SetActive(false);
        }

        public void ToggleUI()
        {
            gameObject.SetActive(!gameObject.activeSelf);
        }
    }
}