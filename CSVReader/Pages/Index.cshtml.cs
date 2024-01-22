using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using CSVReader.Models;
using System.IO;

namespace CSVReader.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        public List<CommonProject> commonProject = new List<CommonProject>();
        public string ErrorText;
        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {

        }

        [BindProperty]
        public IFormFile EmployeeProjectCsv { get; set; }

        public void OnPostAsync()
        {
            if (EmployeeProjectCsv != null)
            {
                List<EmployeeProjectModel> employeeProject = new List<EmployeeProjectModel>();
                
                try 
                { 
                    using (var reader = new StreamReader(EmployeeProjectCsv.OpenReadStream()))

                    {
                        string line;
                        while((line = reader.ReadLine()) != null)
                        {
                            string[] employees = line.Split(';');
                            if(employees.Count() > 0)
                            {
                                EmployeeProjectModel empPrjct = new EmployeeProjectModel()
                                {
                                    EmpID = Int32.Parse(employees[0]),
                                    ProjectID = Int32.Parse(employees[1]),
                                    DateFrom = DateTime.Parse(employees[2])
                                };

                                if (employees[3] == string.Empty || employees[3] == null || employees[3] =="NULL")
                                {
                                    empPrjct.DateTo = DateTime.Now;
                                }
                                else
                                {
                                    empPrjct.DateTo = DateTime.Parse(employees[3]);
                                }
                                employeeProject.Add(empPrjct);
                            }                  
                        }
                    }

                    var sameProject = employeeProject.GroupBy(x => x.ProjectID);

                    foreach(var project in sameProject)
                    {
                        if(project.Count() > 1)
                        {
                            EmployeeProjectModel firstEmployee = project.First();
                            EmployeeProjectModel secondEmployee = project.Last();

                            var compareStartDate = DateTime.Compare(firstEmployee.DateFrom, secondEmployee.DateFrom);

                            var compareFinalDate = DateTime.Compare((DateTime)firstEmployee.DateTo, (DateTime)secondEmployee.DateTo);

                            CommonProject prjct = new CommonProject()
                            {
                                EmpOne = firstEmployee.EmpID,
                                EmpTwo = secondEmployee.EmpID,
                                ProjectID = firstEmployee.ProjectID,
                            };

                            if (compareStartDate > 0)
                            {
                                if(compareFinalDate > 0)
                                {
                                    var daysTogether = ((DateTime)firstEmployee.DateTo).Subtract(firstEmployee.DateFrom);
                                    prjct.TimeTogether = daysTogether.Days;
                                }
                                else
                                {
                                    var daysTogether = ((DateTime)firstEmployee.DateTo).Subtract(secondEmployee.DateFrom);
                                    prjct.TimeTogether = daysTogether.Days;
                                }
                            }
                            else
                            {
                                if (compareFinalDate > 0)
                                {
                                    var daysTogether = ((DateTime)secondEmployee.DateTo).Subtract(firstEmployee.DateFrom);
                                    prjct.TimeTogether = daysTogether.Days;
                                }
                                else
                                {
                                    var daysTogether = ((DateTime)secondEmployee.DateTo).Subtract(secondEmployee.DateFrom);
                                    prjct.TimeTogether = daysTogether.Days;
                                }
                            }

                            if(prjct.TimeTogether > 0)
                            {
                                commonProject.Add(prjct);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    ErrorText = "Incorrect Format!";
                    _logger.LogError(ex.Message);
                }
            }
            else
            {
                ErrorText = "No File Found!";
            }
        }
    }
}
