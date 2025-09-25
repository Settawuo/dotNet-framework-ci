using System.Collections.Generic;
using System.Linq;
using WBBContract;
using WBBContract.Queries.Commons.Master;
using WBBData.Repository;
using WBBEntity.Models;
using WBBEntity.PanelModels;

namespace WBBBusinessLayer.QueryHandlers.Commons.Master
{
    public class GetDormitoryQueryHandler : IQueryHandler<GetDormitoryQuery, List<DomitoryModel>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBBDORM_DORMITORY_MASTER> _FBB_DORM;
        private readonly IEntityRepository<FBBDORM_DORMITORY_DTL> _FBB_DORMdtl;
        private readonly IEntityRepository<FBB_ZIPCODE> _ZIPCODE;
        public GetDormitoryQueryHandler(ILogger logger, IEntityRepository<FBBDORM_DORMITORY_MASTER> FBB_DORM
            , IEntityRepository<FBBDORM_DORMITORY_DTL> FBB_DORMdtl, IEntityRepository<FBB_ZIPCODE> ZIPCODE)
        {
            _logger = logger;
            _FBB_DORM = FBB_DORM;
            _FBB_DORMdtl = FBB_DORMdtl;
            _ZIPCODE = ZIPCODE;
        }

        public List<DomitoryModel> Handle(GetDormitoryQuery query)
        {
            #region SQL
            //select m.dormitory_id,m.dormitory_name_th,m.dormitory_name_en,m.dormitory_no_th,m.dormitory_no_en,
            //d.floor_no,d.room_no
            //from fbbdorm_dormitory_master m,fbbdorm_dormitory_dtl d
            //where m.dormitory_row_id=d.dormitory_row_id
            //order by m.dormitory_id,d.floor_no,d.room_no  
            #endregion

            List<DomitoryModel> list;
            if (query.language == "TH")
            {
                list = (from m in _FBB_DORM.Get()
                        join d in _FBB_DORMdtl.Get() on m.DORMITORY_ROW_ID equals d.DORMITORY_ROW_ID
                        join z in _ZIPCODE.Get() on m.ZIPCODE_ROW_ID_TH equals z.ZIPCODE_ROWID
                        where d.SERVICE_STATUS.ToUpper() == "AVAILABLE" && z.LANG_FLAG == "N" && m.STATE == "In Service"

                        select new DomitoryModel
                        {
                            Pre_dormitory_id = m.DORMITORY_ID,
                            Pre_dormitory_name_th = m.DORMITORY_NAME_TH,
                            Pre_dormitory_no_th = m.DORMITORY_NO_TH,
                            Pre_floor_no = d.FLOOR_NO,
                            Pre_room_no = d.ROOM_NO,
                            Pre_PREPAID_NON_MOBILE = d.PREPAID_NON_MOBILE,
                            Pre_SubcontractId = m.SUB_CONTRACT_LOCATION_CODE,
                            Pre_SubcontractTH = m.SUB_CONTRACT_NAME_TH,
                            Pre_SubcontractID = m.SUB_CONTRACT_LOCATION_CODE,
                            Pre_HOME_NO_TH = m.HOME_NO_TH,
                            Pre_MOO_TH = m.MOO_TH,
                            Pre_SOI_TH = m.SOI_TH,
                            Pre_STREET_NAME_TH = m.STREET_NAME_TH,
                            Pre_PIN_CODE = d.PIN_CODE,
                            Pre_ZipcodeID_TH = m.ZIPCODE_ROW_ID_TH,
                            Pre_Pre_STATE = z.TUMBON + " " + z.AMPHUR + " " + z.PROVINCE,
                            Pre_AddressID = m.ADDRESS_ID,
                            Pre_RegionCode = z.REGION_CODE,
                            Pre_Province = z.PROVINCE
                        }).ToList();
            }
            else
            {
                list = (from m in _FBB_DORM.Get()
                        join d in _FBB_DORMdtl.Get() on m.DORMITORY_ROW_ID equals d.DORMITORY_ROW_ID
                        join z in _ZIPCODE.Get() on m.ZIPCODE_ROW_ID_EN equals z.ZIPCODE_ROWID
                        where d.SERVICE_STATUS.ToUpper() == "AVAILABLE" && z.LANG_FLAG == "Y" && m.STATE == "In Service"
                        select new DomitoryModel
                        {
                            Pre_dormitory_id = m.DORMITORY_ID,
                            Pre_dormitory_name_en = m.DORMITORY_NAME_EN,
                            Pre_dormitory_no_en = m.DORMITORY_NO_EN,
                            Pre_floor_no = d.FLOOR_NO,
                            Pre_room_no = d.ROOM_NO,
                            Pre_PREPAID_NON_MOBILE = d.PREPAID_NON_MOBILE,
                            Pre_SubcontractEN = m.SUB_CONTRACT_NAME_EN,
                            Pre_SubcontractId = m.SUB_CONTRACT_LOCATION_CODE,
                            Pre_HOME_NO_EN = m.HOME_NO_EN,
                            Pre_MOO_EN = m.MOO_EN,
                            Pre_SOI_EN = m.SOI_EN,
                            Pre_STREET_NAME_EN = m.STREET_NAME_EN,
                            Pre_PIN_CODE = d.PIN_CODE,
                            Pre_ZipcodeID_EN = m.ZIPCODE_ROW_ID_EN,
                            Pre_AddressID = m.ADDRESS_ID,
                            Pre_Pre_STATE = z.TUMBON + " " + z.AMPHUR + " " + z.PROVINCE,
                            Pre_SubcontractID = m.SUB_CONTRACT_LOCATION_CODE,
                            Pre_RegionCode = z.REGION_CODE,
                            Pre_Province = z.PROVINCE
                        }).ToList();
            }
            //if (!string.IsNullOrEmpty(query.netnumber))
            //{
            //    list = list.Where(a => a.PREPAID_NON_MOBILE == query.netnumber).ToList();
            //}                

            return list;
        }
    }
}
