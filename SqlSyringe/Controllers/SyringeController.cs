using Microsoft.AspNetCore.Mvc;

namespace SqlSyringe.Controllers {
    /// <summary>
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.Controller" />
    public class SyringeController : Controller {
        /// <summary>
        ///     Returns the Caution note as the Index view.
        /// </summary>
        /// <returns></returns>
        public IActionResult Index() {
            return View("SyringeCaution");
        }
    }
}