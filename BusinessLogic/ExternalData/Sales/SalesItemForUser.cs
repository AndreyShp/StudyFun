namespace BusinessLogic.ExternalData.Sales {
    public class SalesItemForUser {
        public long Id { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }

        public bool IsFree {
            get { return Price == 0m; }
        }
    }
}