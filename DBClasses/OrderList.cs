namespace FurnitureSalon
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("OrderList")]
    public partial class OrderList
    {
        public int id { get; set; }

        [Column(TypeName = "date")]
        public DateTime date_selling { get; set; }

        public int furniture_id { get; set; }

        public int consumer_id { get; set; }

        public int count_product { get; set; }

        [Column(TypeName = "money")]
        public decimal? deliver_price { get; set; }

        [Column(TypeName = "money")]
        public decimal? installation_price { get; set; }

        public int? discount { get; set; }

        [Column(TypeName = "money")]
        public decimal total_sum { get; set; }

        public virtual ConsumerList ConsumerList { get; set; }

        public virtual FurnitureList FurnitureList { get; set; }
    }
}
