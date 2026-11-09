using UnityEngine;
using System.Collections;

public class LightningFlash : MonoBehaviour
{
    [SerializeField] private LineRenderer line;
    [SerializeField] private int points = 8;
    [SerializeField] private float length = 2.5f;
    [SerializeField] private float jaggedness = 0.5f;
    [SerializeField] private float duration = 0.18f;
    [SerializeField] private float refreshRate = 0.03f;

    void Awake()
    {
        line.enabled = false;
    }

    public void Play()
    {
        StopAllCoroutines();
        StartCoroutine(Flash());
    }

    IEnumerator Flash()
    {
        line.positionCount = points;
        line.enabled = true;
        float end = Time.time + duration;

        while (Time.time < end)
        {
            for (int i = 0; i < points; i++)
            {
                float x = length * i / (points - 1);
                float y = (i == 0) ? 0f : Random.Range(-jaggedness, jaggedness);
                line.SetPosition(i, new Vector3(x, y, 0f));
            }

            line.widthMultiplier = (end - Time.time) / duration; // thins out as it fades
            yield return new WaitForSeconds(refreshRate);
        }

        line.enabled = false;
    }
}