using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;

namespace Application.DTOs.SessionDto.SessionFluentValidators
{
    public class UpdateSessionValidator : AbstractValidator<UpdateSessionDto>
    {
        public UpdateSessionValidator()
        {
            RuleFor(x => x.SessionDate)
           .NotEmpty();

            RuleFor(x => x.Notes)
                .MaximumLength(1000);
        }
    }
}
