//------------------------------------------------------------------------------
// <copyright file="WebDataService.svc.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using WBBBusinessLayer;
using WBBContract;
using WBBContract.Commands.ExWebServices;

namespace WBBExternalService
{
    // ReSharper disable once InconsistentNaming
    public class FBBPreOrder : IFBBPreOrder
    {
        private ILogger _logger;
        private readonly ICommandHandler<PreOrderCommand> _preOrderCommandHandle;


        public FBBPreOrder(ILogger logger, ICommandHandler<PreOrderCommand> preOrderCommandHandle)
        {
            _logger = logger;
            _preOrderCommandHandle = preOrderCommandHandle;
        }


        public string UpdateOrderStatus(string preOrderNo, string status)
        {
            if (string.IsNullOrEmpty(preOrderNo))
                return "[preOrderNo] cannot be null.";

            if (string.IsNullOrEmpty(status))
                return "[status] cannot be null.";

            var preOrderCommand = new PreOrderCommand { PreOrderNo = preOrderNo, Status = status };
            _preOrderCommandHandle.Handle(preOrderCommand);

            return preOrderCommand.ReturnMessage;
        }


    }
}
