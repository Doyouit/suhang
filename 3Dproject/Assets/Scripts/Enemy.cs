using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public enum Type { A, B, C, D };
    public Type enemyType;
    public int maxHealth;
    public int curHealth;
    public int score;
    public GameManager manager;
    public Transform target;
    public BoxCollider meleeArea;
    public GameObject bullet;
    public GameObject[] coins;

    public bool isChase;
    public bool isAttack;
    public bool isDead;

    public Rigidbody rigid;
    public BoxCollider boxCollider;
    public MeshRenderer[] meshs;
    public NavMeshAgent nav;
    public Animator anim;

    


    
    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        meshs = GetComponentsInChildren<MeshRenderer>();
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();

        if(enemyType != Type.D)
            Invoke("ChaseStart", 2);

        
    }

    void ChaseStart()
    {
        isChase = true;
        anim.SetBool("isWalk", true);
    }
    
    void Update()
    {
        if(nav.enabled && enemyType != Type.D)
        {
            nav.SetDestination(target.position);
            nav.isStopped = !isChase;
        }
            
        
        
    }

    void FreezeVelocity()
    {
        if (isChase) 
        {
            rigid.velocity = Vector3.zero;
            rigid.angularVelocity = Vector3.zero;
        }
        
    }

    void Targerting()
    {
        if (!isDead && enemyType != Type.D)
        {
            float targetRadius = 1.5f;
            float targetRange = 3f;

            switch (enemyType)
            {
                case Type.A:
                    targetRadius = 1.5f;
                    targetRange = 3f;
                    break;
                case Type.B:
                    targetRadius = 1f;
                    targetRange = 12f;
                    break;
                case Type.C:
                    targetRadius = 0.5f;
                    targetRange = 20f;

                    break;
        }
        RaycastHit[] rayHits = Physics.SphereCastAll(transform.position, targetRadius, transform.forward, targetRange, LayerMask.GetMask("Player"));
        if (rayHits.Length > 0 && !isAttack)
        {
            StartCoroutine(Attack());
        }
        }
        

        
    }

    IEnumerator Attack()
    {
        isChase = false;
        isAttack = true;
        anim.SetBool("isAttack", true);

        switch (enemyType)
        {
            case Type.A:
                yield return new WaitForSeconds(0.2f);
                meleeArea.enabled = true;

                yield return new WaitForSeconds(1f);
                meleeArea.enabled = false;

                yield return new WaitForSeconds(1f);
                break;
            case Type.B:
                yield return new WaitForSeconds(0.1f);
                rigid.AddForce(transform.forward * 20, ForceMode.Impulse);
                meleeArea.enabled = true;

                yield return new WaitForSeconds(0.5f);
                rigid.velocity = Vector3.zero;
                meleeArea.enabled = false;
                yield return new WaitForSeconds(2f);
                break;
            case Type.C:
                yield return new WaitForSeconds(0.5f);
                GameObject instantBullet = Instantiate(bullet, transform.position, transform.rotation);
                Rigidbody rigidBullet = instantBullet.GetComponent<Rigidbody>();
                rigidBullet.velocity = transform.forward * 20;

                yield return new WaitForSeconds(0.5f);
                break;
        }
        

        

        isChase = true;
        isAttack = false;
        anim.SetBool("isAttack", false);
    }

    void FixedUpdate()
    {
        Targerting();
        FreezeVelocity();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Melee")
        {
            Weapon weapon = other.GetComponent<Weapon>();
            curHealth -= weapon.damage;
            if(curHealth<0)
            {
                curHealth = 0;
            }
            Vector3 reacVec = transform.position - other.transform.position;
            StartCoroutine(OnDamage(reacVec, false));
            
        }
        else if (other.tag == "Bullet")
        {

            Bullet bullet = other.GetComponent<Bullet>();

            curHealth -= bullet.damage;

            
            if(curHealth<0)
            {
                curHealth = 0;
            }
            Vector3 reacVec = transform.position - other.transform.position;
            Destroy(other.gameObject);
            StartCoroutine(OnDamage(reacVec, false));
            
        }
    }

    public void HitByGrenade(Vector3 explosionPos)
    {
        curHealth -= 100;
        if(curHealth<0)
            {
                curHealth = 0;
            }
        Vector3 reactVec = transform.position - explosionPos;
        StartCoroutine(OnDamage(reactVec, true));
    }

    IEnumerator OnDamage(Vector3 reacVec, bool isGrenade)
    {
        foreach(MeshRenderer mesh in meshs)
            mesh.material.color = Color.red;

        

        if(curHealth > 0)
        {
            foreach(MeshRenderer mesh in meshs)
                mesh.material.color = Color.white;
            yield return new WaitForSeconds(0.1f);
        }
        else if(!isDead) 
        {
            foreach(MeshRenderer mesh in meshs)
                mesh.material.color = Color.gray;

            gameObject.layer = 13;
            isDead = true;
            isChase = false;
            nav.enabled = false;
            anim.SetTrigger("doDie");
            Player player = target.GetComponent<Player>();
            player.score += score;
            int ranCoin = Random.Range(0, 3);
            Instantiate(coins[ranCoin], transform.position, Quaternion.identity);

            if (isDead)
            {
                switch (enemyType)
            {
                case Type.A:
                    manager.enemyCntA--;
                    
                    break;
                case Type.B:
                    manager.enemyCntB--;
                    
                    break;
                case Type.C:
                    manager.enemyCntC--;
                    
                    break;
                case Type.D:
                    manager.enemyCntD--;
                    
                    break;
            }
            }
            

            if (isGrenade)
            {
                reacVec = reacVec.normalized;
                reacVec += Vector3.up * 3;
                nav.enabled = false;
                rigid.freezeRotation = false;
                rigid.AddForce(reacVec * 5, ForceMode.Impulse); 
                rigid.AddTorque(reacVec * 15, ForceMode.Impulse);
            }
            else 
            {
                reacVec = reacVec.normalized;
                reacVec += Vector3.up;

                rigid.AddForce(reacVec * 5, ForceMode.Impulse); 
            }

            
            Destroy(gameObject, 3);
        }
    }
}
