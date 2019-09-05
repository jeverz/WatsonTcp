﻿using System;
using System.Net.Security;
using System.Net.Sockets;
using System.Threading;

namespace WatsonTcp
{
    public class ClientMetadata : IDisposable
    {
        #region Public-Members

        public TcpClient TcpClient
        {
            get { return _TcpClient; }
        }

        public NetworkStream NetworkStream
        {
            get { return _NetworkStream; }
        }

        public SslStream SslStream
        {
            get { return _SslStream; }
            set { _SslStream = value; }
        }

        public string IpPort
        {
            get { return _IpPort; }
        }

        public SemaphoreSlim ReadLock { get; set; }

        public SemaphoreSlim WriteLock { get; set; }

        #endregion Public-Members

        #region Private-Members

        private bool _Disposed = false;

        private TcpClient _TcpClient;
        private NetworkStream _NetworkStream;
        private SslStream _SslStream;
        private string _IpPort;

        #endregion Private-Members

        #region Constructors-and-Factories

        public ClientMetadata(TcpClient tcp)
        {
            _TcpClient = tcp ?? throw new ArgumentNullException(nameof(tcp));
            _NetworkStream = tcp.GetStream();
            _IpPort = tcp.Client.RemoteEndPoint.ToString();

            ReadLock = new SemaphoreSlim(1);
            WriteLock = new SemaphoreSlim(1);
        }

        #endregion Constructors-and-Factories

        #region Public-Methods

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion Public-Methods

        #region Private-Methods

        protected virtual void Dispose(bool disposing)
        {
            if (_Disposed)
            {
                return;
            }

            if (disposing)
            {
                if (_SslStream != null)
                {
                    _SslStream.Close();
                }

                if (_NetworkStream != null)
                {
                    _NetworkStream.Close();
                }

                if (_TcpClient != null)
                {
                    _TcpClient.Close();
                }
            }

            ReadLock.Dispose();
            WriteLock.Dispose();

            _Disposed = true;
        }

        #endregion Private-Methods
    }
}