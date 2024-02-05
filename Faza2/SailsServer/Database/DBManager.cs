using Microsoft.AspNetCore.SignalR;
using System.Text;
using System.Security.Cryptography;
using Microsoft.Extensions.ObjectPool;
using System;
//using Microsoft.AspNet.SignalR.Hubs;

namespace SailsServer.Database
{

    public class DBManager
    {
        private static DBManager instance;
        public static DBManager Instance { get { if (instance == null) instance = new DBManager(); return instance; } }

        private readonly IIgraciProvider _providerPl;
        private readonly IIgreProvider _providerGm;
        private readonly IIgraciRepository _repoPl;
        private readonly IIgreRepository _repoGm;

        private List<Igre> gamesBeingMade = new List<Igre>();

        private Dictionary<string, IClientProxy> userConns;

        private string waitingOnGame = "";

        string[] wpns = new string[9];

        public DBManager ()
        {
            DatabaseConfig dbc = new DatabaseConfig ();
            dbc.Name = "Data Source=Sails.sqlite";
            _providerPl = new IgraciProvider (dbc);
            _providerGm = new IgreProvider (dbc);
            _repoPl = new IgraciRepository(dbc);
            _repoGm = new IgreRepository(dbc);

            userConns = new Dictionary<string, IClientProxy> ();

            //oruzja
            wpns[0] = "o";
            wpns[1] = "x  +" +
                      " o +" +
                      "  x";
            wpns[2] = "xox";
            wpns[3] = "x x+" +
                      " o +" +
                      "x x";
            wpns[4] = "  x  +" +
                      "     +" +
                      "x o x+" +
                      "     +" +
                      "  x  ";
            wpns[5] = " x +" +
                      "xox+" +
                      " x ";

            wpns[6] = "x+x+x+o+x+x+x+x";

            wpns[7] = " xxx +" +
                      "xxxxx+" +
                      "xxoxx+" +
                      "xxxxx+" +
                      " xxx ";

            wpns[8] = "o  x   x +" +
                      " xx xxx x";

            Console.WriteLine("Singleton setup done!");
        }

        public async Task<Igre?> GetGameFromId(string gameId)
        {
            return (await _providerGm.Get(gameId)).FirstOrDefault();
        }

        public async Task<Igre?> GetActiveGame(string userId)
        {
            return (await _providerGm.GetActiveUser(userId)).FirstOrDefault();
        }

        public async Task<bool> ConnectWithUser(string username, IClientProxy conn)
        {
            //first see if theres a user with that username in the db
            Igraci? i = await GetUserFromName(username);

            if (i != null)
            {
                if (i.DisplayName == username)
                {
                    if (userConns.ContainsKey(i.UserID))
                    {
                        IClientProxy oldConn = userConns[i.UserID];
                        if (oldConn != null)
                        {
                            if (oldConn.Equals(conn)) return true;
                            await oldConn.SendAsync("Disconnect");
                        }
                        userConns.Remove(i.UserID);
                        userConns.Add(i.UserID, conn);
                    }
                    else
                    {
                        userConns.Add(i.UserID, conn);
                    }
                }
                else
                {
                    Console.WriteLine("Username not matching - " + i.DisplayName + " vs " + username + " - creating new.");
                    //new user
                    i = await CreateUserAndSave(username);
                    userConns.Add(i.UserID, conn);
                }
            }
            else
            {
                Console.WriteLine("User returns null - creating new.");
                //new user
                i = await CreateUserAndSave(username);
                userConns.Add(i.UserID, conn);
            }

            Console.WriteLine("Sending ID to user.");
            await conn.SendAsync("SetID", i.UserID);

            return true;
        }

        public async Task<bool> ConnectExisting(string id, IClientProxy conn)
        {
            if (userConns.ContainsKey(id))
            {
                IClientProxy oldConn = userConns[id];
                if (oldConn != null)
                {
                    if (oldConn.Equals(conn)) return true;
                    await oldConn.SendAsync("Disconnect");
                }
                userConns.Remove(id);
                userConns.Add(id, conn);
            }
            else
            {
                userConns.Add(id, conn);
            }

            return true;
        }

        static string GetSha256Hash(SHA256 shaHash, string input)
        {
            // Convert the input string to a byte array and compute the hash.
            byte[] data = shaHash.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }

        public async Task<Igraci> CreateUserAndSave(string username)
        {
            Igraci i = new Igraci();
            i.DisplayName = username;
            i.CurrentGame = "";
            i.UserID = GetSha256Hash(SHA256.Create(), username);

            await _repoPl.Create(i);

            return i;
        }

        public async Task CreateGame(Igre igra)
        {
            await _repoGm.Create(igra);
        }

        public async Task<Igraci?> GetUserFromId(string userId)
        {
            return (await _providerPl.Get(userId)).FirstOrDefault();
        }

        public async Task<Igraci?> GetUserFromName(string user)
        {
            return (await _providerPl.GetName(user)).FirstOrDefault();
        }

        public async void UpdateUser(Igraci igrac)
        {
            await _repoPl.Update(igrac);
        }

        public async void UpdateGame(Igre igra)
        {
            await _repoGm.Update(igra);
        }

        public string UseUpWeapon(string set, string weapon)
        {
            //set = set.Replace(weapon, "");
            int index = set.IndexOf(weapon);
            if (index > -1)
            {
                set = set.Substring(0, index) + ((set.Length <= index + 1) ? "" : set.Substring(index + 1));
            }

            set = set.Replace("  ", " ");
            return set.Trim();
        }

        public async Task<bool> PlayMove(string currPlayer, int moveI, int moveJ, int weapon)
        {
            //Igre currentIgra = (await _providerGm.Get(gameID)).FirstOrDefault()!;
            Igre? currentIgra = await GetActiveGame(currPlayer);


            if (currentIgra == null)
                return false;

            if (currentIgra.GameID == null || currentIgra.GameID == "")
            {
                return false;
            }

            //jel prvi igrac gadja? if so, gadja u board2
            if (currentIgra.Player1 == currPlayer)
            {
                currentIgra.BoardState2 = ModifyBoard(currentIgra.BoardState2, moveI, moveJ, weapon);
                currentIgra.Weapon1 = UseUpWeapon(currentIgra.Weapon1, weapon.ToString());
            }
            else
            {
                currentIgra.BoardState1 = ModifyBoard(currentIgra.BoardState1, moveI, moveJ, weapon);
                currentIgra.Weapon2 = UseUpWeapon(currentIgra.Weapon2, weapon.ToString());
            }

            SaveNextTurn(currentIgra);

            //prosledi korisnicima ovaj paketic informacije
            await ServeGameUpdate(currentIgra);


            //sacuvaj u baziste podataka
            int res = CheckGame(currentIgra);

            if (res != 0)
            {
                //neko je pobedio! javi korisnicima ko
                SaveEndedGame(currentIgra);

                try
                {
                    IClientProxy c1 = userConns[currentIgra.Player1];
                    await c1.SendAsync("EndGame", res);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

                try
                {
                    IClientProxy c2 = userConns[currentIgra.Player2];
                    await c2.SendAsync("EndGame", res);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            return true;
        }

        public Igre? findInInits(string userId)
        {
            Console.WriteLine("Searching memory - scanning through " + gamesBeingMade.Count.ToString() + " games to find one with user ID " + userId + "...");
            for (int i = 0; i < gamesBeingMade.Count; i++)
            {
                if (gamesBeingMade[i].Player1 == userId || gamesBeingMade[i].Player2 == userId)
                {
                    return gamesBeingMade[i];
                }
            }
            return null;
        }

        public async Task<Igre> ServeGameForPlayer(Igre i, bool player2)
        {
            Igre n = new Igre();

            if (player2)
            {
                n.BoardState1 = i.BoardState1.Replace('2', '0');
                n.BoardState2 = i.BoardState2;
                n.Weapon1 = "";
                n.Weapon2 = i.Weapon2;
            }
            else
            {
                n.BoardState1 = i.BoardState1;
                n.BoardState2 = i.BoardState2.Replace('2', '0');
                n.Weapon1 = i.Weapon1;
                n.Weapon2 = "";
            }
            n.GameState = i.GameState;
            n.Player1 = i.Player1;
            n.Player2 = i.Player2;

            return n;
        }

        public async Task ServeGameUpdate(Igre i)
        {

            try
            {
                IClientProxy c1 = userConns[i.Player1];
                Igre n = await ServeGameForPlayer(i, false);

                await c1.SendAsync("UpdateGame", n);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            try
            {
                IClientProxy c2 = userConns[i.Player2];
                Igre n = await ServeGameForPlayer(i, true);

                await c2.SendAsync("UpdateGame", n);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public async Task Reconnect(string id, Igre i)
        {
            IClientProxy conn = userConns[id];
            Metadata metadata = new Metadata();
            Igre n;
            

            if (id == i.Player1)
            {
                //p1
                Igraci p = (await GetUserFromId(i.Player2))!;
                metadata.YourTurn = "1";
                metadata.OpponentName = p.DisplayName;
                n = await ServeGameForPlayer(i, false);
            }
            else
            {
                //p2
                Igraci p = (await GetUserFromId(i.Player1))!;
                metadata.YourTurn = "2";
                metadata.OpponentName = p.DisplayName;
                n = await ServeGameForPlayer(i, true);
            }

            await conn.SendAsync("Reconnect", n, metadata); //n Igre, md Metadata
        }

        public async Task FindGame(string id)
        {
            //proveri jel u igri - persistencija

            Igre? i = (await _providerGm.GetActiveUser(id)).FirstOrDefault();

            if (i != null)
            {
                if (i.GameID != "" && i.GameID != null && i.GameState != "0")
                {
                    await Reconnect(id, i);
                    return; //ne idi dole da tragas
                }
            }

            //nije u igri trenutno, stavi na cekanje ili povezi sa osobom koja ceka
            if (waitingOnGame == "")
            {
                waitingOnGame = id;
            }
            else
            {
                string id2 = waitingOnGame;
                waitingOnGame = "";
                await InitializeGame(id, id2);
            }
        }

        public async Task InitializeGame(string id1, string id2)
        {
            //prvo ukloni ranije init igre od ove osobe ako ih ima, tj ako slucajno nisu uklonjene bile (zbog disconnect il tako nesto)
            Igre? g1 = findInInits(id1);
            if (g1 != null)
            {
                gamesBeingMade.Remove(g1);
            }
            Igre? g2 = findInInits(id2);
            if (g2 != null)
            {
                gamesBeingMade.Remove(g2);
            }

            Igre igra = new Igre();

            Random r = new Random();

            if (r.Next(2) == 0)
            {
                //id1 je player1
                igra.Player1 = id1;
                igra.Player2 = id2;
            }
            else
            {
                //id1 je player2
                igra.Player1 = id2;
                igra.Player2 = id1;
            }

            igra.Weapon1 = "";
            igra.Weapon2 = "";
            igra.BoardState1 = "";
            igra.BoardState2 = "";
            igra.GameState = "1";
            igra.GameID = GetSha256Hash(SHA256.Create(), DateTime.Now.ToLongTimeString());

            gamesBeingMade.Add(igra);

            
            Metadata md1 = new Metadata();
            Metadata md2 = new Metadata();

            try
            {
                IClientProxy p1 = userConns[igra.Player1];
                md1.YourTurn = "1";
                Igraci i = (await GetUserFromId(igra.Player2))!;
                md1.OpponentName = i.DisplayName;
                await p1.SendAsync("InitGame", md1);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            try
            {
                IClientProxy p2 = userConns[igra.Player2];
                md2.YourTurn = "2";
                Igraci i = (await GetUserFromId(igra.Player1))!;
                md2.OpponentName = i.DisplayName;
                await p2.SendAsync("InitGame", md2);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public async Task<bool> AcceptWeapon(string id, string weapons)
        {
            //todo: stavi u memoriju od ovog igraca 

            Igre? i = findInInits(id);

            if (i == null) return false;

            Console.WriteLine("Submitting weapon list to game!");

            if (id == i.Player1)
            {
                i.Weapon1 = weapons;
            }
            else
            {
                i.Weapon2 = weapons;
            }

            if (i.Weapon1 != "" && i.Weapon2 != "")
            {
                //oba igraca imaju oruzja! salji im signal za dalje, i daj im reveal od drugog igraca.

                try
                {
                    IClientProxy p1 = userConns[i.Player1];
                    await p1.SendAsync("RevealWeapon", RevealWeapon(i, false));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }

                try
                {
                    IClientProxy p2 = userConns[i.Player2];
                    await p2.SendAsync("RevealWeapon", RevealWeapon(i, true));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }

            return true;
        }

        public string RevealWeapon(Igre i, bool player2)
        {
            if (player2)
            {
                //otkrij iz prvog igraca nesto
                string[] choices = i.Weapon1.Split(' ');
                Random r = new Random();
                return choices[r.Next(choices.Length)];
            }
            else
            {
                //iz drugog
                string[] choices = i.Weapon2.Split(' ');
                Random r = new Random();
                return choices[r.Next(choices.Length)];
            }
        }

        public async Task<bool> AcceptBoard(string id, string boardState)
        {
            //todo: stavi u memoriju od ovog igraca 
            Igre? i = findInInits(id);

            if (i == null) return false;

            if (id == i.Player1)
            {
                i.BoardState1 = boardState;
            }
            else
            {
                i.BoardState2 = boardState;
            }

            Console.WriteLine(i.BoardState1 + "\n" + i.BoardState2);

            if (i.BoardState1 != "" && i.BoardState2 != "")
            {
                //oba igraca imaju table! salji im signal za dalje i stavljaj game u memoriju, vreme za potapanje

                await StartGame(i);

                try
                {
                    IClientProxy p1 = userConns[i.Player1];
                    Igre n = await ServeGameForPlayer(i, false);
                    await p1.SendAsync("StartGame", n);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }

                try
                {
                    IClientProxy p2 = userConns[i.Player2];
                    Igre n = await ServeGameForPlayer(i, true);
                    await p2.SendAsync("StartGame", n);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }

            return true;
        }

        public async Task<bool> StartGame(Igre igra)
        {
            //todo: stavi u bazu novu igru, izbaci je iz liste gore
            gamesBeingMade.Remove(igra);

            Console.WriteLine("Writing a game with these parameters:");
            Console.WriteLine("Player 1 - " + igra.Weapon1 + " - " + igra.BoardState1);
            Console.WriteLine("Player 2 - " + igra.Weapon2 + " - " + igra.BoardState2);

            await CreateGame(igra);

            return true;
        }

        public int CheckGame(Igre igra)
        {
            //ako je vreme, stavi da je igra neaktivna i da su igraci oslobodjeni ove runde

            if (!(igra.BoardState1.Contains('2')))
            {
                //igrac 1 izgubio, p2 victory
                return 2;
            }
            else if (!(igra.BoardState2.Contains('2')))
            {
                //igrac 2 izgubio, p1 victory
                return 1;
            }

            return 0; // 0 znaci da nije gotovo
        }

        public void SaveEndedGame(Igre igra)
        {
            igra.GameState = "0";
            UpdateGame(igra);
        }

        public void SaveNextTurn(Igre igra)
        {
            if (igra.GameState == "1")
            {
                igra.GameState = "2";
            }
            else
            {
                igra.GameState = "1";
            }
            UpdateGame(igra);
        }

        public string ModifyBoard(string board, int moveI, int moveJ, int weapon)
        {

            string[] wpnRows = wpns[weapon].Split('+');
            int offsetI = 0, offsetJ = 0;
            bool done = false;
            for (int i = 0; i < wpnRows.Length && !done; i++)
            {
                for (int j = 0; j < wpnRows[0].Length; j++)
                {
                    if (wpnRows[i][j] == 'o')
                    {
                        offsetI = i;
                        offsetJ = j;
                        done = true;
                        break;
                    }
                }
            }

            moveI -= offsetI;
            moveJ -= offsetJ;

            string[] boardRows = board.Split('+');

            for (int i = 0; i < wpnRows.Length; i++)
            {
                for (int j = 0; j < wpnRows[0].Length; j++)
                {
                    if ((moveI + i) >= 0 && (moveI + i) < boardRows.Length && (moveJ + j) >= 0 && (moveJ + j) < boardRows[0].Length && wpnRows[i][j] != ' ')
                    {
                        //in bounds
                        string c = "" + boardRows[moveI + i][moveJ + j];
                        int curr = int.Parse(c);
                        if (curr % 2 == 0)
                        {
                            //hit em!
                            curr++;
                            boardRows[moveI + i] = boardRows[moveI + i].Substring(0, moveJ + j) + curr.ToString() + boardRows[moveI + i].Substring(moveJ + j + 1);
                        }
                    }
                }
            }

            string nb = "";

            for (int i = 0; i < boardRows.Length - 1; i++)
            {
                nb += boardRows[i] + "+";
            }
            nb += boardRows[boardRows.Length - 1];

            return nb;
        }

    }
}
