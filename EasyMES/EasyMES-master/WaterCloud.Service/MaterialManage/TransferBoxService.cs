using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using WaterCloud.Code;
using Chloe;
using WaterCloud.Domain.MaterialManage;
using System.IO;
using WaterCloud.Domain.EquipmentManage;

namespace WaterCloud.Service.MaterialManage
{
    /// <summary>
    /// 创 建：超级管理员
    /// 日 期：2020-12-14 12:44
    /// 描 述：流转箱管理服务类
    /// </summary>
    public class TransferBoxService : DataFilterService<TransferBoxEntity>, IDenpendency
    {
        public TransferBoxService(IDbContext context) : base(context)
        {
        }
        #region 获取数据
        public async Task<List<TransferBoxEntity>> GetList(string keyword = "")
        {
            var cachedata = repository.IQueryable();
            if (!string.IsNullOrEmpty(keyword))
            {
                cachedata = cachedata.Where(t => t.F_TransferCode.Contains(keyword) || t.F_Description.Contains(keyword));
            }
            return cachedata.Where(t => t.F_DeleteMark == false).OrderBy(t => t.F_CreatorTime).ToList();
        }

        public async Task<List<TransferBoxEntity>> GetLookList(string keyword = "")
        {
            var query =repository.IQueryable();
            if (!string.IsNullOrEmpty(keyword))
            {
                //此处需修改
                query = query.Where(u => u.F_TransferCode.Contains(keyword) || u.F_Description.Contains(keyword));
            }
            query = GetDataPrivilege("u","", query);
            return query.Where(t => t.F_DeleteMark == false).OrderByDesc(t => t.F_CreatorTime).ToList();
        }

        public async Task<List<TransferBoxEntity>> GetLookList(SoulPage<TransferBoxEntity> pagination,string keyword = "", bool IsTemp = true)
        {
            //获取数据权限
            var query = repository.IQueryable().Where(u => u.F_DeleteMark == false);
            if (!string.IsNullOrEmpty(keyword))
            {
                //此处需修改
                query = query.Where(u => u.F_TransferCode.Contains(keyword) || u.F_Description.Contains(keyword));
            }
            query = query.Where(u => u.F_IsTemp == IsTemp);
            query = GetDataPrivilege("u","",query);
            return await repository.OrderList(query, pagination);
        }

        public async Task<TransferBoxEntity> GetForm(string keyValue)
        {
            var cachedata = await repository.FindEntity(keyValue);
            return cachedata;
        }

        public async Task<TransferBoxEntity> GetLookForm(object keyValue)
        {
            var cachedata = await repository.FindEntity(keyValue);
            return GetFieldsFilterData(cachedata);
        }

        public async Task<List<ReadyTransferBoxEntity>> GetReadyBoxes(string keyValue)
        {
            var data = uniwork.IQueryable<ReadyTransferBoxEntity>(a => a.F_EqpId == keyValue).OrderBy(a=>a.F_CreatorTime).ToList();
            return data;
        }
        #endregion

        /// <summary>
        /// 判断流转箱物料数量是否合适
        /// </summary>
        /// <param name="transferBoxCode"></param>
        /// <param name="materialId"></param>
        /// <param name="num"></param>
        /// <returns></returns>
        public bool IsTransferType(string transferBoxCode, string materialId, float num)
        {
            bool result = false;
            if (!string.IsNullOrEmpty(transferBoxCode) && !string.IsNullOrEmpty(materialId))
            {
                var transfer = uniwork.IQueryable<TransferBoxEntity>(a => a.F_TransferCode == transferBoxCode && a.F_EnabledMark == true && a.F_DeleteMark == false && a.F_TransferState == 0).FirstOrDefault();
                if (transfer != null && transfer.F_TransferMaxNum >= num)
                {
                    if (transfer.F_TransferType == "0")
                    {
                        result = true;
                    }
                    else if (uniwork.IQueryable<TransferTypeMaterialBandingEntity>(a => a.F_TransferType == transfer.F_TransferType && a.F_MaterialId == materialId).Count() > 0)
                    {
                        result = true;
                    }
                }
            }
            return result;
        }
        /// <summary>
        /// 判断流转箱是否可用
        /// </summary>
        /// <param name="transferBoxCode"></param>
        /// <returns></returns>
        public bool IsUseTransfer(string transferBoxCode)
        {
            bool result = false;
            if (!string.IsNullOrEmpty(transferBoxCode))
            {
                if (uniwork.IQueryable<TransferBoxEntity>(a => a.F_TransferCode == transferBoxCode && a.F_EnabledMark == true && a.F_DeleteMark == false && a.F_TransferState == 0).Count() > 0)
                {
                    result = true;
                }
            }
            return result;
        }
        #region 提交数据
        public async Task SubmitForm(TransferBoxEntity entity, string keyValue)
        {
            if (string.IsNullOrEmpty(keyValue))
            {
                entity.F_DeleteMark = false;
                entity.F_TransferState = 0;
                entity.Create();
                await repository.Insert(entity);
            }
            else
            {
                entity.Modify(keyValue);
                await repository.Update(entity);
            }
        }

        public async Task DeleteForm(string keyValue)
        {
            var ids = keyValue.Split(',');
            if (repository.IQueryable().Where(a => ids.Contains(a.F_Id) &&a.F_TransferState>0).Count() > 0)
            {
                throw new Exception("流转箱使用中无法删除");
            }
            await repository.Update(t => ids.Contains(t.F_Id),t=>new TransferBoxEntity { 
                F_DeleteMark=true,
                F_DeleteUserId=currentuser.UserId,
                F_DeleteTime=DateTime.Now            
            });
        }
        public async Task<List<TransferBoxEntity>> CheckFile(string fileFullName)
        {
            if (!FileHelper.IsExcel(fileFullName))
            {
                throw new Exception("文件不是有效的Excel文件!");
            }
            //文件解析
            var list = new ExcelHelper<TransferBoxEntity>().ImportFromExcel(fileFullName);
            //删除文件
            File.Delete(fileFullName);
            foreach (var item in list)
            {
                item.F_Id = Utils.GuId();
                item.F_EnabledMark = true;
                item.F_DeleteMark = false;
                item.F_TransferType = string.IsNullOrEmpty(item.F_TransferType)? "0": item.F_TransferType;
                item.F_TransferState = 0;
                item.F_TransferMaxNum = item.F_TransferMaxNum??125;
                List<string> str = new List<string>();
                if (string.IsNullOrEmpty(item.F_TransferCode))
                {
                    item.F_EnabledMark = false;
                    item.ErrorMsg = "编号不存在";
                    continue;
                }
                else if (repository.IQueryable(a => a.F_TransferCode == item.F_TransferCode).Count() > 0 || list.Where(a => a.F_TransferCode == item.F_TransferCode).Count() > 1)
                {
                    str.Add("编号重复");
                    item.F_EnabledMark = false;
                }
                if (string.IsNullOrEmpty(item.F_TransferType))
                {
                    item.F_EnabledMark = false;
                    item.ErrorMsg = "流转箱类型不存在";
                    continue;
                }
                if (item.F_IsTemp==null)
                {
                    str.Add("临时标识不存在");
                    item.F_EnabledMark = false;
                }
                if (item.F_EnabledMark == false)
                {
                    item.ErrorMsg = string.Join(',', str.ToArray());
                }
            }
            return list;
        }

        public async Task ImportForm(List<TransferBoxEntity> filterList)
        {
            foreach (var item in filterList)
            {
                item.Create();
            }
            await uniwork.Insert(filterList);
        }

        public async Task ReturnForm(string keyValue)
        {
            var ids = keyValue.Split(',');
            if (repository.IQueryable().Where(a => ids.Contains(a.F_Id) && a.F_TransferState !=4).Count() > 0)
            {
                throw new Exception("存在非出库状态流转箱");
            }
            await repository.Update(t => ids.Contains(t.F_Id), t => new TransferBoxEntity
            {
                F_TransferState=0
            });
        }
        public async Task DeleteReadyBox(string transferBoxCode)
        {
            uniwork.BeginTrans();
            await uniwork.Delete<ReadyTransferBoxEntity>(a => a.F_TransferBoxCode == transferBoxCode);
            await uniwork.Update<TransferBoxEntity>(a => a.F_TransferCode == transferBoxCode && a.F_TransferState == 1, a => new TransferBoxEntity
            {
                F_TransferState = 0
            });
            uniwork.Commit();
        }

        public async Task AddReadyBox(ReadyTransferBoxEntity entity)
        {
            if (!IsUseTransfer(entity.F_TransferBoxCode))
            {
                throw new Exception("流转箱不可用");
            }
            var eqp = await uniwork.FindEntity<EquipmentEntity>(entity.F_EqpId);
            if (uniwork.IQueryable<ReadyTransferBoxEntity>(a=>a.F_EqpId==entity.F_EqpId).Count()+1 > eqp.F_OutTransferNum)
            {
                throw new Exception("预绑定流转箱超出设备限制");
            }
            entity.F_Id = Utils.GuId();
            entity.F_CreatorTime = DateTime.Now;
            uniwork.BeginTrans();
            await uniwork.Insert(entity);
            await uniwork.Update<TransferBoxEntity>(a => a.F_TransferCode == entity.F_TransferBoxCode && a.F_TransferState == 0, a => new TransferBoxEntity
            {
                F_TransferState = 1
            });
            uniwork.Commit();
        }
        #endregion

    }
}
