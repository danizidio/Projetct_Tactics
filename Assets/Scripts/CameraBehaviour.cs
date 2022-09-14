using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

[RequireComponent(typeof(CinemachineBrain))]
[RequireComponent(typeof(CinemachineVirtualCamera))]
public class CameraBehaviour : MonoBehaviour
{
    public delegate void _onSearchingPlayer(GameObject p);
    public static _onSearchingPlayer OnSearchingPlayer;

    void FindPlayer(GameObject p)
    {
        GetComponent<CinemachineVirtualCamera>().Follow = p.transform;
    }

    private void OnEnable()
    {
        OnSearchingPlayer += FindPlayer;
    }
    private void OnDisable()
    {
        OnSearchingPlayer -= FindPlayer;
    }
}
