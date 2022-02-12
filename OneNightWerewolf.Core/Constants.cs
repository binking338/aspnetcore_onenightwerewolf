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
        public const string MONITOR_SEE_OTHERS_CARD = "玩家[{0}]的身份是【{1}】";
        public const string MONITOR_SEE_GRAVE_CARD = "{0}号中间牌的身份是【{1}】";
        public const string MONITOR_NONE_PLAYER = "没有找到身份是【{0}】的玩家！";
        public const string MONITOR_FIND_ROLE_PLAYER = "身份是【{0}】的玩家有：[{1}]";
        public const string MONITOR_HUNT = "玩家[{0}]是猎人，猎人带走了[{1}]";
        public const string MONITOR_SWAP_OTHERS = "玩家[{0}]与玩家[{1}]交换了身份牌";
        public const string MONITOR_SWAP_WITH_OTHERS = "你与玩家[{0}]交换了身份牌";
        public const string MONITOR_SWAP_WITH_GRAVE = "你的身份牌与{0}号中间牌交换了";
        public const string MONITOR_SWAP_OHTERS_WITH_GRAVE = "玩家[{0}]的身份牌与{1}号中间牌交换了";
    }
}
