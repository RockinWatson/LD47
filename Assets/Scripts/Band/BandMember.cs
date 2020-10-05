using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts;

public class BandMember : MonoBehaviour
{
    [SerializeField] private BandManager.BandMemberType _memberType;
    public BandManager.BandMemberType GetMemberType() { return _memberType; }

    public enum BandMemberStatus
    {
        NOT_PLAYING = 0,
        STUNNED = 1,
        PLAYING = 2,
    }
    private BandMemberStatus _status = BandMemberStatus.NOT_PLAYING;
    public BandMemberStatus GetStatus() { return _status; }

    [SerializeField] private Animator _animator = null;

    private void Update()
    {
        if(IsNotPlaying())
        {
            //@TODO: Pause anim.
            _animator.speed = 0f;
        }
        else
        {
            //@TODO: Play anim.
            _animator.speed = 1f;
        }
    }

    public bool IsNotPlaying()
    {
        return _status == BandMemberStatus.NOT_PLAYING || _status == BandMemberStatus.STUNNED;
    }

    public void SetPlaying()
    {
        _status = BandMemberStatus.PLAYING;

        AudioManager.Music music = BandManager.GetMusicByMemberType(_memberType);
        AudioManager.Get().UnMute(music);
    }
}
