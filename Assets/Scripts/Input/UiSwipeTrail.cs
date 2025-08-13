using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UiSwipeTrail : MonoBehaviour
{
    [SerializeField] private InputHandler inputHandler;
    [SerializeField] private RectTransform trailContainer;
    [SerializeField] private Image trailPrefab;
    [SerializeField] private float spawnRate = .02f;
    [SerializeField] private float dotLifetime = .3f;
    [SerializeField] private Material trailMaterial;

    private bool drawing = false;
    private Coroutine trailCoroutine;

    public void StartTrail()
    {
        if (trailCoroutine != null) StopCoroutine(trailCoroutine);
        trailCoroutine = StartCoroutine(SpawnDots());
    }

    public void StopTrail()
    {
        drawing = false;
    }

    private IEnumerator SpawnDots()
    {
        drawing = true;
        while (drawing)
        {

            // Create dot
            Image dot = Instantiate(trailPrefab, trailContainer);
            if (trailMaterial != null)
                dot.material = trailMaterial;

            Destroy(dot.gameObject, dotLifetime);

            yield return new WaitForSeconds(spawnRate);
        }
    }
}
