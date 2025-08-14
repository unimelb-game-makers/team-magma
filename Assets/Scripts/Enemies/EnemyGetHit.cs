using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;
using DG.Tweening;

public class EnemyGetHit : MonoBehaviour
{
    public Renderer targetRenderer; // MeshRenderer or SkinnedMeshRenderer
    public float flashDuration = 0.2f;
    public GameObject HitEffectPrefab;
    public float effectLifetime = 0.5f;
    private Color originalColor;

    void Awake()
    {
        originalColor = targetRenderer.material.color;
    }

    public void GetHit()
    {

    }

}