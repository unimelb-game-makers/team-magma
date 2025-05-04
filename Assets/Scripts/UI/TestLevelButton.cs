using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Scenes;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TestLevelButton : MonoBehaviour
{
    [SerializeField] private int levelNumber;

    private void Start()
    {
        // Optional: auto-attach to a Button component
        GetComponent<Button>().onClick.AddListener(OnButtonClick);
    }

    private void OnButtonClick()
    {
        GameManager.Instance.LoadLevelNumber(levelNumber);
    }
}
