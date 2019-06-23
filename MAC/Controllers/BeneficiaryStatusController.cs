using System.Data;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using VPPS.CSI.Domain;
using System.Data.Entity.Infrastructure;
using EFDataAccess.Services;
using EFDataAccess.UOW;
using System.Collections.Specialized;
using System;

namespace MAC.Controllers
{
    [Authorize]
    public class BeneficiaryStatusController : BaseController
    {
        private BeneficiaryStatusService beneficiaryStatusService = new BeneficiaryStatusService(new UnitOfWork());
        private ChildService childService = new ChildService(new UnitOfWork());
        private AdultService adultService = new AdultService(new UnitOfWork());
        private BeneficiaryService beneficiaryService = new BeneficiaryService(new UnitOfWork());

        // GET: SiteGoals
        public ActionResult Index(int? id)
        {
            return View(db.ChildStatusHistories.Where(x => x.ChildStatusHistoryID == id).ToList());
        }

        // GET: Tasks/Create
        public ActionResult Create(int? id)
        {
            ViewBag.ChildStatusListToChild = beneficiaryStatusService.FindAllChildStatuses().Where(x => !x.Description.Equals("Adulto"));

            ChildStatusHistory csh = new ChildStatusHistory();
            csh.BeneficiaryID = id.Value;
            return View(csh);
        }

        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ChildStatusHistory csh)
        {
            if (ModelState.IsValid)
            {
                Beneficiary Beneficiary = beneficiaryService.FetchByBeneficiaryIDandStatusID(csh.BeneficiaryID.Value, csh.ChildStatusID.Value);
                if (Beneficiary != null)
                {
                    csh.CreatedUserID = user.UserID;
                    beneficiaryStatusService.SaveOrUpdate(csh);
                }
                else
                {
                    TempData["Error"] = "A criança não poderá mudar de estágio se for igual ao anterior ou se tiver tornado adulta";
                    return RedirectToAction("Edit", "Beneficiaries", new { id = csh.BeneficiaryID });
                }

            }

            if (csh.ChildID != null && csh.BeneficiaryID == null)
            {
                return RedirectToAction("Edit", "Children", new { id = csh.ChildID });
            }
            else if (csh.AdultID != null && csh.BeneficiaryID == null)
            {
                return RedirectToAction("Edit", "Adults", new { id = csh.AdultID });
            }
            else
            {
                return RedirectToAction("Edit", "Beneficiaries", new { id = csh.BeneficiaryID });
            }

        }


        public ActionResult Edit(int? id)
        {
            if (id == null)
            { return new HttpStatusCodeResult(HttpStatusCode.BadRequest); }

            ChildStatusHistory csh = beneficiaryStatusService.findByID(id.Value);
            ViewBag.ChildStatusListToChild = beneficiaryStatusService.FindAllChildStatuses().Where(x => !x.Description.Equals("Adulto"));

            if (csh == null)
            { return HttpNotFound(); }

            //siteGoal.SitePerformanceComment = " ";
            return View(csh);
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult EditPost(ChildStatusHistory csh)
        {
            if (ModelState.IsValid)
            {
                csh.LastUpdatedUserID = user.UserID;
                beneficiaryStatusService.SaveOrUpdate(csh);
            }

            if (csh.ChildID != null && csh.BeneficiaryID == null)
            {
                return RedirectToAction("Edit", "Children", new { id = csh.ChildID });
            }
            else if (csh.AdultID != null && csh.BeneficiaryID == null)
            {
                return RedirectToAction("Edit", "Adults", new { id = csh.AdultID });
            }
            else
            {
                return RedirectToAction("Edit", "Beneficiaries", new { id = csh.BeneficiaryID });
            }
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            ChildStatusHistory csh = beneficiaryStatusService.fetchByID(id);

            {
                Beneficiary beneficiary = beneficiaryService.FetchById(csh.BeneficiaryID.Value);
                if (csh.ChildStatus.Description == "Initial" || csh.ChildStatus.Description == "Inicial"
                   || csh.ChildStatus.Description == "Adult" || csh.ChildStatus.Description == "Adulto")
                {
                    TempData["Error"] = "O estado Inicial ou Adulto não podem ser removidos.";
                    return RedirectToAction("Edit", "Beneficiaries", new { id = currentScreenID });
                }
                beneficiaryStatusService.Delete(csh);
                return RedirectToAction("Edit", "Beneficiaries", new { id = currentScreenID });
            }
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
