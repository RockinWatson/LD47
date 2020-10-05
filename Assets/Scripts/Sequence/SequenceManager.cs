using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequenceManager : MonoBehaviour
{
    [SerializeField] private List<Sequence> _allSequences;

    [SerializeField] private GameObject[] ARROWS_PREFABS = new GameObject[4];
    [SerializeField] private Vector3[] ARROWS_POS = new Vector3[4];

    [SerializeField] private GameObject TARGET = null;
    public Vector3 GetTargetPos() { return TARGET.transform.position; }
    [SerializeField] private float ACTIVE_TARGET_RANGE = 1f;
    public float GetActiveTargetRange() { return ACTIVE_TARGET_RANGE; }

    [SerializeField] private float ARROW_SPEED = 3f;
    public float GetArrowSpeed() { return ARROW_SPEED; }

    private float _sequenceTimer = 0f;

    public enum TargetScore
    {
        EXCELLENT = 0,
        GOOD = 1,
        OKAY = 2,
        BAD = 3,
    }
    [SerializeField] private float[] TARGET_RANGES = new float[4];
    public float[] GetTargetRanges() { return TARGET_RANGES; }

    static private SequenceManager _instance = null;
    static public SequenceManager Get() { return _instance; }

    private void Awake()
    {
        if(_instance != null)
        {
            Debug.LogError("Something fucky happened - should only be 1 SequenceManager");
            return;
        }

        _instance = this;
    }

    private void Update()
    {
        //@TODO: Figure out how / when to play a new sequence
        if(_sequenceTimer <= 0f)
        {
            PlayRandomSequence();
        }
        else
        {
            _sequenceTimer -= Time.deltaTime;
        }

        //@TEMP: For now, just do it based on key stroke
        if(Input.GetKeyDown(KeyCode.P))
        {
            PlayRandomSequence();
        }

        //@TODO: Maybe also track input for arrows etc and accuracy?
    }

    private void PlayRandomSequence()
    {
        int i = Random.Range(0, _allSequences.Count);
        var sequence = _allSequences[i];

        PlaySequence(sequence);
    }

    private void PlaySequence(Sequence sequence)
    {
        _sequenceTimer = 0f;
        float xPos = 0f;
        foreach(var note in sequence.Arrows)
        {
            int index = (int)note.Arrow;
            GameObject arrow = Instantiate(ARROWS_PREFABS[index], ARROWS_POS[index] + Vector3.right * xPos, Quaternion.identity, this.transform);

            xPos += note.Time;
            _sequenceTimer += note.Time;
        }
        _sequenceTimer *= .75f;
    }
}
