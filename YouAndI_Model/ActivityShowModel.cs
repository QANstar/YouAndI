﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YouAndI_Entity;

namespace YouAndI_Model
{
    public class ActivityShowModel
    {
        public int id { get; set; }
        public int userid { get; set; }
        [Required]
        [StringLength(50)]
        public string title { get; set; }
        public double x { get; set; }
        public double y { get; set; }
        [Required]
        [StringLength(50)]
        public string image { get; set; }
        [Required]
        [StringLength(100)]
        public string location { get; set; }
        [Required]
        [StringLength(100)]
        public string introduction { get; set; }
        public int maxnumber { get; set; }
        public int curnumber { get; set; }
        [Column(TypeName = "smalldatetime")]
        public DateTime starttime { get; set; }
        [Column(TypeName = "smalldatetime")]
        public DateTime endtime { get; set; }
        public int typeID { get; set; }
        [Required]
        [StringLength(50)]
        public string type { get; set; }
        [Required]
        [StringLength(50)]
        public string username { get; set; }
        public bool isStar { get; set; }
        public Payment? payment { get; set; }
        public List<ActivityTag> tags { get; set; }
    }
}
