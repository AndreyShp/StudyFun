using System.Collections.Generic;
using System.Linq;
using BusinessLogic.ExternalData.Sales;

namespace BusinessLogic.SalesGenerator {
    public class SalesCalculator {
        public SalesCalculator(List<SalesItemForUser> allItems, List<SalesItemForUser> selectedItems, decimal discount) {
            AllItems = allItems ?? new List<SalesItemForUser>(0);
            SelectedItems = selectedItems ?? new List<SalesItemForUser>(0);
            HasDiscount = discount > 0;
            Discount = discount * 100;
            SummPrice = AllItems.Select(e => e.Price).Sum();
            SummDiscountPrice = SummPrice;
            SelectedDiscountPrice = SelectedItems.Select(e => e.Price).Sum();
            if (HasDiscount) {
                var partDiscount = 1 - discount;
                SelectedDiscountPrice = SelectedItems.Count == AllItems.Count ? partDiscount * SelectedDiscountPrice : SelectedDiscountPrice;
                SummDiscountPrice *= partDiscount;
            }
        }

        public List<SalesItemForUser> AllItems { get; private set; }
        public List<SalesItemForUser> SelectedItems { get; private set; }

        public decimal SummPrice { get; private set; }
        public decimal SummDiscountPrice { get; private set; }
        public decimal SelectedDiscountPrice { get; private set; }

        public bool HasDiscount { get; private set; }
        public decimal Discount { get; private set; }
    }
}