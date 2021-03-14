using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FeedbackText : MonoBehaviour
{
    public float maxLifetime = 2, lifetime = 0;
    public Vector3 destination;
    public Vector3 startPosition;

    public float speed = 0.3f;

    public float range = 100f;

    public TextMeshProUGUI fText;
    private void Start()
    {
        fText = GetComponent<TextMeshProUGUI>();
        startPosition = transform.position;

        destination = transform.position + new Vector3(Random.Range(-range, range + 1), Random.Range(range / 10, range), Random.Range(-range, range + 1));
        StartCoroutine(spawnFeedback());
    }

    public IEnumerator spawnFeedback()
    {
        yield return new WaitForSeconds(maxLifetime);
        Destroy(this.gameObject);
    }

    private void Update()
    {
        lifetime += Time.deltaTime * speed;
        transform.position = Vector3.Lerp(startPosition, destination, lifetime / maxLifetime);
    }

    public void SetText(bool didHit)
    {
        if (didHit)
        {
            fText.text = "Good!";
            fText.color = Color.green;
        }
        else
        {
            fText.text = "Miss!";
            fText.color = Color.red;
        }

    }
}
