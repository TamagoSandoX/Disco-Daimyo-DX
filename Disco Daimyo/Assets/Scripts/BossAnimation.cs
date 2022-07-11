using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAnimation : MonoBehaviour
{

    Animator animator;

    public int waitTime = 4; //This is how long to wait before the boss script transforms (in seconds)
    public int phaseTwo = 46; //This is how long until phase 2 triggers (seconds after first phase)
    public bool isTwoPhase = true; //Is there a second phase? This could be done better but im lazy

    public float speed = 1.0f; //How fast it shakes
    public float amount = 1.0f; //How much it shakes
    public float speed2 = 2.0f;
    public float amount2 = 2.0f; // As above, but for phase 2 (faster)

    private bool isShaking = false; //Is the boss sprite shaking?

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        StartCoroutine(ChangeSprite());
    }

    // Update is called once per frame
    void Update()
    {
        if (isShaking)
        {
            //Shake();
            animator.SetBool("isShaking", true);
        }
    }

    IEnumerator ChangeSprite()
    {
        //Debug.Log("Waiting for sprite change");
        yield return new WaitForSeconds(waitTime);
       
        //Debug.Log("Sprite changed");
        isShaking = true;
        if (isTwoPhase)
        {
            yield return new WaitForSeconds(phaseTwo);
            speed = speed2;
            amount = amount2;
        }

    }

    private void Shake()
    {
        transform.position = new Vector3((Mathf.Sin(Time.time * speed) * amount),
            transform.position.y, transform.position.z);
    }
}
