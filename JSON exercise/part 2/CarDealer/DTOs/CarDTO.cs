﻿using CarDealer.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarDealer.DTOs
{
    public class CarDTO
    {
        public string Make { get; set; } = null!;

        public string Model { get; set; } = null!;

        public long TraveledDistance { get; set; }

        [JsonProperty("partsId")]
        public ICollection<int> PartsCars { get; set; } = new HashSet<int>();
    }
}
