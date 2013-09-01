﻿using System.Collections.Generic;
using System.Xml.Serialization;

namespace Rnet.Profiles
{

    /// <summary>
    /// Base interface for operating against a <see cref="RnetZone"/>.
    /// </summary>
    [ProfileContract("Zone")]
    [XmlRoot(Namespace = "urn:rnet:profiles::Zone", ElementName = "Zone")]
    public interface IZone : IObject
    {

        /// <summary>
        /// Returns the set of physical devices underneath the zone.
        /// </summary>
        IEnumerable<RnetDevice> Devices { get; }

    }

}
