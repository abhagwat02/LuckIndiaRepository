using LuckIndia.APIs.DTO;
using LuckIndia.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LuckIndia.APIs.Factories
{
    public class AccountFactory : Factory<Account,AccountDto>
    {
        public override Account FromDTO(AccountDto dto)
        {
            var model =  new Account
            {
                CardNumber = dto.CardNumber,
                DateModified = dto.DateModified,
                DateCreated = dto.DateCreated,
                ParentAccountID = dto.ParentAccountID,
                Password = dto.Password,
                UserName = dto.UserName,
                LuckUserID = dto.LuckUserID,
                AccountTypeID = dto.Type.Id,
               // user = new LuckUserFactory().FromDTO(dto.user),
                 Type = new AccountTypeFactory().FromDTO(dto.Type)



            };
            return model;

        }

        public override AccountDto ToDTO(Account model)
        {
            var dto =  new AccountDto
            {
                Id = model.Id,
                CardNumber = model.CardNumber,
                ParentAccountID = model.ParentAccountID,
                UserName = model.UserName,
                Password = model.Password,
                DateCreated = model.DateCreated,
                DateModified = model.DateModified,
                LuckUserID = model.LuckUserID,
                AccountTypeID = model.AccountTypeID,
                user = new LuckUserFactory().ToDTO(model.user),
                Type = new AccountTypeFactory().ToDTO(model.Type)

            };
            return dto;
        }
    }
}