using System;

public class RoomNameGenerator {
    private static short length = 1;

    public static short Length {
        get => length;
        set => length = value;
    }

    private static Random rng = new Random();

    public static string Next() {
        string roomName = "";

        for (int i = 0; i < Length; ++i) {
            roomName += (char) (rng.Next(0, 25) + 65);
        }

        return roomName;
    }
}
