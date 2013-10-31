using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using FolderWatcher.Common;
using System.Threading;

namespace FolderWatcher.Tests.Common
{
    [TestClass]
    public class FolderWatcherTest
    {
        [TestMethod]
        public void FolderWatcher_NewFile()
        {
            string dirName = "FolderWatcher_NewFile";
            if (Directory.Exists(dirName))
                Directory.Delete(dirName, true);

            Directory.CreateDirectory(dirName);

            int newFiles = 0;

            var fw = new FolderWatcher.Common.FolderWatcher();
            fw.FolderToWatch = dirName;
            fw.NewFile += (s, e) => newFiles++;
            fw.InitializeFileList();
            fw.RunAsync();
            File.WriteAllText(Path.Combine(dirName, "testfile"), "asdf");
            Thread.Sleep(1000);
            fw.End();
            fw.Join();

            Directory.Delete(dirName, true);
            Assert.AreEqual(1, newFiles);
        }

        [TestMethod]
        public void FolderWatcher_NewFileWithPattern()
        {
            string dirName = "FolderWatcher_NewFileWithPattern";
            if (Directory.Exists(dirName))
                Directory.Delete(dirName, true);

            Directory.CreateDirectory(dirName);

            int newFiles = 0;

            var fw = new FolderWatcher.Common.FolderWatcher();
            fw.FileFilter = "*.jrq";
            fw.FolderToWatch = dirName;
            fw.NewFile += (s, e) => newFiles++;
            fw.InitializeFileList();
            fw.RunAsync();
            File.WriteAllText(Path.Combine(dirName, "testfile.jrq"), "asdf");
            File.WriteAllText(Path.Combine(dirName, "testfile.txt"), "asdf");
            Thread.Sleep(1000);
            fw.End();
            fw.Join();

            Directory.Delete(dirName, true);
            Assert.AreEqual(1, newFiles);
        }
    }

}
