using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using BYUFagElGamous1_5.Models;
using BYUFagElGamous1_5.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;

namespace BYUFagElGamous1_5.Controllers
{
    [AllowAnonymous]
    [AutoValidateAntiforgeryToken]
    public class HomeController : Controller
    {
        private IHostingEnvironment _hostingEnvironment;

        private readonly ILogger<HomeController> _logger;
        private FagElGamousContext context;

        public HomeController(ILogger<HomeController> logger, FagElGamousContext ctx, IWebHostEnvironment environment)
        {
            _logger = logger;
            context = ctx;
            _hostingEnvironment = environment;
        }

        public IActionResult Index()
        {
            return View();
        }

        //-----------------------------------------
        //               ADD MUMMY
        //-----------------------------------------
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
            //Notes note = new Notes();
            //note.MummyId = entry.Mummy.MummyId;
            //context.Add(note);
            //context.SaveChanges();

            return View("MummyProfile", new MummyProfileViewModel
            {
                Mummy = context.Mummy.OrderByDescending(x => x.MummyId).Select(x => x).First(),
                Location = context.Location.OrderByDescending(x => x.LocationId).Select(x => x).First(),
                Measurement = context.Measurements.OrderByDescending(x => x.MeasurementId).Select(x => x).First(),
                //Notes = context.Notes.OrderByDescending(x=>x.NotesId).Select(x=>x).First()
            });
        }


        //-----------------------------------------
        //               VIEW MUMMIES
        //-----------------------------------------
        [HttpGet]
        public IActionResult ViewMummies(int pageItems = 10, int pageNum = 1)
        {
            //Dictionary to line up each mummy with location based on location
            Dictionary<Mummy, Location> dict = new Dictionary<Mummy, Location>();

            foreach (var x in context.Mummy)
            {
                dict.Add(x, context.Location.Where(y => y.LocationId == x.LocationId).First());
            }
            ViewBag.numItems = pageItems;

            return View(new ViewMummyViewModel
            {
                mumLocs = dict,
                Mummies = context.Mummy
                    .OrderBy(x => x.MummyId)
                    .Skip((pageNum - 1) * pageItems)
                    .Take(pageItems)
                    .ToList(),

                PageNumberInfo = new PageNumberInfo
                {
                    NumItemsPerPage = pageItems,
                    CurrentPage = pageNum,

                    //return total count of mummies
                    TotalNumItems = context.Mummy.Count()
                }
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ViewMummies([FromForm] ViewMummyViewModel viewMummy,
            string searchFor, DateTime DateFrom, DateTime DateTo) //Dictionary<Mummy, Location> mumLocs, Mummy dummyMummy, PageNumberInfo pageNumberInfo,
        {
            //Dictionary to line up each mummy with location based on location
            Dictionary<Mummy, Location> dict = new Dictionary<Mummy, Location>();
            //create new dictionary because it's hard to pass dictionaries from view to model
            foreach (var x in context.Mummy)
            {
                dict.Add(x, context.Location.Where(y => y.LocationId == x.LocationId).First());
            }

            //save viewmodel objects passed in through form to easy to work with objects
            Mummy dummyMummy = viewMummy.mummy;
            PageNumberInfo pageNumberInfo = viewMummy.PageNumberInfo;

            //checks if datefrom and dateto have been entered & fills in logical missing other
            if (DateFrom != DateTime.MinValue && DateTo == DateTime.MinValue)
            {
                DateTo = DateTime.Now;
            }
            if (DateTo != DateTime.MinValue && DateFrom == DateTime.MinValue)
            {
                DateFrom = new DateTime(0000, 00, 00);
            }

            //creates a results query from mummy db
            var results = (from m in context.Mummy
                           .Distinct()
                           select m);

            //if date pickers are left empty 
            // else results are further narrowed by date found
            if (DateFrom != DateTime.MinValue && DateTo != DateTime.MinValue)
            {
                results = from m in results
                           .Where(m => m.DayFound >= DateFrom.Day && m.DayFound <= DateTo.Day)
                           .Where(m => m.MonthFound >= DateFrom.Month && m.MonthFound <= DateTo.Month)
                           .Where(m => m.YearFound >= DateFrom.Year && m.YearFound <= DateTo.Year)
                          select m;
            }

            //Checks all filter inputs > if changed then narrows the results by what has been entered
            if (dummyMummy.AdultChild != null) { results = from m in results.Where(x => x.AdultChild == dummyMummy.AdultChild) select m; }
            if (dummyMummy.AgeRange != null) { results = from m in results.Where(x => x.AgeRange == dummyMummy.AgeRange) select m; }
            if (dummyMummy.Artifacts != null) { results = from m in results.Where(x => x.Artifacts == dummyMummy.Artifacts) select m; }
            if (dummyMummy.BurialAgeAtDeath != null) { results = from m in results.Where(x => x.BurialAgeAtDeath == dummyMummy.BurialAgeAtDeath) select m; }
            if (dummyMummy.BurialAgeMethod != null) { results = from m in results.Where(x => x.BurialAgeMethod == dummyMummy.BurialAgeMethod) select m; }
            if (dummyMummy.BurialGenderMethod != null) { results = from m in results.Where(x => x.BurialGenderMethod == dummyMummy.BurialGenderMethod) select m; }
            if (dummyMummy.BurialPreservation != null) { results = from m in results.Where(x => x.BurialPreservation == dummyMummy.BurialPreservation) select m; }
            if (dummyMummy.BurialSampleTaken != null) { results = from m in results.Where(x => x.BurialSampleTaken == dummyMummy.BurialSampleTaken) select m; }
            if (dummyMummy.BurialWrapping != null) { results = from m in results.Where(x => x.BurialWrapping == dummyMummy.BurialWrapping) select m; }
            if (dummyMummy.FaceBundle != null) { results = from m in results.Where(x => x.FaceBundle == dummyMummy.FaceBundle) select m; }
            if (dummyMummy.Gamous != null) { results = from m in results.Where(x => x.Gamous == dummyMummy.Gamous) select m; }
            if (dummyMummy.Gender != null) { results = from m in results.Where(x => x.Gender == dummyMummy.Gender) select m; }
            if (dummyMummy.HairColor != null) { results = from m in results.Where(x => x.HairColor == dummyMummy.HairColor) select m; }
            if (dummyMummy.HeadDirection != null) { results = from m in results.Where(x => x.HeadDirection == dummyMummy.HeadDirection) select m; }
            if (dummyMummy.Images != null) { results = from m in results.Where(x => x.Images == dummyMummy.Images) select m; }
            if (dummyMummy.LengthOfRemains != null) { results = from m in results.Where(x => x.LengthOfRemains == dummyMummy.LengthOfRemains) select m; }
            if (dummyMummy.PhotoTaken != null) { results = from m in results.Where(x => x.PhotoTaken == dummyMummy.PhotoTaken) select m; }

            //checks the search bar for search
            // then searches remaining query result for text matching search
            if (!string.IsNullOrEmpty(searchFor))
            {
                List<string> _search = searchFor.Split(' ').ToList();

                results = (from c in results
                           where _search.Contains(c.Location.LowPairNs.ToString()) // All location context is brought in by virtual object in mummy 
                           || _search.Contains(c.Location.HighPairNs.ToString()) //ints and bools are converted to string to be compared to string search
                           || _search.Contains(c.Location.LowPairEw.ToString())
                           || _search.Contains(c.Location.HighPairEw.ToString())
                           || _search.Contains(c.Location.Subplot)
                           || _search.Contains(c.Location.BurialNumber.ToString())
                           || _search.Contains(c.AdultChild)
                           || _search.Contains(c.AgeRange)
                           || _search.Contains(c.Artifacts)
                           || _search.Contains(c.BurialAgeAtDeath)
                           || _search.Contains(c.BurialAgeMethod)
                           || _search.Contains(c.BurialGenderMethod)
                           || _search.Contains(c.BurialPreservation)
                           || _search.Contains(c.BurialWrapping)
                           || _search.Contains(c.FaceBundle)
                           || _search.Contains(c.Gamous.ToString())
                           || _search.Contains(c.Gender)
                           || _search.Contains(c.HairColor)
                           || _search.Contains(c.HeadDirection)
                           || _search.Contains(c.LengthOfRemains.ToString())
                           select c);
            }

            ViewBag.DateFrom = DateFrom.ToString();
            ViewBag.DateFrom = DateTo.ToString();
            ViewBag.numItems = viewMummy.PageNumberInfo.numItems;

            return View("ViewMummies", new ViewMummyViewModel
            {
                mumLocs = dict,
                Mummies = results
                    .OrderBy(x => x.MummyId)
                    .Skip((viewMummy.PageNumberInfo.CurrentPage - 1) * viewMummy.PageNumberInfo.NumItemsPerPage)
                    .Take(viewMummy.PageNumberInfo.NumItemsPerPage)
                    .ToList(),

                PageNumberInfo = pageNumberInfo
            });
        }


        //-----------------------------------------
        //             MUMMY PROFILE
        //-----------------------------------------
        [HttpPost]
        public IActionResult MummyProfile(int id)
        {
            Mummy mum = context.Mummy.Where(x => x.MummyId == id).First();
            Measurements msr;
            //Notes note;
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

            return View(new MummyProfileViewModel
            {
                Mummy = mum,
                Location = loc,
                Measurement = msr,
                //Notes = note
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
            else if (type == "notes")
            {
                List<Notes> notes;
                try
                {
                    notes = context.Notes.Where(x => x.MummyId == selector).ToList();
                }
                catch
                {
                    notes = new List<Notes>();
                }

                return PartialView(id, new MummyNotesViewModel
                {
                    Notes = notes,
                    MummyId = selector,
                    NewNote = null
                });
            }
            else if (type == "sample")
            {
                List<Sample> samples = context.Sample.Where(x => x.MummyId == selector).ToList();
                return PartialView(id, new MummySamplesViewModel
                {
                    Samples = samples,
                    MummyId = selector,
                    NewSample = null
                });
            }
            else if (type == "carbon")
            {
                List<CarbonDated> carbons = context.CarbonDated.Where(x => x.MummyId == selector).ToList();
                return PartialView(id, new MummyCarbonViewModel
                {
                    Carbons = carbons,
                    MummyId = selector,
                    NewCarbon = null
                });
            }
            else if (type == "upload")
            {
                return PartialView(id, new MummyUploadViewModel {
                    MummyId = selector
                });
            }
            else if (type == "files")
            {
                IEnumerable<MummyImage> output = context.MummyImage.Where(x => x.MummyId == selector);
                Dictionary<string, string> returnDict = new Dictionary<string, string>();
                foreach (var x in output)
                {
                    if(x.Type == "file")
                    {
                        Images img = context.Images.Where(y => y.ImageId == x.ImageId && x.Type == "file").First();
                        returnDict.Add(img.ImageSource, x.Name);
                    }

                    
                }
                return PartialView(id, returnDict);
            }
            else if (type == "images")
            {
                IEnumerable<MummyImage> output = context.MummyImage.Where(x => x.MummyId == selector);
                Dictionary<string, string> returnDict = new Dictionary<string, string>();
                List<string> images = new List<string>();
                foreach(var x in output)
                {
                    if(x.Type == "image")
                    {
                        Images img = context.Images.Where(y => y.ImageId == x.ImageId).First();
                        returnDict.Add(img.ImageSource, x.Name);
                    }
                    
                    
                }
                return PartialView(id, returnDict);
            }
            else
            {
                Mummy mum = context.Mummy.Where(x => x.MummyId == selector).First();
                return PartialView(id, new MummyProfileViewModel {
                    Mummy = mum,
                    Location = context.Location.Where(x=>x.LocationId == mum.LocationId).First(),
                    Measurement = context.Measurements.Where(x=>x.MeasurementId == mum.MeasurementId).First()
                });
            }
        }

        [HttpPost]
        public ActionResult MeasurementPartialView(string id, int selector, string type)
        {
            if (type == "skull")
            {
                Measurements msr = context.Measurements.Where(x => x.MeasurementId == selector).First();
                return PartialView(id, msr);
            }
            else if (type == "body")
            {
                Measurements msr = context.Measurements.Where(x => x.MeasurementId == selector).First();
                return PartialView(id, msr);
            }
            else
            {
                Measurements msr = context.Measurements.Where(x => x.MeasurementId == selector).First();
                return PartialView(id, msr);
            }

        }

        //-----------------------------------------
        //             DELETE MUMMY
        //-----------------------------------------

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            //find the corresponding tables for the specified mummy
            var mummy = await context.Mummy.FindAsync(id);
            if (mummy != null)
            {
                var location = await context.Location.FindAsync(mummy.LocationId);
                var measurement = await context.Measurements.FindAsync(mummy.MeasurementId);
                var sample = await context.Sample.FindAsync(mummy.SampleId);
                var carbon = await context.CarbonDated.Where(x => x.MummyId == mummy.MummyId).FirstOrDefaultAsync();

                //find each note attatched to mummy
                foreach (var note in context.Notes.Where(x => x.MummyId == mummy.MummyId))
                {
                    var noteDel = await context.Notes.FindAsync(note.NotesId);
                    context.Notes.Remove(noteDel);
                }

                //remove each record from tables
                if (location != null) context.Location.Remove(location);
                if (measurement != null) context.Measurements.Remove(measurement);
                if (sample != null) context.Sample.Remove(sample);
                if (carbon != null) context.CarbonDated.Remove(carbon);

                context.Mummy.Remove(mummy);

                await context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(ViewMummies));
        }



        //-----------------------------------------
        //               EDITS
        //-----------------------------------------
        [HttpPost]
        public IActionResult EditAttributes(int id)
        {
            Mummy mum = context.Mummy.Where(x => x.MummyId == id).First();
            if (mum == null)
            {
                return NotFound();
            }

            return View("EditAttributes", mum);
        }

        [HttpPost("UpdateMummy")]
        public async Task<IActionResult> UpdateMummy(Mummy mummy)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    context.Update(mummy);
                    await context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    return NotFound();
                }

                return View("MummyProfile", new MummyProfileViewModel
                {
                    Mummy = mummy,
                    Location = context.Location.Where(x => x.LocationId == mummy.LocationId).FirstOrDefault(),
                    Measurement = context.Measurements.Where(x => x.MeasurementId == mummy.MeasurementId).FirstOrDefault(),
                    //Notes = context.Notes.Where(x => x.MummyId == mummy.MummyId).FirstOrDefault()
                });
            }
            return View("UpdateMummy", mummy);
        }

        [HttpPost]
        public IActionResult EditLocation(int id)
        {
            Location loc = context.Location.Where(x => x.LocationId == id).First();
            if (loc == null)
            {
                return NotFound();
            }
            return View("EditLocation", loc);
        }

        [HttpPost("UpdateLocation")]
        public async Task<IActionResult> UpdateLocation(Location location)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    context.Update(location);
                    await context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    return NotFound();
                }

                Mummy mummy = context.Mummy.Where(x => x.LocationId == location.LocationId).FirstOrDefault();
                return View("MummyProfile", new MummyProfileViewModel
                {
                    Mummy = mummy,
                    Location = location,
                    Measurement = context.Measurements.Where(x => x.MeasurementId == mummy.MeasurementId).FirstOrDefault(),
                    //Notes = context.Notes.Where(x => x.MummyId == mummy.MummyId).FirstOrDefault()
                });
            }
            return View("UpdateMummy", location);
        }

        [HttpPost("UpdateCarbon")]
        public async Task<IActionResult> UpdateCarbon(CarbonDated carbon)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    context.Update(carbon);
                    await context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    return NotFound();
                }

                Mummy mummy = context.Mummy.Where(x => x.MummyId == carbon.MummyId).FirstOrDefault();
                return View("MummyProfile", new MummyProfileViewModel
                {
                    Mummy = mummy,
                    Location = context.Location.Where(x => x.LocationId == mummy.LocationId).First(),
                    Measurement = context.Measurements.Where(x => x.MeasurementId == mummy.MeasurementId).FirstOrDefault(),
                    //Notes = context.Notes.Where(x => x.MummyId == mummy.MummyId).FirstOrDefault()
                });
            }
            return View("UpdateMummy", carbon);
        }

        [HttpPost("UpdateSample")]
        public async Task<IActionResult> UpdateSample(Sample sample)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    context.Update(sample);
                    await context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    return NotFound();
                }

                Mummy mummy = context.Mummy.Where(x => x.MummyId == sample.MummyId).FirstOrDefault();
                return View("MummyProfile", new MummyProfileViewModel
                {
                    Mummy = mummy,
                    Location = context.Location.Where(x => x.LocationId == mummy.LocationId).First(),
                    Measurement = context.Measurements.Where(x => x.MeasurementId == mummy.MeasurementId).FirstOrDefault(),
                    //Notes = context.Notes.Where(x => x.MummyId == mummy.MummyId).FirstOrDefault()
                });
            }
            return View("UpdateMummy", sample);
        }

        //---------------------------------------
        //          Image Uploading
        // --------------------------------------

        //---------------------AWS Credentials------------------------------
        //If we had more time we would figure out how to remove
        //these from production

        private static readonly string bucketName = "practice-bucket-abcdefg";
        private static readonly RegionEndpoint bucketRegion = RegionEndpoint.USEast1;
        private static readonly string accesskey = "AKIAUFFI6OS7E2BABOGY";
        private static readonly string secretkey = "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";

        //-----------------------------------------------------------

        public static string GetUntilOrEmpty( string text, string stopAt = ".")
        {
            if (!String.IsNullOrEmpty(text))
            {
                int charLocation = text.IndexOf(stopAt, StringComparison.Ordinal);

                if (charLocation > 0)
                {
                    return text.Substring(0, charLocation);
                }
            }

            return String.Empty;
        }

        [HttpPost]
        public IActionResult FileUploadForm(FileUpload upload, int id = 1)
        {
            string config = ConfigurationManager.AppSettings["Test"];

            AmazonS3Client client = new AmazonS3Client(accesskey, secretkey, bucketRegion);
            string folderPath = "";
            MummyImage mumImg = new MummyImage();
            Images img = new Images();
            if (upload.FormFile.ContentType == "image/jpeg")
            {
                folderPath = $"images/{upload.FormFile.FileName}";
                mumImg.Type = "image";
                string name = upload.FormFile.FileName;
                name = GetUntilOrEmpty(name);
                mumImg.Name = name; //here----------------------
            }
            else if (upload.FormFile.ContentType == "application/pdf")
            {
                folderPath = $"documents/{upload.FormFile.FileName}";
                mumImg.Type = "file";
                string name = upload.FormFile.FileName;
                name = GetUntilOrEmpty(name);
                mumImg.Name = upload.FormFile.FileName;
            }

            
            img.ImageSource = $"https://practice-bucket-abcdefg.s3.amazonaws.com/{folderPath}";
            context.Add(img);
            context.SaveChanges();
            mumImg.ImageId = context.Images.OrderByDescending(x => x.ImageId).Select(x => x.ImageId).First();
            mumImg.MummyId = id;
            context.Add(mumImg);
            context.SaveChanges();


            PutObjectRequest request = new PutObjectRequest()
            {
                BucketName = bucketName,
                Key = folderPath
            };

            using (Stream fileToUpload = upload.FormFile.OpenReadStream())
            {
                request.InputStream = fileToUpload;
                request.ContentType = upload.FormFile.ContentType;

                var response = client.PutObjectAsync(request);
                response.Wait();
            }

            Mummy mum = context.Mummy.Where(x => x.MummyId == id).First();

            return View("MummyProfile", new MummyProfileViewModel
            {
                Mummy = mum,
                Location = context.Location.Where(x => x.LocationId == mum.LocationId).First(),
                Measurement = context.Measurements.Where(x => x.MeasurementId == mum.MeasurementId).First()
            });
        }


        [HttpPost]
        public IActionResult AddNote(int id, MummyNotesViewModel note)
        {
            note.NewNote.MummyId = id;
            context.Add(note.NewNote);
            context.SaveChanges();

            List<Notes> notes;
            try
            {
                notes = context.Notes.Where(x => x.MummyId == id).ToList();
            }
            catch
            {
                notes = new List<Notes>();
            }

            Mummy mum = context.Mummy.Where(x => x.MummyId == id).FirstOrDefault();

            return View("MummyProfile", new MummyProfileViewModel
            {

                Mummy = mum,
                Location = context.Location.Where(x => x.LocationId == mum.LocationId).FirstOrDefault(),
                Measurement = context.Measurements.Where(x => x.MeasurementId == mum.MeasurementId).FirstOrDefault(),
                //Notes = context.Notes.Where(x => x.MummyId == mummy.MummyId).FirstOrDefault()
            });
        }

        [HttpPost]
        public IActionResult AddSample(int id, MummySamplesViewModel sample)
        {
            sample.NewSample.MummyId = id;
            context.Add(sample.NewSample);
            context.SaveChanges();

            List<Sample> sp;
            try
            {
                sp = context.Sample.Where(x => x.MummyId == id).ToList();
            }
            catch
            {
                sp = new List<Sample>();
            }

            Mummy mum = context.Mummy.Where(x => x.MummyId == id).FirstOrDefault();

            return View("MummyProfile", new MummyProfileViewModel
            {

                Mummy = mum,
                Location = context.Location.Where(x => x.LocationId == mum.LocationId).FirstOrDefault(),
                Measurement = context.Measurements.Where(x => x.MeasurementId == mum.MeasurementId).FirstOrDefault(),
                //Notes = context.Notes.Where(x => x.MummyId == mummy.MummyId).FirstOrDefault()
            });
        }

        [HttpPost]
        public IActionResult AddCarbon(int id, MummyCarbonViewModel carbon)
        {
            carbon.NewCarbon.MummyId = id;
            context.Add(carbon.NewCarbon);
            context.SaveChanges();

            List<CarbonDated> carb;
            try
            {
                carb = context.CarbonDated.Where(x => x.MummyId == id).ToList();
            }
            catch
            {
                carb = new List<CarbonDated>();
            }

            Mummy mum = context.Mummy.Where(x => x.MummyId == id).FirstOrDefault();

            return View("MummyProfile", new MummyProfileViewModel
            {

                Mummy = mum,
                Location = context.Location.Where(x => x.LocationId == mum.LocationId).FirstOrDefault(),
                Measurement = context.Measurements.Where(x => x.MeasurementId == mum.MeasurementId).FirstOrDefault(),
                //Notes = context.Notes.Where(x => x.MummyId == mummy.MummyId).FirstOrDefault()
            });
        }


        [HttpPost]
        public IActionResult EditMeasurements(int id)
        {
            Measurements msr = context.Measurements.Where(x => x.MeasurementId == id).First();
            return View(msr);
        }

        [HttpPost("UpdateMeasurements")]
        public async Task<IActionResult> UpdateMeasurements(Measurements measurements)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    context.Update(measurements);
                    await context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    return NotFound();
                }
                Mummy mum = context.Mummy.Where(x => x.MeasurementId == measurements.MeasurementId).FirstOrDefault();
                return View("MummyProfile", new MummyProfileViewModel
                {
                    Mummy = mum,
                    Location = context.Location.Where(x => x.LocationId == mum.LocationId).FirstOrDefault(),
                    Measurement = context.Measurements.Where(x => x.MeasurementId == measurements.MeasurementId).FirstOrDefault()
                });
            }
            return View("UpdateMummy", measurements);
        }
        [HttpPost]
        public IActionResult EditCarbon(int id)
        {
            CarbonDated carbon = context.CarbonDated.Where(x => x.CarbonDatedId == id).First();
            return View(carbon);
        }

        [HttpPost]
        public IActionResult EditSample(int id)
        {
            Sample sample = context.Sample.Where(x => x.SampleId == id).First();
            return View(sample);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
