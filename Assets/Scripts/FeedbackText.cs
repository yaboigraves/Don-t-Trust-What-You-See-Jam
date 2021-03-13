using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FeedbackText : MonoBehaviour
{
    public float maxLifetime = 2, lifetime = 0;
    public Vector3 destination;
    public Vector3 startPosition;

    public TextMeshProUGUI fText;
    private void Start()
    {
        fText = GetComponent<TextMeshProUGUI>();
        startPosition = transform.position;
        destination = new Vector3(Random.Range(-1, 1), Random.Range(0, 1), Random.Range(-1, 1));
        StartCoroutine(spawnFeedback());
    }

    public IEnumerator spawnFeedback()
    {
        yield return new WaitForSeconds(maxLifetime);
        Destroy(this.gameObject);
    }

    private void Update()
    {
        lifetime += Time.deltaTime;
        transform.position = Vector3.Lerp(startPosition, destination, lifetime / maxLifetime);
    }

    public void SetText(bool didHit)
    {
        if (didHit)
        {
            fText.text = "Good!";
        }
        else
        {
            fText.text = "Miss!";
        }

    }
}
