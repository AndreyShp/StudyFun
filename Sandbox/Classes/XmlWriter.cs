using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace Sandbox.Classes {
    public class XmlWriter {
        private readonly List<XElement> _elements = new List<XElement>();

        public void Write(XElement element) {
            _elements.Add(element);
        }

        public bool Save(string fileName) {
            try {
                var document = new XDocument();
                document.Add(new XElement("data", _elements));

                string path = Path.GetDirectoryName(fileName);
                if (!Directory.Exists(path)) {
                    Directory.CreateDirectory(path);
                }

                document.Save(fileName, SaveOptions.None);
                return true;
            } catch (Exception e) {
                return false;
            }
        }
    }
}