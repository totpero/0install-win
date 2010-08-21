﻿/*
 * Copyright 2006-2010 Bastian Eicher
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using System.ComponentModel;
using System.IO;
using System.Net;
using System.Threading;
using Common.Helpers;

namespace Common
{
    /// <summary>
    /// Abstract base class for background tasks that write files or directories.
    /// </summary>
    /// <remarks>Sub-classes must initalize the <see cref="Thread"/> member.</remarks>
    public abstract class IOWriteProgress : IProgress
    {
        #region Events
        /// <inheritdoc />
        public event ProgressEventHandler StateChanged;

        private void OnStateChanged()
        {
            // Copy to local variable to prevent threading issues
            ProgressEventHandler stateChanged = StateChanged;
            if (stateChanged != null) stateChanged(this);
        }

        /// <inheritdoc />
        public event ProgressEventHandler ProgressChanged;

        private void OnProgressChanged()
        {
            // Copy to local variable to prevent threading issues
            ProgressEventHandler bytesReceivedChanged = ProgressChanged;
            if (bytesReceivedChanged != null) bytesReceivedChanged(this);
        }
        #endregion

        #region Variables
        /// <summary>Synchronization handle to prevent race conditions with thread startup/shutdown or <see cref="State"/> switched.</summary>
        protected readonly object StateLock = new object();

        /// <summary>The background thread used for executing the task. Sub-classes must initalize this member.</summary>
        protected Thread Thread;
        #endregion

        #region Properties
        /// <summary>
        /// The name of the task.
        /// </summary>
        public abstract string Name { get; }

        private ProgressState _state;
        /// <inheritdoc />
        public ProgressState State
        {
            get { return _state; } protected set { UpdateHelper.Do(ref _state, value, OnStateChanged); }
        }

        public string ErrorMessage { get; protected set; }

        private long _bytesReceived;
        /// <inheritdoc />
        public long BytesProcessed
        {
            get { return _bytesReceived; } protected set { UpdateHelper.Do(ref _bytesReceived, value, OnProgressChanged); }
        }

        private long _bytesTotal;
        /// <inheritdoc />
        public long BytesTotal
        {
            get { return _bytesTotal; }
            protected set { UpdateHelper.Do(ref _bytesTotal, value, OnProgressChanged); }
        }

        /// <inheritdoc />
        public double Progress
        {
            get
            {
                switch (BytesTotal)
                {
                    case -1: return -1;
                    case 0: return 1;
                    default: return BytesProcessed / (double)BytesTotal;
                }
            }
        }
        #endregion

        //--------------------//

        #region Control
        /// <inheritdoc />
        public void Start()
        {
            lock (StateLock)
            {
                if (State != ProgressState.Ready) return;

                State = ProgressState.Started;
                Thread.Start();
            }
        }

        /// <inheritdoc />
        public abstract void Cancel();

        /// <inheritdoc />
        public void Join()
        {
            lock (StateLock)
            {
                if (Thread == null || !Thread.IsAlive) return;
            }

            Thread.Join();
        }

        /// <inheritdoc />
        public void RunSync()
        {
            Start();
            Join();

            lock (StateLock)
            {
                switch (State)
                {
                    case ProgressState.Complete:
                        return;
                    case ProgressState.WebError:
                        throw new WebException(ErrorMessage);
                    case ProgressState.IOError:
                        throw new IOException(ErrorMessage);
                    default:
                        throw new UserCancelException();
                }
            }
        }
        #endregion
    }
}
