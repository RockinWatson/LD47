using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Speakers : MonoBehaviour
{
    public enum SpeakerSide
    {
        LEFT = 0,
        RIGHT = 1,
    }
    [SerializeField] private SpeakerSide _speakerSide = SpeakerSide.LEFT;

    [SerializeField] private Animator[] _speakerMajorAnims = null;
    [SerializeField] private Animator[] _speakerMiniAnims = null;
    [SerializeField] private Light[] _lights = null;

    [SerializeField] private float _animTimePerLight = 0.25f;
    private float _animTimeLight;
    private int _animLightIndex = 0;

    private bool _isAllPlaying = false;

    private void PlaySpeakerMajorAnims(bool play)
    {
        PlayAnims(_speakerMajorAnims, play);
    }

    private void PlaySpeakerMiniAnims(bool play)
    {
        PlayAnims(_speakerMiniAnims, play);
    }

    private void PlayAnims(Animator[] anims, bool play)
    {
        foreach (var anim in anims)
        {
            anim.speed = play ? 1f : 0f;
        }
    }

    private void PlayLightsAnim()
    {
        ResetLights();
    }

    private void ResetLights(bool skipIntensity=false)
    {
        _animLightIndex = 0;
        _animTimeLight = 0f;
        if (!skipIntensity)
        {
            foreach (var light in _lights)
            {
                light.intensity = 0f;
            }
        }
    }

    private void Update()
    {
        UpdateSensors();

        if (_isAllPlaying)
        {
            UpdateLightsAnim();
        }
    }

    private void UpdateSensors()
    {
        if(_speakerSide == SpeakerSide.LEFT)
        {
            if(BandManager.Get().IsAllLeftBandPlaying())
            {
                PlayAll();
            }
            else if(BandManager.Get().IsOneLeftBandPlaying())
            {
                PlayOne();
            }
            else
            {
                PlayNone();
            }
        }
        else
        {
            if (BandManager.Get().IsAllRightBandPlaying())
            {
                PlayAll();
            }
            else if (BandManager.Get().IsOneRightBandPlaying())
            {
                PlayOne();
            }
            else
            {
                PlayNone();
            }
        }
    }

    private void PlayAll()
    {
        if (!_isAllPlaying)
        {
            PlaySpeakerMajorAnims(true);
            PlaySpeakerMiniAnims(true);
            PlayLightsAnim();
            _isAllPlaying = true;
        }
    }

    private void PlayOne()
    {
        PlaySpeakerMajorAnims(true);
        PlaySpeakerMiniAnims(false);
        //ResetLights();
        _isAllPlaying = false;
    }

    private void PlayNone()
    {
        PlaySpeakerMajorAnims(false);
        PlaySpeakerMiniAnims(false);
        ResetLights();
        _isAllPlaying = false;
    }

    private void UpdateLightsAnim()
    {
        if(_animLightIndex < _lights.Length)
        {
            _animTimeLight += Time.deltaTime;
            if (_animTimeLight < _animTimePerLight)
            {
                _lights[_animLightIndex].intensity = _animTimeLight / _animTimePerLight;
            }
            else
            {
                _lights[_animLightIndex].intensity = 1f;
                _animTimeLight = 0f;
                ++_animLightIndex;
            }
        }
        else
        {
            ResetLights(true);
        }
    }
}
