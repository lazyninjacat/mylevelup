using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Constant
{

    public enum GameType
    {
        WORD_SCRAMBLE_GAME,
        FLASH_CARD_GAME,
        KEYBOARD_GAME,
        MATCHING_GAME,
        COUNTING_GAME,
        MEMORY_CARD_GAME,
    }

    // Strings
    public const string SCRAM = "Word_Scramble";
    public const string REWARD = "Choose_Reward";
    public const string FLASH = "Flash_Card";
    public const string KEYB = "Keyboard_Game";
    public const string MATCH = "Matching_Game";
    public const string COUNT = "Counting_Game";
    public const string MEMORY = "Memory_Cards";

    // Ints
    public const int PIC_WIDTH = 75;
    public const int PIC_HEIGHT = 75;
    public const int ACTIVE_PANEL_INDEX = -1;
}
