using System;
using Chloe.Annotations;

namespace WaterCloud.Domain.DingTalkManage
{
    /// <summary>
    /// 创 建：超级管理员
    /// 日 期：2020-11-21 13:56
    /// 描 述：钉钉用户实体类实体类
    /// </summary>
    [TableAttribute("mob_DingUser")]
    public class DingUserEntity : IEntity<DingUserEntity>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [ColumnAttribute("F_Id", IsPrimaryKey = true)]
        public string F_Id { get; set; }
        /// <summary>
        /// 系统用户Id
        /// </summary>
        /// <returns></returns>
        public string F_SysUserId { get; set; }
        /// <summary>
        /// 员工唯一标识ID
        /// </summary>
        /// <returns></returns>
        public string F_UserId { get; set; }
        /// <summary>
        /// 成员名称
        /// </summary>
        /// <returns></returns>
        public string F_UserName { get; set; }
        /// <summary>
        /// 是否已经激活
        /// </summary>
        /// <returns></returns>
        public bool? F_Active { get; set; }
        /// <summary>
        /// 在对应的部门中的排序，Map结构的json字符串，key是部门的Id，value是人员在这个部门的排序值
        /// </summary>
        /// <returns></returns>
        public string F_OrderInDepts { get; set; }
        /// <summary>
        /// 是否为企业的管理员
        /// </summary>
        /// <returns></returns>
        public bool? F_IsAdmin { get; set; }
        /// <summary>
        /// 是否为企业的老板
        /// </summary>
        /// <returns></returns>
        public bool? F_IsBoss { get; set; }
        /// <summary>
        /// 在当前isv全局范围内唯一标识一个用户的身份,用户无法修改
        /// </summary>
        /// <returns></returns>
        public string F_UnionId { get; set; }
        /// <summary>
        /// 在对应的部门中是否为主管：Map结构的json字符串，key是部门的Id，value是人员在这个部门中是否为主管，true表示是，false表示不是
        /// </summary>
        /// <returns></returns>
        public string F_IsLeaderInDepts { get; set; }
        /// <summary>
        ///  是否是部门的主管
        /// </summary>
        /// <returns></returns>
        public bool? F_IsLeader { get; set; }
        /// <summary>
        /// 是否号码隐藏
        /// </summary>
        /// <returns></returns>
        public bool? F_IsHide { get; set; }
        /// <summary>
        /// 成员所属部门id列表
        /// </summary>
        /// <returns></returns>
        public string F_DepartmentId { get; set; }
        /// <summary>
        /// 职位信息
        /// </summary>
        /// <returns></returns>
        public string F_Position { get; set; }
        /// <summary>
        /// 头像url
        /// </summary>
        /// <returns></returns>
        public string F_Avatar { get; set; }
        /// <summary>
        /// 入职时间
        /// </summary>
        /// <returns></returns>
        public DateTime? F_HiredDate { get; set; }
        /// <summary>
        /// 员工工号
        /// </summary>
        /// <returns></returns>
        public string F_JobNumber { get; set; }
        /// <summary>
        /// 是否是高管
        /// </summary>
        /// <returns></returns>
        public bool? F_IsSenior { get; set; }
        /// <summary>
        /// 电子邮件
        /// </summary>
        /// <returns></returns>
        public string F_Email { get; set; }
        /// <summary>
        /// 手机号码
        /// </summary>
        /// <returns></returns>
        public string F_Mobile { get; set; }
        /// <summary>
        /// 部门排序
        /// </summary>
        /// <returns></returns>
        public int? F_Order { get; set; }
        /// <summary>
        /// 工作地
        /// </summary>
        /// <returns></returns>
        public string F_WorkPlace { get; set; }
        /// <summary>
        /// 新增的钉钉用户在系统中的角色
        /// </summary>
        /// <returns></returns>
        public string F_RolesId { get; set; }
        /// <summary>
        /// 是否在班
        /// </summary>
        public bool? F_IsWork { get; set; }
    }
}
