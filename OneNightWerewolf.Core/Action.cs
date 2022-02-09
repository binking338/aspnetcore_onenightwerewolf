using System;
namespace OneNightWerewolf.Core
{
    public enum Action
    {
        /// <summary>
        /// 空白行为
        /// </summary>
        None = 0,
        /// <summary>
        /// 获知其他同伴卡牌角色所在座位序号
        /// 狼人，守夜人
        /// </summary>
        FindBuddy = 1,
        /// <summary>
        /// 选择坟墓区位置查看卡牌角色
        /// 预言家 狼人
        /// </summary>
        SeeGraveCard = 2,
        /// <summary>
        /// 预言家
        /// 选择一位其他玩家查看卡牌角色
        /// </summary>
        SeeOthersCard = 3,
        /// <summary>
        /// 查看自己的卡牌角色
        /// 失眠者
        /// </summary>
        SeeMyCard = 4,
        /// <summary>
        /// 选择两位其他玩家交换卡牌
        /// 捣蛋鬼
        /// </summary>
        SwapOthers = 5,
        /// <summary>
        /// 选择玩家与自己交换卡牌
        /// 强盗
        /// </summary>
        SwapWithOthers = 6,
        /// <summary>
        /// 选择一个坟墓区位置与自己交换卡牌
        /// 酒鬼
        /// </summary>
        SwapWithGraveCard = 7,
        /// <summary>
        /// 拷贝卡牌
        /// </summary>
        CopyOthersCard = 8,
        /// <summary>
        /// 投票
        /// </summary>
        Vote = 50,
        /// <summary>
        /// 投票结果
        /// </summary>
        Tickets = 51,
        /// <summary>
        /// 猎杀
        /// </summary>
        Hunt = 52,
        /// <summary>
        /// 判定生死
        /// </summary>
        Judge = 80,
        /// <summary>
        /// 离开
        /// </summary>
        Leave = 98,
        /// <summary>
        /// 准备
        /// </summary>
        Ready = 99,
    }
}
