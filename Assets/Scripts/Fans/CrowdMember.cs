using Assets.Scripts;
using UnityEngine;

public class CrowdMember : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _rigidbody = null;
    public Rigidbody2D GetRigidbody() { return _rigidbody; }

    [SerializeField] private float _deathTime = 1f;
    private float _deathTimer = 0f;

    public BandMember BandMemberTarget { get; set; }

    public float MoveSpeed;

    private float _initialScaleDistSqrMag;

    private bool _hasAttacked = false;
    private bool _isAlive = true;

    private void Start()
    {
        FanManager.Get().AddFan(this);

        _initialScaleDistSqrMag = (BandMemberTarget.transform.position - this.transform.position).sqrMagnitude;
        //_initialScaleDistSqrMag = (BandMemberTarget.transform.position - this.transform.position).magnitude;
    }

    private void OnDestroy()
    {
        FanManager.Get().RemoveFan(this);
    }

    private void Update()
    {
        if (_isAlive)
        {
            //@TODO: Do some check to see if we're in range and if so, stop moving and stun band member
            if (!IsInRangeOfTarget())
            {
                MoveToBandMember();
            }
            else if (!_hasAttacked)
            {
                //@TODO: Initiate attack.
                AttackTarget();
            }
        }
        else
        {
            UpdateDeath();
        }
    }

    private bool IsInRangeOfTarget()
    {
        float attackRange = FanManager.Get().GetAttackRange();
        return (this.transform.position - BandMemberTarget.transform.position).sqrMagnitude <= attackRange * attackRange;
    }

    private void MoveToBandMember()
    {
        var step = CalcMoveSpeed();
        transform.position = Vector3.MoveTowards(transform.position, BandMemberTarget.transform.position, step);

        AdjustStageScale();
    }

    private float CalcMoveSpeed()
    {
        return MoveSpeed * Time.deltaTime;
    }

    private void AdjustStageScale()
    {
        float stageFinalScale = FanManager.Get().GetStagePerspectiveScaleDown();
        float curScaleSqrMag = (this.transform.position - BandMemberTarget.transform.position).sqrMagnitude;
        //float curScaleSqrMag = (this.transform.position - BandMemberTarget.transform.position).magnitude;
        float scale = 1f - (curScaleSqrMag / _initialScaleDistSqrMag);
        //float scale = (1f - curScaleSqrMag) / _initialScaleDistSqrMag;
        Vector3 newScale = Mathf.Lerp(1f, stageFinalScale, scale) * Vector3.one;
        this.transform.localScale = newScale;
    }

    private void UpdateTrackMute(AudioManager.Music track)
    {
        FindObjectOfType<AudioManager>().Mute(track);
        FindObjectOfType<PlayerController>().UpdateCanTurnOnTrack(track, false);
    }

    private void UpdateTrackUnMute(AudioManager.Music track)
    {
        PlayerController.Get().UpdateCanTurnOnTrack(track, true);
    }

    public bool IsAlive()
    {
        return _isAlive;
    }

    public bool IsAttackingAndAlive()
    {
        return _hasAttacked && _isAlive;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        return;

        var instrument = collision.gameObject.GetComponent<BandProperties>().Instrument[0];

        //Bass Collision
        if (instrument == AudioManager.Music.Bass)
        {
            UpdateTrackMute(AudioManager.Music.Bass);
        }

        //Chords Collision
        if (instrument == AudioManager.Music.Chords)
        {
            UpdateTrackMute(AudioManager.Music.Chords);
        }

        //Drums Collision
        if (instrument == AudioManager.Music.Drums)
        {
            UpdateTrackMute(AudioManager.Music.Drums);
        }

        //LeadMelody  Collision
        if (instrument == AudioManager.Music.Lead)
        {
            UpdateTrackMute(AudioManager.Music.Lead);
            UpdateTrackMute(AudioManager.Music.Melody);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        return;

        var instrument = collision.gameObject.GetComponent<BandProperties>().Instrument[0];

        //Bass Collision
        if (instrument == AudioManager.Music.Bass)
        {
            UpdateTrackUnMute(AudioManager.Music.Bass);
        }

        //Chords Collision
        if (instrument == AudioManager.Music.Chords)
        {
            UpdateTrackUnMute(AudioManager.Music.Chords);
        }

        //Drums Collision
        if (instrument == AudioManager.Music.Drums)
        {
            UpdateTrackUnMute(AudioManager.Music.Drums);
        }

        //LeadMelody  Collision
        if (instrument == AudioManager.Music.Lead)
        {
            UpdateTrackUnMute(AudioManager.Music.Lead);
            UpdateTrackUnMute(AudioManager.Music.Melody);
        }
    }

    private void AttackTarget()
    {
        BandMemberTarget.Stun();

        _hasAttacked = true;
    }

    public void ApplyDeathForce(Vector3 source, Vector3 force)
    {
        if (_isAlive)
        {
            _rigidbody.AddForce(force, ForceMode2D.Impulse);
            //_rigidbody.AddForceAtPosition(force, source, ForceMode2D.Impulse);

            _isAlive = false;

            _deathTimer = _deathTime;
        }
    }

    private void UpdateDeath()
    {
        _deathTimer -= Time.deltaTime;
        if(_deathTimer <= 0f)
        {
            Destroy(this.gameObject);
        }
    }
}
