using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class StatusBarRotate : MonoBehaviour
{
    [SerializeField]GameObject player;
    Vector3 playerpos;
    Vector3 dis;
    private void Start()
    {
        dis=transform.position-playerpos;
    }
    void LateUpdate()
    {
        playerpos = player.transform.position;
        transform.position = playerpos + dis;
    }
}
