using System;
namespace OneNightWerewolf
{
    public enum GamePhase
    {

        Over = -1,
        Start = 0,
        NightDoppelgängerFeign = 1,
        NightDoppelgängerPlay = 2,
        NightWerewolf = 3,
        NightMinion = 4,
        NightMasons = 5,
        NightSeer = 6,
        NightRobber = 7,
        NightTroublemaker = 8,
        NightDrunk = 9,
        NightInsomniac = 10,
        NightDoppelgängerInsomniac = 11,
        Day = 12,
    }
}
