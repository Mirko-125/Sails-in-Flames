using System.Collections;
using System.Collections.Generic;

public enum Weapon
{
    Cannonball,
    Thunderstrike,
    Frostbolt,
    ClusterBomb,
    Grapeshot,
    Fireball,
    DivinePillar,
    Annihilator,
    Minigun
}

public class Turn
{
    public Weapon WeaponName { get; set; }
    public string Player { get; set; }
    public int BoardX { get; set; }
    public int BoardY { get; set; }
}