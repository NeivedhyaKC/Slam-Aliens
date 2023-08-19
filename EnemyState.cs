using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class EnemyState : MonoBehaviour
{
    public EnemyStateMachine.EnemyStateType type;
    public abstract void OnStateEnter();
    public abstract EnemyStateMachine.EnemyStateType OnUpdate();
    public abstract void OnStateExit();
}
