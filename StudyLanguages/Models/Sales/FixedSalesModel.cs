using System;
using System.Collections.Generic;
using System.Web.Mvc;
using BusinessLogic.DataQuery.Money;
using BusinessLogic.Export;
using BusinessLogic.ExternalData;

namespace StudyLanguages.Models.Sales {
    public class FixedSalesModel {
        private readonly Dictionary<SectionId, List<string>> _tablesWithRows;

        public FixedSalesModel(Dictionary<SectionId, List<string>> tablesWithRows,
                               ISalesSettings salesSettings,
                               string uniqueDownloadId) {
            _tablesWithRows = tablesWithRows;
            SummDiscountPrice = salesSettings.SummDiscountPrice;
            UniqueDownloadId = uniqueDownloadId;
            _tablesWithRows = tablesWithRows;
        }

        public string UniqueDownloadId { get; private set; }

        public IEnumerable<SectionId> SectionsIds {
            get { return _tablesWithRows.Keys; }
        }

        public decimal SummDiscountPrice { get; private set; }
        public List<BreadcrumbItem> BreadcrumbsItems { get; set; }

        public Func<SectionId, string> GetHeader { get; set; }

        public Func<SectionId, string> GetTableHeader { get; set; }

        public string GetDownloadUrl(UrlHelper url, SectionId sectionId, string row) {
            const DocumentType DOCUMENT_TYPE = DocumentType.Pdf;
            switch (sectionId) {
                case SectionId.GroupByWords:
                    return url.Action("Download", RouteConfig.GROUP_WORD_CONTROLLER,
                                      new { group = row, type = DOCUMENT_TYPE });
                case SectionId.GroupByPhrases:
                    return url.Action("Download", RouteConfig.GROUP_SENTENCE_CONTROLLER,
                                      new { group = row, type = DOCUMENT_TYPE });
                case SectionId.VisualDictionary:
                    return url.Action("Download", RouteConfig.VISUAL_DICTIONARY_CONTROLLER,
                                      new { group = row, type = DOCUMENT_TYPE });
                case SectionId.FillDifference:
                    return url.Action("Download", RouteConfig.COMPARISON_CONTROLLER,
                                      new { group = row, type = DOCUMENT_TYPE });
                case SectionId.Video:
                    return url.Action("Download", RouteConfig.VIDEO_CONTROLLER,
                                      new { group = row, type = DOCUMENT_TYPE });
                case SectionId.PopularWord:
                    return url.Action("Download", RouteConfig.POPULAR_WORDS_CONTROLLER,
                                      new { type = DOCUMENT_TYPE });
            }
            return null;
        }

        public List<string> GetTableRows(SectionId sectionId) {
            return _tablesWithRows[sectionId];
        }
    }
}