﻿using System;
using System.IO;

namespace Rnet
{

    /// <summary>
    /// Provides the core RNET implementation.
    /// </summary>
    public abstract class RnetConnection : IDisposable
    {

        RnetWriter writer;
        RnetReader reader;

        /// <summary>
        /// Initializes a new connection that communicates with the given <see cref="Stream"/>.
        /// </summary>
        /// <param name="stream"></param>
        protected RnetConnection()
        {

        }

        /// <summary>
        /// Stream providing access to RNet.
        /// </summary>
        internal abstract Stream Stream { get; }

        /// <summary>
        /// Opens the connection to RNet.
        /// </summary>
        public abstract void Open();

        /// <summary>
        /// Gets whether or not the RNet connection is open.
        /// </summary>
        public abstract bool IsOpen { get; }

        /// <summary>
        /// Closes the current connection.
        /// </summary>
        public virtual void Close()
        {
            Dispose();
        }

        /// <summary>
        /// Disposes of the current connection.
        /// </summary>
        public virtual void Dispose()
        {
            writer = null;
            reader = null;
        }

        /// <summary>
        /// Checks whether the connection has been opened.
        /// </summary>
        void CheckOpen()
        {
            if (!IsOpen)
                throw new InvalidOperationException("RnetConnection is not open.");
        }

        /// <summary>
        /// Gets the message writer for this connection.
        /// </summary>
        public RnetWriter Writer
        {
            get { return writer ?? (writer = new RnetWriter(Stream)); }
        }

        /// <summary>
        /// Gets the message reader for this connection.
        /// </summary>
        public RnetReader Reader
        {
            get { return reader ?? (reader = new RnetReader(Stream)); }
        }

        /// <summary>
        /// Sends the specified message to the connected RNet device.
        /// </summary>
        /// <param name="message"></param>
        public void Send(RnetMessage message)
        {
            CheckOpen();
            message.Write(Writer);
        }

        /// <summary>
        /// Invoked when a message is received.
        /// </summary>
        public event EventHandler<RnetMessageReceivedEventArgs> MessageReceived;

        /// <summary>
        /// Invoked when a message is received.
        /// </summary>
        /// <param name="args"></param>
        protected void OnMessageReceived(RnetMessageReceivedEventArgs args)
        {
            if (MessageReceived != null)
                MessageReceived(this, args);
        }

    }

}
