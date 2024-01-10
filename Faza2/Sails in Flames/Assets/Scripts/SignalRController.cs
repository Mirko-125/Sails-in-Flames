using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
public sealed class SignalRController
{
    private SignalRController()
    {
        
    }
    private static readonly object lockObj = new();
    private static SignalRController instance = null;
    public static SignalRController Instance {
        get {
            if (instance == null) {
                lock(lockObj) 
                {
                    instance ??= new SignalRController();
                }
            }
            return instance;
        }
    }
    public void SendSignalR(string player, Weapon weapon, int x, int y) // verovatno posle dodajemo i weapon i koji je player
    {
        Turn turn = new()
        {
            Player = player,
            WeaponName = weapon,
            BoardX = x,
            BoardY = y
        };
        Debug.Log($"{turn.Player},{turn.WeaponName},{turn.BoardX},{turn.BoardY}");
    }
    public void ReceiveSignalR(Turn turn) // nisam siguran da ovako treba
    {
        Debug.Log($"{turn.Player},{turn.WeaponName},{turn.BoardX},{turn.BoardY}");
        // _tiles
    }
}
