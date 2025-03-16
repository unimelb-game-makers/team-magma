using UnityEngine;

public class PlayerStateManager : MonoBehaviour
{
    public static PlayerStateManager Instance { get; private set; }
    public PlayerState CurrentState { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
        SetState(PlayerState.Normal);
    }

    public void SetState(PlayerState newState)
    {
        if (CurrentState == newState) return;

        CurrentState = newState;
        Debug.Log("Player state changed to: " + newState);

        switch (newState)
        {
            case PlayerState.Normal:
                CombatUISystem.Instance.StopBeat();
                break;
            case PlayerState.Combat:
                CombatUISystem.Instance.StartBeat();
                break;
        }
    }

    public void SetNormalState()
    {
        SetState(PlayerState.Normal);
    }

    public void SetCombatState()
    {
        SetState(PlayerState.Combat);
    }

    public void SwitchState()
    {
        if (IsCombat()) {
            SetState(PlayerState.Normal);
        } else
        {
            SetState(PlayerState.Combat);
        }
        
    }

    public bool IsCombat()
    {
        return CurrentState == PlayerState.Combat;
    }
}
