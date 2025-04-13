using System.Collections;
using Scenes;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    public class CreditPageManager : Singleton<CreditPageManager>
    {
        [SerializeField] private CanvasGroup firstCreditPageCanvasGroup;
        [SerializeField] private CanvasGroup secondCreditPageCanvasGroup;
        [SerializeField] private float fadeDuration = 0.5f;
        [SerializeField] private GameObject nextPageButton;
        [SerializeField] private GameObject closeButton;

        private void Start()
        {
            nextPageButton.GetComponent<Button>().onClick.AddListener(ShowSecondCreditPage);
            closeButton.GetComponent<Button>().onClick.AddListener(CloseCreditPage);
        }

        public void ShowFirstCreditPage()
        {
            firstCreditPageCanvasGroup.gameObject.SetActive(true);
            StartCoroutine(FadeIn(firstCreditPageCanvasGroup));
        }

        public void ShowSecondCreditPage()
        {
            secondCreditPageCanvasGroup.gameObject.SetActive(true);
            StartCoroutine(FadeIn(secondCreditPageCanvasGroup));
        }

        private IEnumerator FadeIn(CanvasGroup canvasGroup)
        {
            // Start the fade-in and wait for it to complete
            yield return SceneFadeManager.Instance.FadeCanvasGroup(canvasGroup, 0, 1, fadeDuration);
        }

        public void CloseCreditPage()
        {
            firstCreditPageCanvasGroup.gameObject.SetActive(false);
            firstCreditPageCanvasGroup.alpha = 0;
            
            StartCoroutine(FadeOutCreditPage());
        }

        private IEnumerator FadeOutCreditPage()
        {
            yield return SceneFadeManager.Instance.FadeCanvasGroup(secondCreditPageCanvasGroup, 1, 0, fadeDuration);

            secondCreditPageCanvasGroup.gameObject.SetActive(false);
        
            // Reset selected UI element
            EventSystem.current.SetSelectedGameObject(null);
        }
    }
}
