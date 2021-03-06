﻿using System.Web.Mvc;
using EFDataAccess;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using System.IO;
using EFDataAccess.Logging;
using EFDataAccess.Services;
using VPPS.CSI.Domain;
using EFDataAccess.DTO;
using EFDataAccess.UOW;
using System.Web.UI;
using MAC.Lang;
using System;
using System.Web;

namespace MAC.Controllers
{
    [Authorize]
    public class BaseController : Controller
    {
        public const string ADMIN = "Administrador";
        public const string DATA_SYNC = "Administrador,Oficial-Provincial";
        public const string DATA_LOCK = "Administrador,Oficial-Provincial";
        public const string REPORT_VIEWERS = "Administrador,Oficial-Provincial,Oficial-OCB";
        public const string DATA_CAPTURE = "Administrador,Oficial-Provincial,Oficial-OCB";

        protected ILogger logger = new Logger();
        protected ApplicationDbContext db = new ApplicationDbContext();
        protected int currentScreenID => (int) Session["currentScreenID"];
        protected int previousScreenID => (int)Session["previousScreenID"];
        protected string currentScreenName => (string)Session["currentScreenName"];
        protected string previousScreenName => (string) Session["previousScreenName"];
        protected UserInfo userInfo { get { return (UserInfo) Session["userInfo"]; } }
        protected User user { get { return (User)Session["user"]; } }

        protected override IAsyncResult BeginExecuteCore(AsyncCallback callback, object state)
        {
            string lang = null;
            HttpCookie langCookie = Request.Cookies["culture"];
            if (langCookie != null)
            {
                lang = langCookie.Value;
            }
            else
            {
                var userLanguage = Request.UserLanguages;
                var userLang = userLanguage != null ? userLanguage[0] : "";
                lang = (userLang != "") ? userLang : LanguageMang.GetDefaultLanguage();
            }
            new LanguageMang().SetLanguage(lang);
            return base.BeginExecuteCore(callback, state);
        }

        public void DownloadXlsxFile(string FilePath)
        {
            PrepareResponseToDownload(FilePath);
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.WriteFile(FilePath);
            Response.Flush();
            Response.End();
        }

        public void DownloadXlsStringWriter(string FilePath, StringWriter sw)
        {
            PrepareResponseToDownload(FilePath);
            Response.ContentType = "application/ms-excel";
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();
        }

        public void PrepareResponseToDownload(string FilePath)
        {
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("Content-Disposition", "attachment; filename=" + Path.GetFileName(FilePath));
            Response.Charset = "";
        }

        public void SetAgreggatedReportFiltersOnViewBag()
        {
            SetAgreggatedReportFiltersOnViewBag(0, 0, 0, 0);
        }

        public void SetAgreggatedReportFiltersOnViewBag(int ProvinceID, int DistrictID, int LocationID, int SiteID)
        {
            List<SelectListItem> ProvinceList = new List<SelectListItem>();
            ProvinceList.Insert(0, new SelectListItem { Text = "", Value = "" });
            if (ProvinceID > 0) ProvinceList.Where(x => x.Value == ProvinceID.ToString()).FirstOrDefault().Selected = true;
            ViewBag.ProvID = ProvinceList;

            List<SelectListItem> DistrictList = new List<SelectListItem>();
            DistrictList.Insert(0, new SelectListItem { Text = "", Value = "" });
            if (DistrictID > 0) DistrictList.Where(x => x.Value == DistrictID.ToString()).FirstOrDefault().Selected = true;
            ViewBag.DistID = DistrictList;

            List<SelectListItem> LocationList = new List<SelectListItem>();
            LocationList.Insert(0, new SelectListItem { Text = "", Value = "" });
            if (LocationID > 0) LocationList.Where(x => x.Value == LocationID.ToString()).FirstOrDefault().Selected = true;
            ViewBag.LocationID = LocationList;

            List<SelectListItem> SiteList = new List<SelectListItem>();
            if (SiteID > 0) SiteList.Where(x => x.Value == SiteID.ToString()).FirstOrDefault().Selected = true;
            ViewBag.SiteID = SiteList;
        }

        public void SetPartnerAndChildAndStatusFiltersOnViewBag(int? PartnerID, int? ChildID, int? ChildStatusID)
        {
            List<SelectListItem> PartnerList = new List<SelectListItem>();
            PartnerList.Insert(0, new SelectListItem { Text = "", Value = "" });
            if (PartnerID > 0) PartnerList.Where(x => x.Value == PartnerID.ToString()).FirstOrDefault().Selected = true;
            ViewBag.PartnerID = PartnerList;

            List<SelectListItem> ChildList = new List<SelectListItem>();
            ChildList.Insert(0, new SelectListItem { Text = "", Value = "" });
            if (ChildID > 0) ChildList.Where(x => x.Value == ChildID.ToString()).FirstOrDefault().Selected = true;
            ViewBag.ChildID = ChildList;

            List<SelectListItem> ChildStatusList = new List<SelectListItem>();
            ChildStatusList.AddRange(new SelectList(db.ChildStatus, "StatusID", "Description"));
            ChildStatusList.Insert(0, new SelectListItem { Text = "", Value = "" });
            if (ChildStatusID > 0) ChildStatusList.Where(x => x.Value == ChildStatusID.ToString()).FirstOrDefault().Selected = true;
            ViewBag.StatusID = ChildStatusList;
        }

        protected void SetPreviousAndCurrentViewIDs(int? previousScreenID, int? currentScreenID)
        {
            Session["previousScreenID"] = previousScreenID;
            Session["currentScreenID"] = currentScreenID;
        }

        protected void SetPreviousAndCurrentViewInfo(int? previousScreenID, int? currentScreenID, string previousScreenName, string currentScreenName)
        {
            Session["previousScreenID"] = previousScreenID;
            Session["currentScreenID"] = currentScreenID;
            Session["previousScreenName"] = previousScreenName;
            Session["currentScreenName"] = currentScreenName;
        }

        protected AuditedEntity SetLastUpdatedUser(AuditedEntity entity)
        {
            entity.LastUpdatedUser = user;
            return entity;
        }

        protected AuditedEntity SetCreatedUser(AuditedEntity entity)
        {
            entity.CreatedUser = user;
            return entity;
        }

        public List<object> GetListSafe(string value)
        {
            return (Session[value] == null) ? new List<object>() : (List<object>) Session[value];
        }
    }
}
