namespace FurnitureSalon
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("FurnitureList")]
    public partial class FurnitureList
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public FurnitureList()
        {
            OrderList = new HashSet<OrderList>();
        }

        public int id { get; set; }

        public int furniture_type_id { get; set; }

        public int furniture_name_id { get; set; }

        [Column(TypeName = "money")]
        public decimal price { get; set; }

        public byte[] photo { get; set; }

        public virtual FurnitureName FurnitureName { get; set; }

        public virtual FurnitureType FurnitureType { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderList> OrderList { get; set; }
    }
}
