using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    //Canera follow basics
    [Tooltip("The game object to follow")]
    [SerializeField] GameObject follow;
    [Tooltip("Value between 0 and 1 for how snappy the camera movement is. 1 means no lerp.")]
    [Range(0f, 1f)]
    [SerializeField] float followSpeed;

    //How far the camera was from the target when the asset was loaded. Gets added to the follow position
    private Vector3 offset;

    //Screen shake variables
    float shakeDuration = 0.0f;
    float shakeDurationStart = 0.0f;
    float shakeMag = 0.0f;

    private void Start()
    {
        if (follow != null)
        {
            offset = follow.transform.position - transform.position;
        }
        else
        {
            //If a follow isn't assigned to the script, destroy the script so that there aren't any major errors and
            //put a message in the console
            Debug.LogError("Error: No follow target attatched to camera. Destroying component...");
            Destroy(this);
        }
    }

    private void FixedUpdate()
    {
        Vector3 followPos = follow.transform.position - offset;

        //Handling screen shake
        if (shakeDuration > 0)
        {
            shakeDuration -= Time.fixedDeltaTime;

            //Lerps the shake position, going between the magnitude and 0. The 1 - shakeduration part is always a fraction
            //of the time remaining in the screen shake
            Vector3 randShake = Random.insideUnitCircle * Mathf.Lerp(shakeMag, 0, 1 - (shakeDuration / shakeDurationStart));

            followPos += randShake;
        }
        else
            shakeMag = 0;

        //Lerp the camera towards the target's position
        transform.position = Vector3.Lerp(transform.position, followPos, followSpeed);
    }

    public void ShakeScreen(float duration, float magnitude)
    {
        if (duration > shakeDuration)
        {
            shakeDurationStart = duration;
            shakeDuration = duration;
        }

        if (magnitude > shakeMag)
        {
            shakeMag = magnitude;
        }
    }
}