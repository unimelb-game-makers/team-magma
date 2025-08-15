using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Tempo;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Utilities.ServiceLocator;

public class TapeColorSwitcher : MonoBehaviour, ISyncable
{
    [SerializeField] Image[] PreviousTapeSprite;
    [SerializeField] Image[] CurrentTapeSprite;
    [SerializeField] Image[] AfterTapeSprite;
    Color FastTapeColor;
    Color DefaultTapeColor;
    Color SlowTapeColor;
    [SerializeField] TapeColors tapeColors;
    private Sequence _sequence;
    [SerializeField] private float switchDuration = 0.5f;

    void Start()
    {
        _sequence = DOTween.Sequence();
        SceneManager.sceneLoaded += OnSceneLoaded;
        FastTapeColor = tapeColors.GetColor(TempoMode.Fast);
        DefaultTapeColor = tapeColors.GetColor(TempoMode.Default);
        SlowTapeColor = tapeColors.GetColor(TempoMode.Slow);

        Affect(TempoMode.Default);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ServiceLocator.Instance.Register<ISyncable>(this);

    }

    public void Affect(TempoMode mode)
    {
        _sequence?.Kill();                    
        _sequence = DOTween.Sequence();       
        switch (mode)
        {
            case TempoMode.Slow:
                SetSpriteColor(DefaultTapeColor, PreviousTapeSprite);
                SetSpriteColor(SlowTapeColor, CurrentTapeSprite);
                SetSpriteColor(FastTapeColor, AfterTapeSprite);

                break;
            case TempoMode.Fast:
                SetSpriteColor(SlowTapeColor, PreviousTapeSprite);
                SetSpriteColor(FastTapeColor, CurrentTapeSprite);
                SetSpriteColor(DefaultTapeColor, AfterTapeSprite);

                break;
            case TempoMode.Default:
                SetSpriteColor(FastTapeColor, PreviousTapeSprite);
                SetSpriteColor(DefaultTapeColor, CurrentTapeSprite);
                SetSpriteColor(SlowTapeColor, AfterTapeSprite);


                break;


        }
    }
    public Color GetColor(TempoMode mode)
    {
        switch (mode)
        {
            case TempoMode.Slow:
                return SlowTapeColor;
            case TempoMode.Fast:
                return FastTapeColor;
            case TempoMode.Default:
                return DefaultTapeColor;

        }
        return DefaultTapeColor;
    }
    void SetSpriteColor(Color color, Image[] sprites)
    {
        foreach (Image sprite in sprites)
        {
            _sequence.Join(sprite.DOColor(color, switchDuration));

        }
    }
}
