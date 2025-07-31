using System;
using System.Collections;
using System.Collections.Generic;
using Tempo;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Utilities.ServiceLocator;
[ExecuteInEditMode]
public class TapeScroll : MonoBehaviour, ISyncable
{
    TempoMode mode = TempoMode.Default;
     float angle = 0;
    [SerializeField] private float switchDuration = 1;
    [SerializeField] private float radius = 100;
    [SerializeField] private GameObject slowTape;
    [SerializeField] private GameObject defaultTape;
    [SerializeField] private GameObject fastTape;

    [SerializeField] float startingScale ;

    Coroutine runningCoroutine;

    void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        setTape();
    }

    public void Affect(TempoMode mode, float duration, float effectValue)
    {
        Debug.Log("switching tape");

        if (this.mode == mode)
        {
            return;
        }

        switch (mode)
        {
            case TempoMode.Slow:
                if (runningCoroutine != null) {
                    StopCoroutine(runningCoroutine);
                }
                runningCoroutine = StartCoroutine(Rotation(2* (float)Mathf.PI /3));
                break;
            case TempoMode.Fast:
                if (runningCoroutine != null) {
                    StopCoroutine(runningCoroutine);
                }
                runningCoroutine = StartCoroutine(Rotation(- 2* (float)Mathf.PI /3));

                break;
            case TempoMode.Default:
                if (runningCoroutine != null) {
                    StopCoroutine(runningCoroutine);
                }
                runningCoroutine = StartCoroutine(Rotation(0));
                break;


        }
        this.mode = mode;
    }


    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("register");
        ServiceLocator.Instance.Register<ISyncable>(this);

    }

    IEnumerator Rotation(float targetAngle)
    {   

        float fromAngle = angle;
        float toAngle = fromAngle + DeltaAngleRad(fromAngle, targetAngle);
        float elapsed = 0f;

        while (elapsed < switchDuration)
        {
            elapsed += Time.deltaTime;
            Debug.Log(elapsed);
            float t = elapsed / switchDuration;
            angle = Mathf.Lerp(fromAngle, toAngle, t);
            setTape();
            yield return null;
        }

        angle = toAngle; // Snap to exact end
    }

    float DeltaAngleRad(float from, float to)
    {
        float delta = to - from;
        delta = (delta + Mathf.PI) % (2 * Mathf.PI);
        //in c#, -1%2 = -1 rather than 1

        if (delta < 0)
            delta += 2 * Mathf.PI;
        return delta - Mathf.PI;
    }
    void setTape()
    {
        setPosition(slowTape, angle - 2 * (float)Math.PI / 3f);
        setPosition(defaultTape, angle);
        setPosition(fastTape, angle + 2 * (float)Math.PI / 3f);
    }
    void setPosition(GameObject tape, float angle)
    {
        float distance = 2f - Mathf.Cos(angle);
        if (distance > 2)
        {
            tape.transform.SetAsFirstSibling();
        }
        float scale = 1 / distance;
        tape.transform.localScale = Vector3.one * scale * startingScale;
        Vector3 position = Vector3.zero;
        position.y = Mathf.Sin(angle) * radius;
        tape.transform.localPosition = position;
    }


}
