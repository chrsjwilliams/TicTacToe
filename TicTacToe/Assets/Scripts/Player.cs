using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int playerNum { get; private set; }
    [SerializeField] private Sprite playerIcon;
    public Sprite PlayerIcon{ get; }
    public bool isTurn;

    public void Init(int _playerNum,Sprite icon)
    {
        playerNum = _playerNum;
        playerIcon = icon;
    }

}
