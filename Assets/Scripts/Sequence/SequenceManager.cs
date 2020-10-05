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

    private Dictionary<int, QueuedSequence> _queuedSequences = new Dictionary<int, QueuedSequence>();
    public Dictionary<int, QueuedSequence> QueuedSequences() { return _queuedSequences; }

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
            QueueRandomSequence();
        }
        else
        {
            _sequenceTimer -= Time.deltaTime;
        }

#if UNITY_EDITOR
        //@TEMP: For now, just do it based on key stroke
        if(Input.GetKeyDown(KeyCode.P))
        {
            QueueRandomSequence();
        }
#endif

        //@TODO: Maybe also track input for arrows etc and accuracy?
    }

    private void QueueRandomSequence()
    {
        int i = Random.Range(0, _allSequences.Count);
        var sequence = _allSequences[i];

        QueueSequence(sequence);
    }

    private void QueueSequence(Sequence sequence)
    {
        if (sequence.Arrows != null && sequence.Arrows.Count > 0)
        {
            // New sequence encountered, so restart the cooldown timer.
            _sequenceTimer = 0f;

            // Create the queued sequence for tracking.
            var queuedSequence = new QueuedSequence();

            // Process and setup all the notes in the sequence.
            float xPos = 0f;
            foreach (var notePrefab in sequence.Arrows)
            {
                int arrowIndex = (int)notePrefab.Arrow;
                GameObject arrow = Instantiate(ARROWS_PREFABS[arrowIndex], ARROWS_POS[arrowIndex] + Vector3.right * xPos, Quaternion.identity, this.transform);
                SequenceNote note = arrow.GetComponent<SequenceNote>();
                note.SequenceID = queuedSequence.ID;
                queuedSequence.AddNote(note);

                xPos += notePrefab.Time;
                _sequenceTimer += notePrefab.Time;
            }
            //@NOTE: Adjusting sequence cooldown timer slightly to get sequences going back to back faster...
            _sequenceTimer *= .75f;

            _queuedSequences.Add(queuedSequence.ID, queuedSequence);
        }
        else
        {
            Debug.LogError("Sequence is malformed.");
        }
    }
}
