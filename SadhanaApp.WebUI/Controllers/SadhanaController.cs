using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SadhanaApp.Domain;
using SadhanaApp.WebUI.ViewModels;
using System.Diagnostics;
using System.Security.Claims;

namespace SadhanaApp.WebUI.Controllers
{

    public class SadhanaController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        public SadhanaController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // Display the form to record chanting rounds

        [Authorize]
        public async Task<IActionResult> RecordSadhana()
        {
            var serviceTypes = await _context.ServiceTypes.ToListAsync();

            // Convert to SelectListItem, grouping by ServiceName
            var serviceTypeList = serviceTypes
                .GroupBy(st => st.ServiceName) // Group by ServiceName
                .Select(g => new SelectListItem
                {
                    Value = g.First().ServiceTypeId.ToString(), // Use the ServiceTypeId of the first item in each group
                    Text = g.Key // The key of the group is the ServiceName
                })
                .ToList();

            // Adding "Other" option manually
            serviceTypeList.Add(new SelectListItem { Value = "other", Text = "Other (Please Specify)" });

            // Pass the list to the view
            ViewBag.ServiceTypeList = serviceTypeList.Distinct();

            var viewModel = new ChantingViewModel
            {
                Date = DateTime.Today
            };

            return View(viewModel);
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> RecordSadhana(ChantingViewModel viewModel)
        {
            var serviceTypes = await _context.ServiceTypes.ToListAsync();

            // Convert to SelectListItem, grouping by ServiceName
            var serviceTypeList = serviceTypes
                .GroupBy(st => st.ServiceName) // Group by ServiceName
                .Select(g => new SelectListItem
                {
                    Value = g.First().ServiceTypeId.ToString(), // Use the ServiceTypeId of the first item in each group
                    Text = g.Key // The key of the group is the ServiceName
                })
                .ToList();

            // Adding "Other" option manually
            serviceTypeList.Add(new SelectListItem { Value = "other", Text = "Other (Please Specify)" });
            // Pass the list to the view
            ViewBag.ServiceTypeList = serviceTypeList.Distinct();
            ChantingRecord model = _mapper.Map<ChantingRecord>(viewModel);
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(userIdString, out var userId) && userId > 0)
            {
                model.UserId = userId;

                bool recordExists = await _context.ChantingRecords
           .AnyAsync(cr => cr.Date.Date == viewModel.Date && cr.UserId == userId);
                if (recordExists)
                {
                    ModelState.AddModelError("Date", "A record for this date already exists.");
                    return View(viewModel);
                }

                // Handle 'other' - new service type
                if (string.IsNullOrWhiteSpace(viewModel.SelectedServiceTypeId))
                {
                    model.ServiceTypeId = null; // Set ServiceTypeId to null
                }
                else if (viewModel.SelectedServiceTypeId == "other")
                {
                    var serviceType = new ServiceType
                    {
                        ServiceName = viewModel.CustomServiceTypeInput
                    };
                    _context.ServiceTypes.Add(serviceType);
                    await _context.SaveChangesAsync();
                    model.ServiceTypeId = serviceType.ServiceTypeId;

                    // Setting the navigation property to null to prevent tracking issues
                    model.ServiceType = null;
                }
                else if (int.TryParse(viewModel.SelectedServiceTypeId, out var serviceTypeId))
                {
                    model.ServiceTypeId = serviceTypeId;
                }
                else
                {
                    ModelState.AddModelError("SelectedServiceTypeId", "Invalid Service Type selected.");
                    return View(viewModel);
                }


                try
                {
                    _context.ChantingRecords.Add(model);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("SadhanaHistory");
                }
                catch (Exception ex)
                {
                    // Handle or log the exception
                    ModelState.AddModelError("", "An error occurred while saving the record: " + ex.Message);
                }
            }
            else
            {
                ModelState.AddModelError("UserId", "Invalid UserId.");
            }
            
            return View(viewModel);
        }


        // Display the chanting history of the user
        [Authorize]
        public async Task<IActionResult> SadhanaHistory(int daysFilter = 30, int page = 1, int pageSize = 10)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            // Ensure userId is not null before proceeding
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Define the date range based on filter
            var startDate = DateTime.Today.AddDays(-daysFilter);
            var endDate = DateTime.Today;

            var recordsQuery = _context.ChantingRecords
           .Where(c => c.UserId == int.Parse(userId) && c.Date.Date >= startDate && c.Date.Date <= endDate);


            // Pagination
            var totalRecords = await recordsQuery.CountAsync();
            var records = await recordsQuery
                .OrderByDescending(c => c.Date)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Generate a list of dates for this range
            var allDates = Enumerable.Range(0, (endDate - startDate).Days + 1)
                            .Select(d => startDate.AddDays(d))
                            .ToList();

            // Find missing dates
            var missingDates = allDates.Except(records.Select(r => r.Date)).ToList();

            // ViewModel to pass to the view
            var model = new SadhanaHistoryViewModel
            {
                Records = records,
                TotalRecords = totalRecords,
                CurrentPage = page,
                PageSize = pageSize,
                DaysFilter = daysFilter
            };

            return View(model);
        }

        // If the user is an instructor, display the chanting history of their students
        [Authorize(Roles = "Instructor")]
        public async Task<IActionResult> DevoteeSadhanaHistory()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var devotees = await _context.Users
                .Include(u => u.ChantingRecords)
                .Where(u => u.ShikshaGuruId == int.Parse(userId))
                .ToListAsync();

            // Convert the list of devotees into a SelectList
            ViewBag.DevoteeList = new SelectList(devotees, "UserId", "FirstName");

            return View(devotees);
        }

        [Authorize]
        public IActionResult Graph()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var id = int.Parse(userId);

            // Get the current date without the time part
            var currentDate = DateTime.Today;

            // Filter records for the last 30 days, last 12 months, and last 5 years
            var records = _context.ChantingRecords
                                  .Where(u => u.UserId == id && u.Date >= currentDate.AddDays(-30))
                                  .OrderBy(r => r.Date)
                                  .ToList();

            // For daily progress over the last 30 days:
            var dates = records.Select(r => r.Date.ToString("MM-dd-yyyy")).ToList();
            var totalScoresPerDay = records.Select(r => r.TotalScore ?? 0).ToList();

            // For monthly progress over the last 12 months:
            var monthlyRecords = _context.ChantingRecords
                                         .Where(u => u.UserId == id && u.Date >= currentDate.AddMonths(-12))
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
            var yearlyRecords = _context.ChantingRecords
                                        .Where(u => u.UserId == id && u.Date >= currentDate.AddYears(-5))
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


        // Display the Edit form
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            var record = await _context.ChantingRecords.FindAsync(id);
            if (record == null)
            {
                return NotFound();
            }
            ChantingViewModel model = _mapper.Map<ChantingViewModel>(record);

            var serviceTypes = await _context.ServiceTypes.ToListAsync();

            // Convert to SelectListItem
            var serviceTypeList = serviceTypes.Select(st => new SelectListItem
            {
                Value = st.ServiceTypeId.ToString(),
                Text = st.ServiceName
            }).ToList();

            // Adding "Other" option manually
            serviceTypeList.Add(new SelectListItem { Value = "other", Text = "Other (Please Specify)" });

            // Pass the list to the view
            ViewBag.ServiceTypeList = serviceTypeList;

            return View(model);
        }

        // Handle the Edit form submission
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Edit(int id, ChantingViewModel viewModel)
        {
            if (viewModel.SelectedServiceTypeId != "other")
            {
                ModelState.Remove("CustomServiceTypeInput"); // Clear ModelState errors for this field
            }

            if (!ModelState.IsValid)
            {              
                return View(viewModel); // Return the same view with validation messages
            }

          

            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Not required for Edit as use can update the same record
           // if (int.TryParse(userIdString, out var userId) && userId > 0)
           // {
           //     bool recordExists = await _context.ChantingRecords
           //.AnyAsync(cr => cr.Date.Date == viewModel.Date.Date && cr.UserId == userId);
           //     if (recordExists)
           //     {
           //         ModelState.AddModelError("Date", "A record for this date already exists.");
           //         return View(viewModel);
           //     }
           // }

            if (!int.TryParse(userIdString, out var user) || user <= 0)
            {
                ModelState.AddModelError("UserId", "Invalid UserId.");
                return View(viewModel);
            }

            var existingRecord = await _context.ChantingRecords.FindAsync(id);
            if (existingRecord == null)
            {
                // Handle not found
                return NotFound();
            }

            _mapper.Map(viewModel, existingRecord);

            if (viewModel.SelectedServiceTypeId == "other")
            {
                var serviceType = new ServiceType
                {
                    ServiceName = viewModel.CustomServiceTypeInput
                };
                _context.ServiceTypes.Add(serviceType);
                await _context.SaveChangesAsync();
                existingRecord.ServiceTypeId = serviceType.ServiceTypeId;
            }
            else if (int.TryParse(viewModel.SelectedServiceTypeId, out var serviceTypeId))
            {
                existingRecord.ServiceTypeId = serviceTypeId;
            }
            else
            {
                ModelState.AddModelError("SelectedServiceTypeId", "Invalid Service Type selected.");
                return View(viewModel);
            }

            try
            {
                await _context.SaveChangesAsync();
                return RedirectToAction("SadhanaHistory");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while updating the record: " + ex.Message);
                return View(viewModel);
            }
        }


        [Authorize(Roles = "Instructor")]
        public async Task<IActionResult> StudentProgressGraph()
        {
            var instructorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var viewModel = new InstructorStudentGraphViewModel();

            var students = await _context.Users
                                         .Where(u => u.ShikshaGuruId == int.Parse(instructorId))
                                         .ToListAsync();

            viewModel.Students = students.Select(s => new SelectListItem
            {
                Value = s.UserId.ToString(),
                Text = $"{s.FirstName} {s.LastName}"
            }).ToList();

            return View(viewModel);
        }

        [Authorize(Roles = "Instructor")]
        public async Task<IActionResult> GetStudentGraphData(int studentId)
        {
            try
            {
                var currentDate = DateTime.Today;

                // Fetch daily records for the last 30 days and process them in memory
                var dayRecords = await _context.ChantingRecords
                    .Where(r => r.UserId == studentId && r.Date >= currentDate.AddDays(-30))
                    .ToListAsync();

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
                var monthRecords = await _context.ChantingRecords
                    .Where(r => r.UserId == studentId && r.Date >= currentDate.AddMonths(-12))
                    .ToListAsync();

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
                var yearRecords = await _context.ChantingRecords
                    .Where(r => r.UserId == studentId && r.Date >= currentDate.AddYears(-5))
                    .ToListAsync();

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
                // Log the exception details here
                return StatusCode(500, "Internal server error");
            }
        }


    }
}
