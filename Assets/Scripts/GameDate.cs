using System;

[Serializable]
public struct GameDate {

    public GameDate(int minutes = 0, int hours = 0, int days = 0, int month = 0, int years = 0) {
        minutes %= 60;
        hours %= 24;
        days %= 30;
        month %= 12;
        Time = (ulong) (minutes + hours*60 + days *24*60 + month*30*24*60+years*12*30*24*60);
    }

    private GameDate(ulong time) {
        Time = time;
    }

    public ulong Time { get; private set; }

    public uint Year => (uint) ((Time / 60 / 24 / 30 / 12));
    public uint Month => (uint) ((Time / 60 / 24 / 30) % (12))+1;
    public uint Day => (uint) ((Time/60/24) % (30))+1;
    public uint Hour => (uint) ((Time/60)%(24));
    public uint Minute => (uint) (Time%(60));

    public static GameDate operator ++(GameDate g) => g + 1;
    public static GameDate operator +(GameDate g, int i) => new GameDate(g.Time + (ulong) i);
    public static GameDate operator +(GameDate g, GameDate d) => new GameDate(g.Time + d.Time);
    public static GameDate operator -(GameDate g, GameDate d) => new GameDate(g.Time - d.Time);
}