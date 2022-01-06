using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Models.ViewModels;

namespace Web.Controllers
{
    public class ProfileController : BaseController
    {

        public ActionResult Index()
        {

            ProfileViewModel profileViewItem = new ProfileViewModel();

            var userInfo = _dbContext.Users.FirstOrDefault(u => u.Id == SessionUser.Id && !u.IsDeleted);

            if (userInfo != null)
            {
                profileViewItem.UserItem = userInfo;


                if (userInfo.RoomId.HasValue)
                {


                    var roomItem = (from roomApp in _dbContext.RoomApplications
                                    join room in _dbContext.Rooms on roomApp.RoomId equals room.Id
                                    where roomApp.RoomId == userInfo.RoomId.Value
                                    orderby roomApp.Id descending
                                    select new RoomApplicationViewModel
                                    {
                                        Id = roomApp.Id,
                                        RoomName = room.RoomName,
                                        ApplyDate = roomApp.ApplyDate,
                                        AccommodationStartDate = roomApp.AccommodationStartDate,
                                        AccodomodationEndDate = roomApp.AccodomodationEndDate,
                                        PaymentDate = roomApp.PaymentDate,
                                        RoomApplicationStatus = roomApp.RoomApplicationStatus.ToString()

                                    }).FirstOrDefault();




                    if (roomItem != null)
                    {
                        profileViewItem.RoomItem = roomItem;
                    }
                }
            }


            return View(profileViewItem);
        }
    }
}