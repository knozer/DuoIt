using UnityEngine;
using System.Collections;

public class OutterShockWaveController : MonoBehaviour
{
    //protected CircleCollider2D circleCollider;
    protected GameObject user = null;

    protected static float RADIUS_ENLARGE_TIME = 1f;
    protected static float RADIUS_ENLARGE_SPEED_ORIGINAL = 9f;
    protected static Vector3 ORIGINAL_SCALE;

    protected float timeLeft;

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.isPaused || GameManager.instance.isGameOver || !isActiveAndEnabled || timeLeft <= 0)
        {
            return;
        }
        gameObject.transform.localScale += ORIGINAL_SCALE * RADIUS_ENLARGE_SPEED_ORIGINAL * Time.deltaTime;
        if (timeLeft > 0 && timeLeft - Time.deltaTime < 0)
        {
            user.SetActive(true);
            gameObject.SetActive(false);
        }
        timeLeft -= Time.deltaTime;
    }

    protected void OnEnable()
    {
        timeLeft = RADIUS_ENLARGE_TIME;
        //ORIGINAL_SCALE = gameObject.transform.localScale;
    }

    public void setUser(GameObject player)
    {
        user = player;
        gameObject.transform.localScale = user.transform.localScale;
        ORIGINAL_SCALE = gameObject.transform.localScale;
    }

    public GameObject getUser()
    {
        return user;
    }

    protected void OnDisable()
    {
        user.GetComponent<PlayerController>().enableCollider(true);
        gameObject.transform.localScale = ORIGINAL_SCALE;
        user = null;
    }

    //destroy pickup items
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "PowerBonusLow" || other.tag == "PowerBonusMid")
        {
            other.gameObject.SetActive(false);
        }
    }

}
