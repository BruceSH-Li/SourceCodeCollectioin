using System;
using Chloe.Annotations;

namespace WaterCloud.Domain.InfoManage
{
    /// <summary>
    /// 创 建：超级管理员
    /// 日 期：2020-07-29 16:44
    /// 描 述：信息历史实体类
    /// </summary>
    [TableAttribute("oms_MessageHis")]
    public class MessageHistoryEntity : IEntity<MessageHistoryEntity>
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        /// <returns></returns>
        [ColumnAttribute("F_Id", IsPrimaryKey = true)]
        [AutoIncrement]
        public long F_Id { get; set; }
        /// <summary>
        /// 信息Id
        /// </summary>
        /// <returns></returns>
        public long F_MessageId { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        /// <returns></returns>
        public DateTime? F_CreatorTime { get; set; }
        /// <summary>
        /// 创建用户主键
        /// </summary>
        /// <returns></returns>
        public string F_CreatorUserId { get; set; }
        /// <summary>
        /// 创建用户
        /// </summary>
        /// <returns></returns>
        public string F_CreatorUserName { get; set; }
    }
}
