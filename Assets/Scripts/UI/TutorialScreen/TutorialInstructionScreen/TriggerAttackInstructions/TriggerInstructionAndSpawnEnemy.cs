using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI;

public class TriggerInstructionAndSpawnEnemy : MonoBehaviour
{
    [SerializeField] private TutorialInstructionScreenManager tutorialInstructionScreenManager;
    [SerializeField] private TutorialScreenType screenToShow;
    [SerializeField] private GameObject spawnEnemy;

    private bool isTriggered = false;
    public void ShowInstructionScreen()
    {
        if (isTriggered) return;

        PauseMenuController.BlockEscapeKey(true);
        isTriggered = true;
        switch (screenToShow)
        {
            case TutorialScreenType.BasicAttack2:
                tutorialInstructionScreenManager.ShowBasicAttack2Screen();
                break;
            case TutorialScreenType.BasicAttack3:
                tutorialInstructionScreenManager.ShowBasicAttack3Screen();
                break;
            case TutorialScreenType.RangedAttack:
                tutorialInstructionScreenManager.ShowRangedAttackScreen();
                break;
            case TutorialScreenType.ShieldAttack:
                tutorialInstructionScreenManager.ShowShieldAttackScreen();
                break;
            case TutorialScreenType.HP:
                tutorialInstructionScreenManager.ShowHPScreen();
                break;
        }

        spawnEnemy.SetActive(true);
    }
}
