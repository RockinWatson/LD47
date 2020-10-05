using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BandMember : MonoBehaviour
{
    [SerializeField] private BandManager.BandMemberType _memberType;
    public BandManager.BandMemberType GetMemberType() { return _memberType; }

    private BandManager.BandMemberStatus _status = BandManager.BandMemberStatus.NOT_PLAYING;
    public BandManager.BandMemberStatus GetStatus() { return _status; }

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

    private bool IsNotPlaying()
    {
        return _status == BandManager.BandMemberStatus.NOT_PLAYING || _status == BandManager.BandMemberStatus.STUNNED;
    }
}
