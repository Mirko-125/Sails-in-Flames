using System.Collections;
using System.Collections.Generic;

public class Igre
{
    public string GameID { get; set; }
    public string Player1 { get; set; }
    public string Player2 { get; set; }
    public string BoardState1 { get; set; }
    public string BoardState2 { get; set; }
    public string Weapon1 { get; set; }
    public string Weapon2 { get; set; }
    public string GameState { get; set; }
}

public class GameMetadata
{
    public string OpponentName { get; set; } // cisto da ima displayName
    public string YourTurn { get; set; } //"1" ako je player1, "2" ako je player2
}