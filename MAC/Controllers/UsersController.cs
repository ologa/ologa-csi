using System.Linq;
using System.Net;
using System.Web.Mvc;
using VPPS.CSI.Domain;
using EFDataAccess.Services;
using System.Web.UI.WebControls;
using EFDataAccess.UOW;
using System;
using System.Collections.Generic;

namespace MAC.Controllers
{
    [Allow(Roles = ADMIN)]
    public class UsersController : BaseController
    {
        private UserService UserService = new UserService(new UnitOfWork());

        [HttpGet]
        public JsonResult List()
        {
            return Json(db.Users.ToList(), JsonRequestBehavior.AllowGet);
        }

        public ViewResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";

            if (searchString != null)
            { page = 1; }
            else
            { searchString = currentFilter; }

            ViewBag.CurrentFilter = searchString;
            if (searchString == null) { searchString = ""; }
            var users = UserService.findByContainsUsername(searchString);

            switch (sortOrder)
            {
                case "name_desc":
                    users = users.OrderByDescending(h => h.Username);
                    break;
                default:
                    users = users.OrderBy(h => h.Username);
                    break;
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(PagedList.PagedListExtensions.ToPagedList(users, pageNumber, pageSize));
        }

        // GET: User/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }

            ViewBag.Roles = db.Roles.ToList();
            ViewBag.Sites = db.Sites.ToList();
            return View(user);
        }

        // GET: User/Create
        public ActionResult Create()
        {
            Session["roles"] = new List<UserRole>();
            ViewBag.Roles = db.Roles.ToList();
            ViewBag.Sites = db.Sites.ToList();
            return View(new User());
        }

        // POST: User/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(User user)
        {
            if (ModelState.IsValid)
            {
                user.UserRoles = (List<UserRole>)Session["roles"];
                UserService.CreateFull(user);
                TempData["success"] = "Usuário criado com sucesso.";
            }

            user.UserRoles = (List<UserRole>)Session["roles"];
            ViewBag.Roles = db.Roles.ToList();
            ViewBag.Sites = db.Sites.ToList();

            return RedirectToAction("Edit",new { id = user.UserID });
        }

        // GET: User/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            { return new HttpStatusCodeResult(HttpStatusCode.BadRequest); }

            User user = db.Users.Find(id);

            if (user == null)
            { return HttpNotFound(); }

            Session["roles"] = user.UserRoles.ToList();
            ViewBag.Roles = db.Roles.ToList();
            ViewBag.Sites = db.Sites.ToList();
            return View(user);
        }

        // POST: User/Edit /5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(User user)
        {
            if (Request.Form["update"] != null)
            {
                if (ModelState.IsValid)
                {
                    user.UserRoles = (List<UserRole>) Session["roles"];
                    UserService.UpdateFull(user);
                    TempData["success"] = "Usuário actualizado com sucesso.";
                }
            }
            else if (Request.Form["add"] != null)
            {
                List<UserRole> userRoles = (List<UserRole>) Session["roles"];
                if (!userRoles.Where(x => x.RoleID == user.RoleID).Any())
                { userRoles.Add(new UserRole { User = user, UserID = user.UserID, Role = db.Roles.Single(x => x.RoleID == user.RoleID), RoleID = user.RoleID.Value, CreatedDate = DateTime.Now }); }
                
                // Session store
                // Session["roles"] = userRoles;

                // Database store
                user.UserRoles = userRoles;
                UserService.UpdateFull(user);
                TempData["success"] = "Perfil adicionado com sucesso.";
            }
            else if (Request.Form["del"] != null)
            {
                var roleIDValue = Request.Form["del"];
                List<UserRole> userRoles = (List<UserRole>) Session["roles"];
                userRoles.Remove(userRoles.Single(x => x.RoleID == int.Parse(roleIDValue)));

                // Session store
                // Session["roles"] = userRoles;

                // Database store
                user.UserRoles = userRoles;
                UserService.UpdateFull(user);
                TempData["success"] = "Perfil removido com sucesso.";
            }

            user.UserRoles = db.UserRoles.Where(x => x.UserID == user.UserID).ToList();
            ViewBag.Roles = db.Roles.ToList();
            ViewBag.Sites = db.Sites.ToList();
            return View(user);
        }

        // POST: User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (db.Households.Where(x => x.CreatedUser.UserID == id).Any() ||
                db.Adults.Where(x => x.CreatedUser.UserID == id).Any() ||
                db.Children.Where(x => x.CreatedUser.UserID == id).Any() ||
                db.RoutineVisits.Where(x => x.CreatedUser.UserID == id).Any() ||
                db.CSIs.Where(x => x.CreatedUser.UserID == id).Any() ||
                db.CarePlans.Where(x => x.CreatedUser.UserID == id).Any() ||
                db.ReferenceServices.Where(x => x.CreatedUser.UserID == id).Any())
            {
                TempData["warning"] = "O Usuário não pode ser removido, pois já efectuou registos no sistema.";
                return RedirectToAction("Index");
            }

            User user = UserService.findByID(id);
            UserService.Delete(user);
            UserService.Commit();
            TempData["success"] = "Usuário removido com sucesso.";
            return RedirectToAction("Index");
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
