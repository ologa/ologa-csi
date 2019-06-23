using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Domain.Resources;
using VPPS.CSI.Domain;
using EFDataAccess.DTO;
using Excel = Microsoft.Office.Interop.Excel;
using EFDataAccess.Services;
using EFDataAccess.UOW;
using MAC.ViewModels;
using System.Web.UI.WebControls;
using System.IO;
using System.Collections.Specialized;

namespace MAC.Controllers
{
    [Allow(Roles = DATA_CAPTURE)]
    public class BeneficiariesController : BaseController
    {
        private BeneficiaryService beneficiaryService = new BeneficiaryService(new UnitOfWork());
        private HIVStatusService hivStatusService = new HIVStatusService(new UnitOfWork());
        private BeneficiaryStatusService beneficiaryStatusService = new BeneficiaryStatusService(new UnitOfWork());

        public ViewResult Index(string sortOrder, string currentFilter, string searchString, int? page
            , string partnerFilter, string codeFilter, string partnerName, string beneficiaryCode)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.FirstNameSortParam = String.IsNullOrEmpty(sortOrder) ? "first_name_desc" : "";
            ViewBag.LastNameSortParam = String.IsNullOrEmpty(sortOrder) ? "last_name_desc" : "";
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";

            if (searchString != null)
            { page = 1; }
            else
            { searchString = currentFilter; }

            ViewBag.CurrentFilter = searchString;
            if (searchString == null) { searchString = ""; }

            if (partnerName != null) { page = 1; } else { partnerName = partnerFilter; }
            if (beneficiaryCode != null) { page = 1; } else { beneficiaryCode = codeFilter; }

            ViewBag.partnerFilter = partnerName;
            ViewBag.codeFilter = beneficiaryCode;

            var beneficiaries = beneficiaryService.findBeneficiaryBynameAndPartnerAndCode(searchString, partnerName, beneficiaryCode);

            switch (sortOrder)
            {
                case "first_name_desc":
                    beneficiaries = beneficiaries.OrderByDescending(h => h.FirstName);
                    break;
                case "last_name_desc":
                    beneficiaries = beneficiaries.OrderByDescending(h => h.LastName);
                    break;
                case "Date":
                    beneficiaries = beneficiaries.OrderBy(h => h.DateOfBirth);
                    break;
                default:
                    beneficiaries = beneficiaries.OrderBy(h => h.LastName);
                    break;
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(PagedList.PagedListExtensions.ToPagedList(beneficiaries, pageNumber, pageSize));
        }

        // GET: Beneficiaries/Details/5
        public ActionResult Details(int? id, string fromHouseholdView)
        {
            if (id == null)
            { return new HttpStatusCodeResult(HttpStatusCode.BadRequest); }

            Beneficiary Beneficiary = beneficiaryService.FetchById(id.Value);

            if (Beneficiary == null)
            { return HttpNotFound(); }
            // beneficiaryService.EvaluateBeneficiaryServicesState(Beneficiary);
            ViewBag.HIVStatuses = hivStatusService.findAllBeneficiaryStatuses(Beneficiary);
            ViewBag.CurrentChildStatus = beneficiaryStatusService.FetchChildStatusHistoryByBeneficiary(Beneficiary).LastOrDefault();
            ViewBag.ChildStatusHistoryList = beneficiaryStatusService.FetchChildStatusHistoryByBeneficiary(Beneficiary).ToList();
            ViewBag.fromHouseholdView = fromHouseholdView;

            return View(Beneficiary);
        }

        // GET: Beneficiaries/Create
        public ActionResult Create(int? id)
        {
            Beneficiary Beneficiary = new Beneficiary();
            ViewBag.Kinships = db.SimpleEntities.Where(se => se.Type == "degree-of-kinship" && se.Code != "99").ToList();
            ViewBag.OVCTypes = db.OVCTypes.Where(o => o.Description != "Indeterminado").ToList();
            SetPreviousAndCurrentViewIDs(id, null);
            return View(Beneficiary);
        }

        // POST: Beneficiaries/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Beneficiary Beneficiary)
        {
            ValidateModel(Beneficiary);

            if (ModelState.IsValid)
            {
                if (Beneficiary.HIVStatus.HIV == HIVStatus.POSITIVE && Beneficiary.HIVStatus.HIVInTreatment == null)
                {
                    TempData["Error"] = LanguageResource.HIVStatus_Validation_HIVInTreatment_Required;
                }
                else if (Beneficiary.HIVStatus.HIV == HIVStatus.UNKNOWN && Beneficiary.HIVStatus.HIVUndisclosedReason == null)
                {
                    TempData["Error"] = LanguageResource.HIVStatus_Validation_HIVUndisclosedReason_Required;
                }
                else if (Beneficiary.AgeInYears > 12)
                {
                    TempData["Error"] = "O grupo de Poupança somente deve ser selecionado caso o Beneficiário tenha idade < 12 anos";
                }
                else
                {
                    var OtherKingship = db.SimpleEntities.Where(se => se.Type == "degree-of-kinship" && se.Description == "Outro").SingleOrDefault();
                    if (Beneficiary.KinshipToFamilyHeadID != OtherKingship.SimpleEntityID)
                    {
                        Beneficiary.OtherKinshipToFamilyHead = null;
                    }

                    if (Beneficiary.RegistrationDateDifferentFromHouseholdDate)
                    {
                        Beneficiary.RegistrationDate = Beneficiary.RegistrationDate;
                        Beneficiary.HIVStatus.InformationDate = Beneficiary.RegistrationDate;
                    }
                    
                    Beneficiary.HIVStatus.UserID = user.UserID;
                    if (Beneficiary.HIVStatus.HIV == HIVStatus.POSITIVE)
                    {
                        Beneficiary.HIVStatus.HIVUndisclosedReason = -1;
                        if (Beneficiary.HIVStatus.HIVInTreatment != HIVStatus.IN_TARV)
                        {
                            Beneficiary.HIVStatus.TarvInitialDate = null;
                        }
                    }
                    else if (Beneficiary.HIVStatus.HIV == HIVStatus.UNKNOWN)
                    {
                        Beneficiary.HIVStatus.NID = null;
                        Beneficiary.HIVStatus.HIVInTreatment = -1;
                        Beneficiary.HIVStatus.TarvInitialDate = null;
                    }
                    else
                    {
                        Beneficiary.HIVStatus.NID = null;
                        Beneficiary.HIVStatus.HIVInTreatment = -1;
                        Beneficiary.HIVStatus.HIVUndisclosedReason = -1;
                        Beneficiary.HIVStatus.TarvInitialDate = null;
                    }

                    Beneficiary.CreatedUserID = user.UserID;
                    beneficiaryService.Save(Beneficiary);
                    beneficiaryService.Commit();
                    Beneficiary.HIVStatus.BeneficiaryID = Beneficiary.BeneficiaryID;
                    Beneficiary.HIVStatus.BeneficiaryGuid = Beneficiary.Beneficiary_guid;
                    hivStatusService.SaveOrUpdate(Beneficiary.HIVStatus);
                    hivStatusService.Commit();
                    ChildStatusHistory csh = new ChildStatusHistory();
                    csh.BeneficiaryID = Beneficiary.BeneficiaryID;
                    csh.CreatedUserID = user.UserID;
                    csh.BeneficiaryGuid = Beneficiary.Beneficiary_guid;
                    csh.ChildStatusID = beneficiaryStatusService.FindChildStatusByDescription("Inicial").StatusID;
                    beneficiaryStatusService.SaveOrUpdate(csh);
                    beneficiaryStatusService.Commit();

                    TempData["success"] = Domain.Resources.LanguageResource.Global_Operation_successfully_realized;
                    return RedirectToAction("Edit", new { id = Beneficiary.BeneficiaryID });
                }
            }

            Beneficiary.DateOfBirth = DateTime.Now;
            ViewBag.Kinships = db.SimpleEntities.Where(se => se.Type == "degree-of-kinship" && se.Code != "99").ToList();
            ViewBag.OVCTypes = db.OVCTypes.Where(o => o.Description != "Indeterminado").ToList();
            return View(Beneficiary);
        }

        // GET: Beneficiaries/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            { return new HttpStatusCodeResult(HttpStatusCode.BadRequest); }

            Beneficiary Beneficiary = db.Beneficiaries
                    .Include(x => x.HIVStatus)
                    .Include(x => x.KinshipToFamilyHead)
                    .Include(x => x.OVCType)
                    .Where(x => x.BeneficiaryID == id).FirstOrDefault();

            if (Beneficiary == null)
            { return HttpNotFound(); }

            List<ChildStatusHistory> childStatusHistoryList = beneficiaryStatusService.FetchChildStatusHistoryByBeneficiary(Beneficiary).ToList();
            ViewBag.ChildStatusHistoryList = childStatusHistoryList;
            ViewBag.CurrentChildStatus = childStatusHistoryList.LastOrDefault();
            ViewBag.ChildStatusList = beneficiaryStatusService.FindAllChildStatuses();
            ViewBag.ChildStatusListToChild = beneficiaryStatusService.FindAllChildStatuses().Where(x => !x.Description.Equals("Adulto"));
            ViewBag.Kinships = db.SimpleEntities.Where(se => se.Type == "degree-of-kinship" && se.Code != "99").ToList();
            ViewBag.OVCTypes = db.OVCTypes.Where(o => o.Description != "Indeterminado").ToList();
            ViewBag.HIVStatuses = hivStatusService.findAllBeneficiaryStatuses(Beneficiary);
            SetPreviousAndCurrentViewIDs(null, id);
            return View(Beneficiary);
        }

        // POST: Beneficiaries/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Beneficiary Beneficiary)
        {
            ValidateModel(Beneficiary);

            if (Beneficiary.State == 3)
            {
                TempData["warning"] = Domain.Resources.LanguageResource.Global_Data_is_locked;
                return RedirectToAction("Edit", "Beneficiaries", new { id = Beneficiary.BeneficiaryID });
            }
            else if (ModelState.IsValid)
            {
                if (Beneficiary.HIVStatus.HIV == HIVStatus.POSITIVE && Beneficiary.HIVStatus.HIVInTreatment == null)
                {
                    TempData["Error"] = LanguageResource.HIVStatus_Validation_HIVInTreatment_Required;
                }
                else if (Beneficiary.HIVStatus.HIV == HIVStatus.UNKNOWN && Beneficiary.HIVStatus.HIVUndisclosedReason == null)
                {
                    TempData["Error"] = LanguageResource.HIVStatus_Validation_HIVUndisclosedReason_Required;
                }
                else if (Beneficiary.AgeInYears > 12)
                {
                    TempData["Error"] = "O grupo de Poupança somente deve ser selecionado caso o Beneficiário tenha idade < 12 anos";
                }
                else
                {
                    var OtherKingship = db.SimpleEntities.Where(se => se.Type == "degree-of-kinship" && se.Description == "Outro").SingleOrDefault();
                    if (Beneficiary.KinshipToFamilyHeadID != OtherKingship.SimpleEntityID)
                    {
                        Beneficiary.OtherKinshipToFamilyHead = null;
                    }

                    Beneficiary.HIVStatus.UserID = user.UserID;
                    if (Beneficiary.HIVStatus.HIV == HIVStatus.POSITIVE)
                    {
                        Beneficiary.HIVStatus.HIVUndisclosedReason = -1;
                        if (Beneficiary.HIVStatus.HIVInTreatment != HIVStatus.IN_TARV)
                        {
                            Beneficiary.HIVStatus.TarvInitialDate = null;
                        }
                    }
                    else if (Beneficiary.HIVStatus.HIV == HIVStatus.UNKNOWN)
                    {
                        Beneficiary.HIVStatus.NID = null;
                        Beneficiary.HIVStatus.HIVInTreatment = -1;
                        Beneficiary.HIVStatus.TarvInitialDate = null;
                    }
                    else
                    {
                        Beneficiary.HIVStatus.NID = null;
                        Beneficiary.HIVStatus.HIVInTreatment = -1;
                        Beneficiary.HIVStatus.HIVUndisclosedReason = -1;
                        Beneficiary.HIVStatus.TarvInitialDate = null;
                    }

                    //Beneficiary.LastUpdatedUserID = user.UserID;
                    beneficiaryService.Save(Beneficiary);
                    beneficiaryService.Commit();
                    hivStatusService.SaveOrUpdate(Beneficiary.HIVStatus);
                    hivStatusService.Commit();
                    TempData["success"] = Domain.Resources.LanguageResource.Global_Operation_successfully_realized;
                    return RedirectToAction("Edit", new { id = Beneficiary.BeneficiaryID });
                }
            }

            Beneficiary = db.Beneficiaries.Include(x => x.HIVStatus)
                .Include(x => x.KinshipToFamilyHead)
                .Include(x => x.OVCType)
                .Where(x => x.BeneficiaryID == Beneficiary.BeneficiaryID).FirstOrDefault();

            List<ChildStatusHistory> childStatusHistoryList = beneficiaryStatusService.FetchChildStatusHistoryByBeneficiary(Beneficiary).ToList();
            ViewBag.ChildStatusHistoryList = childStatusHistoryList;
            ViewBag.CurrentChildStatus = childStatusHistoryList.LastOrDefault();
            ViewBag.ChildStatusList = beneficiaryStatusService.FindAllChildStatuses();
            ViewBag.ChildStatusListToChild = beneficiaryStatusService.FindAllChildStatuses().Where(x => !x.Description.Equals("Adulto"));
            ViewBag.Kinships = db.SimpleEntities.Where(se => se.Type == "degree-of-kinship" && se.Code != "99").ToList();
            ViewBag.OVCTypes = db.OVCTypes.Where(o => o.Description != "Indeterminado").ToList();
            ViewBag.HIVStatuses = hivStatusService.findAllBeneficiaryStatuses(Beneficiary);
            return View(Beneficiary);
        }

        // POST: Beneficiaries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            Beneficiary Beneficiary = beneficiaryService.Get(id);
            beneficiaryService.Delete(Beneficiary);
            beneficiaryService.Commit();
            return RedirectToAction("Index", "Beneficiaries");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public void ValidateModel(Beneficiary beneficiary)
        {
            if (beneficiary.AgeInYears < 18 && beneficiary.OVCTypeID == null)
            {
                ModelState.AddModelError("OVCTypeID", "Campo 'Tipo de OVC', obrigatório para crianças.");
            }
        }
    }
}
