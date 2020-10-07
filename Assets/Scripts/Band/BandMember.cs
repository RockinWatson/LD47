using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts;

public class BandMember : MonoBehaviour
{
    [SerializeField] private BandManager.BandMemberType _memberType;
    public BandManager.BandMemberType GetMemberType() { return _memberType; }

    private IEnumerator _slowDeathCoroutine = null;

    public delegate void BandMemberEvent(BandMember member);
    static public BandMemberEvent OnMemberPlaying;
    static public BandMemberEvent OnMemberStunned;

    public enum BandMemberStatus
    {
        NOT_PLAYING = 0,
        STUNNED = 1,
        PLAYING = 2,
    }
    private BandMemberStatus _status = BandMemberStatus.NOT_PLAYING;
    public BandMemberStatus GetStatus() { return _status; }

    [SerializeField] private Animator _animator = null;
    [SerializeField] private RuntimeAnimatorController _playAnim = null;
    [SerializeField] private RuntimeAnimatorController _idleAnim = null;

    private void Update()
    {
        if(IsNotPlaying())
        {
            _animator.runtimeAnimatorController = _idleAnim;

            //@TODO: Pause anim.
            //_animator.speed = 0f;
        }
        else
        {
            _animator.runtimeAnimatorController = _playAnim;

            //@TODO: Play anim.
            //_animator.speed = 1f;
        }
    }

    public bool IsNotPlaying()
    {
        return _status == BandMemberStatus.NOT_PLAYING || _status == BandMemberStatus.STUNNED;
    }

    public bool IsPlaying()
    {
        return !IsNotPlaying();
    }

    public void SetPlaying()
    {
        if (_status != BandMemberStatus.PLAYING)
        {
            SetStatus(BandMemberStatus.PLAYING);

            OnMemberPlaying.Invoke(this);
        }
    }

    public void Stun()
    {
        if (_status != BandMemberStatus.STUNNED && _status != BandMemberStatus.NOT_PLAYING)
        {
            SetStatus(BandMemberStatus.STUNNED);

            OnMemberStunned.Invoke(this);
        }
    }

    private void Mute()
    {
        AudioManager.Music music = BandManager.GetMusicByMemberType(_memberType);
        AudioManager.Get().Mute(music);
    }

    private void Unmute()
    {
        AudioManager.Music music = BandManager.GetMusicByMemberType(_memberType);
        AudioManager.Get().UnMute(music);
    }

    private void Pitch(float pitch)
    {
        AudioManager.Music music = BandManager.GetMusicByMemberType(_memberType);
        AudioManager.Get().Pitch(music, pitch);
    }

    public void SetStatus(BandMemberStatus status)
    {
        _status = status;
        switch(status)
        {
            case BandMemberStatus.PLAYING:
                Pitch(1f);
                Unmute();
                break;
            case BandMemberStatus.NOT_PLAYING:
            case BandMemberStatus.STUNNED:
            default:
                Mute();
                break;
        }
    }

    public void StunDeath()
    {
        Pitch(0.25f);
    }

    public void SlowDeath()
    {
        //_slowDeathCoroutine = SlowDeathRoutine();
        _slowDeathCoroutine = SlowDeathStepRoutine();
        StartCoroutine(_slowDeathCoroutine);
    }

    private IEnumerator SlowDeathRoutine()
    {
        float slowDeathTime = 3f;
        float timer = slowDeathTime;
        Unmute();
        AudioManager.Music music = BandManager.GetMusicByMemberType(_memberType);
        AudioManager.Get().Pitch(music, 1.0f);

        while(true)
        {
            timer -= Time.deltaTime;
            if(timer <= 0f)
            {
                break;
            }
            AudioManager.Get().Pitch(music, timer/slowDeathTime);

            yield return null;
        }

        //yield return null;
    }

    private IEnumerator SlowDeathStepRoutine()
    {
        float slowDeathTime = 3f;
        float timer = slowDeathTime;
        Unmute();
        AudioManager.Music music = BandManager.GetMusicByMemberType(_memberType);
        AudioManager.Get().Pitch(music, 0.75f);

        while (true)
        {
            timer -= Time.deltaTime;
            if (timer <= 2f)
            {
                break;
            }
            yield return null;
        }
        AudioManager.Get().Pitch(music, 0.5f);
        while (true)
        {
            timer -= Time.deltaTime;
            if (timer <= 1f)
            {
                break;
            }
            yield return null;
        }
        AudioManager.Get().Pitch(music, 0.25f);
        while (true)
        {
            if (timer <= 0f)
            {
                break;
            }
            yield return null;
        }
        AudioManager.Get().Pitch(music, 0f);

        //yield return null;
    }

    public void KillSlowDeathCoroutine()
    {
        StopCoroutine(_slowDeathCoroutine);
    }
}
