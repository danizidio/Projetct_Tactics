using UnityEngine;
using Unity.Cinemachine;

[RequireComponent(typeof(CinemachineBrain))]
[RequireComponent(typeof(CinemachineCamera))]
public class CameraBehaviour : MonoBehaviour
{
    public delegate void _onSearchingPlayer(GameObject p);
    public static _onSearchingPlayer OnSearchingPlayer;

    void FindPlayer(GameObject p)
    {
        GetComponent<CinemachineCamera>().Follow = p.transform;
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
