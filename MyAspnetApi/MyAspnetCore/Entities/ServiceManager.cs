﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyAspnetCore.Entities
{
    public class ServiceManager
    {
        public Guid ServiceManagerId {  get; set; }
        public string ServiceName { get; set; }
        public int Status {  get; set; }
    }
}
