using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace GamerKin.ViewModels
{
    public abstract class BaseDto
    {
        public bool InError { get; set; }
        protected List<ValidationResult> ValidationResults { get; set; }
        public string ErrorMessage { get; set; }

        public virtual IEnumerable<ValidationResult> GetValidationResult()
        {
            return Validate(new ValidationContext(this));
        }

        public virtual List<string> GetErrorMessages(IEnumerable<ValidationResult> validationResults)
        {
            return validationResults.Select(x => x.ErrorMessage).ToList();
        }

        public bool IsValid()
        {
            return Validate(new ValidationContext(this)).Count() == 0;
        }

        public virtual IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();
            Validator.TryValidateObject(this, validationContext, results, true);

            return results;
        }

        public void RaiseError()
        {
            InError = true;
            ErrorMessage = GetErrorMessages(GetValidationResult()).FirstOrDefault();
        }

        public void RaiseError(string message)
        {
            InError = true;
            ErrorMessage = message;
        }
    }
}
