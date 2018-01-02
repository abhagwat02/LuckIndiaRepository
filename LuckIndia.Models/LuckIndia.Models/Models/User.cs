using System;
using System.Collections.Generic;

namespace LuckIndia.Models
{

    //[NonDeletable]
    public class User : Model
    {
        /// <summary>
        /// User 
        /// </summary>
        public User()
        {
            this.UniqueId = Guid.NewGuid();
            //this.PasswordSalt = HashingService.Base64EncodeString(Guid.NewGuid().ToString());
            this.IsActive = true;
            this.StartDate = DateTime.UtcNow;
        }

        /// <summary>
        /// Get/Set Id property.
        /// </summary>
        public override int Id { get; set; }

        /// <summary>
        /// Get/Set UniqueId property.
        /// </summary>
        //[NonPatchable]
        public Guid UniqueId { get; private set; }

        /// <summary>
        /// Get/Set FirstName property.
        /// </summary>
        public string FirstName
        {
            get
            {
                return this.GetPropertyValue<string>();
            }
            set
            {
                //new StringValidationService(value)
                //.ValidateNotNullOrWhiteSpace()
                //.ValidateMaxLength(50);

                this.SetPropertyValue(value);
            }
        }

        /// <summary>
        /// Get/Set LastName property.
        /// </summary>
        public string LastName
        {
            get
            {
                return this.GetPropertyValue<string>();
            }
            set
            {
                //new StringValidationService(value)
                //.ValidateNotNullOrWhiteSpace()
                //.ValidateMaxLength(50);

                this.SetPropertyValue(value);
            }
        }

        /// <summary>
        /// Get/Set Email property.
        /// </summary>
        public string Email
        {
            get
            {
                return this.GetPropertyValue<string>();
            }
            set
            {
                //new StringValidationService(value)
                //.ValidateNotNull()
                //.ValidateMaxLength(254);

                this.SetPropertyValue(value);
            }
        }

        /// <summary>
        /// Get/Set UserName property.
        /// </summary>
        public string UserName
        {
            get
            {
                return this.GetPropertyValue<string>();
            }
            set
            {
                //new StringValidationService(value)
                //.ValidateMaxLength(100);

                this.SetPropertyValue(value);
            }
        }

        /// <summary>
        /// Get/Set passwordHash property.
        /// </summary>
     //   [NonPatchable]
        public string PasswordHash { get; set; }

        /// <summary>
        /// Get/Set passwordsalt property.
        /// </summary>
    //    [NonPatchable]
        public string PasswordSalt { get; private set; }

        /// <summary>
        /// Get/Set boolean IsActive property.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Get/Set StartDate property.
        /// </summary>
      //  [NonPatchable]
        public DateTime StartDate { get; private set; }

        /// <summary>
        /// Get/Set Require Password Reset property.
        /// </summary>
        public bool RequirePasswordReset { get; set; }

        /// <summary>
        /// Get/Set Default Language property.
        /// </summary>
        public string DefaultLanguage { get; set; }

        /// <summary>
        /// Get/Set Date Of Birth property.
        /// </summary>
        public DateTime? DateOfBirth { get; set; }

        /// <summary>
        /// Get/Set Gender property.
        /// </summary>
        public string Gender
        {
            get { return this.GetPropertyValue<string>(); }
            set
            {
                //new StringValidationService(value)
                //    .ValidateMaxLength(1);

                this.SetPropertyValue(value);
            }
        }

        /// <summary>
        /// Get/Set UserToken collection property.
        /// </summary>

        /// <summary>
        /// Get/Set UserFile Collection property.
        /// </summary>
  //      public virtual ICollection<UserFile> UserFiles { get; set; }

    //    public virtual ICollection<UserRole> UserRoles { get; set; }

        public virtual ICollection<AccessToken> AccessTokens { get; set; }


    }
}
