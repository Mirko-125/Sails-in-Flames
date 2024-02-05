using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public interface TargetFilterBase
{
    public abstract List<Point> FilterChanges(string oldState, string newState); 
}

public class TargetFilter : TargetFilterBase
{
    public List<Point> FilterChanges(string oldState, string newState)
    {
        List<Point> result = new List<Point>();

        string[] oldrows = oldState.Split('+');
        string[] newrows = newState.Split('+');

        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                if (oldrows[i][j] != newrows[i][j] && newrows[i][j] == '1')
                {
                    result.Add(new Point(j, i));
                }
            }
        }

        return result;
    }
}

public class ShipDecorator : TargetFilterBase
{
    TargetFilterBase tfilter;
    public ShipDecorator(TargetFilterBase tfb)
    {
        tfilter = tfb;
    }

    public List<Point> FilterChanges(string oldState, string newState)
    {
        List<Point> result = tfilter.FilterChanges(oldState, newState);

        string[] oldrows = oldState.Split('+');
        string[] newrows = newState.Split('+');

        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                if (oldrows[i][j] != newrows[i][j] && newrows[i][j] == '3')
                {
                    result.Add(new Point(j + 12, i));
                }
            }
        }

        return result;
    }
}

public class FightManager : MonoBehaviour
{
    [SerializeField] private GameObject[] weaponBtns;

    [SerializeField] private GameObject boomHit;
    [SerializeField] private GameObject boomMiss;

    [SerializeField] private TMP_Text yourUser;
    [SerializeField] private TMP_Text enemyUser;

    [SerializeField] private NetLoader waitOverlay;
    [SerializeField] private NetLoader endOverlay;

    [SerializeField] private GridManager grid;
    UnityHub connHub;

    Igre oldState;

    TargetFilter standardTgt;
    ShipDecorator sinkingTgt;

    private string serverMessage = "Waiting for server response...";
    private string waitingMessage = "Currently the opponents turn...";

    int currentWeapon = 0;
    bool player1 = true;

    // Start is called before the first frame update
    void Start()
    {
        standardTgt = new TargetFilter();
        sinkingTgt = new ShipDecorator(standardTgt);

        if (PlayerPrefs.HasKey("player"))
        {
            yourUser.text = PlayerPrefs.GetString("player");
        }
        if (PlayerPrefs.HasKey("opponent"))
        {
            enemyUser.text = PlayerPrefs.GetString("opponent");
        }

        connHub = FindFirstObjectByType<UnityHub>();

        if (connHub != null)
        {
            if (PlayerPrefs.HasKey("turn"))
            {
                if (PlayerPrefs.GetString("turn") != "1")
                {
                    player1 = false;
                }
                if (PlayerPrefs.GetString("turn") != GameFromStorage().GameState)
                {
                    //not your turn
                    MoveSubmittedMessage();
                    ChangeMessageToOpponent();
                }
            }

            Invoke("StartAll", 0.05f);
        }
    }

    public void StartAll()
    {
        oldState = GameFromStorage();
        print(oldState.Weapon1);
        print(oldState.Weapon2);
        print(oldState.BoardState1);
        print(oldState.BoardState2);
        print(player1);
        InitFight(oldState);
    }

    public Igre GameFromStorage()
    {
        Igre i = new Igre();

        i.Weapon1 = PlayerPrefs.GetString("Weapon1");
        i.Weapon2 = PlayerPrefs.GetString("Weapon2");
        i.BoardState1 = PlayerPrefs.GetString("BoardState1");
        i.BoardState2 = PlayerPrefs.GetString("BoardState2");
        i.GameID = PlayerPrefs.GetString("GameID");
        i.Player1 = PlayerPrefs.GetString("Player1");
        i.Player2 = PlayerPrefs.GetString("Player2");
        i.GameState = PlayerPrefs.GetString("GameState");

        return i;
    }

    public void InitFight(Igre initState)
    {
        string[] friendlyBoard = (player1) ? initState.BoardState1.Split('+') : initState.BoardState2.Split('+');
        string[] enemyBoard = (!player1) ? initState.BoardState1.Split('+') : initState.BoardState2.Split('+');

        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                TileUI tileFriend = GetTile(true, j, i);
                TileUI tileEnemy = GetTile(false, j, i);

                tileFriend.FightSetup(int.Parse("" + friendlyBoard[i][j]));
                tileEnemy.FightSetup(int.Parse("" + enemyBoard[i][j]));
            }
        }

        WeaponUpdate(initState);
    }

    public void WeaponUpdate(Igre state)
    {
        if (player1)
        {
            UpdateButtons(state.Weapon1);
        }
        else
        {
            UpdateButtons(state.Weapon2);
        }
    }

    public void UpdateFight(Igre newState)
    {
        if (player1)
        {
            if (newState.GameState == "2")
            {
                ChangeMessageToOpponent();
            }
            else
            {
                ChangeMessageBackHide();
            }

            List<Point> friendlyStuff = sinkingTgt.FilterChanges(oldState.BoardState1, newState.BoardState1);
            List<Point> enemyStuff = sinkingTgt.FilterChanges(oldState.BoardState2, newState.BoardState2);

            foreach(Point p in friendlyStuff)
            {
                if (p.x > 10)
                {
                    p.x -= 12;
                    TileUI t = GetTile(true, p.x, p.y);
                    t.ShipSink();
                    t.WaterSink();
                    Detonate(true, true, p.x, p.y);
                }
                else
                {
                    TileUI t = GetTile(true, p.x, p.y);
                    t.WaterSink();
                    Detonate(false, true, p.x, p.y);
                }
                
            }
            foreach (Point p in enemyStuff)
            {
                if (p.x > 10)
                {
                    p.x -= 12;
                    TileUI t = GetTile(false, p.x, p.y);
                    t.ShipSink();
                    t.WaterSink();
                    Detonate(true, false, p.x, p.y);
                }
                else
                {
                    TileUI t = GetTile(false, p.x, p.y);
                    t.WaterSink();
                    Detonate(false, false, p.x, p.y);
                }
            }
        }
        else
        {
            if (newState.GameState == "1")
            {
                ChangeMessageToOpponent();
            }
            else
            {
                ChangeMessageBackHide();
            }

            List<Point> friendlyStuff = sinkingTgt.FilterChanges(oldState.BoardState2, newState.BoardState2);
            List<Point> enemyStuff = sinkingTgt.FilterChanges(oldState.BoardState1, newState.BoardState1);

            foreach (Point p in friendlyStuff)
            {
                if (p.x > 10)
                {
                    p.x -= 12;
                    TileUI t = GetTile(true, p.x, p.y);
                    t.ShipSink();
                    t.WaterSink();
                    Detonate(true, true, p.x, p.y);
                }
                else
                {
                    TileUI t = GetTile(true, p.x, p.y);
                    t.WaterSink();
                    Detonate(false, true, p.x, p.y);
                }

            }
            foreach (Point p in enemyStuff)
            {
                if (p.x > 10)
                {
                    p.x -= 12;
                    TileUI t = GetTile(false, p.x, p.y);
                    t.ShipSink();
                    t.WaterSink();
                    Detonate(true, false, p.x, p.y);
                }
                else
                {
                    TileUI t = GetTile(false, p.x, p.y);
                    t.WaterSink();
                    Detonate(false, false, p.x, p.y);
                }
            }
        }

        WeaponUpdate(newState);
        
        oldState = newState;
    }

    public void MoveSubmittedMessage()
    {
        waitOverlay.Reveal();
    }

    public void ChangeMessageToOpponent()
    {
        waitOverlay.Relabel(waitingMessage);
    }

    public void ChangeMessageBackHide()
    {
        waitOverlay.Relabel(serverMessage);
        waitOverlay.Hide();
    }

    public void SomeoneWon(string who)
    {
        if (PlayerPrefs.HasKey("turn"))
        {
            string turn = PlayerPrefs.GetString("turn");

            if (turn == who)
            {
                //it was you
                endOverlay.Reveal();
                endOverlay.Relabel("You won - congratulations, Admiral!");
            }
            else
            {
                endOverlay.Reveal();
                endOverlay.Relabel("You lost - better luck next time...");
            }
        }
    }

    
    public void DetectShot(int x, int y)
    {
        MoveSubmittedMessage();

        if (connHub != null)
        {
            connHub.PerformMove(PlayerPrefs.GetString("userID"), currentWeapon, y, x);
        }

        //Detonate(true, false, x, y);
    }

    public TileUI GetTile(bool friendly, int x, int y)
    {
        TileUI tile;

        if (friendly)
        {
            tile = grid.GetTileAtPosition(x * 100 + y);
        }
        else
        {
            tile = grid.GetTileAtPosition((x + 12) * 100 + y);
        }

        return tile;
    }

    public void Detonate(bool hit, bool friendly, int x, int y)
    {
        GameObject boomPrefab;
        if (hit)
        {
            boomPrefab = boomHit;
        }
        else
        {
            boomPrefab = boomMiss;
        }

        TileUI tile = GetTile(friendly, x, y);

        GameObject tmp = Instantiate(boomPrefab, tile.transform);

    }

    public void UpdateButtons(string wpns)
    {
        //npr 2 2 3 5

        if (wpns.Length == 0)
        {
            SelectWeapon(0);
            for (int i = 1; i < weaponBtns.Length; i++)
            {
                weaponBtns[i].GetComponent<Button>().interactable = false;
                weaponBtns[i].GetComponent<Image>().color = new Color(1, 1, 1, 0);
            }
            return;
        }

        string[] weaponThings = wpns.Split(' ');

        int[] ints = new int[weaponThings.Length];

        for (int i = 0; i < weaponThings.Length; i++)
        {
            ints[i] = int.Parse(weaponThings[i]);
        }
        
        //not cannon, its immune
        for (int i = 1; i < weaponBtns.Length; i++)
        {
            bool found = false;
            for (int j = 0; j < ints.Length && !found; j++)
            {
                if (i == ints[j])
                {
                    found = true;
                }
            }
            if (!found)
            {
                weaponBtns[i].GetComponent<Button>().interactable = false;
                weaponBtns[i].GetComponent<Image>().color = new Color(1, 1, 1, 0);

                if (i == currentWeapon)
                {
                    SelectWeapon(0);
                }
            }
        }
    }

    public void SelectWeapon(int wpn)
    {
        currentWeapon = wpn;

        for (int i = 0; i < weaponBtns.Length; i++)
        {
            if (weaponBtns[i].GetComponent<Button>().interactable)
            {
                if (i == wpn)
                {
                    weaponBtns[i].GetComponent<Image>().color = new Color(1, 1, 1, 1);
                }
                else
                {
                    weaponBtns[i].GetComponent<Image>().color = new Color(1, 1, 1, 0.2f);
                }
            }
        }
    }
}
