using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewObjectiveSet", menuName = "Objectives/Objective Set")]
public class ObjectiveSet : ScriptableObject
{
    public List<Objective> objectives;
}
