using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FanManager : MonoBehaviour
{
    [SerializeField] private GameObject[] FANS_PREFABS = null;
    [SerializeField] private Vector3[] FANS_POS = null;

    private const float INITIAL_SPAWN_COOLDOWN = 20f;
    private float _spawnCooldown = INITIAL_SPAWN_COOLDOWN;
    private const float NORMAL_SPAWN_COOLDOWN = 10f;

    private void Update()
    {
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
}
