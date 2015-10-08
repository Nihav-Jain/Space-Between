using UnityEngine;
using System.Collections;

public class CharacterControllerValues {

    public static int INIT_TRANSLATION_SPEED_X = 0;
    public static int INIT_TRANSLATION_SPEED_Y = 0;
    public static int INIT_TRANSLATION_SPEED_Z = 0;

    public static int[] TRANSLATION_STEP = { 1, 1, 1 };             // x, y, z
    public static int[] MAX_TRANSLATION_SPEED = { 10, 10, 10 };     // x, y, z

    public static int INIT_ROTATION_SPEED_X = 0;
    public static int INIT_ROTATION_SPEED_Y = 0;
    public static int INIT_ROTATION_SPEED_Z = 0;

    public static float[] ROTATION_STEP = { 1f, 1f, 1f };     // x, y, z
    public static int[] MAX_ROTATION_SPEED = { 5, 5, 5 };           // x, y, z

    // keycodes for movement
    public static KeyCode[] TRANSLATION_KEYCODES = { KeyCode.D, KeyCode.A, KeyCode.E, KeyCode.Q, KeyCode.W, KeyCode.S };        // {+X, -Y, +Y, -Y, +Z, -Z}
    public static KeyCode[] ROTATION_KEYCODES = { KeyCode.LeftArrow, KeyCode.RightArrow, KeyCode.UpArrow, KeyCode.DownArrow};   // {+pitch, -pitch, +roll, -roll}

    public static KeyCode RESET_ORIENTATION = KeyCode.Space;

    public static KeyCode[] ALL_MOVEMENT_KEYCODES = { KeyCode.D, KeyCode.A, KeyCode.Q, KeyCode.E, KeyCode.W, KeyCode.S, KeyCode.LeftArrow, KeyCode.RightArrow, KeyCode.UpArrow, KeyCode.DownArrow };
}
