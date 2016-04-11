using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using System.Text.RegularExpressions;

namespace Patcher
{
    public class PatchParser
    {
        private Gravity _gravity;
        public Gravity gravity
        {
            get { return _gravity; }
            set { _gravity = value; }
        }

        private string _gravityPatchFileListURL;

        public PatchParser(string gravityPatchFileListURL)
        {
            _gravityPatchFileListURL = gravityPatchFileListURL;
            _gravity = new Gravity();
        }

        public PatchParser(string gravityPatchFileListURL, string junoYamlFileURL)
        {
            _gravityPatchFileListURL = gravityPatchFileListURL;
            _gravity = new Gravity();
        }

        public bool loadPatchData()
        {
            var task = Network.MakeAsyncRequest(_gravityPatchFileListURL, "text/html");
            string input = task.Result;

            string pattern = @"(([0-9]+)[\t ]+(.+)?$|(\/\/)([0-9]+)[\t ]+(.+)?$)";

            MatchCollection matches = Regex.Matches(input, pattern, RegexOptions.Multiline);

            _gravity.patchList.Clear();
            _gravity.patchListIgnore.Clear();

            foreach (Match match in matches)
            {
                try
                {
                    if (match.Groups[4].Value == @"//")
                    {
                        _gravity.patchListIgnore.Add(new Gravity.Patch
                        {
                            patchNumber = int.Parse(match.Groups[5].Value),
                            patchFileName = match.Groups[6].Value.Trim(),
                        });
                    }
                    else
                    {
                        _gravity.patchList.Add(new Gravity.Patch
                        {
                            patchNumber = int.Parse(match.Groups[2].Value),
                            patchFileName = match.Groups[3].Value.Trim(),
                        });
                    }
                }
                catch { return false; }
            }
            return true;
        }
        public class Gravity
        {
            public List<Patch> patchList, patchListIgnore;

            public Gravity()
            {
                patchList = new List<Patch>();
                patchListIgnore = new List<Patch>();
            }

            public class Patch
            {
                private int _patchNumber;
                public int patchNumber
                {
                    get { return _patchNumber; }
                    set { _patchNumber = value; }
                }

                private string _patchFileName;
                public string patchFileName
                {
                    get { return _patchFileName; }
                    set { _patchFileName = value; }
                }
            }
        }
    }
}
