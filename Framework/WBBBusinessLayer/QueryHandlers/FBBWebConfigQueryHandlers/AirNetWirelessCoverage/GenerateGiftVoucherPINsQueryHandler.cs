using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Queries.FBBWebConfigQueries.AirWirelessCoverage;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Models;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers.AirNetWirelessCoverage
{
    public class GenerateGiftVoucherPINsQueryHandler : IQueryHandler<GiftVoucherQuery, List<GiftVoucherPINModels>>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG_ADMIN> _interfaceLog;
        private readonly IEntityRepository<FBB_VOUCHER_PIN> _VoucherTable;
        private readonly String[] _allowedChars = {"1234567890",
                                         "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ",
                                         "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ1234567890"};

        public GenerateGiftVoucherPINsQueryHandler(ILogger logger, IWBBUnitOfWork uow, IEntityRepository<FBB_INTERFACE_LOG_ADMIN> interfaceLog, IEntityRepository<FBB_VOUCHER_PIN> VoucherTable)
        {
            _logger = logger;
            _uow = uow;
            _interfaceLog = interfaceLog;
            _VoucherTable = VoucherTable;
        }

        public List<GiftVoucherPINModels> Handle(GiftVoucherQuery query)
        {
            InterfaceLogAdminCommand log = null;
            List<GiftVoucherPINModels> VoucherPINList = new List<GiftVoucherPINModels>();
            int PasswordLength = (query.pin_length - 1) - query.fixedChar.Length;
            string outputPIN = _allowedChars[query.pin_type];
            outputPIN = string.Concat(outputPIN.Split(query.exceptedChar.ToCharArray(), StringSplitOptions.RemoveEmptyEntries));
            //outputPIN = Regex.Replace(outputPIN, exceptedChar, "");
            //foreach (char chrtmp in exceptedChar)
            //{
            //    outputPIN = outputPIN.Replace(chrtmp.ToString(), string.Empty);
            //}
            char[] chars = outputPIN.ToCharArray();
            int allowedCharCount = outputPIN.Length;
            DateTime Curr_DateTime = DateTime.Now;

            log = InterfaceLogAdminServiceHelper.StartInterfaceAdminLog(_uow, _interfaceLog, query, Curr_DateTime.ToString(), "GenerateGiftVoucherPINsQueryHandler", "GenerateGiftVoucherPINs", "");
            try
            {
                for (int i = 1; i <= query.AmountPINs;)
                {
                    byte[] randomBytes = new byte[PasswordLength];
                    RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider();
                    crypto.GetNonZeroBytes(randomBytes);
                    StringBuilder result = new StringBuilder(PasswordLength);
                    foreach (byte b in randomBytes)
                    {
                        result.Append(chars[b % allowedCharCount]);
                    }
                    string strPIN = result.ToString().Substring(0, query.fixedPosition) + query.fixedChar + result.ToString().Substring(query.fixedPosition);
                    strPIN += GenChkSum(strPIN);

                    var data = _VoucherTable.Get(t => t.VOUCHER_PIN == strPIN);
                    if (data.Any()) { }
                    else
                    {
                        FBB_VOUCHER_PIN itemVoucherPINs = new FBB_VOUCHER_PIN()
                        {
                            VOUCHER_PIN = strPIN,
                            START_DATE = query.start_date,
                            EXPIRE_DATE = query.expired_date,
                            CREATED_BY = "WebAdmin",
                            CREATED_DATE = Curr_DateTime,
                            PIN_STATUS = "New",

                            TRANSACTION_ID = long.Parse(log.OutInterfaceLogId.ToString())
                        };
                        _VoucherTable.Create(itemVoucherPINs);
                        _uow.Persist();

                        GiftVoucherPINModels PINtmp = new GiftVoucherPINModels()
                        {
                            PINCode = strPIN,
                            StartDate = query.start_date,
                            ExpiredDate = query.expired_date
                        };
                        VoucherPINList.Add(PINtmp);
                        i++;
                    }
                }
                InterfaceLogAdminServiceHelper.EndInterfaceAdminLog(_uow, _interfaceLog, query, log, "Success", "");
                return VoucherPINList;
            }
            catch (Exception ex)
            {
                InterfaceLogAdminServiceHelper.EndInterfaceAdminLog(_uow, _interfaceLog, query, log, "Failed", ex.Message.ToString());
                return VoucherPINList;
            }
        }

        public string GenChkSum(string input)
        {
            for (int i = 0; i <= 9; i++)
            {
                input = input.Replace(i.ToString(), Convert.ToChar(i).ToString());
            }
            byte[] asciiBytes = Encoding.ASCII.GetBytes(input);
            string asctmp = "";
            foreach (byte tmp in asciiBytes)
                asctmp += tmp.ToString();

            char[] _revInput = asctmp.ToCharArray();
            Array.Reverse(_revInput);
            int sum = 0;
            for (int i = 0; i < _revInput.Length; i++)
            {
                //if(i % 2 == 0)
                //{
                //    int inttmp = int.Parse(_revInput[i].ToString()) * 2;
                //    int devidetmp = inttmp / 10;
                //    int modtmp = inttmp % 10;
                //    sum += (devidetmp + modtmp);
                //}
                if (IsPrime(i))
                {
                    int inttmp = int.Parse(_revInput[i].ToString()) * 2;
                    int devidetmp = inttmp / 10;
                    int modtmp = inttmp % 10;
                    sum += (devidetmp + modtmp);
                }
                else
                {
                    sum += int.Parse(_revInput[i].ToString());
                }
            }
            return ((10 - (sum % 10)) % 10).ToString();
        }

        public bool IsPrime(int candidate)
        {
            if ((candidate & 1) == 0)
            {
                if (candidate == 2)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            for (int i = 3; (i * i) <= candidate; i += 2)
            {
                if ((candidate % i) == 0)
                {
                    return false;
                }
            }
            return candidate != 1;
        }
    }
}
