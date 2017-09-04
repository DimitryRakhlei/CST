using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DigiProAssignment {
    class Clipper {
        
        public void toClipboard(byte[] data) {
            Clipboard.Clear();
            Clipboard.SetAudio(data);
        }
        public static byte[] ReadFully(Stream input) {
            using (MemoryStream ms = new MemoryStream()) {
                input.CopyTo(ms);
                return ms.ToArray();
            }
        }

        public byte[] fromClipboard() {
            byte[] data = null;
            if (Clipboard.ContainsAudio()) {
                return data = ReadFully(Clipboard.GetAudioStream());
            }
            return data;
        }
    }
}
