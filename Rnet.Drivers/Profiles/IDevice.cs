﻿using System.ServiceModel;
using System.Threading.Tasks;

namespace Rnet.Profiles.Basic
{

    /// <summary>
    /// Provides basic device information.
    /// </summary>
    [ServiceContract]
    public interface IDevice : Driver
    {

        /// <summary>
        /// Gets the manufacturer of the device.
        /// </summary>
        /// <returns></returns>
        string Manufacturer { get; }

        /// <summary>
        /// Gets the model of the device.
        /// </summary>
        /// <returns></returns>
        string Model { get; }

        /// <summary>
        /// Gets the firmware version of the device.
        /// </summary>
        /// <returns></returns>
        string FirmwareVersion { get; }

    }

}