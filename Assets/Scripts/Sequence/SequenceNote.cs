using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequenceNote : MonoBehaviour
{
    [SerializeField] private Sequence.Arrow _arrow;
    [SerializeField] private SpriteRenderer _sprite = null;

    public enum HitScore
    {
        MISSED = -1,
        NOT_HIT = 0,
        BAD = 1,
        OKAY = 2,
        GOOD = 3,
        EXCELLENT = 4,
        COUNT,
    }
    private HitScore _hitScore = HitScore.NOT_HIT;
    public HitScore GetHitScore() { return _hitScore; }

    public int SequenceID { get; set; }

    private bool _wasActivateable = false;
    private bool _wasHit = false;
    private bool _wasMissed = false;

    private void Update()
    {
        //@TODO: Check input to see if key is hit
        if(IsActivateable())
        {
            _wasActivateable = true;
            CheckInput();
        }
        else if(_wasActivateable && !_wasHit && !_wasMissed)
        {
            _hitScore = HitScore.MISSED;
            _wasMissed = true;
            NotifyNoteHit();
        }

        //@TODO: Move the note along...
        MoveUpdate();
    }

    private void CheckInput()
    {
        switch(_arrow)
        {
            case Sequence.Arrow.UP:
                if(Input.GetKeyDown(KeyCode.UpArrow))
                {
                    ProcessHit();
                }
                break;
            case Sequence.Arrow.LEFT:
                if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    ProcessHit();
                }
                break;
            case Sequence.Arrow.RIGHT:
                if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    ProcessHit();
                }
                break;
            case Sequence.Arrow.DOWN:
                if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    ProcessHit();
                }
                break;
            default:
                Debug.LogError("WTF?");
                break;
        }
    }

    private float GetDeltaXFromTarget()
    {
        Vector3 targetPos = SequenceManager.Get().GetTargetPos();
        Vector3 ourPos = this.transform.position;
        return Mathf.Abs(targetPos.x - ourPos.x);
    }

    private bool IsActivateable()
    {
        return _hitScore == HitScore.NOT_HIT && IsInActiveRange();
    }

    private bool IsInActiveRange()
    {
        return (GetDeltaXFromTarget() <= SequenceManager.Get().GetActiveTargetRange());
    }

    private void ProcessHit()
    {
        //@TODO: Check to see where in the range we are exactly and score the hit
        var targetRanges = SequenceManager.Get().GetTargetRanges();

        float deltaX = GetDeltaXFromTarget();
        if(deltaX <= targetRanges[(int)SequenceManager.TargetScore.EXCELLENT])
        {
            //Debug.Log("EXCELLENT!");
            _hitScore = HitScore.EXCELLENT;
            _sprite.color = Color.blue;
        }
        else if (deltaX <= targetRanges[(int)SequenceManager.TargetScore.GOOD])
        {
            //Debug.Log("GOOD!");
            _hitScore = HitScore.GOOD;
            _sprite.color = Color.green;
        }
        else if (deltaX <= targetRanges[(int)SequenceManager.TargetScore.OKAY])
        {
            //Debug.Log("OKAY!");
            _hitScore = HitScore.OKAY;
            _sprite.color = Color.yellow;
        }
        else if (deltaX <= targetRanges[(int)SequenceManager.TargetScore.BAD])
        {
            //Debug.Log("BAD!");
            _hitScore = HitScore.BAD;
            _sprite.color = Color.red;
        }
        else
        {
            //Debug.Log("MISSED!");
            _hitScore = HitScore.MISSED;
            _sprite.color = Color.magenta;
        }

        NotifyNoteHit();
    }

    private void MoveUpdate()
    {
        this.transform.Translate(-SequenceManager.Get().GetArrowSpeed() * Time.deltaTime, 0f, 0f);
    }

    private void OnBecameInvisible()
    {
        //@TODO: Remove ourselves from the queued sequence registry.
        QueuedSequence queuedSequence = SequenceManager.Get().QueuedSequences()[SequenceID];
        queuedSequence.RemoveNote(this);
        if(!queuedSequence.HasActiveNotes())
        {
            SequenceManager.Get().QueuedSequences().Remove(queuedSequence.ID);
        }

        Destroy(this.gameObject);
    }

    private void NotifyNoteHit()
    {
        //Debug.Log("Notify Note Hit");

        _wasHit = true;

        SequenceManager.Get().QueuedSequences()[SequenceID].NotifyNoteHit(this);
    }
}
