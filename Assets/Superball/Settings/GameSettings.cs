using FriendsGamesTools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettings : SettingsScriptable<GameSettings>
{
    public Vector2 SpawnPos = new Vector2(5,12f);

    private float _gravity = -10;

    /// <summary>
    /// Return a force required for a jump
    /// </summary>
    /// <param name="startPoint">Start jump position.</param>
    /// <param name="endPoint">End jump position.</param>
    /// <param name="duration">jump duration.</param>
    /// <returns></returns>
    public Vector3 GetRequiredForce(Vector3 startPoint, Vector3 endPoint, float duration)
    {
        Vector3 force = Vector3.zero;
        force.x = (endPoint.x - startPoint.x) / duration;
        force.y = (endPoint.y - startPoint.y - _gravity * duration * duration / 2f) / duration;
        force.z = (endPoint.z - startPoint.z) / duration;
        return force;
    }

    /// <summary>
    /// Returns jump position in a given time
    /// </summary>
    /// <param name="startPoint">Start jump position.</param>
    /// <param name="targetPoint">End jump position.</param>
    /// <param name="time">Time passed from the beginning of the jump.</param>
    /// <returns></returns>
    public Vector3 GetTrajectoryPoint(Vector3 startPoint, Vector3 targetPoint, float time, float multiplier=1)
    {
        var force = GetRequiredForce(startPoint, targetPoint, multiplier);
        return new Vector3(startPoint.x + force.x * time, startPoint.y + force.y * time + _gravity * time * time / 2f, startPoint.z + force.z * time);
    }

}
