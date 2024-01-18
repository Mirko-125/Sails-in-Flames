using UnityEngine;
using Microsoft.AspNetCore.SignalR.Client;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using System.Collections;

public class UnityHub : MonoBehaviour
{
    private static HubConnection connection;
    public string username = "";

    public Igre gameStorage = null;

    public void Initialise(string connString)
    {
        Debug.Log("Client spawned.");
        connection = new HubConnectionBuilder() //http://localhost:5165/gameHub i http://204.216.216.204:7333/gameHub
            .WithUrl(connString)
            .Build();

        connection.ServerTimeout = new System.TimeSpan(1, 0, 0, 0);
        connection.HandshakeTimeout = new System.TimeSpan(1, 0, 0, 0);

        connection.Closed += async (error) =>
        {
            //pokazi playeru da mu connection lipsosao ki stara kamila
            Debug.Log(error.ToString());
        };

        Connect();

        if (PlayerPrefs.HasKey("userID"))
        {
            //existing client
            ConnectId(PlayerPrefs.GetString("userID"));

            SceneManager.LoadSceneAsync(1);
        }
        else
        {
            //new client
            //zgrabi iz textbox info rmacije
            ConnectAs(username);
        }
    }

    public IEnumerator SetId(string userId)
    {
        Debug.Log("Setting ID.");
        PlayerPrefs.SetString("userID", userId);
        SceneManager.LoadScene(1);
        yield return null;
    }

    public IEnumerator UpdateGame(Igre gameState)
    {
        Debug.Log("Updating game.");
        yield return null;
    }

    public IEnumerator Reconnect(Igre gameState, GameMetadata md)
    {
        Debug.Log("Reconnecting to game.");

        FindFirstObjectByType<UnityHub>().gameStorage = gameState;
        PlayerPrefs.SetString("turn", md.YourTurn);
        PlayerPrefs.SetString("opponent", md.OpponentName);

        SceneManager.LoadScene(4);

        yield return null;
    }

    public IEnumerator Disconnect()
    {
        Debug.Log("Ow.");
        yield return null;
    }

    public IEnumerator InitGame(GameMetadata md)
    {
        Debug.Log("Initialising game.");

        PlayerPrefs.SetString("turn", md.YourTurn);
        PlayerPrefs.SetString("opponent", md.OpponentName);

        SceneManager.LoadScene(2);

        yield return null;
    }

    public IEnumerator RevealWeapon(string weapon)
    {
        Debug.Log("Phase 2 - revealing opponent weapon.");

        PlayerPrefs.SetString("weapon", weapon);

        SceneManager.LoadScene(3);

        yield return null;
    }

    public IEnumerator StartGame(Igre gameState)
    {
        Debug.Log("Starting game.");

        FindFirstObjectByType<UnityHub>().gameStorage = gameState;

        SceneManager.LoadScene(4);

        yield return null;
    }

    public IEnumerator EndGame(int res)
    {
        Debug.Log("Ending game.");
        yield return null;
    }



    private async void Connect()
    {
        connection.On<string>("SetID", (userID) =>
        {
            Debug.Log("Server message detected.");

            //idemo sad na main menu posto imamo id i smemo da budemo tamo
            UnityMainThreadDispatcher.Instance().Enqueue(SetId(userID));
        });

        connection.On<Igre>("UpdateGame", (gameState) =>
        {
            Debug.Log("Server message detected.");
            //vrv smo u scenu sa potapanje, prosledi na menadzera potapanja nova stanja i sta se radi
            UnityMainThreadDispatcher.Instance().Enqueue(UpdateGame(gameState));
        });

        connection.On<Igre, GameMetadata>("Reconnect", (gameState, md) =>
        {
            Debug.Log("Server message detected.");
            //posalji igraca u scenu sa potapanje odma, s ove informacije iz gamestate i metadata
            UnityMainThreadDispatcher.Instance().Enqueue(Reconnect(gameState, md));
        });

        connection.On("Disconnect", () =>
        {
            Debug.Log("Server message detected.");
            //kazi igracu da nije bio dobar i da sad biva izbacen iz igre (ovo se desava jedino ako se uloguje sa isti username na drugi komp)
            UnityMainThreadDispatcher.Instance().Enqueue(Disconnect());
        });

        connection.On<GameMetadata>("InitGame", (md) =>
        {
            Debug.Log("Server message detected.");
            //turi igraca u weapon select fazu i zapamti metadata o rundi (obrnutim redosledom)
            UnityMainThreadDispatcher.Instance().Enqueue(InitGame(md));
        });

        connection.On<string>("RevealWeapon", (weapon) =>
        {
            Debug.Log("Server message detected.");
            //zapamti sta mu otkriveno i butni ga u board state fazu (2)
            UnityMainThreadDispatcher.Instance().Enqueue(RevealWeapon(weapon));
        });

        connection.On<Igre>("StartGame", (initState) =>
        {
            Debug.Log("Server message detected.");
            //posalji ga u scenu sa potapanje, i potapaj!!
            UnityMainThreadDispatcher.Instance().Enqueue(StartGame(initState));
        });

        connection.On<int>("EndGame", (res) =>
        {
            Debug.Log("Server message detected.");
            //pokazi igracu u popup da je neko pobedio, i daj u taj popup button nazad za main menu znas li svestan li si
            UnityMainThreadDispatcher.Instance().Enqueue(EndGame(res));
        });

        try
        {
            await connection.StartAsync();

            Debug.Log("Connection started");
        }
        catch (System.Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }

    public async void ConnectAs(string user)
    {
        try
        {
            await connection.InvokeAsync("ConnectAs", user);
            
        }
        catch (System.Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }

    public async void ConnectId(string id)
    {
        try
        {
            await connection.InvokeAsync("ConnectId", id);

        }
        catch (System.Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }

    public async void LookForGame(string id)
    {
        try
        {
            await connection.InvokeAsync("LookForGame", id);

        }
        catch (System.Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }

    public async void AcceptWeapons(string id, string weapons)
    {
        try
        {
            await connection.InvokeAsync("AcceptWeapons", id, weapons);

        }
        catch (System.Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }

    public async void AcceptPlacement(string id, string boardState)
    {
        try
        {
            await connection.InvokeAsync("AcceptPlacement", id, boardState);

        }
        catch (System.Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }

    public async void PerformMove(string id, int weapon, int row, int col)
    {
        try
        {
            await connection.InvokeAsync("PerformMove", id, weapon, row, col);

        }
        catch (System.Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }

}