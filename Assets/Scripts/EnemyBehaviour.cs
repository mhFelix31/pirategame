using System.Reflection.Emit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    public SceneAdminScript admin;
    public Ship ship;
    public PlayerController player;

    public float DistanceToKeep = 4;
    public bool SuicideAttack = false;
    public GameObject afterEffect;
    public int damage = 50;
    private float nextShootTimer;
    private bool canShoot;
    private bool Dead;

    public bool ShootStopped = false;

    void FixedUpdate()
    {
        Movement();
        Timers();
        if (ship.Dead && !Dead){
            Dead = true;
            SendScore();
        }
    }


    void Movement(){
        if (SuicideAttack){
            ship.MoveForward();
        }
        else {
            if (Vector3.Distance(transform.position,player.transform.position) > DistanceToKeep)ship.MoveForward();
            else if (ShootStopped)Attack();
            if (!ShootStopped)Attack();
        }
        SteerToFaceTarget();
    }

    void SteerToFaceTarget(){
        Vector3 targetDirection = transform.position -player.transform.position ;
        if(-Vector2.SignedAngle(targetDirection,transform.up)>0)ship.Rotate(1);
        else if (-Vector2.SignedAngle(targetDirection,transform.up)<0)ship.Rotate(-1);        
    }
    void Timers(){
        if (nextShootTimer > 0)nextShootTimer -= Time.deltaTime;
        else canShoot = true;
    }
    void Attack(){
        
        if(!SuicideAttack && canShoot){
            ship.Shoot(ship.cannonsForward);            
            nextShootTimer = ship.attackRate;
            canShoot = false;
        }
    }

    void SendScore(){
        admin.Score ++;
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if(SuicideAttack){
            if (afterEffect != null) afterEffect.SetActive(true);
            this.ship.Damage(100);
            Ship ship = other.gameObject.GetComponent<Ship>() != null?other.gameObject.GetComponent<Ship>():null;
            if (ship != null)
            {
                ship.Damage(damage);
            } 
        }
    }



}
