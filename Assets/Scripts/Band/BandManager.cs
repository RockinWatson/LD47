using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Assets.Scripts;

public class BandManager : MonoBehaviour
{
    [SerializeField] private BandMember[] _bandMembers = new BandMember[4];
    [SerializeField] private BandMemberType[] _memberPriority = new BandMemberType[4]; //@TODO: Priority of members - drums, bass, chords, melody

    [SerializeField] private GameObject _explosionPrefab = null;

    [SerializeField] private float _fullBandAliveGracePeriod = 30f;
    private float _fullBandAliveGraceTimer;
    private bool _hasHadFullBandPlay = false;

    private Dictionary<BandMember, BandMember.BandMemberStatus> _bandMembersLastKnownStatusForParry = new Dictionary<BandMember, BandMember.BandMemberStatus>();
    private Dictionary<BandMember, BandMember.BandMemberStatus> _bandMembersLastKnownStatusForStun = new Dictionary<BandMember, BandMember.BandMemberStatus>();
    private bool _isInStunParry = false;
    private BandMember _stunnedMember = null;

    private bool _isGameOverMan = false;

    static private BandManager _instance = null;
    static public BandManager Get() { return _instance; }

    public enum BandMemberType
    {
        BILLY = 0,
        CACTUS = 1,
        GATOR = 2,
        CHICK = 3,
        COUNT,
    }

    private void Awake()
    {
        if (_instance != null)
        {
            Debug.LogError("Something fucky happened - should only be 1 BandManager");
            return;
        }

        _instance = this;

        _fullBandAliveGraceTimer = _fullBandAliveGracePeriod;

        BandMember.OnMemberStunned += OnBandMemberStunned;
    }

    private void OnDestroy()
    {
        BandMember.OnMemberStunned -= OnBandMemberStunned;
    }

    private void Update()
    {
        if(_isGameOverMan)
        {
            return;
        }

        CheckFullBandAliveRestartCounter();

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.O))
        {
            AttackRandomAttackingFan();
        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            AudioManager.Get().UnMute(AudioManager.Music.FullBand);
            AudioManager.Get().Pitch(AudioManager.Music.FullBand, -5f);
        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            AudioManager.Get().Mute(AudioManager.Music.FullBand);
            AudioManager.Get().Pitch(AudioManager.Music.FullBand, 0f);
        }
#endif
    }

    public bool IsAllLeftBandPlaying()
    {
        return _bandMembers[(int)BandMemberType.CACTUS].IsPlaying() && _bandMembers[(int)BandMemberType.GATOR].IsPlaying();
    }
    public bool IsOneLeftBandPlaying()
    {
        return _bandMembers[(int)BandMemberType.CACTUS].IsPlaying() != _bandMembers[(int)BandMemberType.GATOR].IsPlaying();
    }
    public bool IsAllRightBandPlaying()
    {
        return _bandMembers[(int)BandMemberType.CHICK].IsPlaying() && _bandMembers[(int)BandMemberType.BILLY].IsPlaying();
    }
    public bool IsOneRightBandPlaying()
    {
        return _bandMembers[(int)BandMemberType.CHICK].IsPlaying() != _bandMembers[(int)BandMemberType.BILLY].IsPlaying();
    }

    static public AudioManager.Music GetMusicByMemberType(BandMemberType memberType)
    {
        switch(memberType)
        {
            case BandMemberType.BILLY:
                return AudioManager.Music.Drums;
            case BandMemberType.CACTUS:
                return AudioManager.Music.Chords;
            case BandMemberType.GATOR:
                return AudioManager.Music.Melody;
            case BandMemberType.CHICK:
                return AudioManager.Music.Bass;
            default:
                Debug.LogError("Unkown Band Member Type");
                return AudioManager.Music.Drums;
        }
    }

    static public BandMemberType GetRandomBandMemberType()
    {
        return (BandMemberType)Random.Range(0, (int)BandMemberType.COUNT);
    }

    public BandMember GetRandomBandMember()
    {
        return _bandMembers[Random.Range(0, (int)BandMemberType.COUNT)];
    }

    public void SequenceCompleted(SequenceNote.HitScore hitScore)
    {
        if(_isGameOverMan)
        {
            return;
        }

        //@TODO: React appropriately to the level of sequence completed.
            switch (hitScore)
        {
            case SequenceNote.HitScore.EXCELLENT:
                {
                    if(_isInStunParry)
                    {
                        StunParry();
                    }

                    //@TODO: Try to handle Excellent, then have it fall through... Might need to recurse down the line?
                    var bandMember = AttackRandomAttackingFan();
                    if (!_isInStunParry)
                    {
                        if (bandMember != null)
                        {
                            //@TODO: Boost this to next tier or something?
                            bandMember.SetPlaying();
                        }
                        else
                        {
                            //@TODO: Boost this to next tier or something?
                            StartRandomMemberPlaying();
                        }
                    }
                }
                break;
            case SequenceNote.HitScore.GOOD:
                {
                    //if (_isInStunParry)
                    //{
                    //    StunParry();
                    //}

                    var bandMember = AttackRandomAttackingFan();
                    if (!_isInStunParry)
                    {
                        if (bandMember != null)
                        {
                            bandMember.SetPlaying();
                        }
                        else
                        {
                            StartRandomMemberPlaying();
                        }
                    }
                }
                break;
            case SequenceNote.HitScore.OKAY:
                StartRandomMemberPlaying();
                break;
            case SequenceNote.HitScore.BAD:
                //@TODO: Reduce tier?
                FanManager.Get().DecrementSpawnCooldownByTick();
                break;
            case SequenceNote.HitScore.MISSED:
                //@TODO: Stun random?
                FanManager.Get().DecrementSpawnCooldownByTick(true);
                break;
            case SequenceNote.HitScore.NOT_HIT:
                Debug.LogWarning("This shouldn't happen - shouldn't get NOT HIT at this point.");
                break;
        }

        if (_isInStunParry)
        {
            EndStunParryAttempt();
        }

        Console.Get().BounceHitScoreLight(hitScore);
    }

    private void StartRandomMemberPlaying()
    {
        var member = GetRandomMemberNotPlaying();
        if(member != null)
        {
            member.SetPlaying();
        }
    }

    private BandMember GetRandomMemberNotPlaying()
    {
        var members = _bandMembers.Where((a) => a.IsNotPlaying()).ToArray();
        if(members != null && members.Length > 0)
        {
            return members[Random.Range(0, members.Length)];
        }
        return null;
    }

    private bool IsAnyPlayerNotPlaying()
    {
        foreach(var member in _bandMembers)
        {
            if(member.IsNotPlaying())
            {
                return true;
            }
        }
        return false;
    }

    private BandMember AttackRandomAttackingFan()
    {
        var fan = FanManager.Get().GetRandomAttackingAndAliveFan();
        if(fan != null)
        {
            BandMember targetBandMember = fan.BandMemberTarget;

            var fans = FanManager.Get().GetAllNearbyAliveEnemiesAroundPos(fan.transform.position, 4f);
            if(fans != null && fans.Count > 0)
            {
                //@TODO: Add impulse to all these fuckers from nearest band member position.
                foreach(var targetFan in fans)
                {
                    Vector3 force = (targetFan.transform.position - targetBandMember.transform.position).normalized * 500f;
                    targetFan.ApplyDeathForce(targetBandMember.transform.position, force);
                }
            }

            CreateExplosionAtPos(fan.transform.position);
            Vector3 force2 = (fan.transform.position - targetBandMember.transform.position).normalized * 500f;
            fan.ApplyDeathForce(targetBandMember.transform.position, force2);

            return targetBandMember;
        }
        return null;
    }

    private void CreateExplosionAtPos(Vector3 pos)
    {
        Instantiate(_explosionPrefab, pos, Quaternion.identity, this.transform);
    }

    private void CheckFullBandAliveRestartCounter()
    {
        if (IsAnyPlayerNotPlaying())
        {
            if (_hasHadFullBandPlay || FanManager.Get().IsAtMinSpawnCooldown())
            {
                _fullBandAliveGraceTimer -= Time.deltaTime;
                if (_fullBandAliveGraceTimer <= 0f)
                {
                    GameOverMan();
                }
            }
        }
        else
        {
            _hasHadFullBandPlay = true;
            _fullBandAliveGraceTimer = _fullBandAliveGracePeriod;
        }
    }

    private void OnBandMemberStunned(BandMember stunnedMember)
    {
        if(!_isInStunParry)
        {
            _isInStunParry = true;

            OnBandMemberStunned_WholeBand(stunnedMember);
            //OnBandMemberStunned_SingleMember(stunnedMember);
        }
    }

    private void StunParry()
    {
        StunParry_WholeBand();
        //StunParry_SingleMember();

        EndStunParryAttempt();
    }

    private void EndStunParryAttempt()
    {
        if (_isInStunParry)
        {
            EndStunParryAttempt_WholeBand();
            //EndStunParryAttempt_SingleMember();

            _isInStunParry = false;
        }
    }

    #region OnBandMemberStunned_WholeBand
    private void OnBandMemberStunned_WholeBand(BandMember stunnedMember)
    {
        foreach (var member in _bandMembers)
        {
            if (_bandMembersLastKnownStatusForParry.ContainsKey(member))
            {
                _bandMembersLastKnownStatusForParry[member] = member.GetStatus();
                _bandMembersLastKnownStatusForStun[member] = member.GetStatus();
            }
            else
            {
                _bandMembersLastKnownStatusForParry.Add(member, member.GetStatus());
                _bandMembersLastKnownStatusForStun[member] = member.GetStatus();
            }
            member.SetStatus(BandMember.BandMemberStatus.NOT_PLAYING);
        }

        _bandMembersLastKnownStatusForParry[stunnedMember] = BandMember.BandMemberStatus.PLAYING;

        // Set Reverse track
        AudioManager.Get().UnMute(AudioManager.Music.FullBand);
        AudioManager.Get().Pitch(AudioManager.Music.FullBand, -1f);
    }
    private void StunParry_WholeBand()
    {
        // Restore everyone's former state.
        foreach (var member in _bandMembers)
        {
            member.SetStatus(_bandMembersLastKnownStatusForParry[member]);
        }
    }

    private void EndStunParryAttempt_WholeBand()
    {
        // Set Reverse track
        AudioManager.Get().Mute(AudioManager.Music.FullBand);
        AudioManager.Get().Pitch(AudioManager.Music.FullBand, 0f);

        // Restore everyone's former state.
        foreach (var member in _bandMembers)
        {
            member.SetStatus(_bandMembersLastKnownStatusForStun[member]);
        }
    }
    #endregion

    private void OnBandMemberStunned_SingleMember(BandMember stunnedMember)
    {
        _stunnedMember = stunnedMember;
        _stunnedMember.SlowDeath();
    }

    private void StunParry_SingleMember()
    {
        _stunnedMember.KillSlowDeathCoroutine();
        _stunnedMember.SetStatus(BandMember.BandMemberStatus.PLAYING);
    }

    private void EndStunParryAttempt_SingleMember()
    {
        _stunnedMember.KillSlowDeathCoroutine();
        _stunnedMember = null;
    }

    private void GameOverMan()
    {
        _isGameOverMan = true;

        StartCoroutine(GameOverManRoutine());
    }

    private IEnumerator GameOverManRoutine()
    {
        foreach (var member in _bandMembers)
        {
            member.SetStatus(BandMember.BandMemberStatus.NOT_PLAYING);
        }

        float gameOverTime = 5f;
        float gameOverTimer = 0f;

        AudioManager.Get().UnMute(AudioManager.Music.FullBand);

        while (true)
        {
            gameOverTimer += Time.deltaTime;
            if (gameOverTimer >= gameOverTime)
            {
                break;
            }
            AudioManager.Get().Pitch(AudioManager.Music.FullBand, -5f * gameOverTimer / gameOverTime);
            yield return null;
        }

        SceneManager.LoadScene("GameOverLoop");
    }
}
