using System.Numerics;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SceneAdminScript : MonoBehaviour
{
    public TMPro.TextMeshProUGUI GSTimerLabel,ESTimerLabel,ScoreLabel,TimerLabel;

    public int Score;
    public float SessionTimer = 120;
    public float curTimer;
    
    public float enemySpawnTimer = 5;
    public bool SessionStart;
    
    public PlayerController player;


    public GameObject EndingMenu;

    public List<EnemyPool> enemiesOnPool = new List<EnemyPool>();
    public GameObject[] Enemies;

    public Transform[] SpawnPoints;

    public void GSTimerSlider(float timer){
        GSTimerLabel.text = timer+" s";
        SessionTimer = timer;
    }
    public void ESTimerSlider(float timer){
        ESTimerLabel.text = string.Format("Every {0} Secs",timer);
        enemySpawnTimer = timer;
    }

    public void PlayButton(){
        Time.timeScale = 1;
        SessionStart = true;
        Score = 0;
        curTimer = SessionTimer;
        player.ship.transform.position = UnityEngine.Vector3.zero;        
        player.ship.transform.eulerAngles = UnityEngine.Vector3.zero;
        player.ship.Activate();
        StartCoroutine(SpawnEnemyOnTime());
    }
    public void OptionsButton(){

    }
    public void MainMenuButton(){
        foreach(EnemyPool enemy in enemiesOnPool){
            enemy.Available = true;
            enemy.ship.Dead = true;
            StartCoroutine(enemy.ship.Deactivate(0));
        }
        StopAllCoroutines();
        SessionStart = false;
        Time.timeScale = 0;
    }

    public void PauseButton(){
        Time.timeScale = 0;
        SessionStart = false;
    }

    public void ResumeButton(){        
        Time.timeScale = 1;        
        SessionStart = true;
    }

    public void Ending(){
        EndingMenu.SetActive(true);
        foreach(EnemyPool enemy in enemiesOnPool){
            enemy.Available = true;
            enemy.ship.Dead = true;
            StartCoroutine(enemy.ship.Deactivate(0));
        }
        StopAllCoroutines();
        SessionStart = false;
        Time.timeScale = 0;
    }



    IEnumerator SpawnEnemyOnTime(){
        List<GameObject> enemiesAvailable = new List<GameObject>();
        while (curTimer > 0){
            if (enemiesAvailable.Count > 0){
                SpawnEnemy(enemiesAvailable[0]);                
                enemiesAvailable.RemoveAt(0);
            }
            else {
                enemiesAvailable = IsAvailable(enemiesOnPool);
                if (enemiesAvailable.Count > 0){ 
                    SpawnEnemy(enemiesAvailable[0]);
                }
                else {
                    GameObject enemy = Instantiate(Enemies[Random.Range(0,Enemies.Length)]) as GameObject;
                    EnemyBehaviour enemyBehaviour = enemy.GetComponent<EnemyBehaviour>();
                    enemyBehaviour.admin = this;
                    enemyBehaviour.player = player;

                    SpawnEnemy(enemy);
                    enemiesOnPool.Add(new EnemyPool(enemy.GetComponent<Ship>()));
                }
            }
            
            yield return new WaitForSeconds(enemySpawnTimer);
        }
    }

    void SpawnEnemy(GameObject enemy)
    {
        Ship enemyShip = enemy.GetComponent<Ship>();
        enemy.transform.position = SpawnPoints[Random.Range(0,SpawnPoints.Length)].position;
        enemy.SetActive(true);
        enemyShip.Activate();
    }
    private List<GameObject> IsAvailable(List<EnemyPool> enemiesOnPool)
    {
        List<GameObject> ep = new List<GameObject>();
        for(int i = 0;i < enemiesOnPool.Count;i++){
            if (enemiesOnPool[i].Available){
            ep.Add(enemiesOnPool[i].ship.gameObject);            
            this.enemiesOnPool[i].Available = false;
            }
        }
        return ep;
    }

    // Start is called before the first frame update
    void Awake()
    {
       
        
    }

    public void Active(){
        Time.timeScale = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(SessionStart == true) {
            curTimer -= Time.deltaTime;
            TimerLabel.text = string.Format("{0:0}",curTimer);
            ScoreLabel.text = string.Format("SCORE:\n{0}",Score);
            if (curTimer <= 0 || player.ship.Dead){
                curTimer = 0;
                Ending();
            }
        }
    }
}

public class EnemyPool{
    public EnemyPool(Ship ship){
        this.ship = ship;
        Available = false;
    }
    public Ship ship;
    public bool Available = false;
}