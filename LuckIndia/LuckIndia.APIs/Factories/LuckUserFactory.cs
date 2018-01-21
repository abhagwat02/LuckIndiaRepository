using LuckIndia.APIs.DTO;
using LuckIndia.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LuckIndia.APIs.Factories
{
    public class LuckUserFactory : Factory<LuckUser,UserDto>
    {
        public override LuckUser FromDTO(UserDto dto)
        {
            if (dto != null)
            {
                var model = new LuckUser
                {
                    Address = dto.Address,
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    MiddleName = dto.MiddleName,
                    PhoeNumber = dto.PhoeNumber,
                    DateModified = dto.DateModified,
                    DateCreated = dto.DateCreated,
                    accounts = dto.accounts.Select(x => new AccountFactory().FromDTO(x)).ToList()

                };
                return model;
            }

            return null;
        }

        public override UserDto ToDTO(LuckUser model)
        {
            if (model == null)
            {
                return null;
            }

            var dto = new UserDto
            {
                Id = model.Id,
                Address = model.Address,
                LastName = model.LastName,
                FirstName = model.FirstName,
                MiddleName = model.MiddleName,
                PhoeNumber = model.PhoeNumber,
                DateCreated = model.DateCreated,
                DateModified = model.DateModified,
                accounts = model.accounts.Select(x => new AccountFactory().ToDTO(x)).ToList()
            };
           return dto;
        }
    }
}
