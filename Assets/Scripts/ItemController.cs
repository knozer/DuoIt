using UnityEngine;

public class ItemController : MonoBehaviour
{

    public float bonus;
    public float punish;
    public Sprite rotSprite;

    public float ItemDisappearTime;
    public float ItemRotTime;

    private float timeLeft;

    void Start()
    {
        GameManager.instance.addItemToList(this);
        timeLeft = ItemDisappearTime;
    }

    //Update is called every frame
    void Update()
    {
        if (GameManager.instance.isPaused)
        {
            return;
        }
        timeLeft -= Time.deltaTime;
        if (timeLeft <= 0)
        {
            gameObject.SetActive(false);
        }
        else if (timeLeft <= ItemRotTime)
        {
            GetComponent<SpriteRenderer>().sprite = rotSprite;
        }
    }

    public float getBonus()
    {
        return timeLeft <= ItemRotTime ? punish : bonus;
    }

    void OnDisable()
    {
        GameManager.instance.removeItemFromList(this);
    }

}
