using System.Data;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using VPPS.CSI.Domain;
using Domain.Resources;
using System.Data.Entity.Infrastructure;
using EFDataAccess.Services;
using EFDataAccess.UOW;
using System.Collections.Specialized;
using System;
using MAC.ViewModels;
using EFDataAccess.DTO;
using System.Collections.Generic;
using Excel = Microsoft.Office.Interop.Excel;
using System.IO;

namespace MAC.Controllers
{
    [Authorize]
    public class HIVStatusController : BaseController
    {
        private HIVStatusService hivStatusService = new HIVStatusService(new UnitOfWork());
        private ChildService childService = new ChildService(new UnitOfWork());
        private AdultService adultService = new AdultService(new UnitOfWork());
        private SiteService siteService = new SiteService(new UnitOfWork());
        private BeneficiaryService beneficiaryService = new BeneficiaryService(new UnitOfWork());
        // GET: SiteGoals
        public ActionResult Index(int? id)
        {
            return View(db.HIVStatus.Where(x => x.HIVStatusID == id).ToList());
        }

        // GET: Tasks/Create
        public ActionResult Create(int? id)
        {
            HIVStatus hivStatus = new HIVStatus();
            hivStatus.BeneficiaryID = id.Value;
            return View(hivStatus);
        }

        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public ActionResult Create(HIVStatus hivStatus)
        {
            if (ModelState.IsValid)
            {
                if (hivStatus.TarvInitialDate == null
                    && hivStatus.HIVInTreatment == HIVStatus.IN_TARV
                    && hivStatus.HIV == HIVStatus.POSITIVE)
                {
                    TempData["Error"] = LanguageResource.HIVStatus_Validation_INTARV_Date_Required;
                }
                else if (hivStatus.InformationDate == null)
                {
                    TempData["Error"] = LanguageResource.HIVStatus_Validation_HIVStatus_Date_Required;
                }
                else if (hivStatus.HIV == HIVStatus.POSITIVE && hivStatus.HIVInTreatment == null)
                {
                    TempData["Error"] = LanguageResource.HIVStatus_Validation_HIVInTreatment_Required;
                }
                else if (hivStatus.HIV == HIVStatus.UNKNOWN && hivStatus.HIVUndisclosedReason == null)
                {
                    TempData["Error"] = LanguageResource.HIVStatus_Validation_HIVUndisclosedReason_Required;
                }
                else
                {
                    hivStatus.UserID = user.UserID;
                    if (hivStatus.HIV == HIVStatus.POSITIVE)
                    {
                        hivStatus.HIVUndisclosedReason = -1;
                        if (hivStatus.HIVInTreatment != HIVStatus.IN_TARV)
                        {
                            hivStatus.TarvInitialDate = null;
                        }
                    }
                    else if (hivStatus.HIV == HIVStatus.UNKNOWN)
                    {
                        hivStatus.NID = null;
                        hivStatus.HIVInTreatment = -1;
                        hivStatus.TarvInitialDate = null;
                    }
                    else
                    {
                        hivStatus.NID = null;
                        hivStatus.HIVInTreatment = -1;
                        hivStatus.HIVUndisclosedReason = -1;
                        hivStatus.TarvInitialDate = null;
                    }

                    hivStatusService.SaveOrUpdate(hivStatus);
                    hivStatusService.Commit();
                }
            }

            if (hivStatus.ChildID != 0 && hivStatus.BeneficiaryID == 0)
            {
                return RedirectToAction("Edit", "Children", new { id = hivStatus.ChildID });
            }
            else if (hivStatus.AdultID != 0 && hivStatus.BeneficiaryID == 0)
            {
                return RedirectToAction("Edit", "Adults", new { id = hivStatus.AdultID });
            }
            else
            {
                return RedirectToAction("Edit", "Beneficiaries", new { id = hivStatus.BeneficiaryID });
            }
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            { return new HttpStatusCodeResult(HttpStatusCode.BadRequest); }

            HIVStatus hivStatus = hivStatusService.findByID(id.Value);

            if (hivStatus == null)
            { return HttpNotFound(); }

            return View(hivStatus);
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult EditPost(HIVStatus hivStatus)
        {
            if (ModelState.IsValid)
            {
                if (hivStatus.TarvInitialDate == null
                    && hivStatus.HIVInTreatment == HIVStatus.IN_TARV
                    && hivStatus.HIV == HIVStatus.POSITIVE)
                {
                    TempData["Error"] = LanguageResource.HIVStatus_Validation_INTARV_Date_Required;
                }
                else if (hivStatus.InformationDate == null)
                {
                    TempData["Error"] = LanguageResource.HIVStatus_Validation_HIVStatus_Date_Required;
                }
                else if (hivStatus.HIV == HIVStatus.POSITIVE && hivStatus.HIVInTreatment == null)
                {
                    TempData["Error"] = LanguageResource.HIVStatus_Validation_HIVInTreatment_Required;
                }
                else if (hivStatus.HIV == HIVStatus.UNKNOWN && hivStatus.HIVUndisclosedReason == null)
                {
                    TempData["Error"] = LanguageResource.HIVStatus_Validation_HIVUndisclosedReason_Required;
                }
                else
                {
                    hivStatus.UserID = user.UserID;
                    if (hivStatus.HIV == HIVStatus.POSITIVE)
                    {
                        hivStatus.HIVUndisclosedReason = -1;
                        if (hivStatus.HIVInTreatment != HIVStatus.IN_TARV)
                        {
                            hivStatus.TarvInitialDate = null;
                        }
                    }
                    else if (hivStatus.HIV == HIVStatus.UNKNOWN)
                    {
                        hivStatus.NID = null;
                        hivStatus.HIVInTreatment = -1;
                        hivStatus.TarvInitialDate = null;
                    }
                    else
                    {
                        hivStatus.NID = null;
                        hivStatus.HIVInTreatment = -1;
                        hivStatus.HIVUndisclosedReason = -1;
                        hivStatus.TarvInitialDate = null;
                    }

                    hivStatusService.SaveOrUpdate(hivStatus);
                    hivStatusService.Commit();
                }

            }

            if (hivStatus.ChildID != 0 && hivStatus.BeneficiaryID == 0)
            {
                return RedirectToAction("Edit", "Children", new { id = hivStatus.ChildID });
            }
            else if (hivStatus.AdultID != 0 && hivStatus.BeneficiaryID == 0)
            {
                return RedirectToAction("Edit", "Adults", new { id = hivStatus.AdultID });
            }
            else
            {
                return RedirectToAction("Edit", "Beneficiaries", new { id = hivStatus.BeneficiaryID });
            }
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            HIVStatus hivstatus = hivStatusService.findByID(id);

            {
                Beneficiary beneficiary = beneficiaryService.FetchById(hivstatus.BeneficiaryID);
                if (hivstatus.HIVStatusID == beneficiary.HIVStatusID)
                {
                    TempData["Error"] = LanguageResource.HIVStatus_Validation_First_HIVStatus_Cannot_Be_Removed;
                    return RedirectToAction("Edit", "Beneficiaries", new { id = currentScreenID });
                }
                hivStatusService.Delete(hivstatus);
                hivStatusService.Commit();
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
