using System;

namespace PartyQuiz.Utils.Inputs
{
    [Flags]
    public enum ECommand
    {
        None = 0,
        Escape = 1,
        Forward = 2,
        Back = 3,
        Left = 4,
        Right = 5,
        ShowConsole = 6,
        Alpha1 = 7,
        Alpha2 = 8,
        Alpha3 = 9,
        Alpha4 = 10,
        Alpha5 = 11,
        Alpha6 = 12,
        Alpha7 = 13,
        Alpha8 = 14,
        Alpha9 = 15,
        Alpha0 = 16,
        RotateClockwise = 17,
        RotateCounterclockwise = 18,
        QuickSave = 19,
        QuickLoad = 20,
        Mouse0 = 21,
        Mouse1 = 22,
        Mouse2 = 23,
        Enter = 24,
        LeftControl = 25,
        Tab = 26,
        UpArrow = 27,
        DownArrow = 28,

        /// <summary>
        /// Flag to know if the command button was held
        /// </summary>
        Hold = 1 << 29,

        /// <summary>
        /// Flag to know if the command button was released
        /// </summary>
        Release = 1 << 30,

        /// <summary>
        /// Flag to know if the command button was pressed
        /// </summary>
        Press = 1 << 31,
    }
}