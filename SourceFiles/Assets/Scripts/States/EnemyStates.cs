using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyStates : IState
{
    EnemyControl owner;

    public EnemyStates(EnemyControl owner) { this.owner = owner; }

    public void Enter()
    {
        Debug.Log("entering test state");
    }

    public void Execute()
    {
        Debug.Log("updating test state");
    }

    public void Exit()
    {
        Debug.Log("exiting test state");
    }
}

public class EnemyIdleState : IState
{
    EnemyControl owner;

    public EnemyIdleState(EnemyControl owner) { this.owner = owner; }

    public void Enter()
    {

    }

    public void Execute()
    {
        owner.transform.Rotate(0, 0.5f, 0);
    }

    public void Exit()
    {

    }
}

public class EnemySearchState : IState
{
    EnemyControl owner;

    public EnemySearchState(EnemyControl owner) { this.owner = owner; }

    float range;
    float timer = 1.5f;

    public void Enter()
    {
        range = Random.Range(1, owner.enemyWanderRange);
    }

    public void Execute()
    {
        timer -= Time.deltaTime;

        LocateTarget();

        if (timer <= 0)
        {
            SetRandomDestination();
        }
    }

    public void Exit()
    {

    }

    bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {
        for (int i = 0; i < 30; i++)
        {
            Vector3 randomPoint = center + Random.insideUnitSphere * range;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
            {
                result = hit.position;
                return true;
            }
        }
        result = Vector3.zero;
        return false;
    }

    void SetRandomDestination()
    {
        Vector3 point;
        if (RandomPoint(owner.transform.position, range, out point))
        {
            owner.GetComponent<NavMeshAgent>().SetDestination(point);
            range = Random.Range(1, owner.enemyWanderRange);
            timer = 1.5f;
        }
    }

    void LocateTarget()
    {
        if (owner.curTarget != null)
        {
            float chase = Vector3.Distance(owner.transform.position, owner.curTarget.position);
            if (chase <= owner.chaseRange)
            {
                owner.stateMachine.ChangeState(new EnemyChaseState(owner));
            }
        }
    }
}

public class EnemyChaseState : IState
{
    EnemyControl owner;

    public EnemyChaseState(EnemyControl owner) { this.owner = owner; }

    public void Enter()
    {

    }

    public void Execute()
    {
        ChaseTarget();
    }

    public void Exit()
    {

    }

    void ChaseTarget()
    {
        if (owner.curTarget != null)
        {
            float chase = Vector3.Distance(owner.transform.position, owner.curTarget.position);
            if (chase <= owner.chaseRange && chase > owner.stopRange)
            {
                owner.agent.SetDestination(owner.curTarget.position);
            }
            else if (chase <= owner.chaseRange && chase < owner.stopRange)
            {
                owner.agent.SetDestination(owner.transform.position);
            }
            else if (chase > owner.chaseRange)
            {
                owner.stateMachine.ChangeState(new EnemySearchState(owner));
            }
        }
        else
        {
            owner.stateMachine.ChangeState(new EnemyIdleState(owner));
        }
    }
}
