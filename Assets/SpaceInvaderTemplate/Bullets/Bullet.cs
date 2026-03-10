using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] Vector3 startVelocity;
    [SerializeField] private float timeBeforeDeath = 15.0f;

    // Start is called before the first frame update
    void Awake()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = startVelocity;
        StartCoroutine(DeathTimer(timeBeforeDeath));
    }

    private IEnumerator DeathTimer(float lifeTime)
    {
        yield return new WaitForSeconds(lifeTime);
        Destroy(gameObject);
    }
}
