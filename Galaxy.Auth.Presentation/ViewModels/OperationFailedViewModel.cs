using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Galaxy.Auth.Presentation.ViewModels
{
    public class OperationFailedViewModel
    {
        public List<IdentityError> Errors { get; set; }
    }
}