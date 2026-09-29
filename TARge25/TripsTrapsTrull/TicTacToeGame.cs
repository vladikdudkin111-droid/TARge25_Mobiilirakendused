namespace TARge25;

// See klass sisaldab ainult mängu loogikat.
// Kasutajaliidese elemendid (Button, Grid, Image jne) siia ei kuulu.
// Nii on mängu loogika UI-st eraldatud ja täidame ülesande OOP nõude.
public class TicTacToeGame
{
    // 3x3 massiiv hoiab iga ruudu seisu: null, "X" või "O".
    private readonly string?[,] board = new string?[3, 3];

    // CurrentPlayer näitab, kelle kord on praegu käia.
    public string CurrentPlayer { get; private set; } = "X";

    // IsGameOver takistab pärast võitu või viiki uute käikude tegemist.
    public bool IsGameOver { get; private set; }

    // Tagastab ühe kindla mänguruudu väärtuse.
    public string? GetCell(int row, int column) => board[row, column];

    // Puhastab mängulaua ja määrab uue mängu alustaja.
    public void Reset(string startingPlayer = "X")
    {
        // Kõik üheksa ruutu tehakse uuesti tühjaks.
        for (int row = 0; row < 3; row++)
        {
            for (int column = 0; column < 3; column++)
            {
                board[row, column] = null;
            }
        }

        // Lubame alustajaks ainult X või O. Vale väärtuse korral alustab X.
        CurrentPlayer = startingPlayer == "O" ? "O" : "X";

        // Uus mäng ei ole veel lõppenud.
        IsGameOver = false;
    }

    // Proovib asetada praeguse mängija sümboli valitud ruutu.
    public bool TryMakeMove(int row, int column)
    {
        // Käiku ei saa teha, kui mäng on läbi või ruut on juba kasutatud.
        if (IsGameOver || board[row, column] != null)
            return false;

        // Salvestame X või O mängulaua massiivi.
        board[row, column] = CurrentPlayer;
        return true;
    }

    // Kontrollib kõiki võimalikke võiduridu ja tagastab võitja "X" või "O".
    // Kui võitjat veel pole, tagastab null.
    public string? GetWinner()
    {
        // Kontrollime kolme horisontaalset rida.
        for (int row = 0; row < 3; row++)
        {
            if (Same(board[row, 0], board[row, 1], board[row, 2]))
                return board[row, 0];
        }

        // Kontrollime kolme vertikaalset veergu.
        for (int column = 0; column < 3; column++)
        {
            if (Same(board[0, column], board[1, column], board[2, column]))
                return board[0, column];
        }

        // Kontrollime diagonaali vasakult ülevalt paremale alla.
        if (Same(board[0, 0], board[1, 1], board[2, 2]))
            return board[0, 0];

        // Kontrollime diagonaali paremalt ülevalt vasakule alla.
        if (Same(board[0, 2], board[1, 1], board[2, 0]))
            return board[0, 2];

        return null;
    }

    // Kontrollib, kas mäng lõppes viigiga.
    public bool IsDraw()
    {
        // Kui meil juba on võitja, siis ei ole tegemist viigiga.
        if (GetWinner() != null)
            return false;

        // Kui leiame vähemalt ühe tühja ruudu, saab mäng veel jätkuda.
        for (int row = 0; row < 3; row++)
        {
            for (int column = 0; column < 3; column++)
            {
                if (board[row, column] == null)
                    return false;
            }
        }

        // Kõik ruudud on täidetud ja võitjat pole -> viik.
        return true;
    }

    // Märgib mängu lõppenuks.
    public void FinishGame()
    {
        IsGameOver = true;
    }

    // Vahetab mängija X -> O või O -> X.
    public void NextPlayer()
    {
        CurrentPlayer = CurrentPlayer == "X" ? "O" : "X";
    }

    // Abimeetod kontrollib, kas kolm väärtust on olemas ja omavahel võrdsed.
    private static bool Same(string? first, string? second, string? third)
    {
        return first != null && first == second && second == third;
    }
}
