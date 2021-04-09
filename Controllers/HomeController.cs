using BYUFagElGamous1_5.Models;
using BYUFagElGamous1_5.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace BYUFagElGamous1_5.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private FagElGamousContext context;

        public HomeController(ILogger<HomeController> logger, FagElGamousContext ctx)
        {
            _logger = logger;
            context = ctx;
        }
        public IActionResult Index()
        {
            return View();
        }

        //ADD MUMMY ----------------------------------------------------
        [HttpGet]
        [Authorize(Roles = "SuperAdmin, Researcher")]
        public IActionResult AddMummy()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "SuperAdmin, Researcher")]
        public IActionResult AddMummy(AddMummyViewModel entry)
        {
            Measurements msr = new Measurements();
            Sample sample = new Sample();

            context.Add(entry.Location); //Add the location
            context.SaveChanges();
            //See if there is a more optimized way of doing this so you dont have to run a query every time
            //Set the mummies locationID equal to the location being created
            entry.Mummy.LocationId = context.Location.OrderByDescending(x => x.LocationId).Select(x => x.LocationId).First();
            entry.Location.HighPairNs = entry.Location.LowPairNs + 10; //Create the high pair plot point
            entry.Location.HighPairEw = entry.Location.LowPairEw + 10;
            context.Add(entry.Mummy); //Add the mummy
            context.SaveChanges();

            //set the measurement ID equal to the mummy ID 
            entry.Mummy.MeasurementId = context.Mummy.OrderByDescending(x => x.MummyId).Select(x => x.MummyId).First();
            msr.MeasurementId = entry.Mummy.MeasurementId;

            //Add the measurement to the database and update the mummy
            context.Add(msr);
            context.Update(entry.Mummy);
            context.SaveChanges();

            //Create a note for the mummy
            Notes note = new Notes();
            note.MummyId = entry.Mummy.MummyId;
            context.Add(note);
            context.SaveChanges();

            return View("MummyProfile", new MummyProfileViewModel {
                Mummy = context.Mummy.OrderByDescending(x => x.MummyId).Select(x => x).First(),
                Location = context.Location.OrderByDescending(x => x.LocationId).Select(x => x).First(),
                Measurement = context.Measurements.OrderByDescending(x => x.MeasurementId).Select(x => x).First(),
                Notes = context.Notes.OrderByDescending(x=>x.NotesId).Select(x=>x).First()
            });
        }

        //VIEW MUMMIES -----------------------------------------
        [HttpGet]
        public IActionResult ViewMummies(int numItems = 10, int pageNum = 1)
        {
            //Dictionary to line up each mummy with location based on location
            Dictionary<Mummy, Location> dict = new Dictionary<Mummy, Location>();

            foreach (var x in context.Mummy)
            {
                dict.Add(x, context.Location.Where(y => y.LocationId == x.LocationId).First());
            }

            return View(new ViewMummyViewModel
            {
                mumLocs = dict,
                Mummies = context.Mummy
                    .OrderBy(x => x.MummyId)
                    .Skip((pageNum - 1) * numItems)
                    .Take(numItems)
                    .ToList(),

                PageNumberInfo = new PageNumberInfo
                {
                    NumItemsPerPage = numItems,
                    CurrentPage = pageNum,
                    numItems = numItems,

                    //return total count of mummies
                    TotalNumItems = context.Mummy.Count()
                }
            });
        }

        //MUMMY PROFILE -------------------------------------------
        [HttpPost]
        public IActionResult MummyProfile(int id)
        {
            Mummy mum = context.Mummy.Where(x => x.MummyId == id).First();
            Measurements msr;
            Notes note;
            Location loc = context.Location.Where(x => x.LocationId == mum.LocationId).First();
            try
            {
                msr = context.Measurements.Where(x => x.MeasurementId == mum.MeasurementId).First();
            }
            catch
            {
                //If no measurement is found, create a new measurement
                msr = new Measurements();
            }

            try
            {
                note = context.Notes.Where(x => x.MummyId == mum.MummyId).First();
            }
            catch
            {
                note = new Notes();
            }

            return View(new MummyProfileViewModel
            {
                Mummy = mum,
                Location = loc,
                Measurement = msr,
                Notes = note
            });
        }

        [HttpPost]
        public ActionResult PartialView(string id, int selector, string type)
        {
            if (type == "mummy")
            {
                Mummy mummy = context.Mummy.Where(x => x.MummyId == selector).First();
                return PartialView(id, mummy);
            }
            else if (type == "location")
            {
                Location loc = context.Location.Where(x => x.LocationId == selector).First();
                return PartialView(id, loc);
            }
            else if (type == "measurement")
            {
                Measurements msr = context.Measurements.Where(x => x.MeasurementId == selector).First();
                return PartialView(id, msr);
            }
            else
            {
                Notes note;
                try
                {
                    note = context.Notes.Where(x => x.NotesId == selector).First();
                }
                catch
                {
                    note = new Notes();
                }
                
                return PartialView(id, note);
            }
        }

        [HttpGet]
        public IActionResult TestNotes()
        {
            return View();       
        }

        [HttpPost]
        public IActionResult TestNotes(Notes note)
        {
            note.MummyId = 1;
            note.MeasurmentsId = 1;
            note.LocationId = 1;
            note.SampleId = 1;
            context.Add(note);
            context.SaveChanges();

            return View();
        }

        [HttpPost]
        public IActionResult EditAttributes(int id)
        {
            Mummy mum = context.Mummy.Where(x => x.MummyId == id).First();

            return View("EditAttributes", mum);
            
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
