using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Controller;
using SadhanaApp.Application.Common.Interfaces;
using SadhanaApp.Domain;
using SadhanaApp.WebUI.ViewModels;
using System.Diagnostics;
using System.Security.Claims;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;

namespace SadhanaApp.WebUI.Controllers
{

    public class SadhanaController : Controller
    {

        private readonly IMapper _mapper;
        private readonly ILogger<SadhanaController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public SadhanaController(IMapper mapper, ILogger<SadhanaController> logger, IUnitOfWork unitOfWork)
        {

            _mapper = mapper;
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        // Display the form to record chanting rounds

        [Authorize]
        public async Task<IActionResult> RecordSadhana(string? date)
        {
            try
            {
                _logger.LogInformation("Entering RecordSadhana action method.");
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

                var serviceTypes = _unitOfWork.ServiceRepository.GetAll(st => st.UserId == userId && !st.IsDeleted && !st.IsHidden).ToList();

                

                // List of custom service types to be included every time
                var customServiceTypes = new List<string> { "Others" };


                var serviceTypeList = serviceTypes
                    .Select(st => new SelectListItem
                    {
                        Value = st.ServiceName,
                        Text = st.ServiceName
                    })
                    .ToList();

                // Add custom service types to the list, ensuring they are unique

                foreach (var customType in customServiceTypes)
                {
                    if (!serviceTypeList.Any(st => st.Text == customType))
                    {
                        serviceTypeList.Add(new SelectListItem { Value = customType, Text = customType });
                    }
                }


                ViewBag.ServiceTypeList = serviceTypeList;

                var viewModel = new ChantingViewModel
                {
                    Date = GetNewZealandTime(),
                    SelectedServiceTypeNames = new List<string>() // Initialize the list
                };

               
                if (date != null)
                {
                    viewModel.Date = DateTime.Parse(date);
                }
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in RecordSadhana method for user {UserId}.", User.FindFirstValue(ClaimTypes.NameIdentifier));
                return RedirectToAction("Error", "Home");
            }
        }

        private DateTime GetNewZealandTime()
        {
            var nzTimeZone = TimeZoneInfo.FindSystemTimeZoneById("New Zealand Standard Time");
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, nzTimeZone);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> RecordSadhana(ChantingViewModel viewModel)
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

                bool recordExists = _unitOfWork.SadhanaRepository.GetAll(cr => cr.Date.Date == viewModel.Date && cr.UserId == userId).Any();
                var serviceTypes = _unitOfWork.ServiceRepository.GetAll(st => st.UserId == userId && !st.IsDeleted && !st.IsHidden).ToList();

                // List of custom service types to be included every time
                var customServiceTypes = new List<string> { "Others" };


                var serviceTypeList = serviceTypes
                    .Select(st => new SelectListItem
                    {
                        Value = st.ServiceName,
                        Text = st.ServiceName
                    })
                    .ToList();

                // Add custom service types to the list, ensuring they are unique

                foreach (var customType in customServiceTypes)
                {
                    if (!serviceTypeList.Any(st => st.Text == customType))
                    {
                        serviceTypeList.Add(new SelectListItem { Value = customType, Text = customType });
                    }
                }
                ViewBag.ServiceTypeList = serviceTypeList;
                if (!ModelState.IsValid)
                {
                    return View(viewModel); // Return the view with validation errors
                }


                if (recordExists)
                {
                    TempData["error"] = "A record for this date already exists.";
                    return View(viewModel);
                }

                ChantingRecord model = _mapper.Map<ChantingRecord>(viewModel);
                model.UserId = userId;

                // Set whether 'Others' service type is selected
                model.IsOtherServiceTypeSelected = viewModel.SelectedServiceTypeNames.Contains("Others");

                // If 'Others' is selected, store the custom input; otherwise, store the semicolon-separated string of service type names
                model.CustomServiceTypeInput = model.IsOtherServiceTypeSelected ? viewModel.CustomServiceTypeInput : null;
                model.ServiceTypeNames = model.IsOtherServiceTypeSelected ? null : viewModel.SelectedServiceTypeNamesAsString;

                //_context.ChantingRecords.Add(model);
                _unitOfWork.SadhanaRepository.Add(model);
                TempData["success"] = "Your chanting record has been created successfully.";
                _unitOfWork.Save();

                return RedirectToAction("SadhanaHistory");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in RecordSadhana post method for user {UserId}.", User.FindFirstValue(ClaimTypes.NameIdentifier));
                return RedirectToAction("Error", "Home");
            }
        }

        // Display the chanting history of the user   

        [Authorize]
        public async Task<IActionResult> SadhanaHistory(int offset = 0, int days = 7)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                var endDate = GetNewZealandTime();
                endDate = endDate.AddDays(-offset); // Adjust based on the offset

                var startDate = endDate.AddDays(-days + 1);

                var recordsQuery = _unitOfWork.SadhanaRepository.GetAll(
                    c => c.UserId == int.Parse(userId) && c.Date.Date >= startDate && c.Date.Date <= endDate
                );

                var records = recordsQuery.OrderByDescending(c => c.Date).ToList();

                var model = new SadhanaHistoryViewModel
                {
                    Records = records,
                    WeekOffset = offset,
                    StartDate = startDate,
                    EndDate = endDate,
                    Days = days
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in SadhanaHistory method for user {UserId}.", User.FindFirstValue(ClaimTypes.NameIdentifier));
                return RedirectToAction("Error", "Home");
            }
        }



        [Authorize(Roles = "Instructor")]
        public async Task<IActionResult> DevoteeSadhanaHistory()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                //var devotees = await _context.Users
                //    .Where(u => u.ShikshaGuruId == int.Parse(userId))
                //    .ToListAsync();

                var devotees = _unitOfWork.UserRepository.GetAll(u => u.ShikshaGuruId == int.Parse(userId)).ToList();
                ViewBag.DevoteeList = new SelectList(devotees, "UserId", "FirstName");

                return View(devotees);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in DevoteeSadhanaHistory method for user {UserId}.", User.FindFirstValue(ClaimTypes.NameIdentifier));
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpGet]
        [Authorize(Roles = "Instructor")]
        public async Task<IActionResult> GetDevoteeChantingRecords(int devoteeId, int days = 7)
        {
            try
            {
                DateTime endDate = DateTime.Now;
                DateTime startDate = endDate.AddDays(-days);

                var chantingRecords = _unitOfWork.SadhanaRepository.GetAll(
                    c => c.UserId == devoteeId && c.Date.Date >= startDate && c.Date.Date <= endDate
                ).ToList();

                // Future change - Might want to convert the records to a suitable format or DTO
                // to send as JSON, depending on your application's needs

                return Json(chantingRecords);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in GetDevoteeChantingRecords method.");
                return Json(new { success = false, message = ex.Message });
            }
        }


        [Authorize]
        public IActionResult Graph()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var id = int.Parse(userId);

                // Get the current date without the time part
                var currentDate = DateTime.Today;

                // Filter records for the last 30 days, last 12 months, and last 5 years

                var records = _unitOfWork.SadhanaRepository.GetAll(c => c.UserId == id && c.Date >= currentDate.AddDays(-30))
                                          .OrderBy(r => r.Date)
                                          .ToList();

                // For daily progress over the last 30 days:

                var dates = records.Select(r => r.Date.ToString("MM-dd-yyyy")).ToList();

                var totalScoresPerDay = records.Select(r => r.TotalScore ?? 0).ToList();

                var monthlyRecords = _unitOfWork.SadhanaRepository.GetAll(c => c.UserId == id && c.Date >= currentDate.AddMonths(-12))
                                                 .ToList();

                var monthlyData = monthlyRecords.GroupBy(r => new { r.Date.Year, r.Date.Month })
                                                .Select(g => new
                                                {
                                                    Month = $"{g.Key.Month}-{g.Key.Year}",
                                                    TotalScore = g.Sum(x => x.TotalScore) ?? 0
                                                })
                                                .ToList();
                var months = monthlyData.Select(m => m.Month).ToList();
                var totalScoresPerMonth = monthlyData.Select(m => m.TotalScore).ToList();

                // For yearly progress over the last 5 years:
                var yearlyRecords = _unitOfWork.SadhanaRepository.GetAll(c => c.UserId == id && c.Date >= currentDate.AddYears(-5))
                                                .ToList();
                var yearlyData = yearlyRecords.GroupBy(r => r.Date.Year)
                                              .Select(g => new
                                              {
                                                  Year = g.Key.ToString(),
                                                  TotalScore = g.Sum(x => x.TotalScore) ?? 0
                                              })
                                              .ToList();
                var years = yearlyData.Select(y => y.Year).ToList();
                var totalScoresPerYear = yearlyData.Select(y => y.TotalScore).ToList();

                var viewModel = new GraphViewModel
                {
                    Dates = dates,
                    TotalScoresPerDay = totalScoresPerDay,
                    Months = months,
                    TotalScoresPerMonth = totalScoresPerMonth,
                    Years = years,
                    TotalScoresPerYear = totalScoresPerYear
                };

                return View(viewModel);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in Graph method for user {UserId}.", User.FindFirstValue(ClaimTypes.NameIdentifier));
                return RedirectToAction("Error", "Home");
            }
        }

        // Display the Edit form
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

                var record = _unitOfWork.SadhanaRepository.Get(c => c.Id == id && c.UserId == userId);

                if (record == null || record.UserId != userId)
                {
                    return RedirectToAction("Error", "Home");
                }


                ChantingViewModel model = _mapper.Map<ChantingViewModel>(record);

                //var serviceTypes = await _context.ServiceTypes
                //    .Where(st => st.UserId == userId)
                //    .ToListAsync();

                var serviceTypes = _unitOfWork.ServiceRepository.GetAll(st => st.UserId == userId && !st.IsDeleted && !st.IsHidden).ToList();


                // Create SelectListItem list from serviceTypes
                var serviceTypeList = serviceTypes.Select(st => new SelectListItem
                {
                    Value = st.ServiceName,
                    Text = st.ServiceName
                }).ToList();

                // Extract common services
                var commonServices = GetCommonServices(userId, record.Id);

                // Include common services in the serviceTypeList
                foreach (var service in commonServices)
                {
                    serviceTypeList.Add(new SelectListItem { Value = service, Text = service });
                }

                // Add the "Others" option
                serviceTypeList.Add(new SelectListItem { Value = "Others", Text = "Others" });

                // Assign the serviceTypeList to ViewBag
                ViewBag.ServiceTypeList = serviceTypeList;


                // Assuming `ServiceTypeNames` in `ChantingRecord` holds the semicolon-separated service type names
                // Split the string into a list of names and then map them to their corresponding IDs for the SelectList
                if (!string.IsNullOrWhiteSpace(record.ServiceTypeNames))
                {
                    var selectedNames = record.ServiceTypeNames.Split(';').ToList();
                    model.SelectedServiceTypeNames = selectedNames;
                }

                // Handle custom service type input
                if (record.IsOtherServiceTypeSelected)
                {
                    model.CustomServiceTypeInput = record.CustomServiceTypeInput;
                }

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in Edit Get method for user {UserId}.", User.FindFirstValue(ClaimTypes.NameIdentifier));
                return RedirectToAction("Error", "Home");
            }
        }


        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Edit(int id, ChantingViewModel viewModel)
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

                var existingRecord = _unitOfWork.SadhanaRepository.Get(cr => cr.Id == id && cr.UserId == userId);

                if (existingRecord == null)
                {
                    return NotFound();
                }

                // Map the common properties
                _mapper.Map(viewModel, existingRecord);

                // Handle custom service type input
                if (viewModel.SelectedServiceTypeNames.Contains("Others"))
                {
                    existingRecord.IsOtherServiceTypeSelected = true;
                    existingRecord.CustomServiceTypeInput = viewModel.CustomServiceTypeInput;
                    existingRecord.ServiceTypeNames = null;
                }
                else
                {
                    existingRecord.IsOtherServiceTypeSelected = false;
                    existingRecord.CustomServiceTypeInput = null;
                }

                existingRecord.ServiceTypeNames = viewModel.SelectedServiceTypeNamesAsString;

                try
                {
                    _unitOfWork.SadhanaRepository.Update(existingRecord);
                    _unitOfWork.Save();
                    TempData["success"] = "Your chanting record has been updated successfully.";
                    return RedirectToAction("SadhanaHistory");
                }
                catch (Exception ex)
                {
                    var serviceTypes = _unitOfWork.ServiceRepository.GetAll(st => st.UserId == userId && !st.IsDeleted && !st.IsHidden).ToList();

                    // Create SelectListItem list from serviceTypes
                    var serviceTypeList = serviceTypes.Select(st => new SelectListItem
                    {
                        Value = st.ServiceName,
                        Text = st.ServiceName
                    }).ToList();

                    // Extract common services
                    var commonServices = GetCommonServices(userId, existingRecord.Id);

                    // Include common services in the serviceTypeList
                    foreach (var service in commonServices)
                    {
                        serviceTypeList.Add(new SelectListItem { Value = service, Text = service });
                    }

                    // Add the "Others" option
                    serviceTypeList.Add(new SelectListItem { Value = "Others", Text = "Others" });

                    // Assign the serviceTypeList to ViewBag
                    ViewBag.ServiceTypeList = serviceTypeList;

                    ModelState.AddModelError("", "An error occurred while updating the record: " + ex.Message);
                    return View(viewModel);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in Edit Post method for user {UserId}.", User.FindFirstValue(ClaimTypes.NameIdentifier));
                return RedirectToAction("Error", "Home");
            }
        }





        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                // var record = await _context.ChantingRecords.FindAsync(id);
                var record = _unitOfWork.SadhanaRepository.Get(c => c.Id == id);
                if (record == null)
                {
                    TempData["error"] = "Your chanting record could not be found.";
                    return Json(new { success = false, message = "Record not found." });
                }

                //_context.ChantingRecords.Remove(record);
                _unitOfWork.SadhanaRepository.Remove(record);
                _unitOfWork.Save();
                TempData["success"] = "Your chanting record has been deleted successfully.";
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in Delete Post method for user {UserId}.", User.FindFirstValue(ClaimTypes.NameIdentifier));
                return Json(new { success = false, message = "An error occurred while deleting the record." });
            }
        }





        //[Authorize(Roles = "Instructor")]
        //public async Task<IActionResult> StudentProgressGraph()
        //{
        //    try
        //    {
        //        var instructorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //        var viewModel = new InstructorStudentGraphViewModel();

        //        var students = _unitOfWork.UserRepository.GetAll(u => u.ShikshaGuruId == int.Parse(instructorId)).ToList();
        //        viewModel.Students = students.Select(s => new SelectListItem
        //        {
        //            Value = s.UserId.ToString(),
        //            Text = $"{s.FirstName} {s.LastName}"
        //        }).ToList();

        //        return View(viewModel);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error occurred in StudentProgressGraph method for user {UserId}.", User.FindFirstValue(ClaimTypes.NameIdentifier));
        //        return RedirectToAction("Error", "Home");
        //    }
        //}

        [Authorize(Roles = "Instructor")]
        public async Task<IActionResult> StudentProgressGraph()
        {
            try
            {
                var instructorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var viewModel = new InstructorStudentGraphViewModel();

                // Get students
                var students = _unitOfWork.UserRepository.GetAll(u => u.ShikshaGuruId == int.Parse(instructorId)).ToList();
                viewModel.Students = students.Select(s => new SelectListItem
                {
                    Value = s.UserId.ToString(),
                    Text = $"{s.FirstName} {s.LastName}"
                }).ToList();

                // Assuming you have a similar method to fetch Sadhana records for students
                // This is where you would add similar logic as in the Graph method
                // For each student, fetch their Sadhana records and aggregate them
                // You might need to adjust your ViewModel to accommodate this data

                // Example (you'll need to adjust based on your actual data structure):
                foreach (var student in students)
                {
                    var currentDate = DateTime.Today;
                    var studentRecords = _unitOfWork.SadhanaRepository.GetAll(c => c.UserId == student.UserId && c.Date >= currentDate.AddDays(-30))
                                                     .OrderBy(r => r.Date)
                                                     .ToList();

                    // Process records similar to the Graph method
                    // Aggregate daily, monthly, and yearly data
                    // Add this data to the ViewModel
                }

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in StudentProgressGraph method for user {UserId}.", User.FindFirstValue(ClaimTypes.NameIdentifier));
                return RedirectToAction("Error", "Home");
            }
        }


        [Authorize(Roles = "Instructor")]
        public async Task<IActionResult> GetStudentGraphData(int studentId)
        {
            try
            {
                var currentDate = DateTime.Today;

                // Fetch daily records for the last 30 days and process them in memory

                var dayRecords = _unitOfWork.SadhanaRepository.GetAll(r => r.UserId == studentId && r.Date >= currentDate.AddDays(-30)).ToList();

                var dailyData = dayRecords
                    .GroupBy(r => r.Date)
                    .Select(g => new
                    {
                        Date = g.Key.ToString("MM-dd-yyyy"),
                        TotalScore = g.Sum(x => x.TotalScore ?? 0)
                    })
                    .OrderBy(g => g.Date)
                    .ToList();

                // Fetch monthly records for the last 12 months and process them in memory

                var monthRecords = _unitOfWork.SadhanaRepository.GetAll(r => r.UserId == studentId && r.Date >= currentDate.AddMonths(-12)).ToList();

                var monthlyData = monthRecords
                    .GroupBy(r => new { Year = r.Date.Year, Month = r.Date.Month })
                    .Select(g => new
                    {
                        Month = $"{g.Key.Month}/{g.Key.Year}",
                        TotalScore = g.Sum(x => x.TotalScore ?? 0)
                    })
                    .OrderBy(g => g.Month)
                    .ToList();

                // Fetch yearly records for the last 5 years and process them in memory

                var yearRecords = _unitOfWork.SadhanaRepository.GetAll(r => r.UserId == studentId && r.Date >= currentDate.AddYears(-5)).ToList();

                var yearlyData = yearRecords
                    .GroupBy(r => r.Date.Year)
                    .Select(g => new
                    {
                        Year = g.Key.ToString(),
                        TotalScore = g.Sum(x => x.TotalScore ?? 0)
                    })
                    .OrderBy(g => g.Year)
                    .ToList();

                var graphData = new
                {
                    Daily = dailyData,
                    Monthly = monthlyData,
                    Yearly = yearlyData
                };

                return Json(graphData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in GetStudentGraphData method for user {UserId}.", User.FindFirstValue(ClaimTypes.NameIdentifier));
                return RedirectToAction("Error", "Home");
            }
        }

        [Authorize]
        public async Task<IActionResult> GetMissingDates()
        {
            try
            {
                // Retrieve the user's unique identifier from the claims
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                // Retrieve the user's registration date from the database
                // Replace this with the actual method to get the user's registration date
                var userRegistrationDate = await GetUserRegistrationDate(int.Parse(userId));

                var startDate = userRegistrationDate.Date;
                var endDate = DateTime.Today;

                // Fetch the records from the database for the user since their registration date

                var records = _unitOfWork.SadhanaRepository.GetAll(c => c.UserId == int.Parse(userId) && c.Date.Date >= startDate && c.Date.Date <= endDate)
                                          .Select(c => c.Date)
                                          .ToList();

                // Generate a list of all dates from the user's registration date to today
                var allDates = Enumerable.Range(0, (endDate - startDate).Days + 1)
                                .Select(d => startDate.AddDays(d))
                                .ToList();

                // Find the dates where there are no records
                var missingDates = allDates.Except(records).ToList();

                // Return the partial view with the missing dates
                return PartialView("_MissingDatesPartial", missingDates);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in GetMissingDates method for user {UserId}.", User.FindFirstValue(ClaimTypes.NameIdentifier));
                return RedirectToAction("Error", "Home");
            }
        }

        private async Task<DateTime> GetUserRegistrationDate(int userId)
        {
            try
            {
                var user = _unitOfWork.UserRepository.Get(u => u.UserId == userId);
                return user.DateRegistered;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in GetUserRegistrationDate method for user {UserId}.", User.FindFirstValue(ClaimTypes.NameIdentifier));
                throw;
            }
        }

        public async Task<IActionResult> GetReadingTitles(string term)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var readingTitles = _unitOfWork.SadhanaRepository.GetAll(c => c.UserId == int.Parse(userId) &&
                                                                           c.ReadingTitle.Contains(term))
                                                .Select(c => c.ReadingTitle)
                                                .Distinct()
                                                .ToList();

                return Json(readingTitles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in GetReadingTitles method for user {UserId}.", User.FindFirstValue(ClaimTypes.NameIdentifier));
                throw;
            }
        }

        public async Task<IActionResult> GetHearingTitles(string term)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);


                var readingTitles = _unitOfWork.SadhanaRepository.GetAll(c => c.UserId == int.Parse(userId) &&
                                                                                          c.HearingTitle.Contains(term))
                                                .Select(c => c.HearingTitle)
                                                .Distinct()
                                                .ToList();

                return Json(readingTitles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in GetHearingTitles method for user {UserId}.", User.FindFirstValue(ClaimTypes.NameIdentifier));
                throw;
            }
        }


        public async Task<IActionResult> GetCustomServiceTypes(string term)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            // Fetch the list of CustomServiceTypeInputs where IsOtherServiceTypeSelected is true
            var customServiceTypes = _unitOfWork.SadhanaRepository.GetAll(c => c.UserId == int.Parse(userId) &&
                                 c.IsOtherServiceTypeSelected && c.CustomServiceTypeInput.Contains(term))
                                 .Select(c => c.CustomServiceTypeInput)
                                                .Distinct()
                                                .ToList(); ;


            return Json(customServiceTypes);
        }


        private List<string> GetCommonServices(int userId, int recordId)
        {
            var hiddenServices = _unitOfWork.ServiceRepository.GetAll(st => st.UserId == userId && !st.IsDeleted && st.IsHidden)
                    .Select(entity => entity.ServiceName.Trim()) // Trimming each service name
            .Distinct()
            .ToList();

            var usedServices = _unitOfWork.SadhanaRepository.GetAll(st => st.UserId == userId && st.Id == recordId)
                   .SelectMany(entity =>
                       entity.ServiceTypeNames?.Split(';') ?? Array.Empty<string>())
                   .Select(name => name.Trim())
                   .Distinct()
                   .ToList();


            // Finding common elements between hiddenServices and usedServices
            var commonServices = hiddenServices.Intersect(usedServices).ToList();
            return commonServices;
        }


    }
}
