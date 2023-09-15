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
    private float _distance;//�G����v���C���[�܂ł̋���
    [SerializeField] private float _enemySpeed = 3.5f;//�G�ʂɐݒ�
    [SerializeField] private float _afterEnemyAttackedWaitTime = 2f;
    private bool isActive;
    private Vector3 targetPos;//player�̈ʒu
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
    /// �v���C���[�����G�O�̎��̓���
    /// </summary>
    void EnemyIdle()
    {

    }

    /// <summary>
    /// �G�̍��G�ƒǐ�
    /// </summary>
    /// <param name="enemyType"></param>
    /// <param name="enemySearchRange"></param>
    void EnemyAction(EnemyType enemyType, float enemySearchRange)
    {
        _distance = (_player.transform.position - transform.position).sqrMagnitude;
        //���G
        if (_distance < enemySearchRange * enemySearchRange)
        {

            if (_distance > 25)
            {
                EnemyMove();        //enemy�̒ǐ�(���G�͈͓����U���͈͓��̎�)
            }
            else if (_distance <= 25)
            {
                
                switch (enemyType)      //enemy�̍U��()
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
            EnemyIdle();        //���G�͈͊O�̎�
        }
    }
    /// <summary>
    /// �F�̍U��
    /// </summary>
    IEnumerator BearAttackSelect()
    {
        if (isActive == false)
        {
            int randomAction = Random.Range(0, 2);
            
            if (randomAction == 0)//�U��A
            {
                targetPos = new Vector3(_player.transform.position.x,this.transform.position.y,_player.transform.position.z);
                isActive = true;
                agent.isStopped = true;
                yield return StartCoroutine(BearAttackA(targetPos));
                agent.isStopped = false;

            }
            else if (randomAction == 1)//�U���a
            {
                isActive = true;
                agent.isStopped= true;
                yield return StartCoroutine(BearAttackB());
                agent.isStopped = false;
            }
        }
    }

    /// <summary>
    ///���̍U��
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

            if (randomAction == 0)//�U��A
            {
                targetPos = new Vector3(_player.transform.position.x, this.transform.position.y, _player.transform.position.z);
                isActive = true;
                agent.isStopped = true;
                yield return StartCoroutine(FishAttackA(targetPos));
                agent.isStopped = false;

            }
            else if (randomAction == 1)//�U���a
            {
                isActive = true;
                yield return StartCoroutine(FishAttackB());

            }
        }
    }

    /// <summary>
    /// ���̍U��
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

    //���ˑ��x�v�Z
    

    private void EnemyMove()
    {
            agent.SetDestination(_player.transform.position);       
    }

    //----------�F�̍U������----------
    IEnumerator BearAttackA(Vector3 targetPos)
    {
        yield return new WaitForSeconds(_afterEnemyAttackedWaitTime);
        float distance = Vector3.Distance(transform.position, targetPos);  // �ړ��������v�Z
        float duration = distance / _enemySpeed;  // �ړ��ɂ����鎞�Ԃ��v�Z

        float elapsedTime = 0f;
        transform.LookAt(targetPos);
        while (elapsedTime < duration)
        {
            // ���݈ʒu����ڕW�ʒu�Ɍ������Ĉړ�
            transform.position = Vector3.Lerp(transform.position, targetPos, elapsedTime / duration);

            elapsedTime += Time.deltaTime;
           yield return null;
        }
        isActive = false;
    }

    //�T�b�Ԉړ�����
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

    //----------�F�̍U������----------*/
}
