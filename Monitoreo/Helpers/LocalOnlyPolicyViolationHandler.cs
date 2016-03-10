using FluentSecurity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Monitoreo.Helpers
{
    public class LocalOnlyPolicyViolationHandler : IPolicyViolationHandler
    {
        public ActionResult Handle(PolicyViolationException exception)
        {
            return new HttpUnauthorizedResult(exception.Message);
        }
    }
}