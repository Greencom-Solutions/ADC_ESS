using Latest_Staff_Portal.Models;
using Latest_Staff_Portal.ViewModel;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ZXing;

namespace Latest_Staff_Portal.Controllers
{
    public class TechnicalController : Controller
    {
        //Lab Sample management
        public ActionResult LabSampleManagementList()
        {
            try
            {
                if (Session["Username"] == null)
                    return RedirectToAction("Login", "Login");
                return View();
            }
            catch (Exception ex)
            {
                var erroMsg = new Error();
                erroMsg.Message = ex.Message.Replace("'", "");
                return View("~/Views/Common/ErrorMessange.cshtml", erroMsg);
            }
        }
        public PartialViewResult LabSampleManagementListPartialView()
        {
            try
            {
                var UserID = Session["UserID"].ToString();
                var StaffNo = Session["Username"].ToString();
                var employeeView = Session["EmployeeData"] as EmployeeView;
                var SampleList = new List<LabSampleManagement>();


                //var page = $"LabSampleManagement?$filter=Created_By eq '{StaffNo}'&$format=json";
                var page = $"LabSampleManagement?$format=json";

                var httpResponse = Credentials.GetOdataData(page);
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    var details = JObject.Parse(result);

                    foreach (JObject config in details["value"])
                    {
                        var sample = new LabSampleManagement
                        {
                            Sample_ID = (string)config["Sample_ID"],
                            Month_Date = (string)config["Month_Date"],
                            Month_Name = (string)config["Month_Name"],
                            Global_Dimension_1_Code = (string)config["Global_Dimension_1_Code"],
                            Global_Dimension_2_Code = (string)config["Global_Dimension_2_Code"],
                            Description = (string)config["Description"],
                            Received_At = (string)config["Received_At"],
                            Analysed_At = (string)config["Analysed_At"],
                            Exported_At = (string)config["Exported_At"],
                            TurnAround_Time = (string)config["TurnAround_Time"],
                            Status = (string)config["Status"],
                            Created_By = (string)config["Created_By"],
                            Staff_No = (string)config["Staff_No"],
                            Staff_Name = (string)config["Staff_Name"],
                            Time_Created = (string)config["Time_Created"],
                            Date_Created = (string)config["Date_Created"]
                        };

                        SampleList.Add(sample);
                    }
                }
                return PartialView("~/Views/Technical/PartialViews/LabSampleManagementListPartialView.cshtml", SampleList.OrderByDescending(x => x.Sample_ID));
            }
            catch (Exception ex)
            {
                var erroMsg = new Error
                {
                    Message = ex.Message.Replace("'", "")
                };
                return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
            }
        }

        [HttpPost]
        public ActionResult LabSampleManagementDocumentView(string Sample_ID)
        {
            try
            {
                if (Session["Username"] == null)
                    return RedirectToAction("Login", "Login");
                var StaffNo = Session["Username"].ToString();
                var LabSample = new LabSampleManagement();
                var page = "LabSampleManagement?$filter=Sample_ID eq '" + Sample_ID + "'&$format=json";
                var httpResponse = Credentials.GetOdataData(page);
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    var details = JObject.Parse(result);
                    foreach (JObject config in details["value"])
                    {
                        LabSample = new LabSampleManagement
                        {
                            Sample_ID = (string)config["Sample_ID"],           
                            Month_Date = (string)config["Month_Date"],
                            Month_Name = (string)config["Month_Name"],
                            Global_Dimension_1_Code = (string)config["Global_Dimension_1_Code"],
                            Global_Dimension_2_Code = (string)config["Global_Dimension_2_Code"],
                            Description = (string)config["Description"],
                            Received_At = (string)config["Received_At"],
                            Analysed_At = (string)config["Analysed_At"],
                            Exported_At = (string)config["Exported_At"],
                            TurnAround_Time = (string)config["TurnAround_Time"],
                            Status = (string)config["Status"],
                            Created_By = (string)config["Created_By"],                   
                            Staff_No = (string)config["Staff_No"],
                            Staff_Name = (string)config["Staff_Name"],
                            Time_Created = (string)config["Time_Created"],
                            Date_Created = (string)config["Date_Created"]

                        };
                    }
                }

                return View(LabSample);
            }
            catch (Exception ex)
            {
                var erroMsg = new Error
                {
                    Message = ex.Message.Replace("'", "")
                };
                return View("~/Views/Common/ErrorMessange.cshtml", erroMsg);
            }
        }
        public PartialViewResult LabSampleManagementLinesPartialView(string Sample_ID, string Status)
        {
            try
            {
                var UserID = Session["UserID"].ToString();
                var StaffNo = Session["Username"].ToString();
                var LabSampleLines = new List<LabSampleMgtLines>();
                var employeeView = Session["EmployeeData"] as EmployeeView;
                var role = Session["ESSRoleSetup"] as ESSRoleSetup;
                var page = $"LabSampleMgtLines?$filter=Sample_ID eq '{Sample_ID}'&$format=json";

                var httpResponse = Credentials.GetOdataData(page);
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    var details = JObject.Parse(result);

                    foreach (JObject config in details["value"])
                    {
                        var LabSampleLine = new LabSampleMgtLines
                        {
                            Line_No = (int)config["Line_No"],
                            Sample_ID = (string)config["Sample_ID"],
                            Sample_Code = (string)config["Sample_Code"],
                            Type_of_Sampling = (string)config["Type_of_Sampling"],
                            Sample_Description = (string)config["Sample_Description"],
                            Sample_Type = (string)config["Sample_Type"],
                            Source = (string)config["Source"],
                            Storage_Location = (string)config["Storage_Location"],
                            Sampling_Date = (string)config["Sampling_Date"],
                            Received_At = (string)config["Received_At"],
                            Tested = (bool)config["Was_it_Tested_x003F_"],
                            TestedBy = (string)config["By_Who_x003F_"],
                            Analysed_At = (string)config["Analysed_At"],
                            Results = (string)config["Results"],
                            Exported_At = (string)config["Exported_At"],
                            TurnAround_Time = (string)config["TurnAround_Time"],
                            Staff_No = (string)config["Staff_No"],
                            Staff_Name = (string)config["Staff_Name"],
                            Quantity_ml = (string)config["Quantity_ml"],
                            Notes = (string)config["Notes"]

                        };



                        LabSampleLines.Add(LabSampleLine);
                    }
                }
                ViewBag.DocNo = Sample_ID;
                ViewBag.Status = Status;
                return PartialView("~/Views/Technical/PartialViews/LabSampleManagementLinesPartialView.cshtml", LabSampleLines.OrderByDescending(x => x.Line_No));
            }
            catch (Exception ex)
            {
                var erroMsg = new Error
                {
                    Message = ex.Message.Replace("'", "")
                };
                return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
            }
        }

        public ActionResult NewLabSampleManagement()
        {
            try
            {
                var UserID = Session["UserID"].ToString();
                var StaffNo = Session["Username"].ToString();
                var employeeView = Session["EmployeeData"] as EmployeeView;

                if (Session["Username"] == null)
                {
                    return RedirectToAction("Login", "Login");
                }

                var labSample = new LabSampleManagement();

                #region Employees
                var EmpleyeeList = new List<DropdownList>();
                var pageWp = $"EmployeeList?$format=json";
                var httpResponseWp = Credentials.GetOdataData(pageWp);
                using (var streamReader = new StreamReader(httpResponseWp.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    var details = JObject.Parse(result);

                    foreach (var jToken in details["value"])
                    {
                        var config1 = (JObject)jToken;
                        var dropdownList = new DropdownList
                        {
                            Text = $"{(string)config1["FullName"]} - {(string)config1["No"]}",
                            Value = (string)config1["No"]
                        };
                        EmpleyeeList.Add(dropdownList);
                    }
                }
                #endregion

                //labSample.ListOfEmployees = EmpleyeeList.Select(x =>
                //    new SelectListItem
                //    {
                //        Text = x.Text,
                //        Value = x.Value
                //    }).ToList();

                return PartialView("~/Views/Technical/PartialViews/NewLabSampleManagement.cshtml", labSample);
            }
            catch (Exception ex)
            {
                var erroMsg = new Error();
                erroMsg.Message = ex.Message;
                return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
            }
        }
        public JsonResult SubmitLabSampleManagement(LabSampleManagement labSample)
        {
            try
            {
                var UserID = Session["UserID"].ToString();
                var StaffNo = Session["Username"].ToString();
                var employeeView = Session["EmployeeData"] as EmployeeView;
                DateTime receivedAt = DateTime.ParseExact(labSample.Received_At.Replace("-", "/"), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                DateTime analysedAt = DateTime.ParseExact(labSample.Analysed_At.Replace("-", "/"), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                DateTime exportedAt = DateTime.ParseExact(labSample.Exported_At.Replace("-", "/"), "dd/MM/yyyy", CultureInfo.InvariantCulture);


                string res2 = "";
                res2 = Credentials.ObjNav.InsertLabSample(
                    labSample.Description,
                    receivedAt,
                    analysedAt,
                    exportedAt,
                    UserID
                );

                if (res2 != "")
                {
                    var redirect = res2;

                    return Json(new { message = redirect, success = true }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var redirect = "Error adding record. Try again";
                    return Json(new { message = redirect, success = false }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message.Replace("'", ""), success = false },
                    JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult NewLabSampleLine(string Sample_ID)
        {
            try
            {
                var UserID = Session["UserID"].ToString();
                var StaffNo = Session["Username"].ToString();
                var employeeView = Session["EmployeeData"] as EmployeeView;

                if (Session["Username"] == null)
                {
                    return RedirectToAction("Login", "Login");
                }

                var labSampleLines = new LabSampleMgtLines();

                labSampleLines.Sample_ID = Sample_ID;



                #region Dim1
                var Dim1List = new List<DropdownList>();
                var pageDim1 = $"DimensionValueList?$filter=Dimension_Code eq 'REGIONS'&$format=json";
                var httpResponseDim1 = Credentials.GetOdataData(pageDim1);
                using (var streamReader = new StreamReader(httpResponseDim1.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    var details = JObject.Parse(result);

                    foreach (var jToken in details["value"])
                    {
                        var config1 = (JObject)jToken;
                        var dropdownList = new DropdownList
                        {
                            Text = $"{(string)config1["Name"]}",
                            Value = (string)config1["Code"]
                        };
                        Dim1List.Add(dropdownList);
                    }
                }
                #endregion



                #region Employees
                var EmpleyeeList = new List<DropdownList>();
                var pageWp = $"Employees?$format=json";
                var httpResponseWp = Credentials.GetOdataData(pageWp);
                using (var streamReader = new StreamReader(httpResponseWp.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    var details = JObject.Parse(result);

                    foreach (var jToken in details["value"])
                    {
                        var config1 = (JObject)jToken;
                        var dropdownList = new DropdownList
                        {
                            Text = $"{(string)config1["FullName"]} - {(string)config1["No"]}",
                            Value = (string)config1["No"]
                        };
                        EmpleyeeList.Add(dropdownList);
                    }
                }
                #endregion



                labSampleLines.ListOfRegions = Dim1List.Select(x =>
                    new SelectListItem
                    {
                        Text = x.Text,
                        Value = x.Value
                    }).ToList();

                labSampleLines.ListOfEmployees = EmpleyeeList.Select(x =>
                    new SelectListItem
                    {
                        Text = x.Text,
                        Value = x.Value
                    }).ToList();


                return PartialView("~/Views/Technical/PartialViews/NewLabSampleLine.cshtml", labSampleLines);
            }
            catch (Exception ex)
            {
                var erroMsg = new Error();
                erroMsg.Message = ex.Message;
                return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
            }
        }
        public JsonResult SubmitLabSampleLine(LabSampleMgtLines data)
        {
            try
            {
                var UserID = Session["UserID"].ToString();
                var StaffNo = Session["Username"].ToString();
                var employeeView = Session["EmployeeData"] as EmployeeView;

                if (string.IsNullOrEmpty(data.TestedBy))
                {
                    data.TestedBy = "";
                }

                if (string.IsNullOrEmpty(data.Analysed_At))
                {
                    data.Analysed_At = DateTime.Now.ToString("dd/MM/yyyy");
                }

                if (string.IsNullOrEmpty(data.Results))
                {
                    data.Results = "";
                }


                DateTime ReceivedAt = DateTime.ParseExact(data.Received_At.Replace("-", "/"), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                DateTime AnalysedAt = DateTime.ParseExact(data.Analysed_At.Replace("-", "/"), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                DateTime SamplingDate = DateTime.ParseExact(data.Sampling_Date.Replace("-", "/"), "dd/MM/yyyy", CultureInfo.InvariantCulture);


                int createRes= 0;
                bool updateRes = false;


                if (data.Line_No==0)
                {
                    createRes = Credentials.ObjNav.InsertSampleLine(
                       data.Sample_ID,
                       data.Sample_Type,
                       StaffNo,
                       data.Quantity_ml.ToString(),
                       data.Storage_Location,
                       data.Notes,
                       data.Source,
                       data.Sample_Code,
                       int.Parse(data.Type_of_Sampling),
                       data.Sample_Description,
                       ReceivedAt,
                       AnalysedAt,
                       SamplingDate,
                       data.Tested,
                       data.TestedBy,
                       data.Results

                   );



                    if (createRes != 0 )
                    {
                        var redirect = createRes;
                        return Json(new { message = redirect, success = true }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        var redirect = "Error submitting record. Try again";
                        return Json(new { message = redirect, success = false }, JsonRequestBehavior.AllowGet);
                    }

                }
                else {
                    updateRes=Credentials.ObjNav.ModifySampleLine(
                        data.Line_No,
                           data.Sample_ID,
                           data.Sample_Type,
                           StaffNo,
                           data.Quantity_ml.ToString(),
                           data.Storage_Location,
                           data.Notes,
                           data.Source,
                           data.Sample_Code,
                           int.Parse(data.Type_of_Sampling),
                           data.Sample_Description,
                           ReceivedAt,
                           AnalysedAt,
                           SamplingDate,
                           data.Tested,
                           data.TestedBy,
                           data.Results
                     );

                    if (updateRes)
                    {
                        var redirect = createRes;
                        return Json(new { message = redirect, success = true }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        var redirect = "Error submitting record. Try again";
                        return Json(new { message = redirect, success = false }, JsonRequestBehavior.AllowGet);
                    }
                }

               

            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message.Replace("'", ""), success = false },
                    JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult DeleteLabSampleLine(LabSampleMgtLines labSampleLine)
        {
            try
            {
                var UserID = Session["UserID"].ToString();
                var StaffNo = Session["Username"].ToString();
                var employeeView = Session["EmployeeData"] as EmployeeView;
                Credentials.ObjNav.DeleteSampleLine(
                    labSampleLine.Sample_ID,
                    labSampleLine.Line_No
                );
                return Json(new { message = "Record successfully deleted", success = true }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message.Replace("'", ""), success = false },
                    JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult updateLabSampleLine(LabSampleMgtLines currentData)
        {
            try
            {
                var UserID = Session["UserID"].ToString();
                var StaffNo = Session["Username"].ToString();
                var employeeView = Session["EmployeeData"] as EmployeeView;

                if (Session["Username"] == null)
                {
                    return RedirectToAction("Login", "Login");
                }

                var labSampleLines = new LabSampleMgtLines();
                labSampleLines.Line_No = currentData.Line_No;
                labSampleLines.Sample_Code = currentData.Sample_Code;
                labSampleLines.Sample_ID = currentData.Sample_ID;
                labSampleLines.Type_of_Sampling = currentData.Type_of_Sampling;
                labSampleLines.Sample_Description  = currentData.Sample_Description;
                labSampleLines.Sample_Type = currentData.Sample_Type;
                labSampleLines.Source = currentData.Source;
                labSampleLines.Storage_Location = currentData.Storage_Location;
                labSampleLines.Sampling_Date = currentData.Sampling_Date;
                labSampleLines.Received_At = currentData.Received_At;
                labSampleLines.Tested = currentData.Tested;
                labSampleLines.TestedBy = currentData.TestedBy;
                labSampleLines.Analysed_At = currentData.Analysed_At;
                labSampleLines.Results = currentData.Results;
                labSampleLines.Staff_No = currentData.Staff_No;
                labSampleLines.Quantity_ml = currentData.Quantity_ml;
                labSampleLines.Notes = currentData.Notes;

                #region Dim1
                var Dim1List = new List<DropdownList>();
                var pageDim1 = $"DimensionValueList?$filter=Dimension_Code eq 'REGIONS'&$format=json";
                var httpResponseDim1 = Credentials.GetOdataData(pageDim1);
                using (var streamReader = new StreamReader(httpResponseDim1.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    var details = JObject.Parse(result);

                    foreach (var jToken in details["value"])
                    {
                        var config1 = (JObject)jToken;
                        var dropdownList = new DropdownList
                        {
                            Text = $"{(string)config1["Name"]}",
                            Value = (string)config1["Code"]
                        };
                        Dim1List.Add(dropdownList);
                    }
                }
                #endregion

                #region Employees
                var EmpleyeeList = new List<DropdownList>();
                var pageWp = $"Employees?$format=json";
                var httpResponseWp = Credentials.GetOdataData(pageWp);
                using (var streamReader = new StreamReader(httpResponseWp.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    var details = JObject.Parse(result);

                    foreach (var jToken in details["value"])
                    {
                        var config1 = (JObject)jToken;
                        var dropdownList = new DropdownList
                        {
                            Text = $"{(string)config1["FullName"]} - {(string)config1["No"]}",
                            Value = (string)config1["No"]
                        };
                        EmpleyeeList.Add(dropdownList);
                    }
                }
                #endregion

                labSampleLines.ListOfRegions = Dim1List.Select(x =>
                    new SelectListItem
                    {
                        Text = x.Text,
                        Value = x.Value
                    }).ToList();

                labSampleLines.ListOfEmployees = EmpleyeeList.Select(x =>
                    new SelectListItem
                    {
                        Text = x.Text,
                        Value = x.Value
                    }).ToList();


                return PartialView("~/Views/Technical/PartialViews/NewLabSampleLine.cshtml", labSampleLines);
            }
            catch (Exception ex)
            {
                var erroMsg = new Error();
                erroMsg.Message = ex.Message;
                return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
            }
        }
        public JsonResult SubmitUpdatedLabSampleLine(LabSampleMgtLines data)
        {
            try
            {
                var UserID = Session["UserID"].ToString();
                var StaffNo = Session["Username"].ToString();
                var employeeView = Session["EmployeeData"] as EmployeeView;

                if (string.IsNullOrEmpty(data.TestedBy))
                {
                    data.TestedBy = "";
                }

                if (string.IsNullOrEmpty(data.Analysed_At))
                {
                    data.Analysed_At = DateTime.Now.ToString("dd/MM/yyyy");
                }

                if (string.IsNullOrEmpty(data.Results))
                {
                    data.Results = "";
                }


                DateTime ReceivedAt = DateTime.ParseExact(data.Received_At.Replace("-", "/"), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                DateTime AnalysedAt = DateTime.ParseExact(data.Analysed_At.Replace("-", "/"), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                DateTime SamplingDate = DateTime.ParseExact(data.Sampling_Date.Replace("-", "/"), "dd/MM/yyyy", CultureInfo.InvariantCulture);


                int res2 = 0;

                res2 = Credentials.ObjNav.InsertSampleLine(
                    data.Sample_ID,
                    data.Sample_Type,
                    StaffNo,
                    data.Quantity_ml.ToString(),
                    data.Storage_Location,
                    data.Notes,
                    data.Source,
                    data.Sample_Code,
                    int.Parse(data.Type_of_Sampling),
                    data.Sample_Description,
                    ReceivedAt,
                    AnalysedAt,
                    SamplingDate,
                    data.Tested,
                    data.TestedBy,
                    data.Results

                );

                if (res2 != 0)
                {
                    var redirect = res2;
                    return Json(new { message = redirect, success = true }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var redirect = "Error adding record. Try again";
                    return Json(new { message = redirect, success = false }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message.Replace("'", ""), success = false },
                    JsonRequestBehavior.AllowGet);
            }
        }



        public JsonResult SendLabSampleDocForApproval(string Sample_ID)
        {
            try
            {
                var UserID = Session["UserID"].ToString();
                var StaffNo = Session["Username"].ToString();
                var employeeView = Session["EmployeeData"] as EmployeeView;

                Credentials.ObjNav.SendLabSampleMgtForApproval(Sample_ID);
                Credentials.ObjNav.UpdateApprovalEntrySenderID(50227, Sample_ID, UserID);
                
                return Json(new { message = "Document successfully sent for approval", success = true }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message.Replace("'", ""), success = false },
                    JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult CancelLabSampleDocApproval(string Sample_ID)
        {
            try
            {
                var UserID = Session["UserID"].ToString();
                var StaffNo = Session["Username"].ToString();
                var employeeView = Session["EmployeeData"] as EmployeeView;

                Credentials.ObjNav.CancelLabSampleMgtForApproval(Sample_ID);
               
                return Json(new { message = "Document approval request successfully cancelled", success = true }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message.Replace("'", ""), success = false },
                    JsonRequestBehavior.AllowGet);
            }
        }



    }
}