using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Linq;

namespace Sandbox.Classes.Translators {
    public class XmlTranslationReader {
        public List<Item> Read(string fileName) {
            var reader = new StreamReader(fileName, Encoding.UTF8);
            XDocument xDocument = XDocument.Load(reader);

            var result = new List<Item>();
            IEnumerable<XElement> items = xDocument.Root.Elements("item");
            foreach (XElement xElement in items) {
                var item = new Item {
                    Source = xElement.Element("source").Value,
                    Translation = xElement.Element("translation").Value,
                };

                IEnumerable<XElement> bests = xElement.Elements("best");
                foreach (XElement best in bests) {
                    item.Best.Add(best.Value);
                }

                IEnumerable<XElement> others = xElement.Elements("other");
                foreach (XElement other in others) {
                    item.Other.Add(other.Value);
                }

                result.Add(item);
            }
            return result;
        }

        #region Nested type: Item

        public class Item {
            public Item() {
                Best = new List<string>();
                Other = new List<string>();
            }

            public string Source { get; set; }
            public string Translation { get; set; }

            public List<string> Best { get; set; }
            public List<string> Other { get; set; }
        }

        #endregion
    }
}