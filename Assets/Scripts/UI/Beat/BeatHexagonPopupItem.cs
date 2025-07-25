using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BeatHexagonPopupItem : MonoBehaviour
{
    [SerializeField] private Image image;

    [SerializeField] private RectTransform rectTransform;

    public RectTransform Rect => rectTransform;
    public Image Image => image;
}
