using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Patcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Network.ftpDownloader ftpDownloader;
        Queue<PatchParser.Gravity.Patch> downloadPatchQueue;
        ConfigParser.Config.patch_list patchData;
        PatchManager patchManager;
        VersionParser versionParser;
        ConfigParser configParser;
        public MainWindow()
        {
            InitializeComponent();
            versionParser = new VersionParser(AppDomain.CurrentDomain.BaseDirectory + @"\version.txt");
            configParser = new ConfigParser(AppDomain.CurrentDomain.BaseDirectory + @"\config.yaml");
            ftpDownloader = new Network.ftpDownloader();
            patchManager = new PatchManager();
            ftpDownloader.ftpClient.DownloadFileCompleted += FtpClient_DownloadFileCompleted;
            ftpDownloader.ftpClient.DownloadProgressChanged += FtpClient_DownloadProgressChanged;
        }

        private void FtpClient_DownloadProgressChanged(object sender, System.Net.DownloadProgressChangedEventArgs e)
        {
            double percent = ((double)e.BytesReceived / (double)ftpDownloader.currentFileSize) * 100;
            labelDownloadProgress.Content = string.Format("{0}/{1} bytes ({2}%)", e.BytesReceived, ftpDownloader.currentFileSize, (int)percent);
            progressDownload.Value = percent;
        }

        private void FtpClient_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            /*---- Step 5 Apply patch ---- */
            patchManager.doPatchGrf(AppDomain.CurrentDomain.BaseDirectory,
                AppDomain.CurrentDomain.BaseDirectory + ftpDownloader.currentFileName,
                AppDomain.CurrentDomain.BaseDirectory + "data.grf");

            /*---- Step 6 Update local version ---- */
            versionParser.writeVersion(downloadPatchQueue.Last().patchNumber);

            /*---- Step n Download next patch ---- */
            if (downloadPatchQueue.Any())
            {
                var nextQueue = downloadPatchQueue.Dequeue();
                downloadFile(patchData, nextQueue);
            }
        }

        private void downloadFile(ConfigParser.Config.patch_list patchData, PatchParser.Gravity.Patch patch)
        {
            ftpDownloader.download(patchData.patchFTPHost + patch.patchFileName, AppDomain.CurrentDomain.BaseDirectory + patch.patchFileName, "anonymous", "anonymous@domain.com");
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            /* ---- Step 1 Read config file ---- */
            var config = configParser.readConfig();

            /*---- Step 2 Get patch data ---- */
            patchData = config.patchList.First(patch => string.Equals(config.patchType, patch.patchType));

            var patchParser = new PatchParser(patchData.patchListURL);
            if (patchParser.loadPatchData())
            {
                /*---- Step 3 Check current version ---- */
                uint currentVersion = versionParser.readVersion();
                if (patchParser.gravity.patchList.LastOrDefault().patchNumber > versionParser.readVersion()) // New patch found
                {
                    /*---- Step 4 Download patch ---- */
                    downloadPatchQueue = new Queue<PatchParser.Gravity.Patch>(patchParser.gravity.patchList);
                    foreach (var patch in downloadPatchQueue.ToList())
                    {
                        if (patch.patchNumber < currentVersion) downloadPatchQueue.Dequeue();
                        else break;
                    }
                    downloadFile(patchData, downloadPatchQueue.Dequeue());
                }
            }
        }
    }
}
