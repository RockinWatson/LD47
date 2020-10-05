using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequenceNote : MonoBehaviour
{
    [SerializeField] private Sequence.Arrow _arrow;
    [SerializeField] private SpriteRenderer _sprite = null;

    private void Update()
    {
        //@TODO: Check input to see if key is hit
        if(IsInActiveRange())
        {
            CheckInput();
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
            Debug.Log("EXCELLENT!");
            _sprite.color = Color.blue;
        }
        else if (deltaX <= targetRanges[(int)SequenceManager.TargetScore.GOOD])
        {
            Debug.Log("GOOD!");
            _sprite.color = Color.green;
        }
        else if (deltaX <= targetRanges[(int)SequenceManager.TargetScore.OKAY])
        {
            Debug.Log("OKAY!");
            _sprite.color = Color.yellow;
        }
        else if (deltaX <= targetRanges[(int)SequenceManager.TargetScore.BAD])
        {
            Debug.Log("BAD!");
            _sprite.color = Color.red;
        }
    }

    private void MoveUpdate()
    {
        this.transform.Translate(-SequenceManager.Get().GetArrowSpeed() * Time.deltaTime, 0f, 0f);
    }

    private void OnBecameInvisible()
    {
        Destroy(this.gameObject);
    }
}
