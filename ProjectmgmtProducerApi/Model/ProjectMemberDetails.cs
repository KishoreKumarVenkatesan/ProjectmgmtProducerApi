using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace ProjectmgmtProducerApi.Model
{
    public class ProjectMemberDetails:IValidatableObject
    {
        [Required(ErrorMessage = "PartitionKey is Required")]
        [JsonProperty(PropertyName = "partitionKey")]
        public string PartitionKey { get; set; }

        [Key]
        [Required(ErrorMessage = "Member ID is Required")]
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [Required(ErrorMessage = "Member Name is Required")]
        [JsonProperty(PropertyName = "memberName")]
        public string MemberName { get; set; }

        [Required(ErrorMessage = "Years Of Experience is Required")]
        [JsonProperty(PropertyName = "yearsOfExperience")]
        public string YearsOfExperience { get; set; }

        [Required(ErrorMessage = "Skillset is Required")]
        [JsonProperty(PropertyName = "skillset")]
        public string Skillset { get; set; }

        [Required(ErrorMessage = "Current Profile Description is Required")]
        [JsonProperty(PropertyName = "currentProfileDescription")]
        public string CurrentProfileDescription { get; set; }

        [Required(ErrorMessage = "Project StartDate is Required")]
        [JsonProperty(PropertyName = "projectStartDate")]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? ProjectStartDate { get; set; }

        [Required(ErrorMessage = "Project EndDate is Required")]
        [JsonProperty(PropertyName = "projectEndDate")]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? ProjectEndDate { get; set; }

        [Required(ErrorMessage = "Allocation Percentage is Required")]
        [DisplayFormat(DataFormatString = @"{0:#\%}")]
        [JsonProperty(PropertyName = "allocationPercentage")]
        public double AllocationPercentage { get; set; }


        IEnumerable<ValidationResult> IValidatableObject.Validate(ValidationContext validationContext)
        {
            if (ProjectEndDate < ProjectStartDate)
            {
                yield return new ValidationResult("ProjectEndDate must be greater then ProjectStartDate");
            }
            if(Convert.ToInt32(YearsOfExperience) <= 4)
            {
                yield return new ValidationResult("Years of Experience must be greater than 4");
            }
          if(!(String.IsNullOrEmpty(Skillset)))
            { 
               if( Skillset.Split(',').Length < 3)
                {
                    yield return new ValidationResult("Must Possess atleast 3 SkillSet");
                }

            }
        }
    }
}
