namespace FurnitureSalon
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("FurnitureName")]
    public partial class FurnitureName
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public FurnitureName()
        {
            FurnitureList = new HashSet<FurnitureList>();
        }

        public int id { get; set; }

        [Required]
        [StringLength(50)]
        public string furniture_name { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FurnitureList> FurnitureList { get; set; }
    }
}
