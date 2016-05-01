using UnityEngine;
using System.Collections;
using System;

public class PlayerController : MonoBehaviour
{
    /// <summary>
    /// static parameters
    /// </summary>
    //private static float degree = (float)Math.PI / 360;

    //skill
    private static float ENLARGE_TIME = 5;

    //bonus
    private static float PowerBonusDecreasePerSecond = 0.05f;   //how much decrease per second
    public static int POWER_MIN_RANGE = 0;
    public static int POWER_MAX_RANGE = 5;

    //enlarge
    private static Vector3 ENLARGE_SIZE = new Vector3(5, 5, 0);
    private static float MASS_ORIGINAL;
    private static float MASS_ENLARGE = 3f;

    //speed
    private static float SpeedOriginal = 32000f;
    private static float FrictionScale = 16800f;
    private static float SpeedChange = 2f;
    private static float SpeedCharge = SpeedOriginal * 40;

    public GameObject Opponent;
    public GameObject OutterShockWave;
    public GameObject InnerShockWave;

    public bool isEnlarged;

    /// <summary>
    /// non-static parameters
    /// </summary>

    private Rigidbody2D rb2d;
    private Vector3 ScaleOriginal;
    private Vector3 ScaleEnlarged;

    //status
    //0:Charge, 1:Enlarge, 2:ShockWave, 3:controller reverse
    private float[] statusTimeLeft = { 0, 0, 0 };
    private float powerCount = 0f;
    private int[] PowerLevels = { 1, 2, 4 };

    //movement
    private float friction;
    private float chargeTime;
    private float angel;
    private float curSpeed;
    private bool isRotate;
    private bool isReversed;
    private bool isCharge = false;


    //animation
    Animator animator;

	//sound
	public AudioClip eatSound;
	public AudioClip bounceSound;
	public AudioClip fallSound;
	public AudioClip enlargeSound;
	public AudioClip chargeSound;
	public AudioClip shockwaveSound;


    void Start()
    {
        //set basic component
        rb2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        MASS_ORIGINAL = rb2d.mass;
        ScaleOriginal = transform.localScale;
        ScaleEnlarged = ScaleOriginal + ENLARGE_SIZE;

        //set movement
        friction = rb2d.drag;
        //F*t=
        chargeTime = SpeedCharge * Time.fixedDeltaTime / friction / FrictionScale;
        angel = 180;
        curSpeed = SpeedOriginal;
        isRotate = true;
        isReversed = false;

        //powerCountText.text = powerCount.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.isPaused || GameManager.instance.isGameOver)
        {
            return;
        }
        if (powerCount > 0)
        {
            powerCount = Math.Max(powerCount - Time.deltaTime * PowerBonusDecreasePerSecond, 0);
        }

        PlayerRotation();

        CheckStatusTime();
    }

    public int getSkillLevel()
    {
        for (int i = PowerLevels.Length - 1; i >= 0; i--)
        {
            int thisLevel = PowerLevels[i];
            if (powerCount > thisLevel)
            {
                return i;
            }
        }
        return -1;
    }

    //check each status' left time
    private void CheckStatusTime()
    {
        for (int i = 0; i < statusTimeLeft.Length; i++)
        {
            if (statusTimeLeft[i] > 0 && statusTimeLeft[i] - Time.deltaTime < 0)
            {
                switch (i)
                {
                    case 0:
                        PlayerCharge(false);
                        break;
                    case 1:
                        PlayerEnlarge(false);
                        break;
                    case 2:
                        CreateShockWave();
                        break;
                }
            }
            statusTimeLeft[i] = Math.Max(statusTimeLeft[i] - Time.deltaTime, 0);
        }
    }

    private void FixedUpdate()
    {
        if (GameManager.instance.isGameOver)
        {
            return;
        }
        PlayerMovement(curSpeed);
    }




    /// <summary>
    /// Movement Code
    /// </summary>
    //control player rotation
    private void PlayerRotation()
    {
        if (!isCharge && (isRotate != isReversed))
        {
            transform.Rotate(0, 0, angel * Time.deltaTime);
        }
    }

    //control player movement
    private void PlayerMovement(float speed)
    {
        if (isCharge || (isRotate == isReversed))
        {
            rb2d.AddRelativeForce(new Vector2(0, 1) * speed);
        }
    }

    public void PlayerStop() {
        rb2d.drag=100000f;
    }


    public void ChangeRotationStatus()
    {
        isRotate = !isRotate;
    }


    public void ChangeRotationDirection()
    {
        angel = -angel;
    }




    /// <summary>
    /// Pick Item Code
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
		if (other.tag == "PowerBonusLow") {
			SoundManager.instance.PlaySingle (eatSound);
			PickUpItem (other);
		} else if (other.tag == "PowerBonusMid") {
			SoundManager.instance.PlaySingle (eatSound);
			PickUpItem (other);
		} else if (other.tag == "Player") {
			Debug.Log ("Player Bounce");
			SoundManager.instance.PlaySingle (bounceSound);
		}

    }

	private void OnCollisionEnter2D(Collision2D other){
		if (other.gameObject.tag == "Player") {
			Debug.Log ("Player Bounce");
			SoundManager.instance.PlaySingle (bounceSound);
		}
	}

    private void PickUpItem(Collider2D other)
    {
        powerCount += other.GetComponent<ItemController>().getBonus();
        powerCount = Math.Max(powerCount, POWER_MIN_RANGE);
        powerCount = Math.Min(powerCount, POWER_MAX_RANGE);
        other.gameObject.SetActive(false);
    }

    public void enableCollider(bool isEnabled)
    {
        CircleCollider2D coll = GetComponent<CircleCollider2D>();
        coll.enabled = isEnabled;
        rb2d.isKinematic = !isEnabled;
    }



    /// <summary>
    /// Skill Code
    /// </summary>
    public void PlayerEnlarge(bool isEnlarged)
    {
        this.isEnlarged = isEnlarged;
        if (isEnlarged)
        {
            statusTimeLeft[1] = ENLARGE_TIME;
            transform.localScale = ScaleEnlarged;
            rb2d.mass = MASS_ORIGINAL * MASS_ENLARGE;
            curSpeed = SpeedOriginal * SpeedChange;
        }
        else
        {
            statusTimeLeft[1] = 0;
            transform.localScale = ScaleOriginal;
            rb2d.mass = MASS_ORIGINAL;
            curSpeed = SpeedOriginal;
        }
    }


    private void PlayerCharge(bool isCharge)
    {
        this.isCharge = isCharge;
        if (isCharge)
        {
            statusTimeLeft[0] = chargeTime;
            PlayerMovement(SpeedCharge);
        }
        else {
            statusTimeLeft[0] = 0;
        }
    }

    private void CreateShockWave()
    {
		SoundManager.instance.PlaySingle (shockwaveSound);
        GameObject outterWave = Instantiate(OutterShockWave, gameObject.transform.localPosition, Quaternion.identity) as GameObject;
        outterWave.GetComponent<OutterShockWaveController>().setUser(gameObject);
        GameObject innerWave = Instantiate(InnerShockWave, gameObject.transform.localPosition, Quaternion.identity) as GameObject;
        innerWave.GetComponent<InnerShockWaveController>().setUser(gameObject);

        //gameObject.SetActive(false);
        outterWave.SetActive(true);
        innerWave.SetActive(true);
    }

    /*
    private void PlayerControllerReverse(bool isReversed)
    {
        if (isReversed)
        {
            statusTimeLeft[2] = StatusDurationTime;
        }
        this.isReversed = isReversed;
    }
    */

    public void SkillHandler()
    {
        switch (getSkillLevel())
        {
            case 0:
                powerCount = 0;
                PlayerCharge(true);
				SoundManager.instance.PlaySingle (chargeSound);
                break;
            case 1:
                powerCount = 0;
                PlayerEnlarge(true);
				SoundManager.instance.PlaySingle (enlargeSound);
                break;
            case 2:
                powerCount = 0;
                enableCollider(false);
                statusTimeLeft[2] = 0.5f;
                break;
        }
        return;
    }




    /// <summary>
    /// Other Code
    /// </summary>
    /// <returns></returns>
    public int getPowerCount()
    {
        return (int)Math.Ceiling(powerCount);
    }

    public void triggerPlayerFall()
    {
        Debug.Log("Player Start Fall");
		SoundManager.instance.PlaySingle (fallSound);
        animator.SetTrigger("playerFall");
    }

}
