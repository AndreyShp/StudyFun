using System;
using System.Web.Script.Serialization;

namespace Sandbox.Classes.Video.Getters.Data {
    public class YoutubeVideoData : IVideoData {
        public string ThumnailUrl { get; set; }

        public string Keywords { get; set; }

        public string Status { get; set; }

        public long ViewCount { get; set; }

        public string Author { get; set; }

        public string Cid { get; set; }

        public string Oid { get; set; }

        public string Of { get; set; }

        public long LengthSeconds { get; set; }

        #region IVideoData Members

        [ScriptIgnore]
        public string Vid { get; set; }

        [ScriptIgnore]
        public string HtmlCode { get; set; }

        [ScriptIgnore]
        public string Title { get; set; }

        [ScriptIgnore]
        public byte[] ThumnailImage { get; set; }

        [ScriptIgnore]
        public int? Rating {
            get { return ViewCount > 0 ? (int?) ViewCount : null; }
        }

        #endregion
    }
}