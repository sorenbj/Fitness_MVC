using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVCFitnessApp.Models
{
    public class FilePathMember
    {
        //Primary key
        public int FilePathMemberID { get; set; }
        [StringLength(255)]
        public string FileName { get; set; }
        public string FileType { get; set; }
        //Foreign key
        public int MemberID { get; set; }

        //Navigation property. A filepath entity is associated with one member entity
        public virtual Member member { get; set; }
    }
}