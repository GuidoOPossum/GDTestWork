using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemie : MonoBehaviour
{
    public float Hp;
    public float Damage;
    public float AtackSpeed;
    public float AttackRange = 2;


    public Animator AnimatorController;
    public NavMeshAgent Agent;

    private float lastAttackTime = 0;
    private bool isDead = false;


    public virtual void Start()
    {
        SceneManager.Instance.AddEnemie(this);
        Agent.SetDestination(SceneManager.Instance.Player.transform.position);

    }

    private void Update()
    {
        if(isDead)
        {
            return;
        }

        if (Hp <= 0)
        {
            Die();
            Agent.isStopped = true;
            return;
        }

        var distance = Vector3.Distance(transform.position, SceneManager.Instance.Player.transform.position);
     
        if (distance <= AttackRange)
        {
            Agent.isStopped = true;
            if (Time.time - lastAttackTime > AtackSpeed)
            {
                lastAttackTime = Time.time;
                
                AnimatorController.SetTrigger("Attack");
            }
        }
        else
        {
            Agent.SetDestination(SceneManager.Instance.Player.transform.position);
        }
        AnimatorController.SetFloat("Speed", Agent.speed); 
        Debug.Log(Agent.speed);

    }

    // бо у них однаковий контролер
    public void DamageEnemy()
    {
        SceneManager.Instance.Player.Hp -= Damage;
    }

    public virtual void Die()
    {
        SceneManager.Instance.RemoveEnemie(this);
        isDead = true;
        AnimatorController.SetTrigger("Die");
        // додає гравцю здоровя за вбивство
        SceneManager.Instance.Player.Hp += 1;
    }

}
