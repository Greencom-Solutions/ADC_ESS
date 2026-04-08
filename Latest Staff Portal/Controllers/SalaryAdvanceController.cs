using Latest_Staff_Portal.Models;
using Latest_Staff_Portal.ViewModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static Latest_Staff_Portal.Models.CommonClass;

namespace Latest_Staff_Portal.Controllers
{
    public class SalaryAdvanceController : Controller
    {
        public ActionResult SalaryAdvance()
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
                erroMsg.Message = ex.Message;
                return View("~/Views/Common/ErrorMessange.cshtml", erroMsg);
            }
        }

        public PartialViewResult SalaryAdvanceList()
        {
            try
            {
                var employee = Session["EmployeeData"] as EmployeeView;
                // Auto-detected user filter fields: Employee_No, Employee_Name
                var userId = employee?.UserID;
                var StaffNo = Session["Username"].ToString();

                //add filter based on your requirements
                var rsrceReq = $"SalaryAdvanceCard?$filter=Employee_No eq '{StaffNo}'&$format=json";
                var httpResponse = Credentials.GetOdataData(rsrceReq);

                using var streamReader = new StreamReader(httpResponse.GetResponseStream());
                var result = streamReader.ReadToEnd();

                var odataResponse = JsonConvert.DeserializeObject<ODataResponse<SalaryAdvanceModel>>(result);
                var data = odataResponse?.Value ?? [];

                return PartialView("PartialViews/SalaryAdvanceList", data);
            }
            catch (Exception ex)
            {
                var erroMsg = new Error();
                erroMsg.Message = ex.Message.Replace("'", "");
                return PartialView("~/Views/Common/ErrorMessange.cshtml", erroMsg);
            }
        }

        public ActionResult SalaryAdvanceDocumentView(string code)
        {
            try
            {

                var rsrceReq = $"SalaryAdvanceCard?$filter=Loan_No eq '{code}'&$format=json";

                var httpResponse = Credentials.GetOdataData(rsrceReq);
                using var streamReader = new StreamReader(httpResponse.GetResponseStream());
                var result = streamReader.ReadToEnd();
                var odataResponse = JsonConvert.DeserializeObject<ODataResponse<SalaryAdvanceModel>>(result);
                var document = odataResponse?.Value?.FirstOrDefault();

                if (document == null)
                {
                    throw new Exception("Document not found");
                }

                return View("SalaryAdvanceDocumentView", document);
            }
            catch (Exception ex)
            {
                var erroMsg = new Error();
                erroMsg.Message = ex.Message.Replace("'", "");
                return View("~/Views/Common/ErrorMessange.cshtml", erroMsg);
            }
        }

        // Method to create new document form
        public ActionResult NewSalaryAdvance()
        {
            try
            {
                if (Session["Username"] == null)
                    return RedirectToAction("Login", "Login");

                var StaffNo = Session["Username"].ToString();
                var viewModel = new NewSalaryAdvanceViewModel();

                return View("~/Views/SalaryAdvance/PartialViews/NewSalaryAdvance.cshtml", viewModel);
            }
            catch (Exception ex)
            {
                var erroMsg = new Error();
                erroMsg.Message = ex.Message.Replace("'", "");
                return View("~/Views/Common/ErrorMessange.cshtml", erroMsg);
            }
        }

        public JsonResult SubmitNewSalaryAdvance(NewSalaryAdvanceViewModel viewModel)
        {
            try
            {
                var employee = Session["EmployeeData"] as EmployeeView;
                var staffNo = Session["Username"].ToString();
                var UserID = employee?.UserID;

                if (string.IsNullOrEmpty(staffNo))
                {
                    return Json(new { message = "User session expired. Please login again.", success = false }, JsonRequestBehavior.AllowGet);
                }

                var result = "";

                Credentials.ObjNav.fnInsertSalaryAdvance(staffNo, viewModel.Amountrequested, viewModel.Installment, viewModel.Reasontext);
                return Json(new { message = "Salary Advance document successfully created", success = true, docNo = result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message.Replace("'", ""), success = false }, JsonRequestBehavior.AllowGet);
            }
        }

        // TODO: Replace Credentials.ObjNav with your actual Business Central function
        public JsonResult SendForReview(string docNo)
        {
            try
            {
                var employee = Session["EmployeeData"] as EmployeeView;
                var UserID = employee?.UserID;

                // REPLACE WITH YOUR BUSINESS CENTRAL FUNCTION:
                Credentials.ObjNav.SendSalaryAdvanceForApproval(docNo);
                Credentials.ObjNav.UpdateApprovalEntrySenderID(69011, docNo, UserID);

                return Json(new { message = "Document successfully sentv for review!", success = true },
                    JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message.Replace("'", ""), success = false },
                    JsonRequestBehavior.AllowGet);
            }
        }

        // TODO: Replace Credentials.ObjNav with your actual Business Central function
        public JsonResult CancelSalaryAdvanceApprovalRequest(string docNo)
        {
            try
            {
                var employee = Session["EmployeeData"] as EmployeeView;
                var UserID = employee?.UserID;

                if (string.IsNullOrEmpty(docNo))
                {
                    return Json(new { message = "Document Number is required", success = false },
                        JsonRequestBehavior.AllowGet);
                }


                Credentials.ObjNav.CancelSalaryAdvanceApproval(docNo);

                return Json(new { message = "Approval Request cancelled successfully", success = true },
                    JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message.Replace("'", ""), success = false },
                    JsonRequestBehavior.AllowGet);
            }
        }

        // Method to open update header form
        public PartialViewResult UpdateSalaryAdvanceHeader(string docNo)
        {
            try
            {
                if (Session["Username"] == null)
                    return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml",
                        new Error { Message = "Session expired. Please login again." });

                var employee = Session["EmployeeData"] as EmployeeView;
                var StaffNo = Session["Username"].ToString();


                var rsrceReq = $"SalaryAdvance?$filter=Loan_No eq '{docNo}'&$format=json";


                var httpResponse = Credentials.GetOdataData(rsrceReq);
                using var streamReader = new StreamReader(httpResponse.GetResponseStream());
                var result = streamReader.ReadToEnd();
                var odataResponse = JsonConvert.DeserializeObject<ODataResponse<SalaryAdvanceModel>>(result);
                var document = odataResponse?.Value?.FirstOrDefault();

                if (document == null)
                {
                    return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml",
                        new Error { Message = "Document not found" });
                }

                return PartialView("PartialViews/UpdateSalaryAdvanceHeader", document);
            }
            catch (Exception ex)
            {
                var erroMsg = new Error();
                erroMsg.Message = ex.Message;
                return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
            }
        }

        // Method to submit updated header
        public JsonResult SubmitUpdatedSalaryAdvanceheader(SalaryAdvanceModel viewModel)
        {
            try
            {
                var employee = Session["EmployeeData"] as EmployeeView;
                var staffNo = Session["Username"].ToString();
                var UserID = employee?.UserID;

                if (string.IsNullOrEmpty(staffNo))
                {
                    return Json(new { message = "User session expired. Please login again.", success = false },
                        JsonRequestBehavior.AllowGet);
                }

                // TODO: REPLACE WITH YOUR BUSINESS CENTRAL UPDATE FUNCTION
                // Example: var success = Credentials.ObjNav.FnUpdateSalaryAdvance(viewModel.No, viewModel.Property1, viewModel.Property2, UserID);
                // Example: var success = Credentials.ObjNav.UpdateSalaryAdvance(viewModel.Loan_No, viewModel.Field1, viewModel.Field2);

                bool success = true; // Placeholder - replace with actual BC function call

                return Json(new
                {
                    message = success ? "SalaryAdvance header updated successfully" : "Failed to update SalaryAdvance header",
                    success = success
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message.Replace("'", ""), success = false },
                    JsonRequestBehavior.AllowGet);
            }
        }

        // LINES SECTION - UNCOMMENT AND IMPLEMENT WHEN LINES ARE NEEDED
        /*
        public PartialViewResult SalaryAdvanceLines(string docNo, string status)
        {
            try
            {
                ViewBag.Status = status;
                // Use the detected primary key for filtering
                
                var rsrceReq = $"SalaryAdvance?$filter=Loan_No eq '{docNo}'&$format=json";
                

                var httpResponse = Credentials.GetOdataData(rsrceReq);

                using var streamReader = new StreamReader(httpResponse.GetResponseStream());
                var result = streamReader.ReadToEnd();

                var odataResponse = JsonConvert.DeserializeObject<ODataResponse<SalaryAdvanceModel>>(result);
                var lines = odataResponse?.Value ?? [];

                return PartialView("PartialViews/SalaryAdvanceLines", lines);
            }
            catch (Exception ex)
            {
                var erroMsg = new Error();
                erroMsg.Message = ex.Message.Replace("'", "");
                return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
            }
        }
        */

        // Line management methods
        public PartialViewResult AddSalaryAdvanceLine()
        {
            try
            {
                // Return partial view for adding lines
                // You might want to create a separate view model for lines
                return PartialView("PartialViews/AddSalaryAdvanceLine");
            }
            catch (Exception ex)
            {
                var erroMsg = new Error();
                erroMsg.Message = ex.Message;
                return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
            }
        }

        public JsonResult DeleteSalaryAdvanceLine(string code, string lineNo)
        {
            try
            {
                // TODO: REPLACE WITH YOUR BUSINESS CENTRAL FUNCTION
                // Credentials.ObjNav.FnDeleteSalaryAdvanceLines(Convert.ToInt32(lineNo), code);

                return Json(new { message = "Line deleted successfully", success = true },
                    JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message.Replace("'", ""), success = false },
                    JsonRequestBehavior.AllowGet);
            }
        }
    }
}