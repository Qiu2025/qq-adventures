using UnityEngine;
using System.Collections;

public class Key : MonoBehaviour
{
    public Vector3 playerOffset;
    public float animationDuration = 0.5f;

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            GetComponent<Collider2D>().enabled = false;

            transform.SetParent(col.transform);

            StartCoroutine(MoveAndShrink(col.transform));
        }
    }

    private IEnumerator MoveAndShrink(Transform player)
    {
        Vector3 startPos = transform.localPosition;
        Vector3 targetPos = playerOffset;
        Vector3 startScale = transform.localScale;
        Vector3 targetScale = startScale * 0.5f;

        float elapsed = 0f;

        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / animationDuration;

            transform.localPosition = Vector3.Lerp(startPos, targetPos, t);
            transform.localScale = Vector3.Lerp(startScale, targetScale, t);

            yield return null;
        }
        
        transform.localPosition = targetPos;
        transform.localScale = targetScale;
    }
}