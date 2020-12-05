using UnityEngine;

public class SurfaceSoundController : MonoBehaviour
{
    // get ground hit sound to play
    public AudioSource groundHit;

    // tells time in seconds since last collision
    float count;

    // Start is called before the first frame update
    void Start()
    {
        count = 0;
    }

    // Update is called once per frame
    void Update()
    {
        // each frame, add the time the frame took to counter
        count += Time.deltaTime;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (count > 1) // if greater than 1 second has passed since the noise was last played
        {
            Debug.Log("play sound now"); // play it again
            groundHit.Play(0);
            count = 0; // reset counter
        }
    }
}