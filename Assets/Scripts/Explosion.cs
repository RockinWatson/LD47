using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] Animator _animator = null;

    private float _lifeTimer = 0f;

    private void Start()
    {
        _lifeTimer = _animator.GetCurrentAnimatorStateInfo(0).length;
    }

    private void Update()
    {
        _lifeTimer -= Time.deltaTime;
        if(_lifeTimer <= 0f)
        {
            Destroy(this.gameObject);
        }
    }
}
