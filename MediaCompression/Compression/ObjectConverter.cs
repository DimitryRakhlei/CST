using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Compression {
    internal static class ObjectConverter {
        /// <summary>
        ///     Takes an object and turns it into a byte array.
        /// </summary>
        /// <param name="obj">Object to be turned into bytes</param>
        /// <returns></returns>
        public static byte[] ObjectToByteArray(object obj) {
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream()) {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        /// <summary>
        ///     Retrieves an object from bytes
        /// </summary>
        /// <param name="arrBytes">data to be converted into an object</param>
        /// <returns></returns>
        public static object ByteArrayToObject(byte[] arrBytes) {
            using (MemoryStream memStream = new MemoryStream()) {
                BinaryFormatter binForm = new BinaryFormatter();
                memStream.Write(arrBytes, 0, arrBytes.Length);
                memStream.Seek(0, SeekOrigin.Begin);
                object obj = binForm.Deserialize(memStream);
                return obj;
            }
        }
    }
}