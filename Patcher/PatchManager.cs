using System;
using System.Diagnostics;
using System.IO;
using GRF.Core;
using GRF.FileFormats.RgzFormat;
using GRF.FileFormats.ThorFormat;
using GRF.IO;
using Utilities.Extension;
using System.Threading;

namespace Patcher
{
    class PatchManager
    {
        GrfHolder _grfSource = new GrfHolder();
        GrfHolder _grfAdd = new GrfHolder();

        public void doPatchGrf(string roDirectory, string grfPatch, string grfTarget)
        {
            var patcherExeName = Path.GetFileName(Process.GetCurrentProcess().MainModule.FileName);
            if (grfPatch.IsExtension(".grf", ".gpf"))
            {
                if (!_grfSource.IsOpened) _grfSource.Open(grfTarget);
                else _grfSource.Reload();

                _grfAdd.Open(grfPatch);

                _grfSource.QuickMerge(_grfAdd, SyncMode.Synchronous);
                _grfSource.Commands.ClearCommands();
                _grfAdd.Close();
            }
            else if (grfPatch.IsExtension(".rgz"))
            {
                using (var rgz = new Rgz(grfPatch))
                {
                    if (rgz.Table.ContainsFile(patcherExeName))
                    {
                        // Updating on this executable, do something...!
                        throw new NotImplementedException();
                    }
                    foreach (var entry in rgz.Table)
                    {
                        entry.ExtractFromRelative(roDirectory);
                    }
                }
            }
            else throw new InvalidDataException("Invalid extension, must be grf, gpf or rgz");
            
            GrfPath.Delete(grfPatch);
        }

        public void cancelPath()
        {
            if (_grfSource != null) { if (!_grfSource.IsClosed) { _grfSource.Cancel(); } }
                

            while (_grfSource != null && _grfSource.IsOpened && _grfSource.IsBusy)
            {
                Thread.Sleep(200);
            }
        }
    }
}
