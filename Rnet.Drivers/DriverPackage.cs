﻿using System.Threading.Tasks;

namespace Rnet.Drivers
{

    /// <summary>
    /// Base driver package class. <see cref="DrivePackage"/>s are searched for <see cref="Driver"/> instances that
    /// support specific RNET devices.
    /// </summary>
    public abstract class DriverPackage
    {

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        protected DriverPackage()
        {

        }

        /// <summary>
        /// Returns a driver for the specified RNET device or <c>null</c> if the package does not contain one.
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        protected internal abstract Task<Driver> GetDriver(RnetDevice device);

    }

}