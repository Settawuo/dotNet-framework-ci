using AIRNETEntity.Models;
using AIRNETEntity.PanelModels;
using System;
using System.Linq;
using WBBContract;
using WBBContract.Commands;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;


namespace WBBBusinessLayer.CommandHandlers
{
    public class RegAisRabbitCommandHandler : ICommandHandler<RegAisRabbitCommand>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_CUST_CONTACT> _custContact;
        private readonly IEntityRepository<FBB_CUST_PROFILE> _custProfile;
        private readonly IEntityRepository<FBB_IMPLEMENT_REGIS_RABBIT> _impRegRabbit;

        private readonly IAirNetEntityRepository<AIR_SALE_ORD_CUSTOMER> _airSaleOrderCustomer;
        private readonly IAirNetEntityRepository<AIR_SALE_ORD_FLOW> _airSaleOrderFlow;
        private readonly IAirNetEntityRepository<AIR_SALE_ORDER> _airSaleOrder;
        private readonly IAirNetEntityRepository<AIR_SALE_ORD_CONTACT> _airSaleOrdContact;
        private readonly IAirNetEntityRepository<AIR_SALE_ORD_IA> _airSaleOrderIA;

        private readonly IWBBUnitOfWork _uow;

        public RegAisRabbitCommandHandler(ILogger logger
                                        , IEntityRepository<FBB_CUST_CONTACT> custContact
                                        , IEntityRepository<FBB_CUST_PROFILE> custProfile
                                        , IEntityRepository<FBB_IMPLEMENT_REGIS_RABBIT> impRegRabbit
                                        , IAirNetEntityRepository<AIR_SALE_ORD_CUSTOMER> airSaleOrderCustomer
                                        , IAirNetEntityRepository<AIR_SALE_ORD_FLOW> airSaleOrderFlow
                                        , IAirNetEntityRepository<AIR_SALE_ORDER> airSaleOrder
                                        , IAirNetEntityRepository<AIR_SALE_ORD_CONTACT> airSaleOrdContact
                                        , IAirNetEntityRepository<AIR_SALE_ORD_IA> airSaleOrderIA
                                        , IWBBUnitOfWork uow)
        {
            _logger = logger;
            _custContact = custContact;
            _custProfile = custProfile;
            _impRegRabbit = impRegRabbit;
            _airSaleOrderCustomer = airSaleOrderCustomer;
            _airSaleOrderFlow = airSaleOrderFlow;
            _airSaleOrder = airSaleOrder;
            _airSaleOrdContact = airSaleOrdContact;
            _airSaleOrderIA = airSaleOrderIA;
            _uow = uow;
        }

        public void Handle(RegAisRabbitCommand command)
        {
            try
            {
                #region sql
                //Select * from WBB.FBB_CUST_PROFILE c
                //Where c.cust_non_mobile = '@NON_MOBILE' 
                //and c.cust_id_card_num = '@IDCARD';
                //and c.rabbit_register_date is not null    
                #endregion
                var chkDupProfile = from custprofile in _custProfile.Get()
                                    where custprofile.CUST_NON_MOBILE == command.Non_Mobile
                                    && custprofile.CUST_ID_CARD_NUM == command.IdCard
                                    && custprofile.RABBIT_REGISTER_DATE != null
                                    select custprofile;

                if (!chkDupProfile.Any())
                {
                    #region sql
                    //select Count(*) from WBB.FBB_IMPLEMENT_REGIS_RABBIT r
                    //where r.cust_non_mobile = '@NON_MOBILE'
                    //and r.cust_id_card_num = '@IDCARD';
                    #endregion
                    var chkDupRegis = from impRegRabbit in _impRegRabbit.Get()
                                      where impRegRabbit.CUST_NON_MOBILE == command.Non_Mobile
                                      && impRegRabbit.CUST_ID_CARD_NUM == command.IdCard
                                      select impRegRabbit;

                    if (!chkDupRegis.Any())
                    {
                        #region sql
                        //Select * from WBB.FBB_CUST_PROFILE c
                        //Where c.cust_non_mobile = '@NON_MOBILE' 
                        //and c.cust_id_card_num = '@IDCARD';
                        #endregion
                        var chkProfile = from custprofile in _custProfile.Get()
                                         where custprofile.CUST_NON_MOBILE == command.Non_Mobile
                                         && custprofile.CUST_ID_CARD_NUM == command.IdCard
                                         select custprofile;

                        if (!chkProfile.Any())
                        {
                            #region check at airnet
                            #region sql
                            //select i.non_mobile_no,v.id_card_no,v.tax_id,c.email_address,v.first_name,v.last_name,v.gender,v.birth_date,c.mobile_no
                            //from air_sale_ord_customer v,air_sale_ord_flow f,air_sale_order o,air_sale_ord_contact c ,air_sale_ord_ia i
                            //where (v.id_card_no = '1100800997945'  or  v.tax_id = '1100800997945') 
                            //and i.non_mobile_no ='8900014443'
                            //and v.order_no=f.order_no and v.order_no = o.order_no
                            //and v.order_no = c.order_no and v.order_no = i.order_no 
                            //and c.primary_contact_type = 'Y' 
                            //and f.flow_seq != -1  
                            //order by v.upd_dtm desc;
                            #endregion                                                       
                            var result = (from v in _airSaleOrderCustomer.Get()
                                          join f in _airSaleOrderFlow.Get() on v.ORDER_NO equals f.ORDER_NO
                                          join o in _airSaleOrder.Get() on v.ORDER_NO equals o.ORDER_NO
                                          join c in _airSaleOrdContact.Get() on v.ORDER_NO equals c.ORDER_NO
                                          join i in _airSaleOrderIA.Get() on v.ORDER_NO equals i.ORDER_NO
                                          where (v.ID_CARD_NO == command.IdCard || v.TAX_ID == command.IdCard)
                                          && i.NON_MOBILE_NO == command.Non_Mobile
                                          && f.FLOW_SEQ != -1
                                          orderby v.UPD_DTM descending
                                          select new RegAisRabbitPanelModel
                                          {
                                              BIRTH_DATE = v.BIRTH_DATE,
                                              EMAIL_ADDRESS = c.EMAIL_ADDRESS,
                                              FIRST_NAME = v.FIRST_NAME,
                                              GENDER = v.GENDER,
                                              ID_CARD_NO = v.ID_CARD_NO,
                                              LAST_NAME = v.LAST_NAME,
                                              MOBILE_NO = c.MOBILE_NO,
                                              NON_MOBILE_NO = i.NON_MOBILE_NO,
                                              TAX_ID = v.TAX_ID
                                          });

                            if (result.Any())
                            {
                                var regRabbit = new FBB_IMPLEMENT_REGIS_RABBIT();
                                var data = result.FirstOrDefault();

                                regRabbit.CUST_NON_MOBILE = data.NON_MOBILE_NO.ToSafeString();
                                regRabbit.CUST_ID_CARD_NUM = data.ID_CARD_NO != null ? data.ID_CARD_NO : data.TAX_ID;
                                regRabbit.CUST_NAME = data.FIRST_NAME.ToSafeString();
                                regRabbit.CUST_SURNAME = data.LAST_NAME.ToSafeString();
                                regRabbit.CUST_BIRTHDAY = data.BIRTH_DATE;
                                regRabbit.CUST_GENDER = data.GENDER.ToSafeString();
                                regRabbit.RABBIT_REGISTER_DATE = DateTime.Now.Date;
                                regRabbit.RABBIT_EMAIL = data.EMAIL_ADDRESS == null ? command.Email : data.EMAIL_ADDRESS + ";" + command.Email;
                                regRabbit.CONTACT_MOBILE_PHONE = data.MOBILE_NO.ToSafeString();
                                regRabbit.CREATED_BY = "system";
                                regRabbit.CREATED_DATE = DateTime.Now;
                                regRabbit.UPDATED_BY = "system";
                                regRabbit.UPDATED_DATE = DateTime.Now;

                                _impRegRabbit.Create(regRabbit);
                                _uow.Persist();

                                command.Return_Code = 1;
                                command.Return_Desc = "succ";
                            }
                            else
                            {
                                command.Return_Code = -1;
                                command.Return_Desc = "err";
                            }
                            #endregion
                        }
                        else
                        {
                            #region sql
                            //Update WBB.FBB_CUST_PROFILE Set rabbit_register_date = @DateTime 
                            //Where cust_non_mobile = '@NON_MOBILE'
                            #endregion
                            var profile = chkProfile.FirstOrDefault();

                            profile.RABBIT_REGISTER_DATE = DateTime.Now.Date;
                            _custProfile.Update(profile);


                            #region sql
                            //select t.contact_email from FBB_CUST_CONTACT t
                            //where t.cust_non_mobile = '@NON_MOBILE'
                            //and t.contact_seq = 1
                            #endregion
                            var contact = (from custContact in _custContact.Get()
                                           where custContact.CUST_NON_MOBILE == command.Non_Mobile
                                           && custContact.CONTACT_SEQ == 1
                                           select custContact);

                            if (contact.Any())
                            {
                                var c = contact.FirstOrDefault();
                                if (string.IsNullOrEmpty(c.CONTACT_EMAIL))
                                    c.CONTACT_EMAIL = command.Email;
                                else
                                    c.CONTACT_EMAIL = c.CONTACT_EMAIL + ";" + command.Email;

                                _custContact.Update(c);
                            }
                            _uow.Persist();

                            command.Return_Code = 1;
                            command.Return_Desc = "succ";
                        }
                    }
                    else
                    {
                        command.Return_Code = 0;
                        command.Return_Desc = "dupp";
                    }
                }
                else
                {
                    command.Return_Code = 0;
                    command.Return_Desc = "dupp";
                }
            }
            catch (Exception ex)
            {
                command.Return_Code = -1;
                command.Return_Desc = ex.GetErrorMessage();
                _logger.Info(ex.GetErrorMessage());
            }

        }
    }
}
