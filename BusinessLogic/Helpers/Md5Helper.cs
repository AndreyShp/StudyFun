using System.Security.Cryptography;
using System.Text;

namespace BusinessLogic.Helpers {
    public class Md5Helper {
        private readonly Encoding _encoding;

        private readonly MD5 _md5 = MD5.Create();

        public Md5Helper() {
            _encoding = Encoding.UTF8;
        }

        public Md5Helper(Encoding encoding) {
            _encoding = encoding;
        }

        public string GetHash(string strData) {
            return GetHash(strData, 1);
        }

        public string GetHash(string strData, int count) {
            byte[] data = _encoding.GetBytes(strData);
            int tryNumber = 0;
            do {
                data = _md5.ComputeHash(data);
            } while (++tryNumber < count);
            var sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++) {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }
    }
}