using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.AI;
using UnityEngine.UI;
using System;


public class Player : MonoBehaviour
{
    [SerializeField] private Slider doubleAttackCooldownSlider;
    [SerializeField] private Button doubleAttackButton;
    private NavMeshAgent Agent;
    [SerializeField] private float _hp = 10;
    public float Hp
    {
        get
        {
            return _hp;
        }
        set
        {
            _hp = value;
            if (value <= 0)
            {
                Die();
            }

        }
    }
    private float damage;
    [SerializeField] private float attackDamage = 2;
    [SerializeField] private float doubleAttackDamage = 3;
    [SerializeField] private float doubleAttackCooldown = 3;
    private float progress_doubleAttackCooldown = 0;
    public float AtackSpeed;
    public float AttackRange = 2;

    private bool _cooldown = false;
    //private float lastAttackTime = 0;
    private bool isDead = false;
    public Animator AnimatorController;

 
    Enemie closestEnemie = null;

    private void Start()
    {
        Agent = GetComponent<NavMeshAgent>();

    }

    private void Update()
    {
        if (isDead)
        {
            return;
        }
        DoubleAttackButtonCheck(Time.deltaTime);
        // управління
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 moveDirection = new Vector3(horizontal, 0f, vertical).normalized;
        // переміщення реалізую через навмеш бо це простіше
        // якщо гравець має стрибати/падати з платформи то це не підходить
        Agent.SetDestination(transform.position + moveDirection);
        AnimatorController.SetFloat("Speed", Agent.velocity.magnitude);
    }


    private void DoubleAttackButtonCheck(float delta)
    {
        // зменшує кд
        progress_doubleAttackCooldown = Mathf.MoveTowards(progress_doubleAttackCooldown, 0.0f, delta);

        // перевіряє чи на кд
        if (progress_doubleAttackCooldown > 0.0f)
        {
            doubleAttackCooldownSlider.value = 1.0f - progress_doubleAttackCooldown/doubleAttackCooldown;
            doubleAttackButton.interactable = false;
            
            return;
        }
        doubleAttackCooldownSlider.value = 0.0f;
        //doubleAttackButton.interactable = true;
        
        // так бо інакше атакує трупи
        Enemie closestEnemie = GetClosestEnemy();
        
        if (closestEnemie != null)
        {
            var distance = Vector3.Distance(transform.position, closestEnemie.transform.position);
            doubleAttackButton.interactable = distance <= AttackRange;
        }
        
    }

    private void Die()
    {
        isDead = true;
        AnimatorController.SetTrigger("Die");
        Agent.isStopped = true;
        SceneManager.Instance.GameOver();
    }

    public void DoubleAttack()
    {
        if (!_cooldown && progress_doubleAttackCooldown == 0)
        {
            progress_doubleAttackCooldown = doubleAttackCooldown;
            damage = doubleAttackDamage;
            AnimatorController.SetInteger("AttackType", 1);
            closestEnemie = GetClosestEnemy();
            StartCoroutine(Cooldown());
            AttackClosestEnemy();
        }
    }

    public void Attack()
    {
        
        if (!_cooldown)
        {
            AnimatorController.SetInteger("AttackType", 0);
            damage = attackDamage;
            closestEnemie = GetClosestEnemy();
            StartCoroutine(Cooldown());
            AttackClosestEnemy();
        }
    }

    public void DamageEnemy()
    {
        // повертається до ворога
        if (closestEnemie != null)
        {
            var distance = Vector3.Distance(transform.position, closestEnemie.transform.position);
            if (distance <= AttackRange)
            {
                closestEnemie.Hp -= damage;

            }
        }
    }

    private Enemie GetClosestEnemy()
    {
        // блок з ворогами
        // якщо робе не чіпай але у випадку великої кількості ворогів буде ресурсозатратним
        var enemies = SceneManager.Instance.Enemies;
        Enemie closestEnemie = null;
        for (int i = 0; i < enemies.Count; i++)
        {
            var enemie = enemies[i];
            if (enemie == null)
            {
                continue;
            }

            if (closestEnemie == null)
            {
                closestEnemie = enemie;
                continue;
            }

            var distance = Vector3.Distance(transform.position, enemie.transform.position);
            var closestDistance = Vector3.Distance(transform.position, closestEnemie.transform.position);

            if (distance < closestDistance)
            {
                closestEnemie = enemie;
            }

        }
        return closestEnemie;


    }

    private void AttackClosestEnemy()
    {
        // повертається до ворога
        if (closestEnemie != null)
        {
            var distance = Vector3.Distance(transform.position, closestEnemie.transform.position);
            if (distance <= AttackRange)
            {
                transform.LookAt(closestEnemie.transform);
                transform.transform.rotation = Quaternion.LookRotation(closestEnemie.transform.position - transform.position);
                //lastAttackTime = Time.time;
                //closestEnemie.Hp -= Damage;
            }
        }
        AnimatorController.SetTrigger("Attack");
    }


    private IEnumerator Cooldown()
    {
        _cooldown = true;
        Agent.isStopped = true;
        yield return new WaitForSeconds(AtackSpeed);
        Agent.isStopped = false;
        _cooldown = false;
    }

}
