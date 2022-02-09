using System;
namespace OneNightWerewolf.Core
{
    /// <summary>
    /// 游戏阶段
    /// </summary>
    public enum Phase
    {
        /// <summary>
        /// 开始
        /// </summary>
        Start = 0,
        /// <summary>
        /// 晚上
        /// </summary>
        Night = 10,
        /// <summary>
        /// 白天
        /// </summary>
        Dawn = 20,
        /// <summary>
        /// 投票
        /// </summary>
        Vote = 30,
        /// <summary>
        /// 游戏结局
        /// </summary>
        Judge = 40,
        /// <summary>
        /// 结束
        /// </summary>
        Over = 99
    }

    public static class PhaseExtensions
    {
        public static string Readable(this Phase phase)
        {
            string readable = "";
            switch (phase)
            {
                case Phase.Start:
                    readable = "开始";
                    break;
                case Phase.Over:
                    readable = "结束";
                    break;
                case Phase.Night:
                    readable = "夜晚";
                    break;
                case Phase.Dawn:
                    readable = "白天";
                    break;
                case Phase.Vote:
                    readable = "投票";
                    break;
                case Phase.Judge:
                    readable = "胜负";
                    break;
            }
            return readable;
        }

    }
}
