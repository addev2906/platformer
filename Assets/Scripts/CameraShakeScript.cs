using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShakeScript : MonoBehaviour
{
    public IEnumerator Shake(float duration,float magnitude)
    {
        Vector3 originalPos = transform.localPosition; //Storing original camera position

        float elapsed = 0f;

        while (elapsed<duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = new Vector3(x, y, originalPos.z);

            elapsed += Time.deltaTime;

            yield return null;        //Waiting until next frame is played
        }

        transform.localPosition = originalPos;
    }
    public void MoveCamera(Vector2 playerVel)
    {
        if (playerVel.x>0)
        {

            transform.localPosition += new Vector3(32, 0, 0);
        }

        if (playerVel.x < 0)
        {
            transform.localPosition += new Vector3(-32, 0, 0);
        }
    }
}
