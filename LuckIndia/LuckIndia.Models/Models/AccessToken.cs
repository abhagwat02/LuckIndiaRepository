
using LuckIndia.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;

namespace LuckIndia.Models
{
    //[NonDeletable]
    public class AccessToken : Model, IExpirable
    {
        public AccessToken()
        {
            this.StartDate = DateTime.UtcNow;
        }

        /// <summary>
        /// Get/Set Id property.
        /// </summary>
       
        public override int Id { get; set; }

        /// <summary>
        /// Get/Set ApplicationTokenId property.
        /// </summary>
        //[NonPatchable]
        [Index("IX_UniqueAccessTokenForApplicationUser", 1, IsUnique = true)]
        public int ApplicationId { get; set; }

        /// <summary>
        /// Get/Set UserTokenId property.
        /// </summary>
        //[NonPatchable]
        [Index("IX_UniqueAccessTokenForApplicationUser", 2, IsUnique = true)]
        public int UserId { get; set; }

        /// <summary>
        /// Get/Set Token property.
        /// </summary>
        //[NonPatchable]
        public string Token
        {
            get
            {
                return this.GetPropertyValue<string>();
            }

            set
            {
                //new StringValidationService(value)
                //.ValidateNotNullOrWhiteSpace()
                //.ValidateMaxLength(500);

                this.SetPropertyValue(value);
            }
        }

        private DateTime _startDate;

        //[NonPatchable]
        public DateTime StartDate
        {
            get { return DateTime.SpecifyKind(_startDate, DateTimeKind.Utc); }
            private set { _startDate = DateTime.SpecifyKind(value, DateTimeKind.Utc); }
        }

        private DateTime? _endDate;
        public DateTime? EndDate
        {
            get { return _endDate.HasValue ? DateTime.SpecifyKind(_endDate.Value, DateTimeKind.Utc) : _endDate; }
            set
            {
                if (value != null)
                {
                    var newDate = DateTime.SpecifyKind(value.Value, DateTimeKind.Utc);

                    if (value < this.StartDate)
                    {
                        throw new InvalidDataException("EndDate can't be before StartDate.");
                    }

                    _endDate = newDate;
                }
                else
                {
                    _endDate = null;
                }
            }
        }
        /// <summary>
        /// Get/Set ApplicationToken property.
        /// </summary>
       // public virtual Application Application { get; set; }

        /// <summary>
        /// Get/Set UserToken property.
        /// </summary>
        public virtual User User { get; set; }

        /// <summary>
        /// Get/Set ModelEvent property.
        /// </summary>
       // public virtual ICollection<ModelEvent> ModelEvents { get; set; }
    }
}
