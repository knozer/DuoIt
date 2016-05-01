using UnityEngine;
using System.Collections;

public class InnerShockWaveController : OutterShockWaveController
{
    //protected static float RADIUS_ENLARGE_SPEED_ORIGINAL = 2f;
    protected static float RADIUS_ENLARGE_SPEED_LOW = RADIUS_ENLARGE_SPEED_ORIGINAL * 0.75f;
    bool counter = false;

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.isPaused || GameManager.instance.isGameOver || !isActiveAndEnabled || timeLeft <= 0)
        {
            return;
        }
        gameObject.transform.localScale += ORIGINAL_SCALE * (counter ? RADIUS_ENLARGE_SPEED_LOW : RADIUS_ENLARGE_SPEED_ORIGINAL) * Time.deltaTime;
        if (timeLeft > 0 && timeLeft - Time.deltaTime < 0)
        {
            user.SetActive(true);
            gameObject.SetActive(false);
        }
        timeLeft -= Time.deltaTime;
    }

    void OnCollisionStay2D(Collision2D coll)
    {
        if (coll.gameObject.tag == "Player")
        {

            PlayerController script = coll.gameObject.GetComponent<PlayerController>();
            counter = script.isEnlarged;
        }
        else if (coll.gameObject.tag == "PickUp")
        {

        }
    }
}
