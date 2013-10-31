using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FolderWatcher.Common
{
    public class EventArgsT<T> : EventArgs
    {
        public T Param { get; set; }

        public EventArgsT(T p) : base() { Param = p; }
    }
    
    public class FolderWatcher
    {
        public event EventHandler<EventArgsT<string>> NewFile;

        public string FolderToWatch { get; set; }
        public string FileFilter { get; set; }
        public bool SyncEventThreadContext { get; set; }
        public bool IgnoreStartingFiles { get; set; }

        protected BackgroundWorker _worker = null;
        protected SynchronizationContext _syncContext = null;
        protected bool _shouldRun = true;
        protected bool _hasRunOnce = false;

        private Semaphore _workerFinsished = new Semaphore(1, 1);

        public FolderWatcher()
        {
            SyncEventThreadContext = true;
            IgnoreStartingFiles = false;
        }

        public void RunAsync()
        {
            _shouldRun = true;
            _hasRunOnce = false;
            _syncContext = SyncEventThreadContext ? SynchronizationContext.Current : null;
            _workerFinsished.WaitOne();

            if (_worker == null)
            {
                _worker = new BackgroundWorker();
                _worker.DoWork += _worker_DoWork;
            }
            _worker.RunWorkerAsync();
        }

        public void End()
        {
            _shouldRun = false;
        }

        public void Join()
        {
            _workerFinsished.WaitOne();
            _workerFinsished.Release();
        }

        protected HashSet<string> _existingFiles;

        protected FileInfo[] GetFilesInWatchFolder()
        {
            return new DirectoryInfo(FolderToWatch).GetFiles(FileFilter ?? "*", SearchOption.TopDirectoryOnly);
        }

        public void InitializeFileList()
        {
            _existingFiles = new HashSet<string>();
            if (IgnoreStartingFiles)
                foreach (var file in GetFilesInWatchFolder())
                    _existingFiles.Add(file.FullName);
        }

        public void ScanForNewFiles()
        {
            foreach (var file in GetFilesInWatchFolder())
            {
                if (_existingFiles.Contains(file.FullName))
                    continue;

                _existingFiles.Add(file.FullName);
                OnNewFile(file.FullName);
            }
        }

        void _worker_DoWork(object sender, DoWorkEventArgs e)
        {
            // Run while should run is true and run at least once
            while (_shouldRun || !_hasRunOnce)
            {
                _hasRunOnce = true;

                // First run
                if (_existingFiles == null)
                {
                    InitializeFileList();
                }

                // Check for new files
                ScanForNewFiles();

                Thread.Sleep(500);
            }
            _workerFinsished.Release();
        }

        public void OnNewFile(string filename)
        {
            RunInContext(e =>
            {
                if (NewFile != null)
                    NewFile(this, new EventArgsT<string>(filename));
            }, null);
        }

        protected void RunInContext(SendOrPostCallback d, object state)
        {
            if (_syncContext == null)
                d(state);
            else
                _syncContext.Post(d, state);
        }


    }
}
