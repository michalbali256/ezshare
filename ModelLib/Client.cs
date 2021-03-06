﻿using System;
using System.Text;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace EzShare
{
    namespace ModelLib
    {
        /// <summary>
        /// Provides wrapper over TcpClient, abstract class for UpClient and DownClient
        /// </summary>
        public abstract class Client : IDisposable
        {
            protected TcpClient SimpleClient;
            protected NetworkStream Stream;

            public delegate void ClientClosedEventHandler(Client sender);
            /// <summary>
            /// Event that is invoked when Client was closed
            /// </summary>
            public event ClientClosedEventHandler ClientClosed;

            /// <summary>
            /// Represents types of requests that can be sent
            /// </summary>
            protected enum EMessage
            {
                Part,
                Closing,
            }

            /// <summary>
            /// Represents response of part request
            /// </summary>
            public enum ERequestPartResponse
            {
                NotAvailable,
                OK,
                NeverAvailable
            }

            /// <summary>
            /// IP and port of remote end point.
            /// </summary>
            public ConnectInfo ConnectInfo { get; protected set; }
            /// <summary>
            /// Number if available bytes to read from underlying TcpClient
            /// </summary>
            public int Available { get { return SimpleClient.Available; } }

            /// <summary>
            /// Counter of bytes that were downloaded.
            /// </summary>
            public int DownloadedBytes { get; set; }
            /// <summary>
            /// Counter of bytes that were uploaded.
            /// </summary>
            public int UploadedBytes { get; set; }



            /// <summary>
            /// Asynchronously reads bytes from remote client.
            /// </summary>
            /// <param name="count">The number of bytes to expect</param>
            /// <returns></returns>
            public async Task<byte[]> ReadBytesAsync(int count)
            {
                byte[] bytes = new byte[count];
                int rd = 0;
                //reads until expected number of bytes were read - sometimes it takes more than one NetworkStream.ReadAsync to read it all.
                while (rd < count)
                {
                    int now = await Stream.ReadAsync(bytes, rd, count - rd);
                    rd += now;
                    DownloadedBytes += now;
                    if (now == 0)//if 0 bytes were read, it means connection is bad
                        throw new InvalidOperationException("Connection ended.");
                }
                return bytes;
            }
            /// <summary>
            /// Asynchronously reads 8 bytes from remote client.
            /// </summary>
            /// <returns>Long, that was read</returns>
            public async Task<long> ReadLongAsync()
            {
                return BitConverter.ToInt64(await ReadBytesAsync(8), 0);
            }
            /// <summary>
            /// Asynchronously reads 1 byte from remote client.
            /// </summary>
            /// <returns>Byte, that was read</returns>
            public async Task<byte> ReadByteAsync()
            {
                return (await ReadBytesAsync(1))[0];
            }
            /// <summary>
            /// Asynchronously reads 4 bytes from remote client.
            /// </summary>
            /// <returns>Integer, that was read</returns>
            public async Task<int> ReadIntAsync()
            {
                return BitConverter.ToInt32(await ReadBytesAsync(4), 0);
            }
            /// <summary>
            /// Reads 32 bytes long Id from remote client
            /// </summary>
            /// <returns>Task that returns 32 bytes long string representing id</returns>
            public async Task<string> ReadIdAsync()
            {
                byte[] buffer = await ReadBytesAsync(32);

                StringBuilder b = new StringBuilder();
                for (int i = 0; i < 32; ++i)
                    b.Append((char)buffer[i]);
                return b.ToString();
            }

            /// <summary>
            /// Asynchronously sends id.
            /// </summary>
            /// <param name="id">The id that should be 32 chars long</param>
            public async Task SendIdAsync(string id)
            {
                byte[] buffer = new byte[32];
                for (int i = 0; i < 32; ++i)
                    buffer[i] = (byte)id[i];
                await SendBytesAsync(buffer);
            }
            /// <summary>
            /// Asynchronously sends a byte.
            /// </summary>
            /// <param name="b">Byte to send</param>
            /// <returns></returns>
            public async Task SendByteAsync(byte b)
            {
                await SendBytesAsync(new byte[] { b });
            }
            /// <summary>
            /// Asynchronously sends a 4 bytes.
            /// </summary>
            /// <param name="integer">Integer to send</param>
            /// <returns></returns>
            public async Task SendIntAsync(int integer)
            {
                await SendBytesAsync(BitConverter.GetBytes(integer));
            }
            /// <summary>
            /// Asynchronously sends 8 bytes.
            /// </summary>
            /// <param name="integer">Long to send</param>
            /// <returns></returns>
            public async Task SendLongAsync(long integer)
            {
                await SendBytesAsync(BitConverter.GetBytes(integer));
            }
            /// <summary>
            /// Asynchronously sends bytes.
            /// </summary>
            /// <param name="buffer">Bytes to send</param>
            /// <returns></returns>
            public async Task SendBytesAsync(byte[] buffer)
            {
                await Stream.WriteAsync(buffer, 0, buffer.Length);
                UploadedBytes += buffer.Length;
            }


            /// <summary>
            /// Releases resources and closes the connection.
            /// </summary>
            public virtual void Close()
            {
                Stream.Close(100);
                OnClientClosed();
            }


            /// <summary>
            /// Invokes ClientClosed event
            /// </summary>
            protected void OnClientClosed()
            {
                ClientClosed?.Invoke(this);
            }

            public void Dispose()
            {
                ((IDisposable)SimpleClient).Dispose();
            }
        }
    }
}
