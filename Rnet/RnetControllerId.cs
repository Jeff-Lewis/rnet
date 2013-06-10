﻿namespace Rnet
{

    /// <summary>
    /// Specifies an RNET ControllerID component of a <see cref="RnetDeviceId"/>.
    /// </summary>
    public struct RnetControllerId
    {

        public static readonly RnetControllerId AllDevices = 0x7f;

        /// <summary>
        /// Implicitly converts a <see cref="RnetControllerId"/> to a <see cref="Byte"/>.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static implicit operator byte(RnetControllerId id)
        {
            return id.Value;
        }

        /// <summary>
        /// Implicitly converts a <see cref="Byte"/> to a <see cref="RnetControllerId"/>.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static implicit operator RnetControllerId(byte value)
        {
            return new RnetControllerId(value);
        }

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="value"></param>
        public RnetControllerId(byte value)
            : this()
        {
            Value = value;
        }

        /// <summary>
        /// Gets or sets the underlying value of the controller ID.
        /// </summary>
        public byte Value { get; set; }

        public override string ToString()
        {
            return Value.ToString();
        }

    }

}