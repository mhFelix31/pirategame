using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Ship : MonoBehaviour
{
    public Health health;
    public TMPro.TextMeshPro healthText;
    public float speed,turnSpeed;

    public float attackRate;
    public List<Sprite> hullStates = new List<Sprite>();
    public Sprite hullDead,sailDead;
    public List<Sprite> sailStates = new List<Sprite>();
    public SpriteRenderer hull,sail;
    public Vector2 sceneBounds;

    public GameObject[] cannonsForward,cannonsLeft,cannonsRight,cannonsBack;
    private List<GameObject> cannonballPool = new List<GameObject>();
    public GameObject cannonBall;

    private Rigidbody2D rb;
    private float deathTimer = 5;
    public bool Dead = false;



    void Awake()
    {
        Activate();
        rb = GetComponent<Rigidbody2D>();
        CBPool();
    }
    public void Activate(){
        StopAllCoroutines();
        hull.sprite = hullStates[0];
        sail.sprite = sailStates[0];
        health.cur = health.max;        
        healthText.text = string.Format("{0}/{1}",health.cur,health.max);
        transform.localScale = Vector3.one;
        Dead = false;
    }

    void CBPool(){
        int totalCannons = cannonsForward.Length+cannonsBack.Length+cannonsLeft.Length+cannonsRight.Length;
        for (int i = 0; i < (5/attackRate)*(totalCannons); i++){
            GameObject cb = Instantiate(cannonBall,this.transform.position,this.transform.rotation) as GameObject;
            cb.transform.parent = this.transform;
            cb.SetActive(false);
            cannonballPool.Add(cb);
        }
    }
    
    void Update()
    {
        healthText.transform.eulerAngles = Vector3.zero;
    }

    public void MoveForward(){
        
        Vector3 checker = (-1*transform.up * speed * Time.deltaTime);

        checker.x = (checker.x + transform.position.x < sceneBounds.x && checker.x + transform.position.x> -sceneBounds.x)?checker.x:0;        
        checker.y = (checker.y + transform.position.y < sceneBounds.y && checker.y + transform.position.y > -sceneBounds.y)?checker.y:0;
        if (!Dead)
        rb.AddForce(checker);
    }

    public void Rotate(float direction){
        if (!Dead)
        rb.AddTorque(direction * turnSpeed * Time.deltaTime);
    }


    public void Damage(int damage){
        health.cur -= damage;
        healthText.text = string.Format("{0}/{1}",health.cur,health.max);
        if (health.cur <= 0)Death();
        else UpdateShipState();

    }

    void Death(){
        hull.sprite = hullDead;
        sail.sprite = sailDead;
        Dead = true;
        rb.velocity = Vector3.zero;
        StartCoroutine(Deactivate(deathTimer));
    }
    

    public IEnumerator Deactivate(float waitTimer){
        float rateWillDisappear = 1-(1/waitTimer*0.1f);
        
        
        while(waitTimer > 0){
            yield return new WaitForSeconds(0.1f);
            transform.localScale *= rateWillDisappear;
            waitTimer -= 0.1f;
        }
        gameObject.SetActive(false);
    }


    void UpdateShipState(){
        
        if (hullStates.Count > 0)hull.sprite = hullStates[(int) (health.cur/(health.max/(hullStates.Count-1)))];
        if (sailStates.Count > 0)sail.sprite = sailStates[(int) (health.cur/(health.max/(sailStates.Count-1)))];
    }

    public void Shoot(GameObject[] cannons){
        if (cannons.Length > 0){
            for (int i = 0; i < cannons.Length; i++){
                GameObject cb = cannonballPool[0];
                cb.transform.position = cannons[i].transform.position;
                cb.transform.rotation = cannons[i].transform.rotation;
                cb.SetActive(true);
                cb.GetComponent<ProjectileScript>().Launch(rb.velocity);
                cannonballPool.Remove(cb);
                cannonballPool.Add(cb);
            }
        }
    }

}

[System.Serializable]
public class Health{
    public int cur,max;
}
