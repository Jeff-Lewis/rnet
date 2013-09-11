﻿using System;
using System.ComponentModel.Composition;
using System.Diagnostics.Contracts;

using Rnet.Service.Host.Processors;

namespace Rnet.Service.Host.Processors
{

    [RequestProcessor(typeof(RnetController), -50)]
    public class ControllerRequestProcessor : DeviceRequestProcessor
    {

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="target"></param>
        [ImportingConstructor]
        protected ControllerRequestProcessor(
            BusModule module)
            : base(module)
        {
            Contract.Requires<ArgumentNullException>(module != null);
        }

    }

}