using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVCFitnessApp.Models
{
    public class FilePath
    {
        //Primary key
        public int FilePathID { get; set; }  
        [StringLength(255)]
        public string FileName { get; set; }
        public string FileType { get; set; }
        //Foreign key
        public int CoachID { get; set; }  

        //Navigation property. A filepath entity is associated with one coach entity
        public virtual Coach coach { get; set; }  
    }
}