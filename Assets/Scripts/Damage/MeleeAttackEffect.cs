using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;

public class MeleeAttackEffect : MonoBehaviour
{
    private PlayerController playerController;
    private MeleeAttackBox _meleeAttackBox = null;

    void Start()
    {
        playerController = transform.parent.GetComponent<PlayerController>();
        if (playerController == null)
        {
            Debug.LogError("PlayerController not found on parent!");
        }
    }

    public void PlayWeakMeleeAttackEffect()
    {
        PlayMeleeAttackEffect(playerController.weakAttackRange, playerController.weakMeleeAttackRecoverTime, playerController.weakAttackDamage);
    }

    public void PlayStrongMeleeAttackEffect()
    {
        PlayMeleeAttackEffect(playerController.strongAttackRange, playerController.strongMeleeAttackRecoverTime, playerController.strongAttackDamage);
    }

    public void PlayMeleeAttackEffect(float attackRange, float attackRecoverTime, float attackDamage)
    {
        if (transform.parent == null)
        {
            Debug.LogWarning("No parent found for this object. Cannot determine player transform.");
            return;
        }

        Transform playerTransform = transform.parent;

        // Spawn position in front of the player
        Vector3 origin = playerTransform.position;
        Vector3 forward = (playerTransform.forward * playerController.attackForwardOffset) + origin;

        // Spawn slash VFX rotated to face player's forward direction
        GameObject attackBox = Instantiate(playerController.MeleeAttackPrefab, forward, Quaternion.LookRotation(playerTransform.forward));
        attackBox.transform.Rotate(-90f, 0f, 0f); // Rotate from Y-up to Z-forward

        // Scale the effect
        float scale = attackRange * 0.3f;
        attackBox.transform.localScale = new Vector3(scale, scale, scale);

        // Get the melee attack logic component
        _meleeAttackBox = attackBox.GetComponent<MeleeAttackBox>();

        // Parent the effect to the player if desired (or remove this line to keep it world-space)
        _meleeAttackBox.transform.parent = playerTransform;

        // Set damage value
        MeleeDamager damager = attackBox.GetComponent<MeleeDamager>();
        if (damager != null)
        {
            damager.Damage = attackDamage;
        }

        // Set target tags
        if (_meleeAttackBox != null)
        {
            var canAttackList = new List<string> { "Enemy" };
            _meleeAttackBox.SendMessage("EditCanAttack", canAttackList);
        }
    }
}
