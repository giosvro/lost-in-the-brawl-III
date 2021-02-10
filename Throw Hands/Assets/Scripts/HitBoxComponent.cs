﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBoxComponent : Bolt.EntityBehaviour<ICustomPlayerState>
{
    // Start is called before the first frame update
    public PlayerStatus player;
    public PlayerType myLimbs;
    public Rigidbody2D myRgbody;
    public float impactForce = 30.0f;
    public GameObject playerObject;

    private void Start()
    {
       
        if (BoltNetwork.IsClient)
        {
            impactForce = 30f;
        }else
        {
            impactForce = -30f;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("limbHitbox"))
        {
            if(collision.gameObject.GetComponent<LimbHitComponent>().LimbComponent.playerType != myLimbs && collision.gameObject.GetComponent<LimbHitComponent>().Damaging )
            {
                collision.gameObject.GetComponent<LimbHitComponent>().Damaging = false;
                player.TakeDamage();
                state.Animator.SetTrigger("Damage");

                if (player.isFlipped)
                {
                    impactForce *= -1;
                }

                if (player.state.Health == 1 || player.state.EnemyHealth == 1)
                {
                    impactForce *= 1.5f;
                }

                if (collision.gameObject.GetComponent<LimbHitComponent>().rdbody.velocity.x > 0.0f)
                {
                    myRgbody.AddForce(new Vector2(-impactForce, 0f), ForceMode2D.Impulse);
                }
                else
                {
                    myRgbody.AddForce(new Vector2(impactForce, 0f), ForceMode2D.Impulse);
                }

                collision.gameObject.GetComponent<LimbHitComponent>().rdbody.velocity = Vector2.zero;
                collision.gameObject.GetComponent<LimbHitComponent>().rdbody.AddForce(new Vector2(0f, -5.0f),ForceMode2D.Impulse);
                collision.gameObject.GetComponent<LimbHitComponent>().limb.layer = LayerMask.NameToLayer("TransparentFX");
                MakeSlowMotion();
            }
            
        }
    }

    private void MakeSlowMotion()
    {
        bool slowed = false;

        if (player.state.Health == 1 || player.state.EnemyHealth == 1)
        {
            Time.timeScale = 0.05f;
            Time.fixedDeltaTime = Time.timeScale * .02f;
        }
    }

    public float slowSeconds = 40.0f;

    private void Update()
    {
        Time.timeScale += (1f / slowSeconds) * Time.unscaledDeltaTime;
        Time.timeScale = Mathf.Clamp(Time.timeScale, 0f, 1f);
    }

}
