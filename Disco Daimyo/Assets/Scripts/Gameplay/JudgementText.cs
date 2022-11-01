using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JudgementText : MonoBehaviour
{
    public Sprite disco;
    public Sprite good;
    public Sprite hmm;
    public Sprite miss;
    SpriteRenderer SpriteRenderer;

    public Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        SpriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    public void Disco()
    {
        SpriteRenderer.sprite = disco;
    }

    public void Good()
    {
        SpriteRenderer.sprite = good;
    }

    public void Hmm()
    {
        SpriteRenderer.sprite = hmm;
    }

    public void Miss()
    {
        SpriteRenderer.sprite = miss;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
