using System;
using System.Diagnostics;
using System.IO;
using GRF.Core;
using GRF.FileFormats.RgzFormat;
using GRF.FileFormats.ThorFormat;
using GRF.IO;
using Utilities.Extension;

namespace Patcher
{
    class PatchManager
    {
        public static void doPatchGrf(string roDirectory, string grfPatch, string grfTarget)
        {
            var patcherExeName = Path.GetFileName(Process.GetCurrentProcess().MainModule.FileName);
            if (grfPatch.IsExtension(".grf", ".gpf"))
            {
                using (var output = new GrfHolder(grfTarget, GrfLoadOptions.OpenOrNew))
                {
                    using (var grf = new GrfHolder(grfPatch))
                    {
                        //output.Merge(grf);
                        //output.Patch(grf, "test.grf");
                        if (!output.IsBusy)
                        {
                            output.QuickMerge(grf);
                            //output.QuickSave();
                        }
                    }
                }

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
    }
}
