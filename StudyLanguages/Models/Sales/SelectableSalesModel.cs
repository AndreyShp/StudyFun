using System;
using System.Collections.Generic;
using BusinessLogic.ExternalData.Sales;
using BusinessLogic.SalesGenerator;

namespace StudyLanguages.Models.Sales {
    public class SelectableSalesModel {
        public SelectableSalesModel(string tableHeader,
                                    SalesCalculator salesCalculator,
                                    string uniqueDownloadId) {
            TableHeader = tableHeader;
            AllItems = salesCalculator.AllItems;
            SelectedItems = salesCalculator.SelectedItems;
            HasDiscount = salesCalculator.HasDiscount;
            Discount = salesCalculator.Discount;
            SummPrice = salesCalculator.SummPrice;
            SummDiscountPrice = salesCalculator.SummDiscountPrice;
            SelectedDiscountPrice = salesCalculator.SelectedDiscountPrice;
            UniqueDownloadId = uniqueDownloadId;
        }

        public string UniqueDownloadId { get; private set; }

        public string TableHeader { get; private set; }

        public List<SalesItemForUser> AllItems { get; private set; }
        public List<SalesItemForUser> SelectedItems { get; private set; }

        public decimal SummPrice { get; private set; }
        public decimal SummDiscountPrice { get; private set; }
        public decimal SelectedDiscountPrice { get; private set; }

        public bool HasDiscount { get; private set; }
        public decimal Discount { get; private set; }

        public List<BreadcrumbItem> BreadcrumbsItems { get; set; }

        public string SelectedName { get; set; }

        public bool IsAllChecked {
            get { return string.IsNullOrEmpty(SelectedName); }
        }

        public bool IsSelected(string name) {
            return IsAllChecked || name.Equals(SelectedName, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}