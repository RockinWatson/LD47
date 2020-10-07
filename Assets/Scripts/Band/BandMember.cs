using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts;

public class BandMember : MonoBehaviour
{
    [SerializeField] private BandManager.BandMemberType _memberType;
    public BandManager.BandMemberType GetMemberType() { return _memberType; }

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
            _status = BandMemberStatus.PLAYING;

            AudioManager.Music music = BandManager.GetMusicByMemberType(_memberType);
            AudioManager.Get().UnMute(music);

            OnMemberPlaying.Invoke(this);
        }
    }

    public void Stun()
    {
        if (_status != BandMemberStatus.STUNNED)
        {
            _status = BandMemberStatus.STUNNED;

            AudioManager.Music music = BandManager.GetMusicByMemberType(_memberType);
            AudioManager.Get().Mute(music);

            OnMemberStunned.Invoke(this);
        }
    }
}
