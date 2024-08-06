using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using WaterCloud.Code;
using Chloe;
using WaterCloud.Domain.RunMonitor;
using WaterCloud.Domain.SystemOrganize;
using WaterCloud.DingTalk;
using WaterCloud.Service.DingTalkManage;
using WaterCloud.Domain.SystemSecurity;
using WaterCloud.Service.SystemSecurity;

namespace WaterCloud.Service.RunMonitor
{
    /// <summary>
    /// 创 建：超级管理员
    /// 日 期：2021-01-25 08:29
    /// 描 述：看板账户服务类
    /// </summary>
    public class BoardAccountService : DataFilterService<BoardAccountEntity>, IDenpendency
    {
        public DingTalkConfigService _dingService { get; set; }
        private FilterIPService ipApp;
        public BoardAccountService(IDbContext context) : base(context)
        {
            ipApp = new FilterIPService(context);
        }
        #region 获取数据
        public async Task<List<BoardAccountEntity>> GetList(string keyword = "")
        {
            var data = repository.IQueryable();
            if (!string.IsNullOrEmpty(keyword))
            {
                //此处需修改
                data = data.Where(t => t.F_Account.Contains(keyword));
            }
            return data.Where(t => t.F_DeleteMark == false).OrderByDesc(t => t.F_CreatorTime).ToList();
        }

        public async Task<List<BoardAccountEntity>> GetLookList(string keyword = "")
        {
            var query = repository.IQueryable().Where(t => t.F_DeleteMark == false);
            if (!string.IsNullOrEmpty(keyword))
            {
                //此处需修改
                query = query.Where(t => t.F_Account.Contains(keyword));
            }
             //权限过滤
             query = GetDataPrivilege("u", "", query);
            return query.OrderByDesc(t => t.F_CreatorTime).ToList();
        }

        public async Task<List<BoardAccountEntity>> GetLookList(SoulPage<BoardAccountEntity> pagination,string keyword = "",string id="")
        {
            //反格式化显示只能用"等于"，其他不支持
            Dictionary<string, Dictionary<string, string>> dic = new Dictionary<string, Dictionary<string, string>>();
            Dictionary<string, string> enabledTemp = new Dictionary<string, string>();
            enabledTemp.Add("有效", "1");
            enabledTemp.Add("无效", "0");
            dic.Add("F_EnabledMark", enabledTemp);
            pagination = ChangeSoulData(dic, pagination);
            var query = repository.IQueryable().Where(u => u.F_DeleteMark==false);
            if (!string.IsNullOrEmpty(keyword))
            {
                //此处需修改
                query = query.Where(t => t.F_Account.Contains(keyword));
            }
            if(!string.IsNullOrEmpty(id))
            {
                query= query.Where(u=>u.F_Id==id);
            }
            //权限过滤
            query = GetDataPrivilege("u","",query);
            return await repository.OrderList(query, pagination);
        }

        public async Task<BoardAccountEntity> GetForm(string keyValue)
        {
            var data = await repository.FindEntity(keyValue);
            return data;
        }
        #endregion

        public async Task<BoardAccountEntity> GetLookForm(object keyValue)
        {
            var data = await repository.FindEntity(keyValue);
            return GetFieldsFilterData(data);
        }

        #region 提交数据
        public async System.Threading.Tasks.Task SubmitForm(BoardAccountEntity entity, string keyValue)
        {
            if (string.IsNullOrEmpty(keyValue))
            {
                entity.F_DeleteMark = false;
                entity.Create();
                await repository.Insert(entity);
            }
            else
            {
                entity.Modify(keyValue); 
                await repository.Update(entity);
            }
        }

        public async System.Threading.Tasks.Task DeleteForm(string keyValue)
        {
            var ids = keyValue.Split(',');
            await repository.Delete(t => ids.Contains(t.F_Id.ToString()));
        }

        public async Task<OperatorBoard> CheckLogin(string username, string password)
        {
            BoardAccountEntity boardAccount = await repository.FindEntity(t => t.F_Account == username);
            if (boardAccount != null)
            {
                if (boardAccount.F_EnabledMark == true)
                {
                    if (password == boardAccount.F_Password)
                    {
                        return boardAccount.MapTo<OperatorBoard>();
                    }
                    else
                    {
                        throw new Exception("密码不正确，请重新输入");
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

        public async Task<OperatorBoard> CheckLoginSkip()
        {
            BoardAccountEntity boardAccount = repository.IQueryable(a => a.F_EnabledMark == true && a.F_DeleteMark == false).FirstOrDefault();
            if (boardAccount != null)
            {
                return boardAccount.MapTo<OperatorBoard>();
            }
            else
            {
                throw new Exception("不存在看板账号，请添加");
            }
        }

        public async Task<UserEntity> CheckDingLogin(string loginTmpCode)
        {
            var dingconfig = await _dingService.GetBoardLoginForm();
            string appId = dingconfig.F_AppId;
            string appSecret = dingconfig.F_AppSecret;
            var userDetail = DingTalkUserApi.GetUserDetail(loginTmpCode, appId, appSecret);
            var user = uniwork.IQueryable<UserEntity>().Where(a => a.F_DingTalkUnionId == userDetail.Unionid).FirstOrDefault();
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
        public async Task<UserEntity> CheckDingLogin(string username,string password)
        {
            var dingconfig = await _dingService.GetBoardLoginForm();
            string appId = dingconfig.F_AppId;
            string appSecret = dingconfig.F_AppSecret;
            var user = uniwork.IQueryable<UserEntity>().Where(a => a.F_Account == username).FirstOrDefault();
            if (user == null)
            {
                throw new Exception("账户不存在,请联系管理员");
            }
            else
            {
                if (user.F_EnabledMark == true && user.F_DeleteMark == false)
                {
                    var userinfo = uniwork.IQueryable<UserLogOnEntity>(a => a.F_UserId == user.F_Id).FirstOrDefault();
                    string dbPassword = Md5.md5(DESEncrypt.Encrypt(Md5.md5(password.ToLower(),32).ToLower(), userinfo.F_UserSecretkey).ToLower(), 32).ToLower();
                    if (dbPassword != userinfo.F_UserPassword)
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
                    return user;
                }
                else
                {
                    throw new Exception("账户不可用,请联系管理员");
                }
            }
        }
        #endregion

    }
}
