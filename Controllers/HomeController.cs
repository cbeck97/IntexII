using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Transfer;
using BYUFagElGamous1_5.Models;
using BYUFagElGamous1_5.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace BYUFagElGamous1_5.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        private IHostingEnvironment _hostingEnvironment;
        private AmazonS3Client _s3Client = new AmazonS3Client(RegionEndpoint.EUWest2);
        private string _bucketName = "mis-pdf-library";//this is my Amazon Bucket name
        private static string _bucketSubdirectory = String.Empty;

        private readonly ILogger<HomeController> _logger;
        private FagElGamousContext context;

        public HomeController(ILogger<HomeController> logger, FagElGamousContext ctx, IHostingEnvironment environment)
        {
            _logger = logger;
            context = ctx;
            _hostingEnvironment = environment;
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

        //VIEW MUMMIES -----------------------------------------
        [HttpGet]
        public IActionResult ViewMummies(int pageNum = 1)
        {
            //Dictionary to line up each mummy with location based on location
            Dictionary<Mummy, Location> dict = new Dictionary<Mummy, Location>();

            foreach (var x in context.Mummy)
            {
                dict.Add(x, context.Location.Where(y => y.LocationId == x.LocationId).First());
            }
            int pageItems = ViewBag.numItems = 10;

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
        public async Task<IActionResult> ViewMummies([FromForm] ViewMummyViewModel viewMummy,
            string searchFor, string inputDateFilter) //Dictionary<Mummy, Location> mumLocs, Mummy dummyMummy, PageNumberInfo pageNumberInfo,
        {
            //Dictionary to line up each mummy with location based on location
            Dictionary<Mummy, Location> dict = new Dictionary<Mummy, Location>();

            foreach (var x in context.Mummy)
            {
                dict.Add(x, context.Location.Where(y => y.LocationId == x.LocationId).First());
            }
            Mummy dummyMummy = viewMummy.mummy;
            PageNumberInfo pageNumberInfo = viewMummy.PageNumberInfo;

            var field = searchFor; //Create variables and assign them values of parameter
            var value = searchFor;

            DateTime fromdate = new DateTime();
            DateTime todate = new DateTime();
            if (!string.IsNullOrEmpty(inputDateFilter))
            {
                fromdate = DateTime.Parse(inputDateFilter.Split('-')[0].Trim());
                todate = DateTime.Parse(inputDateFilter.Split('-')[1].Trim());
                todate = new DateTime(todate.Year, todate.Month, todate.Day, 23, 59, 59);
                if (fromdate == todate)
                {
                    fromdate = DateTime.Now.AddDays(-30);//last 30 days
                    todate = DateTime.Now;
                }
            }

            var results = (from m in context.Mummy
                           .Include(x => x.Location)
                           .Include(x => x.Notes)
                           select m);

            if (!string.IsNullOrEmpty(inputDateFilter))
            {
                results = from m in results
                           .Where(m => m.DayFound >= fromdate.Day && m.DayFound <= todate.Day)
                           .Where(m => m.MonthFound >= fromdate.Month && m.MonthFound <= todate.Month)
                           .Where(m => m.YearFound >= fromdate.Year && m.YearFound <= todate.Year)
                          select m;
            }

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

            if (!string.IsNullOrEmpty(searchFor))
            {
                List<string> _search = searchFor.Split(' ').ToList();

                results = (from c in results
                           where _search.Contains(c.Location.LowPairNs.ToString())
                           || _search.Contains(c.Location.HighPairNs.ToString())
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
                           || _search.Contains(c.LengthOfRemains.ToString()) //+add all colums that we need to search
                           select c);
            }
            int pageItems = ViewBag.numItems = 10;
            return View("ViewMummies", new ViewMummyViewModel
            {
                mumLocs = dict,
                Mummies = results.ToList(),
                PageNumberInfo = pageNumberInfo
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

            //try
            //{
            //    note = context.Notes.Where(x => x.MummyId == mum.MummyId).First();
            //}
            //catch
            //{
            //    note = new Notes();
            //}

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
                return PartialView(id, samples);
            }
            else
            {
                List<CarbonDated> carbons = context.CarbonDated.Where(x => x.MummyId == selector).ToList();
                return PartialView(id, carbons);
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

        //---------------------------------------
        // Image Uploading
        // --------------------------------------
        [AllowAnonymous]
        public IActionResult UploadFiles()
        {
            return View(new UploadFilesViewModel());
        }

        [HttpPost("UploadFiles")]
        [AllowAnonymous]
        public IActionResult UploadFiles(UploadFilesViewModel uploadFiles)
        {
            long size = uploadFiles.files.Sum(f => f.Length);

            foreach (var formFile in uploadFiles.files)
            {
                if (formFile.Length > 0)
                {
                    var filename = ContentDispositionHeaderValue
                            .Parse(formFile.ContentDisposition)
                            .FileName
                            .TrimStart().ToString();
                    filename = _hostingEnvironment.WebRootPath + $@"\uploads" + $@"\{formFile.FileName}";
                    size += formFile.Length;
                    using (var fs = System.IO.File.Create(filename))
                    {
                        formFile.CopyTo(fs);
                        fs.Flush();
                    }//these code snippets saves the uploaded files to the project directory

                    uploadToS3(filename);//this is the method to upload saved file to S3

                }
            }

            return RedirectToAction("ViewMummies", "Home");
        }

        public async Task UploadImage(IFormFile file)
        {
            var credentials = new BasicAWSCredentials("access", "secret key");
            var config = new AmazonS3Config
            {
                RegionEndpoint = Amazon.RegionEndpoint.EUNorth1
            };
            using var client = new AmazonS3Client(credentials, config);
            await using var newMemoryStream = new MemoryStream();
            file.CopyTo(newMemoryStream);

            var uploadRequest = new TransferUtilityUploadRequest
            {
                InputStream = newMemoryStream,
                Key = file.FileName,
                BucketName = "your-bucket-name",
                CannedACL = S3CannedACL.PublicRead
            };

            var fileTransferUtility = new TransferUtility(client);
            await fileTransferUtility.UploadAsync(uploadRequest);
        }
        public void uploadToS3(string filePath)
        {
            try
            {
                TransferUtility fileTransferUtility = new
                    TransferUtility(new AmazonS3Client(Amazon.RegionEndpoint.EUWest2));

                string bucketName;


                if (_bucketSubdirectory == "" || _bucketSubdirectory == null)
                {
                    bucketName = _bucketName; //no subdirectory just bucket name  
                }
                else
                {   // subdirectory and bucket name  
                    bucketName = _bucketName + @"/" + _bucketSubdirectory;
                }

                // 1. Upload a file, file name is used as the object key name.
                fileTransferUtility.Upload(filePath, bucketName);
                Console.WriteLine("Upload 1 completed");
            }
            catch (AmazonS3Exception s3Exception)
            {
                Console.WriteLine(s3Exception.Message,
                                  s3Exception.InnerException);
            }
        }
        // End Image Uploading
        //----------------------------------------

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


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
