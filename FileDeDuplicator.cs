using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace HtmlExporterPlugin
{
    class FileDeDuplicator
    {
        private Dictionary<string, string> DuplicateDictionary = new Dictionary<string, string>();

        private static string CalculateMD5(string filename)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = new BufferedStream(File.OpenRead(filename), 1048576))
                {
                    var hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
        }

        public void clear()
        {
            DuplicateDictionary.Clear();
        }

        public string GetUniqueFile(string fullfilename, string storedfilename, bool DoCheck)
        {
            if (!DoCheck || !File.Exists(fullfilename))
            {
                return storedfilename;
            }

            string md5 = CalculateMD5(fullfilename);
            if (DuplicateDictionary.ContainsKey(md5))
            {
                return DuplicateDictionary[md5];
            }
            else
            {
                DuplicateDictionary[md5] = storedfilename;
                return storedfilename;
            }
        }
    }
}
