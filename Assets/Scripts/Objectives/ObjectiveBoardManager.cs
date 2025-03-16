using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class ObjectiveBoardManager : MonoBehaviour
{
    public ObjectiveSet currentObjectiveSet;
    public GameObject objectiveTextPrefab;
    public Transform objectiveBoard;
    public Sprite defaultSprite;
    public Sprite completedSprite;

    private void Start()
    {
        if (currentObjectiveSet != null)
        {
            DisplayObjectives(currentObjectiveSet.objectives);
        }
        else
        {
            Debug.LogWarning("ObjectiveSet is not assigned!");
        }
    }

    public void DisplayObjectives(List<Objective> objectives)
    {
        // Sort objectives so completed ones appear first
        var sortedObjectives = objectives.OrderByDescending(obj => obj.IsCompleted).ToList();

        // Clear existing objectives (except the title)
        foreach (Transform child in objectiveBoard)
        {
            if (child.CompareTag("ObjectiveItem")) // Only destroy objects tagged as "ObjectiveItem"
            {
                Destroy(child.gameObject);
            }
        }

        // Display each sorted objective
        foreach (Objective objective in sortedObjectives)
        {
            GameObject objText = Instantiate(objectiveTextPrefab, objectiveBoard);

            // Find the TMP text component within the prefab (assuming it's a child)
            TextMeshProUGUI textComponent = objText.GetComponentInChildren<TextMeshProUGUI>();
            Image iconImage = objText.GetComponentInChildren<Image>();

            if (textComponent != null)
            {
                textComponent.text = $"{objective.Title}: {objective.Description}";

                if (objective.IsCompleted)
                {
                    textComponent.fontStyle = FontStyles.Strikethrough;  // Add strikethrough

                    if (iconImage != null)
                    {
                        // Replace with your specified completed image
                        iconImage.sprite = completedSprite;  // Assign the completed sprite in the Inspector
                    }
                }
                else
                {
                    textComponent.fontStyle = FontStyles.Normal;  // Reset to normal if not completed

                    if (iconImage != null)
                    {
                        // Replace with your specified default image
                        iconImage.sprite = defaultSprite;  // Assign the default sprite in the Inspector
                    }
                }
            }
            else
            {
                Debug.LogWarning("TextMeshProUGUI component not found in the objective prefab!");
            }
        }
    }


    public void UpdateObjectiveStatus(Objective objective, bool isCompleted)
    {
        objective.IsCompleted = isCompleted;
        DisplayObjectives(currentObjectiveSet.objectives); // Refresh display
    }
}


