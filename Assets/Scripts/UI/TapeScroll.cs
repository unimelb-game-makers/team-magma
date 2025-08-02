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
    //angle corresponds to the default tape. 
    float angle = 0;
    const float SlowTapeAngleOffset = - 2* (float)Mathf.PI /3;
    const float FastTapeAngleOffset = 2 * (float)Mathf.PI / 3;
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
                //rotate default tape to -fastTapeAngleOffset, 
                //slow tape will be at the display angle 
                runningCoroutine = StartCoroutine(Rotation(-SlowTapeAngleOffset));
                break;
            case TempoMode.Fast:
                if (runningCoroutine != null) {
                    StopCoroutine(runningCoroutine);
                }
                //rotate default tape to -fastTapeAngleOffset, 
                //fast tape will be at the display angle 
                runningCoroutine = StartCoroutine(Rotation(-FastTapeAngleOffset));

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
        ServiceLocator.Instance.Register<ISyncable>(this);

    }

    //rotate the angle of default tape to target angle
    IEnumerator Rotation(float targetAngle)
    {   

        float fromAngle = angle;
        float toAngle = fromAngle + DeltaAngleRad(fromAngle, targetAngle);
        float elapsed = 0f;

        while (elapsed < switchDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / switchDuration;
            angle = Mathf.Lerp(fromAngle, toAngle, t);
            setTape();
            yield return null;
        }

        angle = toAngle; 
    }

    // find the closest angle rotation between from and to 
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
        setPosition(slowTape, angle + SlowTapeAngleOffset);
        setPosition(defaultTape, angle);
        setPosition(fastTape, angle + FastTapeAngleOffset);
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
