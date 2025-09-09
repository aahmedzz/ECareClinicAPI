using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace ECareClinicAPI.Filters
{
	public class ValidateModelAttribute : ActionFilterAttribute
	{
		public override void OnActionExecuting(ActionExecutingContext context)
		{
			if (!context.ModelState.IsValid)
			{
				var errors = context.ModelState.Values
					.SelectMany(v => v.Errors)
					.Select(e => e.ErrorMessage)
					.ToArray();

				context.Result = new BadRequestObjectResult(new
				{
					Success = false,
					Errors = errors
				});
			}
		}
	}
}
