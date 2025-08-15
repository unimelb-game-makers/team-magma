using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;
using Narrative;

[RequireComponent(typeof(Collider))]
public class InteractableItem : MonoBehaviour
{
    [Header("Ink Story JSON")]
    [SerializeField]
    private TextAsset inkJSONAsset;
    private Story _story;
    [SerializeField] private string _storylets_prefix = "story_";
    [SerializeField] private float _interactionRange = 5f;
    private StoryletsManager _storylets_manager = null;
    private GameObject _player = null;
    private Coroutine _playerInRangeCheckCoroutine = null;

    // Start is called before the first frame update
    void Start()
    {
        _story = new Story(inkJSONAsset.text);
        _storylets_prefix = _storylets_prefix + "_";
        _storylets_manager = new StoryletsManager(_story);
        _storylets_manager.AddStorylets(_storylets_prefix);
        _storylets_manager.Refresh();
    }

    private void OnMouseDown()
    {
        Interact();
    }

    public void Interact()
    {
        UserInterface.InteractIndicator.Instance.HideUI();

        if (!IsPlayerInRange()) return;

        if (PlayerStateManager.Instance.IsCombat())
        {
            Debug.Log("Can't interact with items during combat!");
            return;
        }

        string _storylet_to_play = _storylets_manager.PickPlayableStorylet();
        DialogueUI.Instance.Talk(_story, _storylet_to_play);

        _playerInRangeCheckCoroutine = StartCoroutine(PlayerInRangeCheck());
    }

    private bool IsPlayerInRange()
    {
        if (_player == null) _player = GameObject.FindGameObjectWithTag("Player");
        //check distance 
        return Vector3.Distance(_player.transform.position, transform.position) < _interactionRange;
    }

    /// <summary>
    /// Coroutine to check if player is in range
    /// </summary>
    /// <returns></returns>
    private IEnumerator PlayerInRangeCheck()
    {
        while (true)
        {
            if (!IsPlayerInRange())
            {
                StopCoroutine(_playerInRangeCheckCoroutine);
                _playerInRangeCheckCoroutine = null;
                DialogueUI.Instance.HideUI();
                yield break;
            }
            yield return new WaitForSeconds(1f);
        }
    }

    void Update()
    {
        _storylets_manager?.Tick();//JASPER WROTE THIS

        if (Input.GetMouseButtonDown(0))  // Left mouse button
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // Get the hit object
                GameObject hitObject = hit.collider.gameObject;

                // Check if the hit object is this object or a child of this object
                if (hitObject == gameObject || hitObject.transform.IsChildOf(transform))
                {
                    Interact();
                }
            }
        }
    }
}
