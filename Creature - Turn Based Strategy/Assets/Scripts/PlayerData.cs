using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public string playerName;
    public float randomNumber;
    public float[] position;


    public PlayerData(PlayerController360 _player)
    {
        playerName = _player.PlayerName;
        randomNumber = _player.jumpForce;
        position = new float[3];
        position[0] = _player.transform.position.x;
        position[1] = _player.transform.position.y;
        position[2] = _player.transform.position.z;
    }
}
