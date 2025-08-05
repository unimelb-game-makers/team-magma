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
    private TempoMode _mode = TempoMode.Default;
    //angle corresponds to the default tape. 
    private float _angle = 0;
    private const float SLOW_TAPE_ANGLE_OFFSET = - 2* (float)Mathf.PI /3;
    private const float FAST_TAPE_ANGLE_OFFSET = 2 * (float)Mathf.PI / 3;
    [SerializeField] private float switchDuration = 1;
    [SerializeField] private float radius = 100;
    [SerializeField] private GameObject slowTape;
    [SerializeField] private GameObject defaultTape;
    [SerializeField] private GameObject fastTape;

    [SerializeField] float startingScale ;
    private Coroutine _runningCoroutine;
    void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SetTape();
    }

    public void Affect(TempoMode mode)
    {
        if (_mode == mode)
        {
            return;
        }

        switch (mode)
        {
            case TempoMode.Slow:
                if (_runningCoroutine != null) {
                    StopCoroutine(_runningCoroutine);
                }
                //rotate default tape to -fastTapeAngleOffset, 
                //slow tape will be at the display angle 
                _runningCoroutine = StartCoroutine(Rotation(-SLOW_TAPE_ANGLE_OFFSET));
                break;
            case TempoMode.Fast:
                if (_runningCoroutine != null) {
                    StopCoroutine(_runningCoroutine);
                }
                //rotate default tape to -fastTapeAngleOffset, 
                //fast tape will be at the display angle 
                _runningCoroutine = StartCoroutine(Rotation(-FAST_TAPE_ANGLE_OFFSET));

                break;
            case TempoMode.Default:
                if (_runningCoroutine != null) {
                    StopCoroutine(_runningCoroutine);
                }
                _runningCoroutine = StartCoroutine(Rotation(0));
                break;


        }
        _mode = mode;
    }


    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ServiceLocator.Instance.Register<ISyncable>(this);

    }

    //rotate the angle of default tape to target angle
    private IEnumerator Rotation(float targetAngle)
    {   

        float fromAngle = _angle;
        float toAngle = fromAngle + DeltaAngleRad(fromAngle, targetAngle);
        float elapsed = 0f;

        while (elapsed < switchDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / switchDuration;
            _angle = Mathf.Lerp(fromAngle, toAngle, t);
            SetTape();
            yield return null;
        }

        _angle = toAngle; 
    }

    // find the closest angle rotation between from and to 
    private float DeltaAngleRad(float from, float to)
    {
        float delta = to - from;
        delta = (delta + Mathf.PI) % (2 * Mathf.PI);
        //in c#, -1%2 = -1 rather than 1

        if (delta < 0)
            delta += 2 * Mathf.PI;
        return delta - Mathf.PI;
    }

    private void SetTape()
    {
        SetPosition(slowTape, _angle + SLOW_TAPE_ANGLE_OFFSET);
        SetPosition(defaultTape, _angle);
        SetPosition(fastTape, _angle + FAST_TAPE_ANGLE_OFFSET);
    }

    private void SetPosition(GameObject tape, float angle)
    {
        float distance = 2f - Mathf.Cos(angle);
        if (distance > 2)
        {
            tape.transform.SetAsFirstSibling();
        }
        float scale = 1 / distance;
        tape.transform.localScale = Vector3.one * (scale * startingScale);
        Vector3 position = Vector3.zero;
        position.y = Mathf.Sin(angle) * radius;
        tape.transform.localPosition = position;
    }


}
