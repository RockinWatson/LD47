using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Console : MonoBehaviour
{
    [SerializeField] private Light _blueLight = null;
    [SerializeField] private Light _greenLight = null;
    [SerializeField] private Light _yellowLight = null;
    [SerializeField] private Light _magentaLight = null;
    private Dictionary<Light, float> _lightStartingIntensityMap = new Dictionary<Light, float>();

    [SerializeField] private SpriteRenderer _dialRod = null;
    [SerializeField] private GameObject _dialRodPivot = null;

    [SerializeField] private SpriteRenderer[] _bandMemberDials = new SpriteRenderer[4];
    [SerializeField] private Color _bandMemberDialColorOff = new Color(31, 23, 23, 255);

    static private Console _instance = null;
    static public Console Get() { return _instance; }

    private void Awake()
    {
        if (_instance != null)
        {
            Debug.LogError("Something fucky happened - should only be 1 Console");
            return;
        }

        _instance = this;

        _lightStartingIntensityMap.Add(_blueLight, _blueLight.intensity);
        _lightStartingIntensityMap.Add(_greenLight, _greenLight.intensity);
        _lightStartingIntensityMap.Add(_yellowLight, _yellowLight.intensity);
        _lightStartingIntensityMap.Add(_magentaLight, _magentaLight.intensity);

        BandMember.OnMemberPlaying += OnBandMemberPlaying;
        BandMember.OnMemberStunned += OnBandMemberStunned;
    }

    private void OnDestroy()
    {
        BandMember.OnMemberPlaying -= OnBandMemberPlaying;
        BandMember.OnMemberStunned -= OnBandMemberStunned;
    }

    private void OnBandMemberPlaying(BandMember member)
    {
        _bandMemberDials[(int)member.GetMemberType()].color = Color.white;
    }

    private void OnBandMemberStunned(BandMember member)
    {
        _bandMemberDials[(int)member.GetMemberType()].color = _bandMemberDialColorOff;
    }

    public void BounceHitScoreLight(SequenceNote.HitScore hitScore)
    {
        Light light = GetHitScoreLight(hitScore);
        BounceLight(light, 4, 0.2f);
    }

    private Light GetHitScoreLight(SequenceNote.HitScore hitScore)
    {
        switch(hitScore)
        {
            case SequenceNote.HitScore.EXCELLENT:
                return _blueLight;
            case SequenceNote.HitScore.GOOD:
                return _greenLight;
            case SequenceNote.HitScore.OKAY:
                return _yellowLight;
            case SequenceNote.HitScore.BAD:
                return _magentaLight;
            case SequenceNote.HitScore.MISSED:
                return _magentaLight;
            default:
                return _magentaLight;
        }
    }

    public void BounceLight(Light light, int numBounces, float bounceLength)
    {
        IEnumerator coroutine = BounceLightRoutine(light, numBounces, bounceLength);
        StartCoroutine(coroutine);
        //StartCoroutine(BounceLightRoutine(light, numBounces, bounceLength));
    }

    private IEnumerator BounceLightRoutine(Light light, int numBounces, float bounceLength)
    {
        float startingIntensity = _lightStartingIntensityMap[light];
        light.intensity = 0f;

        for(int i = 0; i < numBounces; ++i)
        {
            yield return TransitionLight(light, 0f, startingIntensity, bounceLength);

            yield return TransitionLight(light, startingIntensity, 0f, bounceLength);
        }

        yield return TransitionLight(light, 0f, startingIntensity, bounceLength);
    }

    private IEnumerator TransitionLight(Light light, float startingIntensity, float endingIntensity, float time)
    {
        float transition = endingIntensity - startingIntensity;

        float timer = 0f;
        while (true)
        {
            timer += Time.deltaTime;
            if (timer >= time)
            {
                break;
            }

            light.intensity = startingIntensity + timer / time * transition;

            yield return null;
        }
    }
}
