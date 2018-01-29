using LuckIndia.Models;
using LuckIndia.Models.DTO;
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
            if (dto != null)
            {
                var model = new AccountType
                {
                    TypeName = dto.TypeName,
                    DateCreated = dto.DateCreated,
                    DateModified = dto.DateModified

                };
                return model;
            }
            return null;
        }

        public override AccountTypeDto ToDTO(AccountType model)
        {
            if (model != null)
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
            return null;
        }
    }
}