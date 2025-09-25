using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using WBBBusinessLayer.QueryHandlers;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Commands.FBBWebConfigCommands;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Models;

namespace WBBBusinessLayer.CommandHandlers.FBBWebConfigHandlers
{
    public class GenerateGiftVoucherPINsCommandHandler : ICommandHandler<GenerateGiftVoucherPINsCommand>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG_ADMIN> _interfaceLog;
        private readonly IEntityRepository<FBB_VOUCHER_PIN> _VoucherTable;
        private readonly String[] _allowedChars = {"1234567890",
                                         "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ",
                                         "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ1234567890"};
        private BackgroundWorker bw = new BackgroundWorker();
        private GenerateGiftVoucherPINsCommand _command;
        private List<FBB_VOUCHER_PIN> oldPINList = new List<FBB_VOUCHER_PIN>();

        public GenerateGiftVoucherPINsCommandHandler(ILogger logger, IWBBUnitOfWork uow, IEntityRepository<FBB_INTERFACE_LOG_ADMIN> interfaceLog, IEntityRepository<FBB_VOUCHER_PIN> VoucherTable)
        {
            _logger = logger;
            _uow = uow;
            _VoucherTable = VoucherTable;
            _interfaceLog = interfaceLog;
            //bw.DoWork += new DoWorkEventHandler(genPINs);
        }

        public void Handle(GenerateGiftVoucherPINsCommand command)
        {
            try
            {
                _command = command;
                genPINs();
                //bw.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void genPINs()
        //private void genPINs(object sender, DoWorkEventArgs e)
        {
            //BackgroundWorker worker = sender as BackgroundWorker;

            InterfaceLogAdminCommand log = null;
            int PasswordLength = (_command.pin_length - 1) - _command.fixedChar.Trim('?').Length;
            string outputPIN = _allowedChars[_command.pin_type];
            outputPIN = string.Concat(outputPIN.Split(_command.exceptedChar.ToCharArray(), StringSplitOptions.RemoveEmptyEntries));
            //outputPIN = Regex.Replace(outputPIN, exceptedChar, "");
            //foreach (char chrtmp in exceptedChar)
            //{
            //    outputPIN = outputPIN.Replace(chrtmp.ToString(), string.Empty);
            //}
            char[] chars = outputPIN.ToCharArray();
            int allowedCharCount = outputPIN.Length;
            DateTime Curr_DateTime = DateTime.Now;
            log = InterfaceLogAdminServiceHelper.StartInterfaceAdminLog(_uow, _interfaceLog, _command, Curr_DateTime.ToString("yyyyMMddHHmmss"), "GenerateGiftVoucherPINsCommandHandler", "GenerateGiftVoucherPINs", "");
            try
            {
                oldPINList = _VoucherTable.Get().ToList();
                List<FBB_VOUCHER_PIN> newPINList = new List<FBB_VOUCHER_PIN>();
                for (int i = 1; i <= _command.AmountPINs;)
                {
                    byte[] randomBytes = new byte[PasswordLength];
                    RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider();
                    crypto.GetNonZeroBytes(randomBytes);
                    StringBuilder result = new StringBuilder(PasswordLength);
                    foreach (byte b in randomBytes)
                    {
                        result.Append(chars[b % allowedCharCount]);
                    }
                    //string strPIN = result.ToString().Substring(0, _command.fixedPosition) + _command.fixedChar + result.ToString().Substring(_command.fixedPosition);
                    string strPIN = MergePIN(_command.fixedChar, result.ToString());
                    strPIN += GenChkSum(strPIN);

                    var chkdulp_old = oldPINList.Where(x => x.VOUCHER_PIN == strPIN);
                    var chkdulp_new = newPINList.Where(x => x.VOUCHER_PIN == strPIN);
                    if (chkdulp_old.Any() || chkdulp_new.Any()) { }
                    else
                    {
                        FBB_VOUCHER_PIN itemVoucherPINs = new FBB_VOUCHER_PIN()
                        {
                            VOUCHER_PIN = strPIN,
                            START_DATE = _command.start_date,
                            EXPIRE_DATE = _command.expired_date,
                            CREATED_BY = "FBBOR014",
                            CREATED_DATE = Curr_DateTime,
                            PIN_STATUS = "New",
                            VOUCHER_MASTER_ID = _command.voucher_master_id,
                            LOT = _command.lot,

                            //TRANSACTION_ID = 0
                            TRANSACTION_ID = long.Parse(log.OutInterfaceLogId.ToString())
                        };
                        _VoucherTable.Create(itemVoucherPINs);
                        _uow.Persist();
                        newPINList.Add(itemVoucherPINs);

                        i++;
                    }
                }
                InterfaceLogAdminServiceHelper.EndInterfaceAdminLog(_uow, _interfaceLog, _command, log, "Success", "");
            }
            catch (Exception ex)
            {
                InterfaceLogAdminServiceHelper.EndInterfaceAdminLog(_uow, _interfaceLog, _command, log, "Failed", ex.Message.ToString());
            }
        }

        private string GenChkSum(string input)
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

        private bool IsPrime(int candidate)
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

        private string MergePIN(string format, string genpin)
        {
            StringBuilder result = new StringBuilder();
            char[] cgenpin = genpin.ToCharArray();
            int i = 0;
            foreach (char ctmp in format)
            {
                if (ctmp == '?')
                {
                    result.Append(cgenpin[i]);
                    i++;
                }
                else
                {
                    result.Append(ctmp);
                }
            }
            return result.ToString();
        }
    }
}
