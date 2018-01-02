﻿using LuckIndia.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LuckIndia.APIs.DTO
{
    public class UserDto : IModelDTO
    {
        public string Url { get; set; }
        public int? Id { get; set; }
        public String FirstName { get; set; }
        public String MiddleName { get; set; }
        public String LastName { get; set; }
        public long PhoeNumber { get; set; }
        public String Address { get; set; }
        public IEnumerable<AccountDto> accounts { get; set; }

        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }


    }
}