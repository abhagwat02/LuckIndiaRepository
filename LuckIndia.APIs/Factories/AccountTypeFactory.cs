﻿using LuckIndia.APIs.DTO;
using LuckIndia.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LuckIndia.APIs.Factories
{
    public class AccountTypeFactory : Factory<AccountType, AccountTypeDto>
    {
        public override AccountType FromDTO(AccountTypeDto dto)
        {
            var model =  new AccountType
            {
                TypeName = dto.TypeName,
                DateCreated = dto.DateCreated,
                DateModified = dto.DateModified
                
            };

            return model;
        }

        public override AccountTypeDto ToDTO(AccountType model)
        {
            var dto = new AccountTypeDto
            {
                Id = model.Id,
                TypeName = model.TypeName,
                DateCreated = model.DateCreated,
                DateModified = model.DateModified,
            };
            return dto;
        }
    }
}