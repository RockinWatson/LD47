using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FanManager : MonoBehaviour
{
    [SerializeField] private GameObject[] FANS_PREFABS = null;
    [SerializeField] private Vector3[] FANS_POS = null;

    private Random random = new Random();

    private List<CrowdMember> _allFans = new List<CrowdMember>();
    public void AddFan(CrowdMember fan)
    {
        _allFans.Add(fan);
    }
    public void RemoveFan(CrowdMember fan)
    {
        _allFans.Remove(fan);
    }

    [SerializeField] private float INITIAL_SPAWN_COOLDOWN = 20f;
    private float _spawnCooldown = 0f;
    [SerializeField] private float NORMAL_SPAWN_COOLDOWN = 10f;
    [SerializeField] private float SPAWN_COOLDOWN_DECREASE_RATE_PER_SEC = 0.05f;
    [SerializeField] private float MIN_SPAWN_COOLDOWN = 0.1f;
    public bool IsAtMinSpawnCooldown() { return Mathf.Approximately(NORMAL_SPAWN_COOLDOWN, MIN_SPAWN_COOLDOWN); }
    public void DecrementSpawnCooldownByTick(bool andRampUpRate=false) {
        NORMAL_SPAWN_COOLDOWN -= SPAWN_COOLDOWN_DECREASE_RATE_PER_SEC;
        if (andRampUpRate)
        {
            SPAWN_COOLDOWN_DECREASE_RATE_PER_SEC += SPAWN_COOLDOWN_DECREASE_RATE_PER_SEC;
        }
    }

    [SerializeField] private float ATTACK_RANGE = 3f;
    public float GetAttackRange() { return ATTACK_RANGE; }

    [SerializeField] private float STAGE_PERSPECTIVE_SCALE_DOWN = .3f;
    public float GetStagePerspectiveScaleDown() { return STAGE_PERSPECTIVE_SCALE_DOWN; }

    static private FanManager _instance = null;
    static public FanManager Get() { return _instance; }

    private void Awake()
    {
        if (_instance != null)
        {
            Debug.LogError("Something fucky happened - should only be 1 FanManager");
            return;
        }

        _instance = this;

        _spawnCooldown = INITIAL_SPAWN_COOLDOWN;
    }

    private void Update()
    {
        NORMAL_SPAWN_COOLDOWN = Mathf.Max(MIN_SPAWN_COOLDOWN, NORMAL_SPAWN_COOLDOWN - Time.deltaTime * SPAWN_COOLDOWN_DECREASE_RATE_PER_SEC);
        _spawnCooldown -= Time.deltaTime;
        if(_spawnCooldown <= 0f)
        {
            SpawnRandomFan();

            //@TODO: Consider scaling this as time progresses to be shorter and shorter.
            _spawnCooldown = NORMAL_SPAWN_COOLDOWN;
        }
    }

    private void SpawnRandomFan()
    {
        var fanGO = Instantiate(FANS_PREFABS[Random.Range(0, FANS_PREFABS.Length)], FANS_POS[Random.Range(0, FANS_POS.Length)], Quaternion.identity, this.transform);
        CrowdMember crowdMember = fanGO.GetComponent<CrowdMember>();
        crowdMember.BandMemberTarget = BandManager.Get().GetRandomBandMember();
    }

    public List<CrowdMember> GetAllNearbyAliveEnemiesAroundPos(Vector3 pos, float radius)
    {
        var nearbyFans = new List<CrowdMember>();

        foreach(var fan in _allFans)
        {
            if(fan.IsAlive() && (pos - this.transform.position).sqrMagnitude <= radius*radius)
            {
                nearbyFans.Add(fan);
            }
        }

        return nearbyFans;
    }

    public CrowdMember GetRandomAttackingAndAliveFan()
    {
        var fans = _allFans.Where((a) => a.IsAttackingAndAlive()).ToArray();
        if(fans != null && fans.Length > 0)
        {
            return fans[Random.Range(0, fans.Length)];
        }
        return null;
    }
}
