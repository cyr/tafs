using System;
using System.ServiceProcess;
using System.Threading;
using Dokan;
using tafs.FileSystem;

namespace tafs.Service
{
    public partial class Service : ServiceBase
    {
        private Thread _dokanThread;
        const char DRIVE_LETTER = 'M';

        public Service()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            Start();
        }

        public void Start()
        {
            StartDokanThread();
        }

        private void StartDokanThread()
        {
            _dokanThread = new Thread(DokanStart) { IsBackground = true };
            _dokanThread.Start();
        }

        private void DokanStart()
        {
            int status = MountDokanDevice();

            Console.Write(status);
        }

        private static int MountDokanDevice()
        {
            var dokanOperations = new TafsDokanOperations(@"D:\");
            var dokanOptions = new DokanOptions { DebugMode = true, MountPoint = GetMountPoint(), ThreadCount = 5 };

            return DokanNet.DokanMain(dokanOptions, dokanOperations);
        }

        private static string GetMountPoint()
        {
            return DRIVE_LETTER + @":\";
        }

        protected override void OnStop()
        {
            DokanNet.DokanUnmount(DRIVE_LETTER);
        }
    }
}
