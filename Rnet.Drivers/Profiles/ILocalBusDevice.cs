﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rnet.Profiles.Basic
{

    /// <summary>
    /// Provides the profile for the bus device.
    /// </summary>
    [ProfileProvider]
    public class BusDevice : DeviceProfileProvider
    {

        /// <summary>
        /// Implements the bus device profile.
        /// </summary>
        class Profile : DeviceProfileBase, IObject, IDevice, IBusDevice
        {

            /// <summary>
            /// Initializes a new instance.
            /// </summary>
            /// <param name="device"></param>
            public Profile(RnetLocalDevice device)
                : base(device)
            {

            }

            public string Manufacturer
            {
                get { return "Jerome Haltom"; }
            }

            public string Model
            {
                get { return "RNET .Net Device"; }
            }

            public string FirmwareVersion
            {
                get { return typeof(Rnet.RnetBus).Assembly.GetName().Version.ToString(); }
            }

            public string Name
            {
                get { return Model; }
            }

        }

        public override Task<IEnumerable<Driver>> GetDeviceProfilesAsync(RnetDevice target)
        {
            var d = target as RnetLocalDevice;
            if (d == null)
                return base.GetDeviceProfilesAsync(target);

            return Task.FromResult<IEnumerable<Driver>>(new[] { new Profile(d) });
        }

    }


}