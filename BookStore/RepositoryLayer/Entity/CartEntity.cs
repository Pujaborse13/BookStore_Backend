using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace RepositoryLayer.Entity
{
    public class CartEntity
    {
   
            [Key]
            [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
            public int CartId { get; set; }

            [ForeignKey("Users")]
            public int CustomerId { get; set; }  // FK to User

            [ForeignKey("Book")]
            public int BookId { get; set; }  // FK to Book

            public int Quantity { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public Decimal SinglUnitPrice { get; set; }

            public bool IsPurchased { get; set; } = false;

            // Navigation properties
            public virtual UserEntity UserEntity { get; set; }
            public virtual BookEntity BookEntity { get; set; }
    }

}