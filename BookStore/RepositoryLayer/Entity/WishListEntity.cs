using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace RepositoryLayer.Entity
{
    public class WishListEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int WishListId{ get; set; }

        [ForeignKey("UserEntity")]
        public int AddedBy { get; set; }  // FK to User

        [ForeignKey("BookEntity")]
        public int BookId { get; set; }  // FK to Book


        // Navigation properties
        public virtual UserEntity UserEntity { get; set; }
        public virtual BookEntity BookEntity { get; set; }


    }
}
