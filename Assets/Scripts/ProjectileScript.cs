using System.Numerics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D),typeof(Collider2D))]
public class ProjectileScript : MonoBehaviour
{

    public bool isOnPool,wasLaunch;

    public int damage;
    public float speed;
    public float radius = 0.15f;
    private Rigidbody2D rb;

    public GameObject afterEffect;

    // Start is called before the first frame update
    void Awake()
    {
        rb = this.GetComponent<Rigidbody2D>();
        //Launch(UnityEngine.Vector2.zero);
    }


    public void Launch(UnityEngine.Vector2 velocityParent){
        rb.AddForce((UnityEngine.Vector2)transform.up*speed + velocityParent * 2);
        isOnPool = false;
        StartCoroutine(activate(0.1f));
    }
    // Update is called once per frame
    private void FixedUpdate() {
        if (rb.velocity.magnitude <=0 && isOnPool == false && wasLaunch == true) {
            if (afterEffect != null) afterEffect.SetActive(true);
               StartCoroutine(deactivate(0.1f));
        }
    }
    void OnCollisionEnter2D(Collision2D other)
    {
        if (afterEffect != null) afterEffect.SetActive(true);
            StartCoroutine(deactivate(0.8f));
        Ship ship = other.gameObject.GetComponent<Ship>() != null?other.gameObject.GetComponent<Ship>():null;
        if (ship != null)
        {
            ship.Damage(damage);
        } 
        rb.velocity = UnityEngine.Vector2.zero;


    }

    IEnumerator activate(float waitTime){
        yield return new WaitForSeconds(waitTime);
        this.GetComponent<CircleCollider2D>().radius = radius;
        wasLaunch = true;
    }
    IEnumerator deactivate(float waitTime){
        yield return new WaitForSeconds(waitTime);
        afterEffect.SetActive(false);
        wasLaunch = false;
        isOnPool = true;
        this.GetComponent<CircleCollider2D>().radius = 0.0001f;
        this.gameObject.SetActive(false); 
               
    }
}
