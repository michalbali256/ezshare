using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace ModelLib.GeneratedCode
{

    class PartFile
    {

        enum ePartStatus
        {
            Missing,
            Processing,
            Available
        }

        BinaryReader fileReader;
        BinaryWriter fileWriter;

        public int NumberOfParts { get; set; }

        private void openFile(string filePath)
        {

            var stream = File.Open(filePath, FileMode.OpenOrCreate);
            
            fileReader = new BinaryReader(stream);
            fileWriter = new BinaryWriter(stream);

            
        }



    }
}
