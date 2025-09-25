using System;
using System.Collections.Generic;
using System.Linq;
using WBBContract;
using WBBContract.Queries.Commons.Account;
using WBBData.Repository;
using WBBEntity.Models;
using WBBEntity.PanelModels.Account;

namespace WBBBusinessLayer.QueryHandlers.Commons.Account
{
    public class GetUserDataQueryHandler : IQueryHandler<GetUserDataQuery, UserModel>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_USER> _user;
        private readonly IEntityRepository<FBB_GROUP_PERMISSION> _groupPermission;
        private readonly IEntityRepository<FBB_PROGRAM_PERMISSION> _programPermission;
        private readonly IEntityRepository<FBB_PROGRAM> _program;
        private readonly IEntityRepository<FBB_COMPONENT> _component;
        private readonly IEntityRepository<FBB_COMPONENT_PERMISSION> _componentPermission;

        public GetUserDataQueryHandler(ILogger logger,
            IEntityRepository<FBB_USER> user,
            IEntityRepository<FBB_GROUP_PERMISSION> groupPermission,
            IEntityRepository<FBB_PROGRAM_PERMISSION> programPermission,
            IEntityRepository<FBB_PROGRAM> program,
            IEntityRepository<FBB_COMPONENT> component,
            IEntityRepository<FBB_COMPONENT_PERMISSION> componentPermission)
        {
            _logger = logger;
            _user = user;
            _groupPermission = groupPermission;
            _programPermission = programPermission;
            _program = program;
            _component = component;
            _componentPermission = componentPermission;
        }

        public UserModel Handle(GetUserDataQuery query)
        {
            #region full query
            //var getUser = from u in _user.Get()
            //           join gp in _groupPermission.Get() on u.USER_ID equals gp.USER_ID
            //           join pp in _programPermission.Get() on gp.GROUP_ID equals pp.GROUP_ID
            //           join p in _program.Get() on pp.PROGRAM_ID equals p.PROGRAM_ID
            //           where u.USER_NAME == query.UserName
            //           && u.ACTIVE_FLAG == "Y" && gp.ACTIVE_FLG == "Y" && pp.ACTIVE_FLG == "Y" && p.ACTIVE_FLG == "Y"
            //           select new UserModel
            //           {
            //               UserId = u.USER_ID,
            //               UserName = u.USER_NAME,
            //               FirstNameInThai = u.FIRST_NAME,
            //               LastNameInThai = u.LAST_NAME,
            //               MobileNumber = u.MOBILE_NUMBER,
            //               Email = u.EMAIL,
            //           };
            #endregion

            string userTemp = query.UserName;

            if (query.AuthenType == "SSOPartner")
            {
                query.UserName = "telewiz_reg";
                _logger.Info("Get permission by: " + query.UserName);
            }
            else if (query.AuthenType == "SSO")
            {
                query.UserName = "employee_reg";
                _logger.Info("Get permission by: " + query.UserName);
            }

            var getUser = (from u in _user.Get()
                          join gp in _groupPermission.Get() on u.USER_ID equals gp.USER_ID
                          where (u.USER_NAME.ToLower() == query.UserName.ToLower()|| u.PIN_CODE == query.PinCode)
                                && u.ACTIVE_FLAG == "Y"
                                && gp.ACTIVE_FLG == "Y"
                          select new UserModel
                          {
                              UserId = u.USER_ID,
                              UserName = u.USER_NAME,
                              FirstNameInThai = u.FIRST_NAME,
                              LastNameInThai = u.LAST_NAME,
                              MobileNumber = u.MOBILE_NUMBER,
                              Email = u.EMAIL,
                              GroupId = gp.GROUP_ID
                          }).ToList();

            if (query.UserName != "")
                getUser = getUser.Where(t => t.UserName.ToLower() == query.UserName.ToLower()).ToList();


            decimal groupId = 0;
            decimal.TryParse(query.GroupId, out groupId);
            if (groupId != 0) {
                getUser = getUser.Where(p => p.GroupId.ToString() == groupId.ToString()).ToList();
                //getUser = (List<UserModel>)getUser.Where(p => p.GroupId.ToString() == groupId.ToString()); 
            }

            var groupIdList = new List<decimal>();
            groupIdList = getUser.Select(s => s.GroupId).ToList();

            var getProgram = from pp in _programPermission.Get()
                             join p in _program.Get() on pp.PROGRAM_ID equals p.PROGRAM_ID
                             where groupIdList.Contains(pp.GROUP_ID)
                             && pp.ACTIVE_FLG == "Y" && p.ACTIVE_FLG == "Y"
                             orderby p.ORDER_BY
                             select new ProgramModel
                             {
                                 ProgramID = p.PROGRAM_ID,
                                 ProgramName = p.PROGRAM_NAME,
                                 ProgramDescription = p.PROGRAM_DESCRIPTION,
                                 ProgramCode = p.PROGRAM_CODE,
                                 ParentID = p.PARENT_ID
                             };

            var parentProgram = getProgram.Where(w => w.ParentID == 0);

            var program = new List<ProgramModel>();

            decimal temp = 0;
            foreach (var parent in parentProgram)
            {
                if (temp != parent.ProgramID)
                {

                    var subProgram = getProgram.Where(w => w.ParentID == parent.ProgramID);
                    parent.ProgramModels = new List<ProgramModel>();

                    decimal temps = 0;
                    foreach (var s in subProgram)
                    {
                        if (temps != s.ProgramID)
                        {
                            //Code for get ProgramModels for FBBREPORT_PAYG and FBBREPORT_DORM.
                            // if (s.ProgramCode == "FBBREPORT_PAYG" || s.ProgramCode == "FBBREPORT_DORM")
                            // {
                            var subReportProgram = getProgram.Where(w => w.ParentID == s.ProgramID);
                            s.ProgramModels = new List<ProgramModel>();
                            decimal rptTemps = 0;
                            foreach (var rpt in subReportProgram)
                            {
                                if (rptTemps != rpt.ProgramID)
                                {
                                    s.ProgramModels.Add(rpt);
                                    rptTemps = rpt.ProgramID;
                                }
                            }
                            // }

                            parent.ProgramModels.Add(s);
                            temps = s.ProgramID;
                        }
                    }

                    program.Add(parent);
                    temp = parent.ProgramID;
                }
            }

            var model = new UserModel();

            if (getUser.Any())
            {
                var user = getUser.FirstOrDefault();
                model.UserId = user.UserId;
                model.UserName = user.UserName;
                model.FirstNameInThai = user.FirstNameInThai;
                model.LastNameInThai = user.LastNameInThai;
                model.MobileNumber = user.MobileNumber;
                model.Email = user.Email;

                model.Groups = groupIdList;
                model.ProgramModel = program;

                model.ComponentModel = (from a in _component.Get()
                                        join b in _componentPermission.Get() on a.COMPONENT_ID equals b.COMPONENT_ID
                                        //where groupIdList.Contains(b.GROUP_ID)
                                        orderby a.COMPONENT_TYPE, b.GROUP_ID
                                        select new ComponentModel
                                        {
                                            ProgramID = a.PROGRAM_ID,
                                            ComponentName = a.COMPONENT_NAME,
                                            ComponentType = a.COMPONENT_TYPE,
                                            GroupID = b.GROUP_ID,
                                            EnableFlag = b.ENABLE_FLG,
                                            ReadOnlyFlag = b.READ_ONLY_FLG
                                        }).ToList();
            }
            if (userTemp != "")
                model.UserName = userTemp;

            return model;
        }
    }
}
