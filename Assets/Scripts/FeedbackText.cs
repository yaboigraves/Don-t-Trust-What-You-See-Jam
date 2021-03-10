using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeedbackText : MonoBehaviour
{

    public float maxLifetime = 2, lifetime = 0;
    public Vector3 destination;
    public Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.position;
        destination = new Vector3(Random.Range(-1, 1), Random.Range(0, 1), Random.Range(-1, 1));
    }

    public IEnumerator spawnFeedback()
    {
        yield return new WaitForSeconds(lifetime);
        Destroy(this.gameObject);
    }

    private void Update()
    {
        lifetime += Time.deltaTime;
        transform.position = Vector3.Lerp(startPosition, destination, lifetime / maxLifetime);
    }
}
