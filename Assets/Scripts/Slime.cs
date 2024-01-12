using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : Enemie
{
    
    public bool doubling = true;
    [SerializeField] private float doublingHP = 5.0f;
    [SerializeField] private float doublingDamage = 1.0f;

    // по причині що старт викликаєсть після Instantiate і наступна хвиля може початись коли є маленькі слайми
    public override void Start()
    {
        if (doubling)
            SceneManager.Instance.AddEnemie(this);
        
        Agent.SetDestination(SceneManager.Instance.Player.transform.position);

    }

    public override void Die()
    {
        if (doubling)
        {
            for (int i = 0; i < 2; i++)
            {
                Slime slimePref = this;
                Vector3 pos = transform.position;
                Slime newSlime = Instantiate(slimePref, pos, Quaternion.identity);
                newSlime.doubling = false;
                newSlime.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
                newSlime.Damage = doublingDamage;
                newSlime.Hp = doublingHP;
                // по причині що старт викликаєсть після Instantiate і наступна хвиля може початись коли є маленькі слайми
                SceneManager.Instance.AddEnemie(newSlime);
            }

        }
        //
        base.Die();
    }


}
