namespace SailsServer.Database
{

    public class DBManager
    {
        private readonly IIgraciProvider _providerPl;
        private readonly IIgreProvider _providerGm;
        private readonly IIgraciRepository _repoPl;
        private readonly IIgreRepository _repoGm;

        private List<Igre> gamesBeingMade = new List<Igre>();

        string[] wpns = new string[9];

        public DBManager ()
        {
            DatabaseConfig dbc = new DatabaseConfig ();
            dbc.Name = "Data Source=Sails.sqlite";
            _providerPl = new IgraciProvider (dbc);
            _providerGm = new IgreProvider (dbc);
            _repoPl = new IgraciRepository(dbc);
            _repoGm = new IgreRepository(dbc);

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
        }

        public async Task<Igre> GetGameFromId(string gameId)
        {
            return (await _providerGm.Get(gameId)).FirstOrDefault()!;
        }

        public async Task<Igraci> GetUserFromId(string userId)
        {
            return (await _providerPl.Get(userId)).FirstOrDefault()!;
        }

        public async void UpdateUser(Igraci igrac)
        {
            await _repoPl.Update(igrac);
        }

        public async void UpdateGame(Igre igra)
        {
            await _repoGm.Update(igra);
        }

        public async Task<bool> PlayMove(Igre currentIgra, string currPlayer, int moveI, int moveJ, int weapon)
        {
            //Igre currentIgra = (await _providerGm.Get(gameID)).FirstOrDefault()!;

            if (currentIgra == null)
                return false;

            //jel prvi igrac gadja?
            if (currentIgra.Player1 == currPlayer)
            {

            }
            else
            {

            }

            //sacuvaj u baziste podataka

            return true;
        }

        public Igre? findInInits(string gameId)
        {
            for (int i = 0; i < gamesBeingMade.Count; i++)
            {
                if (gamesBeingMade[i].GameID == gameId)
                {
                    return gamesBeingMade[i];
                }
            }
            return null;
        }

        public async void InitializeGame()
        {
            //todo: napravi novi gameid, i stavi igru zajedno sa oba igraca u listu 
        }

        public async Task<bool> AcceptWeapon()
        {
            //todo: stavi u memoriju od ovog igraca 
            return false;
        }

        public async Task<int> RevealWeapon()
        {
            return 0;
        }

        public async Task<bool> AcceptBoard()
        {
            //todo: stavi u memoriju od ovog igraca 
            return false;
        }

        public async Task<bool> StartGame(Igre igra)
        {
            //todo: stavi u bazu novu igru, izbaci je iz liste gore
            return true;
        }

        public async Task<int> CheckGame(Igre igra)
        {
            //todo: ako je vreme, stavi da je igra neaktivna i da su igraci oslobodjeni ove runde

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

        public async void SaveEndedGame(Igre igra)
        {
            igra.GameState = "0";
            UpdateGame(igra);
        }

        public async void SaveNextTurn(Igre igra)
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
