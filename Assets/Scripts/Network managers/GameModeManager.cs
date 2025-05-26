// GameModeManager.cs

public enum GameMode
{
    SinglePlayer,
    MultiPlayer
}

public static class GameModeManager
{
    public static GameMode CurrentMode = GameMode.SinglePlayer;
}
