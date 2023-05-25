using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Tank))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(NavMeshAgent))]
public class FollowerAI : MonoBehaviour {
    #region Variables
    private NavMeshAgent navMesh;
    private Tank tank;
    #endregion

    #region Properties
    public void startFollowerAI() {
        tank = this.GetComponent<Tank>();
        navMesh = GetComponent<NavMeshAgent>();
        navMesh.speed = tank.Speed;
    }
    #endregion
    
    #region Builtin Methods
    void FixedUpdate() {
        navMesh.destination = tank.PlayerTankPosition;
    }
    #endregion

    #region Custom Methods
    #endregion
}
