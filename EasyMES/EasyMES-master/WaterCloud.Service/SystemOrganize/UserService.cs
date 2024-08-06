/*******************************************************************************
 * Copyright © 2020 WaterCloud.Framework 版权所有
 * Author: WaterCloud
 * Description: WaterCloud快速开发平台
 * Website：
*********************************************************************************/
using WaterCloud.Code;
using WaterCloud.Domain.SystemOrganize;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chloe;
using WaterCloud.DataBase;
using WaterCloud.DingTalk;
using WaterCloud.Service.DingTalkManage;
using WaterCloud.Domain.DingTalkManage;
using WaterCloud.Domain.ClassTask;
using WaterCloud.Domain.SystemSecurity;
using WaterCloud.Service.SystemSecurity;

namespace WaterCloud.Service.SystemOrganize
{
    public class UserService : DataFilterService<UserEntity>, IDenpendency
    {
        private SystemSetService syssetApp;
        public DingTalkConfigService _dingService { get; set; }
        private FilterIPService ipApp;
        /// <summary>
        /// 缓存操作类
        /// </summary>

        private string cacheKey = "watercloud_userdata_";
        private string cacheKeyOperator = "watercloud_operator_";// +登录者token
        //获取类名

        public UserService(IDbContext context) : base(context)
        {
            syssetApp = new SystemSetService(context);
            ipApp = new FilterIPService(context);
        }

        public async Task<List<UserExtend>> GetLookList(SoulPage<UserExtend> pagination, string keyword)
        {
            //反格式化显示只能用"等于"，其他不支持
            Dictionary<string, Dictionary<string, string>> dic = new Dictionary<string, Dictionary<string, string>>();
            Dictionary<string, string> enabledTemp = new Dictionary<string, string>();
            enabledTemp.Add("有效", "1");
            enabledTemp.Add("无效", "0");
            dic.Add("F_EnabledMark", enabledTemp);
            Dictionary<string, string> sexTemp = new Dictionary<string, string>();
            sexTemp.Add("男", "1");
            sexTemp.Add("女", "0");
            dic.Add("F_Gender", sexTemp);
            pagination = ChangeSoulData(dic, pagination);
            //获取数据权限
            var query = GetQuery().Where(u => u.F_IsAdmin == false);
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(u => u.F_Account.Contains(keyword) || u.F_RealName.Contains(keyword) || u.F_MobilePhone.Contains(keyword));
            }
            query = GetDataPrivilege("u", "", query);
            var data = await repository.OrderList(query, pagination);
            var roles = uniwork.IQueryable<RoleEntity>().ToList();
            var orgs = uniwork.IQueryable<OrganizeEntity>().ToList();
            foreach (var item in data)
            {
                string[] roleIds = item.F_RoleId.Split(',');
                string[] departmentIds = item.F_DepartmentId.Split(',');
                item.F_DepartmentName = string.Join(',', orgs.Where(a => departmentIds.Contains(a.F_Id)).Select(a => a.F_FullName).ToList());
                item.F_RoleName = string.Join(',', roles.Where(a => roleIds.Contains(a.F_Id)).Select(a => a.F_FullName).ToList());
            }
            return data;
        }

        public async Task<List<UserExtend>> GetList(string keyword)
        {
            var cachedata = GetQuery().Where(t => t.F_IsAdmin == false);
            if (!string.IsNullOrEmpty(keyword))
            {
                cachedata = cachedata.Where(t => t.F_Account.Contains(keyword) || t.F_RealName.Contains(keyword) || t.F_MobilePhone.Contains(keyword));
            }
            var data = cachedata.OrderBy(t => t.F_Account).ToList();
            var roles = uniwork.IQueryable<RoleEntity>().ToList();
            var orgs = uniwork.IQueryable<OrganizeEntity>().ToList();
            foreach (var item in data)
            {
                string[] roleIds = item.F_RoleId.Split(',');
                string[] departmentIds = item.F_DepartmentId.Split(',');
                item.F_DepartmentName = string.Join(',', orgs.Where(a => departmentIds.Contains(a.F_Id)).Select(a => a.F_FullName).ToList());
                item.F_RoleName = string.Join(',', roles.Where(a => roleIds.Contains(a.F_Id)).Select(a => a.F_FullName).ToList());
            }
            return data;
        }

        public async System.Threading.Tasks.Task SubmitUserForm(UserEntity userEntity)
        {
            await repository.Update(userEntity);
            await CacheHelper.Remove(cacheKey + userEntity.F_Id);
            await CacheHelper.Remove(cacheKey + "list");
        }

        public async Task<List<UserEntity>> GetUserList(string keyword)
        {
            var cachedata =await repository.CheckCacheList(cacheKey + "list");
            if (!string.IsNullOrEmpty(keyword))
            {
                cachedata = cachedata.Where(t => t.F_Account.Contains(keyword) || t.F_RealName.Contains(keyword) || t.F_MobilePhone.Contains(keyword)).ToList();
            }
            return cachedata.Where(t => t.F_EnabledMark ==true && t.F_DeleteMark == false).OrderBy(t => t.F_Account).ToList();
        }

        public async Task<dynamic> GetManList()
        {
            var cachedata = await repository.CheckCacheList(cacheKey + "list");
            return cachedata.Where(t => t.F_EnabledMark == true && t.F_DeleteMark == false && !t.F_RealName.Contains("超级"))
                .OrderBy(t => t.F_Account).Select(t => new
                {
                    name = t.F_RealName,
                    code=t.F_Id,

                }).ToList();
        }

        public async Task<UserEntity> GetForm(string keyValue)
        {
            var cachedata = await repository.CheckCache(cacheKey, keyValue);
            return cachedata;
        }
        public async Task<UserEntity> GetFormExtend(string keyValue)
        {
            var cachedata =await repository.CheckCache(cacheKey, keyValue);
            string[] temp;
            if (cachedata.F_RoleId!=null)
            {
                temp = cachedata.F_RoleId.Split(',');
                cachedata.F_RoleName = string.Join(",", uniwork.IQueryable<RoleEntity>().Where(a => temp.Contains(a.F_Id)).Select(a => a.F_FullName).ToList().ToArray());
            }
            if (cachedata.F_DepartmentId != null)
            {
                temp = cachedata.F_DepartmentId.Split(',');
                cachedata.F_DepartmentName = string.Join(",", uniwork.IQueryable<OrganizeEntity>().Where(a => temp.Contains(a.F_Id)).Select(a => a.F_FullName).ToList().ToArray());
            }

            return cachedata;
        }
        public async Task<UserEntity> GetLookForm(string keyValue)
        {
            var cachedata = await repository.CheckCache(cacheKey, keyValue);
            string[] temp;
            if (cachedata.F_RoleId != null)
            {
                temp = cachedata.F_RoleId.Split(',');
                cachedata.F_RoleName = string.Join(",", uniwork.IQueryable<RoleEntity>().Where(a => temp.Contains(a.F_Id)).Select(a => a.F_FullName).ToList().ToArray());
            }
            if (cachedata.F_DepartmentId != null)
            {
                temp = cachedata.F_DepartmentId.Split(',');
                cachedata.F_DepartmentName = string.Join(",", uniwork.IQueryable<OrganizeEntity>().Where(a => temp.Contains(a.F_Id)).Select(a => a.F_FullName).ToList().ToArray());
            }
            return GetFieldsFilterData(cachedata);
        }
        public IQuery<UserExtend> GetQuery()
        {
            var query = repository.IQueryable(t => t.F_DeleteMark == false)
                .LeftJoin<RoleEntity>((a, b) => a.F_DutyId == b.F_Id)
                .LeftJoin<SystemSetEntity>((a, b, c) => a.F_OrganizeId == c.F_Id)
                .Select((a, b, c) => new UserExtend
                {
                    F_Id = a.F_Id,
                    F_IsSenior = a.F_IsSenior,
                    F_SecurityLevel = a.F_SecurityLevel,
                    F_Account = a.F_Account,
                    F_DingTalkNick = a.F_DingTalkNick,
                    F_IsAdmin = a.F_IsAdmin,
                    F_Birthday = a.F_Birthday,
                    F_CompanyName = c.F_CompanyName,
                    F_CreatorTime = a.F_CreatorTime,
                    F_CreatorUserId = a.F_CreatorUserId,
                    F_DepartmentId = a.F_DepartmentId,
                    F_Description = a.F_Description,
                    F_DingTalkOpenId = a.F_DingTalkOpenId,
                    F_DingTalkUnionId = a.F_DingTalkUnionId,
                    F_DutyId = a.F_DutyId,
                    F_DutyName = b.F_FullName,
                    F_Email = a.F_Email,
                    F_EnabledMark = a.F_EnabledMark,
                    F_Gender = a.F_Gender,
                    F_HeadIcon = a.F_HeadIcon,
                    F_HeadImgUrl = a.F_HeadImgUrl,
                    F_IsBoss = a.F_IsBoss,
                    F_IsLeaderInDepts = a.F_IsLeaderInDepts,
                    F_ManagerId = a.F_ManagerId,
                    F_MobilePhone = a.F_MobilePhone,
                    F_NickName = a.F_NickName,
                    F_OrganizeId = a.F_OrganizeId,
                    F_RealName = a.F_RealName,
                    F_Remark = a.F_RealName,
                    F_RoleId = a.F_RoleId,
                    F_Signature = a.F_Signature,
                    F_SortCode = a.F_SortCode,
                    F_WeChat = a.F_WeChat,
                    F_WxNickName = a.F_WxNickName,
                    F_WxOpenId = a.F_WxOpenId,
                    F_IsPlanMan=a.F_IsPlanMan,
                    F_DepartmentName = "",
                    F_RoleName = "",
                });
            return query;
        }
        public async System.Threading.Tasks.Task DeleteForm(string keyValue)
        {
            uniwork.BeginTrans();
            await repository.Delete(t => t.F_Id == keyValue);
            await uniwork.Delete<UserLogOnEntity>(t => t.F_UserId == keyValue);
            await uniwork.Delete<DingUserEntity>(t => t.F_UserId == keyValue);
            await uniwork.Delete<ControlJobUserBandingEntity>(t => t.F_UserId == keyValue);
            uniwork.Commit();
            await CacheHelper.Remove(cacheKey + keyValue);
            await CacheHelper.Remove(cacheKey + "list");
        }
        public async System.Threading.Tasks.Task SubmitForm(UserEntity userEntity, UserLogOnEntity userLogOnEntity, string keyValue)
        {
            if (!string.IsNullOrEmpty(keyValue))
            {
                userEntity.Modify(keyValue);
                await CacheHelper.Remove(cacheKey + keyValue);
                await CacheHelper.Remove(cacheKey + "list");
            }
            else
            {
                userEntity.Create();
                userEntity.F_IsPlanMan = false;
                userLogOnEntity.F_Id= Utils.GuId();
                userLogOnEntity.F_UserId = userEntity.F_Id;
                 userLogOnEntity.F_ErrorNum = 0;
                userLogOnEntity.F_UserOnLine = false;
                userLogOnEntity.F_LogOnCount = 0;
                await CacheHelper.Remove(cacheKey + "list");
            }
            uniwork.BeginTrans();
            if (!string.IsNullOrEmpty(keyValue))
            {
                await repository.Update(userEntity);
            }
            else
            {
                userLogOnEntity.F_Id = userEntity.F_Id;
                userLogOnEntity.F_UserId = userEntity.F_Id;
                userLogOnEntity.F_UserSecretkey = Md5.md5(Utils.CreateNo(), 16).ToLower();
                userLogOnEntity.F_UserPassword = Md5.md5(DESEncrypt.Encrypt(Md5.md5(userLogOnEntity.F_UserPassword, 32).ToLower(), userLogOnEntity.F_UserSecretkey).ToLower(), 32).ToLower();
                await repository.Insert(userEntity);
                await uniwork.Insert(userLogOnEntity);
            }
            uniwork.Commit();
        }
        public async System.Threading.Tasks.Task UpdateForm(UserEntity userEntity)
        {
            await repository.Update(userEntity);
            await CacheHelper.Remove(cacheKey + userEntity.F_Id);
            await CacheHelper.Remove(cacheKey + "list");        }
        /// <summary>
        /// 登录判断
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<UserEntity> CheckLogin(string username, string password,string localurl)
        {
            //根据登录公司查找公司
            //if (!string.IsNullOrEmpty(localurl))
            //{
            //    var setTemp=(await syssetApp.GetList()).Where(a=> localurl.Contains(a.F_HostUrl)).FirstOrDefault();
            //    if (setTemp!=null)
            //    {
            //        repository = new RepositoryBase<UserEntity>(setTemp.F_DbString,setTemp.F_DBProvider);
            //        uniwork = new RepositoryBase(setTemp.F_DbString, setTemp.F_DBProvider);
            //    }
            //}
            UserEntity userEntity =await repository.FindEntity(t => t.F_Account == username);
            if (userEntity != null)
            {
                if (userEntity.F_EnabledMark == true)
                {
                    //缓存用户账户信息
                    var userLogOnEntity=await CacheHelper.Get<OperatorUserInfo>(cacheKeyOperator + "info_" + userEntity.F_Id);
                    if (userLogOnEntity==null)
                    {
                        userLogOnEntity = new OperatorUserInfo();
                        UserLogOnEntity entity =await uniwork.FindEntity<UserLogOnEntity>(userEntity.F_Id);
                        userLogOnEntity.F_UserPassword = entity.F_UserPassword;
                        userLogOnEntity.F_UserSecretkey = entity.F_UserSecretkey;
                        userLogOnEntity.F_AllowEndTime = entity.F_AllowEndTime;
                        userLogOnEntity.F_AllowStartTime = entity.F_AllowStartTime;
                        userLogOnEntity.F_AnswerQuestion = entity.F_AnswerQuestion;
                        userLogOnEntity.F_ChangePasswordDate = entity.F_ChangePasswordDate;
                        userLogOnEntity.F_FirstVisitTime = entity.F_FirstVisitTime;
                        userLogOnEntity.F_LastVisitTime = entity.F_LastVisitTime;
                        userLogOnEntity.F_LockEndDate = entity.F_LockEndDate;
                        userLogOnEntity.F_LockStartDate = entity.F_LockStartDate;
                        userLogOnEntity.F_LogOnCount = entity.F_LogOnCount;
                        userLogOnEntity.F_PreviousVisitTime = entity.F_PreviousVisitTime;
                        userLogOnEntity.F_Question = entity.F_Question;
                        userLogOnEntity.F_Theme = entity.F_Theme;
                        await CacheHelper.Set(cacheKeyOperator + "info_" + userEntity.F_Id, userLogOnEntity);
                    }
                    if (userLogOnEntity == null)
                    {
                        throw new Exception("账户未初始化设置密码,请联系管理员");
                    }
                    string dbPassword = Md5.md5(DESEncrypt.Encrypt(password.ToLower(), userLogOnEntity.F_UserSecretkey).ToLower(), 32).ToLower();
                    if (dbPassword == userLogOnEntity.F_UserPassword)
                    {
                        if (userEntity.F_Account != GlobalContext.SystemConfig.SysemUserCode)
                        {
                            var list = userEntity.F_RoleId.Split(',');
                            var rolelist =uniwork.IQueryable<RoleEntity>(a=>list.Contains(a.F_Id)&&a.F_EnabledMark==true).ToList();
                            if (rolelist.Count() == 0)
                            {
                                throw new Exception("账户未设置权限,请联系管理员");
                            }
                        }
                        DateTime lastVisitTime = DateTime.Now;
                        int LogOnCount = (userLogOnEntity.F_LogOnCount).ToInt() + 1;
                        if (userLogOnEntity.F_LastVisitTime != null)
                        {
                            userLogOnEntity.F_PreviousVisitTime = userLogOnEntity.F_LastVisitTime.ToDate();
                        }
                        userLogOnEntity.F_LastVisitTime = lastVisitTime;
                        userLogOnEntity.F_LogOnCount = LogOnCount;
                        userLogOnEntity.F_UserOnLine = true;
                        await CacheHelper.Remove(cacheKeyOperator + "info_" + userEntity.F_Id);
                        await CacheHelper.Set(cacheKeyOperator + "info_" + userEntity.F_Id, userLogOnEntity);
                        await OperatorProvider.Provider.ClearCurrentErrorNum();
                        return userEntity;
                    }
                    else
                    {
                        //登录错误不超过3次
                        int num = await OperatorProvider.Provider.AddCurrentErrorNum();
                        string erornum = (4 - num).ToString();
                        if (num == 4)
                        {
                            FilterIPEntity ipentity = new FilterIPEntity();
                            ipentity.F_Id = Utils.GuId();
                            ipentity.F_StartIP = WebHelper.Ip;
                            ipentity.F_CreatorTime = DateTime.Now;
                            ipentity.F_DeleteMark = false;
                            ipentity.F_EnabledMark = true;
                            ipentity.F_Type = false;
                            //封禁12小时
                            ipentity.F_EndTime = DateTime.Now.AddHours(12);
                            await ipApp.SubmitForm(ipentity, null);
                            await OperatorProvider.Provider.ClearCurrentErrorNum();
                            throw new Exception("密码不正确，IP被锁定");
                        }
                        else
                        {
                            throw new Exception("密码不正确，请重新输入，还有" + erornum + "次机会");
                        }
                    }
                }
                else
                {
                    throw new Exception("账户被系统锁定,请联系管理员");
                }
            }
            else
            {
                throw new Exception("账户不存在，请重新输入");
            }
        }

        public async System.Threading.Tasks.Task CheckSystemPassword(string password)
        {
            string dbPassword = Md5.md5(GlobalContext.SystemConfig.BoardPassword,32).ToLower();
            if (dbPassword != password)
            {
                throw new Exception("密码不正确");
            }
        }
        public async Task<UserEntity> CheckDingLogin(string loginTmpCode)
        {
            var dingconfig = await _dingService.GetLoginForm();
            string appId = dingconfig.F_AppId;
            string appSecret = dingconfig.F_AppSecret;
            var userDetail= DingTalkUserApi.GetUserDetail(loginTmpCode, appId, appSecret);
            var user = repository.IQueryable().Where(a => a.F_DingTalkUnionId == userDetail.Unionid).FirstOrDefault();
            if (user==null)
            {
                user = new UserEntity();
                user.F_DingTalkOpenId = userDetail.OpenId;
                user.F_DingTalkNick = userDetail.UserId;
                user.F_DingTalkUnionId = userDetail.Unionid;
                return user;
            }
            else
            {
                if (user.F_EnabledMark==true && user.F_DeleteMark==false)
                {
                    return user;
                }
                else
                {
                    throw new Exception("账户不可用,请联系管理员");
                }
            }
        }

        public async Task<UserEntity> CheckDingLoginApp(string loginTmpCode)
        {
            var dingconfig = await _dingService.GetAppletForm(true);
            var dingdingtoken = await CacheHelper.Get<string>(dingconfig.F_AgentId);
            if (string.IsNullOrEmpty(dingdingtoken))
            {

                string appKey = dingconfig.F_AppKey;
                string appSecret = dingconfig.F_AppSecret;
                dingdingtoken = DingTalkHelper.GetAccessToken2(appKey, appSecret);
                await CacheHelper.Set(dingconfig.F_AgentId, dingdingtoken, 2, false);
            }
            var userDetail = DingTalkUserApi.GetUserDetailByAuthCode(dingdingtoken, loginTmpCode);
            var user = repository.IQueryable().Where(a => a.F_DingTalkUnionId == userDetail.Unionid).FirstOrDefault();
            if (user == null)
            {
                user = new UserEntity();
                user.F_DingTalkOpenId = userDetail.OpenId;
                user.F_DingTalkNick = userDetail.UserId;
                user.F_DingTalkUnionId = userDetail.Unionid;
                return user;
            }
            else
            {
                if (user.F_EnabledMark == true && user.F_DeleteMark == false)
                {
                    return user;
                }
                else
                {
                    throw new Exception("账户不可用,请联系管理员");
                }
            }
        }

        public async Task<bool> CheckApiTokenExist(string apitoken)
        {
            return await CacheHelper.Exists(cacheKeyOperator + apitoken);

        }


        public async System.Threading.Tasks.Task DingBandingUser(UserEntity user, string OpenId, string Nick, string UnionId)
        {
            var userEntity = repository.IQueryable().Where(a => a.F_DingTalkUnionId == UnionId).FirstOrDefault();
            if (userEntity != null)
            {
                throw new Exception("用户账号已被绑定");
            }
            else
            {
                var dingconfig = await _dingService.GetAppletForm();
                var dingdingtoken = await CacheHelper.Get<string>(dingconfig.F_AgentId);
                if (string.IsNullOrEmpty(dingdingtoken))
                {

                    string appKey = dingconfig.F_AppKey;
                    string appSecret = dingconfig.F_AppSecret;
                    dingdingtoken = DingTalkHelper.GetAccessToken2(appKey, appSecret);
                    await CacheHelper.Set(dingconfig.F_AgentId, dingdingtoken, 2, false);
                }
                string userId = DingTalkUserApi.getUseridByUnionid(dingdingtoken, UnionId);
                var dingUserTemp = DingTalkUserApi.GetUserDetail(dingdingtoken, userId);
                DingUserEntity dingUser = new DingUserEntity();
                dingUser.F_Id = Utils.GuId();
                dingUser.F_Active = dingUserTemp.Active;
                dingUser.F_Avatar = dingUserTemp.Avatar;
                dingUser.F_IsAdmin = dingUserTemp.IsAdmin;
                dingUser.F_DepartmentId = string.Join(",", dingUserTemp.Department.ToArray());
                dingUser.F_HiredDate = Utils.LongDateTimeToDateTimeString(dingUserTemp.HiredDate);
                dingUser.F_IsHide = dingUserTemp.IsHide;
                dingUser.F_Email = dingUserTemp.Email;
                dingUser.F_IsBoss = dingUserTemp.IsBoss;
                dingUser.F_IsLeader = dingUserTemp.IsLeader;
                dingUser.F_IsLeaderInDepts = dingUserTemp.IsLeaderInDepts;
                dingUser.F_IsSenior = dingUserTemp.IsSenior;
                dingUser.F_JobNumber = dingUserTemp.JobNumber;
                dingUser.F_Mobile = dingUserTemp.Mobile;
                dingUser.F_Order = (int)dingUserTemp.Order;
                dingUser.F_OrderInDepts = dingUserTemp.OrderInDepts;
                dingUser.F_Position = dingUserTemp.Position;
                dingUser.F_RolesId = dingUserTemp.RolesId;
                dingUser.F_SysUserId = user.F_Id;
                dingUser.F_UnionId = dingUserTemp.Unionid;
                dingUser.F_UserId = dingUserTemp.UserId;
                dingUser.F_UserName = dingUserTemp.UserName;
                uniwork.BeginTrans();
                await uniwork.Insert(dingUser);
                await repository.Update(a => a.F_Id == user.F_Id, a => new UserEntity
                {
                    F_DingTalkOpenId = OpenId,
                    F_DingTalkNick= Nick,
                    F_DingTalkUnionId= UnionId
                });
                uniwork.Commit();
                var list = new List<string>();
                list.Add(dingUser.F_UserId);
                WorkMessageApi.SendMessagebyText(dingdingtoken, long.Parse(dingconfig.F_AgentId), EnumClass.RecvType.UseridList, list, "钉钉账号已成功绑定MES网页端   " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            }
        }
        public async System.Threading.Tasks.Task<UserDetailInfo> DingBandingUser(UserEntity user, string code)
        {
            var dingconfig = await _dingService.GetAppletForm(true);
            var dingdingtoken = await CacheHelper.Get<string>(dingconfig.F_AgentId);
            if (string.IsNullOrEmpty(dingdingtoken))
            {

                string appKey = dingconfig.F_AppKey;
                string appSecret = dingconfig.F_AppSecret;
                dingdingtoken = DingTalkHelper.GetAccessToken2(appKey, appSecret);
                await CacheHelper.Set(dingconfig.F_AgentId, dingdingtoken, 2, false);
            }
            var dingUserTemp = DingTalkUserApi.GetUserDetailByAuthCode(dingdingtoken, code);
            var userTemp = repository.IQueryable().Where(a => a.F_Id == user.F_Id).FirstOrDefault();

                if (!(userTemp!=null&& userTemp.F_EnabledMark == true && userTemp.F_DeleteMark == false))
            { 
                    throw new Exception("账户不可用,请联系管理员");
                }
            
            var userEntity = repository.IQueryable().Where(a => a.F_DingTalkUnionId == dingUserTemp.Unionid).FirstOrDefault();
            if (userEntity != null)
            {
                throw new Exception("用户账号已被绑定");
            }
            else
            {
                //string userId = DingTalkUserApi.getUseridByUnionid(dingdingtoken, entity.F_DingTalkUnionId);
                //var dingUserTemp = DingTalkUserApi.GetUserDetail(dingdingtoken, userId);
                DingUserEntity dingUser = new DingUserEntity();
                dingUser.F_Id = Utils.GuId();
                dingUser.F_Active = dingUserTemp.Active;
                dingUser.F_Avatar = dingUserTemp.Avatar;
                dingUser.F_IsAdmin = dingUserTemp.IsAdmin;
                dingUser.F_DepartmentId = string.Join(",", dingUserTemp.Department.ToArray());
                dingUser.F_HiredDate = Utils.LongDateTimeToDateTimeString(dingUserTemp.HiredDate);
                dingUser.F_IsHide = dingUserTemp.IsHide;
                dingUser.F_Email = dingUserTemp.Email;
                dingUser.F_IsBoss = dingUserTemp.IsBoss;
                dingUser.F_IsLeader = dingUserTemp.IsLeader;
                dingUser.F_IsLeaderInDepts = dingUserTemp.IsLeaderInDepts;
                dingUser.F_IsSenior = dingUserTemp.IsSenior;
                dingUser.F_JobNumber = dingUserTemp.JobNumber;
                dingUser.F_Mobile = dingUserTemp.Mobile;
                dingUser.F_Order = (int)dingUserTemp.Order;
                dingUser.F_OrderInDepts = dingUserTemp.OrderInDepts;
                dingUser.F_Position = dingUserTemp.Position;
                dingUser.F_RolesId = dingUserTemp.RolesId;
                dingUser.F_SysUserId = user.F_Id;
                dingUser.F_UnionId = dingUserTemp.Unionid;
                dingUser.F_UserId = dingUserTemp.UserId;
                dingUser.F_UserName = dingUserTemp.UserName;
                uniwork.BeginTrans();
                await uniwork.Insert(dingUser);
                await repository.Update(a => a.F_Id == user.F_Id, a => new UserEntity
                {
                    F_DingTalkOpenId = dingUserTemp.OpenId,
                    F_DingTalkNick = dingUserTemp.UserName,
                    F_DingTalkUnionId = dingUserTemp.Unionid
                });
                uniwork.Commit();
                var list = new List<string>();
                list.Add(dingUser.F_UserId);
                WorkMessageApi.SendMessagebyText(dingdingtoken, long.Parse(dingconfig.F_AgentId), EnumClass.RecvType.UseridList, list, "钉钉账号已成功绑定MES网页端   " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                return dingUserTemp;
            }
        }
        public async System.Threading.Tasks.Task DingCancle(string keyValue)
        {
            if (string.IsNullOrEmpty(keyValue))
            {
                throw new Exception("主键为空");
            }
            uniwork.BeginTrans();
            await repository.Update(a => a.F_Id == keyValue, a => new UserEntity
            {
                F_DingTalkOpenId = null,
                F_DingTalkNick = null,
                F_DingTalkUnionId = null
            });
            await uniwork.Delete<DingUserEntity>(a => a.F_SysUserId == keyValue);
            uniwork.Commit();
        }
        
    }
}
