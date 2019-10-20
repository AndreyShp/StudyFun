using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Sandbox.Classes.Video {
    public class VideoLinksHelper {
        private readonly string _fullFileName;

        public VideoLinksHelper(string fullFileName) {
            _fullFileName = fullFileName;
        }

        public List<Tuple<string, List<string>>> Analyze() {
            var linksByDomains = new Dictionary<string, List<string>>();

            string[] lines = File.ReadAllLines(_fullFileName);
            int rowNum = 0;
            foreach (string line in lines) {
                rowNum++;
                Uri uri;
                if (!Uri.TryCreate(line, UriKind.Absolute, out uri)) {
                    Console.WriteLine("Не удалось преобразовать в абсолютный путь ссылку {0} из строки {1}", line,
                                      rowNum);
                    continue;
                }

                string domain = uri.Host;
                List<string> links;
                if (!linksByDomains.TryGetValue(domain, out links)) {
                    links = new List<string>();
                    linksByDomains.Add(domain, links);
                }
                links.Add(line);
            }

            List<Tuple<string, List<string>>> result = linksByDomains
                .OrderByDescending(e => e.Value.Count)
                .Select(e => new Tuple<string, List<string>>(e.Key, e.Value))
                .ToList();
            return result;
        }
    }
}