using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITriggerCheckable
{
    bool IsWithinAggroRange { get; set; }
    bool IsWithinAttackRange { get; set; }
    bool HasReachedPatrolPoint { get; set; }
    void SetAggroRangeBool(bool isWithinAggroRange);
    void SetAttackRangeBool(bool isWithinAttackRange);
    void SetHasReachedPatrolPointBool(bool hasReachedPatrolPoint);
}
