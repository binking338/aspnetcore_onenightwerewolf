using System;
namespace OneNightWerewolf.Core
{
    public static class Constants
    {
        public const string MONITOR_WINNING_CAMP_NONE = "没有阵营胜利";
        public const string MONITOR_WINNING_CAMP_VILLIAGE = "胜利阵营是【村民阵营】";
        public const string MONITOR_WINNING_CAMP_WEREWOLF = "胜利阵营是【狼人阵营】";
        public const string MONITOR_WINNER = "恭喜，你胜利了";
        public const string MONITOR_LOSER = "抱歉，你失败了";
        public const string MONITOR_DEAD = "最终身份是【{0}】，{1}票死亡！";
        public const string MONITOR_SURVIVOR = "最终身份是【{0}】，{1}票幸存！";
        public const string MONITOR_SEE_MY_CARD = "你的身份是【{1}】";
        public const string MONITOR_SEE_OTHERS_CARD = "[{0}]的身份是【{1}】";
        public const string MONITOR_SEE_GRAVE_CARD = "{0}号牌的身份是【{1}】";
        public const string MONITOR_NONE_PLAYER = "没有找到身份是【{0}】的玩家！";
        public const string MONITOR_FIND_ROLE_PLAYER = "身份是【{0}】的玩家有：[{1}]";
        public const string MONITOR_HUNT = "[{0}]是猎人，他带走了[{1}]";
        public const string MONITOR_SWAP_OTHERS = "[{0}]与[{1}]换了牌";
        public const string MONITOR_SWAP_WITH_OTHERS = "你与[{0}]换了牌";
        public const string MONITOR_SWAP_WITH_GRAVE = "你的牌与{0}号牌交换了";
        public const string MONITOR_SWAP_OHTERS_WITH_GRAVE = "[{0}]的牌与{1}号牌交换了";
    }
}
