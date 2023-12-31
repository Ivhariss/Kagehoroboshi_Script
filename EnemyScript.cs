using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using TMPro;
using Unity.VisualScripting;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class EnemyScript : MonoBehaviour
{
    public enum EnemyType
    {
        Bird,
        Beast,
        Fish,
        Ghost,
        Statue
    }
    [SerializeField]private EnemyType enemyType;
    [SerializeField]private EnemyStatus enemyStatus;
    [SerializeField] private float enemySeachRange;
    private NavMeshAgent agent;
    private GameObject _player;
    private float _distance;//敵からプレイヤーまでの距離
    [SerializeField] private float _enemySpeed = 3.5f;//敵別に設定
    [SerializeField] private float _afterEnemyAttackedWaitTime = 2f;
    private bool isActive;
    private Vector3 targetPos;//playerの位置
    [SerializeField] private FishJump FishJump;
    private void Awake()
    {
        enemyStatus = GameObject.Find("EnemyStats").GetComponent<EnemyStatus>();
       agent = this.GetComponent<NavMeshAgent>();
    }
    private void Start()
    {
        _player = GameObject.Find("Player");
        agent.speed = _enemySpeed;
        agent.angularSpeed = 200f;
        //EnemyAction(this.enemyType, 10f);
        
    }
    private void Update()
    {
        EnemyAction(this.enemyType, 15);
    }
    /// <summary>
    /// プレイヤーが索敵外の時の動き
    /// </summary>
    void EnemyIdle()
    {

    }

    /// <summary>
    /// 敵の索敵と追跡
    /// </summary>
    /// <param name="enemyType"></param>
    /// <param name="enemySearchRange"></param>
    void EnemyAction(EnemyType enemyType, float enemySearchRange)
    {
        _distance = (_player.transform.position - transform.position).sqrMagnitude;
        //索敵
        if (_distance < enemySearchRange * enemySearchRange)
        {

            if (_distance > 25)
            {
                EnemyMove();        //enemyの追跡(索敵範囲内かつ攻撃範囲内の時)
            }
            else if (_distance <= 25)
            {
                
                switch (enemyType)      //enemyの攻撃()
                {
                    case EnemyType.Bird:
                        if(isActive == false)StartCoroutine(BirdAttack());
                        break;
                    case EnemyType.Fish:
                        StartCoroutine(FishAttackSelect());
                        break;
                    case EnemyType.Beast:
                        if(isActive == false)StartCoroutine(BearAttackSelect());
                        break;
                }
            }
        }
        else
        {
            EnemyIdle();        //索敵範囲外の時
        }
    }
    /// <summary>
    /// 熊の攻撃
    /// </summary>
    IEnumerator BearAttackSelect()
    {
        if (isActive == false)
        {
            int randomAction = Random.Range(0, 2);
            
            if (randomAction == 0)//攻撃A
            {
                targetPos = new Vector3(_player.transform.position.x,this.transform.position.y,_player.transform.position.z);
                isActive = true;
                agent.isStopped = true;
                yield return StartCoroutine(BearAttackA(targetPos));
                agent.isStopped = false;

            }
            else if (randomAction == 1)//攻撃Ｂ
            {
                isActive = true;
                agent.isStopped= true;
                yield return StartCoroutine(BearAttackB());
                agent.isStopped = false;
            }
        }
    }

    /// <summary>
    ///鳥の攻撃
    /// </summary>
    IEnumerator BirdAttack()
    {
        agent.speed = 0;
        yield return new WaitForSeconds(0.5f);
        agent.speed = _enemySpeed;
        float elapsedTime = 0f;
        float duration = 5f;
        while (elapsedTime < duration)
        {
            agent.speed = 2.5f;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        agent.speed = 2f;
        isActive = false;
    }

    IEnumerator FishAttackSelect()
    {
        if (isActive == false)
        {
            int randomAction = Random.Range(0, 2);

            if (randomAction == 0)//攻撃A
            {
                targetPos = new Vector3(_player.transform.position.x, this.transform.position.y, _player.transform.position.z);
                isActive = true;
                agent.isStopped = true;
                yield return StartCoroutine(FishAttackA(targetPos));
                agent.isStopped = false;

            }
            else if (randomAction == 1)//攻撃Ｂ
            {
                isActive = true;
                yield return StartCoroutine(FishAttackB());

            }
        }
    }

    /// <summary>
    /// 魚の攻撃
    /// </summary>
    IEnumerator FishAttackA(Vector3 targetPos)
    {
        yield return new WaitForSeconds(_afterEnemyAttackedWaitTime);
        Debug.Log("jump");
        FishJump.JumpFish(targetPos);
        yield return new WaitForSeconds(3f);
        isActive = false;
    }

    IEnumerator FishAttackB()
    {
        Debug.Log("B");
        isActive = false;
        yield return null;
    }

    //放射速度計算
    

    private void EnemyMove()
    {
            agent.SetDestination(_player.transform.position);       
    }

    //----------熊の攻撃処理----------
    IEnumerator BearAttackA(Vector3 targetPos)
    {
        yield return new WaitForSeconds(_afterEnemyAttackedWaitTime);
        float distance = Vector3.Distance(transform.position, targetPos);  // 移動距離を計算
        float duration = distance / _enemySpeed;  // 移動にかかる時間を計算

        float elapsedTime = 0f;
        transform.LookAt(targetPos);
        while (elapsedTime < duration)
        {
            // 現在位置から目標位置に向かって移動
            transform.position = Vector3.Lerp(transform.position, targetPos, elapsedTime / duration);

            elapsedTime += Time.deltaTime;
           yield return null;
        }
        isActive = false;
    }

    //５秒間移動する
    IEnumerator BearAttackB()
    {       
        agent.speed = 0;
        yield return new WaitForSeconds(0.5f);
        agent.speed = _enemySpeed;
        float elapsedTime = 0f;
        float duration = 5f;
        while (elapsedTime < duration)
        {
            agent.speed = 2.5f;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        agent.speed = 2f;
        isActive = false;
    }

    //----------熊の攻撃処理----------*/
}
