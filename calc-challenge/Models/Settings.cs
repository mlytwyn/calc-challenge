﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace calc_challenge.Models
{
    public class Settings
    {
        public List<string> Delimiters = [];
        public int MaxDigits = 0;
        public bool AllowNegativeDigits = false;
        public int MaxNumberSize = 1000;
    }
}
