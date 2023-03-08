using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace ProjectmgmtProducerApi.Model
{
    public class ProjectTaskDetails:IValidatableObject
    {

        [Required(ErrorMessage = "PartitionKey is Required")]
        [JsonProperty(PropertyName = "partitionKey")]
        public string PartitionKey { get; set; }

        [Required(ErrorMessage = "Member ID is Required")]
        [JsonProperty(PropertyName = "id")]
        [Key]
        public string Id { get; set; }

        [Required(ErrorMessage = "Member Name is Required")]
        [JsonProperty(PropertyName = "memberName")]
        public string MemberName { get; set; }

        
        [Required(ErrorMessage = "TaskID is Required")]
        [JsonProperty(PropertyName = "taskID")]
        public int TaskID { get; set; }

        [Required(ErrorMessage = "TaskName is Required")]
        [JsonProperty(PropertyName = "taskName")]
        public string TaskName { get; set; }

        [JsonProperty(PropertyName = "deliverables")]
        public string Deliverables { get; set; }

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

        [Required(ErrorMessage = "Task StartDate is Required")]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        [JsonProperty(PropertyName = "taskStartDate")]
       
        public DateTime TaskStartDate { get; set; }

        [Required(ErrorMessage = "Task EndDate is Required")]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        [JsonProperty(PropertyName = "taskEndDate")]
       
        public DateTime TaskEndDate { get; set; }



      

        //public ProjectMemberDetails MemberDetails { get; set; }

        IEnumerable<ValidationResult> IValidatableObject.Validate(ValidationContext validationContext)
        {
            if (TaskEndDate < TaskStartDate)
            {
                yield return new ValidationResult("TaskEndDate must be greater then TaskStartDate");
            }

            if (TaskEndDate > ProjectEndDate)
            {
                yield return new ValidationResult("TaskEndDate must be lesser then ProjectEndDate");
            }
        }
    }
}
