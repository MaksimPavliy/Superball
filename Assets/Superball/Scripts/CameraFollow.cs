using Superball;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CameraFollow : MonoBehaviour
{
    public Transform ball;
    public Vector3 offset;
    public FinishLine finish;

    void LateUpdate()
    {
        if (GameManager.instance.IsPlaying)
        {
            transform.position = new Vector3(ball.position.x + offset.x, ball.position.y + offset.y, offset.z);
        }
    }
}
