using Latest_Staff_Portal.Models;
using Latest_Staff_Portal.ViewModel;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Latest_Staff_Portal.Controllers
{
    public class StandingImprestController : Controller
    {
        // GET: StandingImprest
        public ActionResult StandingImprestList()
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
        public PartialViewResult StandingImprestListPartialView()
        {
            try
            {
                var UserID = Session["UserID"].ToString();
                var StaffNo = Session["Username"].ToString();
                var employeeView = Session["EmployeeData"] as EmployeeView;
                var ImpList = new List<StandingImprest>();

                var role = Session["ESSRoleSetup"] as ESSRoleSetup;
                var page = $"StandingImprest?$filter=Account_No eq '{StaffNo}'&$format=json";

                var httpResponse = Credentials.GetOdataData(page);
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    var details = JObject.Parse(result);

                    foreach (JObject config in details["value"])
                    {
                        var ImList = new StandingImprest
                        {
                            No = (string)config["No"],
                            Dimension_Set_ID = (int)config["Dimension_Set_ID"],
                            Date = (string)config["Date"],
                            Posting_Date = (string)config["Posting_Date"],
                            Account_No = (string)config["Account_No"],
                            Account_Name = (string)config["Account_Name"],
                            Standing_Imprest_Type = (string)config["Standing_Imprest_Type"],
                            Cheque_Date = (string)config["Cheque_Date"],
                            Paying_Bank_Account = (string)config["Paying_Bank_Account"],
                            Bank_Name = (string)config["Bank_Name"],
                            Payment_Narration = (string)config["Payment_Narration"],
                            Currency_Code = (string)config["Currency_Code"],
                            Total_Amount = (int)config["Total_Amount"],
                            Shortcut_Dimension_1_Code = (string)config["Shortcut_Dimension_1_Code"],
                            Department_Name = (string)config["Department_Name"],
                            Shortcut_Dimension_2_Code = (string)config["Shortcut_Dimension_2_Code"],
                            Project_Name = (string)config["Project_Name"],
                            Available_Amount = (int)config["Available_Amount"],
                            Committed_Amount = (int)config["Committed_Amount"],
                            AIE_Receipt = (string)config["AIE_Receipt"],
                            Strategic_Plan = (string)config["Strategic_Plan"],
                            Reporting_Year_Code = (string)config["Reporting_Year_Code"],
                            Workplan_Code = (string)config["Workplan_Code"],
                            Activity_Code = (string)config["Activity_Code"],
                            Status = (string)config["Status"],
                            Posted = (bool)config["Posted"],
                            Posted_By = (string)config["Posted_By"],
                            Posted_Date = (string)config["Posted_Date"],
                            Expenditure_Requisition_Code = (string)config["Expenditure_Requisition_Code"],
                            Payment_Ref_No = (string)config["Payment_Ref_No"],
                            Payment_Date = (string)config["Payment_Date"]

                        };

                        ImpList.Add(ImList);
                    }
                }

                return PartialView("~/Views/StandingImprest/PartialViews/StandingImprestListPartialView.cshtml", ImpList.OrderByDescending(x => x.No));
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
        public ActionResult StandingImprestDocumentView(string DocNo)
        {
            try
            {

                var employeeView = Session["EmployeeData"] as EmployeeView;
                var StaffNo = Session["Username"].ToString();

                #region Employee Data

                var pageData = "EmployeeList?$filter=No eq '" + StaffNo + "'&$format=json";

                var httpResponseEmpl = Credentials.GetOdataData(pageData);
                using (var streamReader = new StreamReader(httpResponseEmpl.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);

                    if (details["value"].Count() > 0)
                        foreach (JObject config in details["value"])
                        {
                            //Dim1 = (string)config["Global_Dimension_1_Code"];
                            // Dim2 = (string)config["GlobalDimension2Code"];
                        }
                }

                #endregion

                #region Region

                var Dim1List = new List<DimensionValues>();
                /*var pageDepartment =
                    "DimensionValueList?$filter=Global_Dimension_No eq 1 and Blocked eq false&$format=json";*/

                var pageDepartment = "DimensionValueList?$filter=Dimension_Code eq 'REGIONS' and Blocked eq false&$format=json";

                var httpResponseDepartment = Credentials.GetOdataData(pageDepartment);
                using (var streamReader = new StreamReader(httpResponseDepartment.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);


                    foreach (JObject config in details["value"])
                    {
                        var Department = new DimensionValues();
                        Department.Code = (string)config["Code"];
                        Department.Name = (string)config["Name"];
                        Dim1List.Add(Department);
                    }
                }

                #endregion

                #region Department

                var Dim2List = new List<DimensionValues>();
                /*var pageDivision ="DimensionValueList?$filter=Global_Dimension_No eq 2 and Blocked eq false&$format=json";*/
                var pageDivision = "DimensionValueList?$filter=Dimension_Code eq 'DEPARTMENT' and Blocked eq false&$format=json";

                var httpResponseDivision = Credentials.GetOdataData(pageDivision);
                using (var streamReader = new StreamReader(httpResponseDivision.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);


                    foreach (JObject config in details["value"])
                    {
                        var DList = new DimensionValues();
                        DList.Code = (string)config["Code"];
                        DList.Name = (string)config["Name"];
                        Dim2List.Add(DList);
                    }
                }

                #endregion


                #region StrategicPlan

                var StratPlanCList = new List<RespCenter>();
                var pageStratPlan = "AllCSPS?$format=json";

                var httpResponseStratPlan = Credentials.GetOdataData(pageStratPlan);
                using (var streamReader = new StreamReader(httpResponseStratPlan.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);


                    foreach (JObject config in details["value"])
                    {
                        var RCList = new RespCenter();
                        RCList.Code = (string)config["Code"];
                        RCList.Name = (string)config["Description"] + "-" + (string)config["Code"];
                        StratPlanCList.Add(RCList);
                    }
                }

                #endregion

                #region WorkplanActivities

                var WorkplanActivitiesList = new List<RespCenter>();
                var pageWorkplanActivities = "WorkplanActivities?$format=json";

                var httpWorkplanActivities = Credentials.GetOdataData(pageWorkplanActivities);
                using (var streamReader = new StreamReader(httpWorkplanActivities.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);


                    foreach (JObject config in details["value"])
                    {
                        var RCList = new RespCenter();
                        RCList.Code = (string)config["Code"];
                        RCList.Name = (string)config["Descriptions"] + "-" + (string)config["Code"];
                        WorkplanActivitiesList.Add(RCList);
                    }
                }
                #endregion

                #region ImplementationYears

                var ImplementationYearsList = new List<RespCenter>();
                var pageImplementationYears = "ImplementationYears?$format=json";

                var httpImplementationYears = Credentials.GetOdataData(pageImplementationYears);
                using (var streamReader = new StreamReader(httpImplementationYears.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);


                    foreach (JObject config in details["value"])
                    {
                        var RCList = new RespCenter();
                        RCList.Code = (string)config["Annual_Year_Code"];
                        RCList.Name = (string)config["Description"];
                        ImplementationYearsList.Add(RCList);
                    }
                }
                #endregion

                #region ExpenseRequisitions
                var ExpenseRequisitionsList = new List<RespCenter>();
                var pageExpenseRequisitions = "ExpenditureRequisitions?$format=json";

                var httpExpenseRequisitions = Credentials.GetOdataData(pageExpenseRequisitions);
                using (var streamReader = new StreamReader(httpExpenseRequisitions.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);


                    foreach (JObject config in details["value"])
                    {
                        var RCList = new RespCenter();
                        RCList.Code = (string)config["Annual_Year_Code"];
                        RCList.Name = (string)config["Description"];
                        ExpenseRequisitionsList.Add(RCList);
                    }
                }
                #endregion




                if (Session["Username"] == null) return RedirectToAction("Login", "Login");

              
                var ImpDoc = new StandingImprest();

                var page = $"StandingImprest?$filter=No eq '{DocNo}'&$format=json";
                var httpResponse = Credentials.GetOdataData(page);
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    var details = JObject.Parse(result);

                    foreach (JObject config in details["value"])
                    {
                        ImpDoc = new StandingImprest
                        {
                            No = (string)config["No"],
                            Dimension_Set_ID = (int)config["Dimension_Set_ID"],
                            Date = (string)config["Date"],
                            Posting_Date = (string)config["Posting_Date"],
                            Account_No = (string)config["Account_No"],
                            Account_Name = (string)config["Account_Name"],
                            Standing_Imprest_Type = (string)config["Standing_Imprest_Type"],
                            Cheque_Date = (string)config["Cheque_Date"],
                            Paying_Bank_Account = (string)config["Paying_Bank_Account"],
                            Bank_Name = (string)config["Bank_Name"],
                            Payment_Narration = (string)config["Payment_Narration"],
                            Currency_Code = (string)config["Currency_Code"],
                            Total_Amount = (int)config["Total_Amount"],
                            Shortcut_Dimension_1_Code = (string)config["Shortcut_Dimension_1_Code"],
                            Department_Name = (string)config["Department_Name"],
                            Shortcut_Dimension_2_Code = (string)config["Shortcut_Dimension_2_Code"],
                            Project_Name = (string)config["Project_Name"],
                            Available_Amount = (int)config["Available_Amount"],
                            Committed_Amount = (int)config["Committed_Amount"],
                            AIE_Receipt = (string)config["AIE_Receipt"],
                            Strategic_Plan = (string)config["Strategic_Plan"],
                            Reporting_Year_Code = (string)config["Reporting_Year_Code"],
                            Workplan_Code = (string)config["Workplan_Code"],
                            Activity_Code = (string)config["Activity_Code"],
                            Status = (string)config["Status"],
                            Posted = (bool)config["Posted"],
                            Posted_By = (string)config["Posted_By"],
                            Posted_Date = (string)config["Posted_Date"],
                            Expenditure_Requisition_Code = (string)config["Expenditure_Requisition_Code"],
                            Payment_Ref_No = (string)config["Payment_Ref_No"],
                            Payment_Date = (string)config["Payment_Date"]
                        };
                    }
                }

                ImpDoc.ListOfStratPlans = StratPlanCList.Select(x =>
                    new SelectListItem
                    {
                        Text = x.Name,
                        Value = x.Code
                    }).ToList();

                ImpDoc.ListOfImplementationYears = ImplementationYearsList.Select(x =>
                  new SelectListItem
                  {
                      Text = x.Name,
                      Value = x.Code
                  }).ToList();


                return View(ImpDoc);
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
        public PartialViewResult StandingImprestLines(string DocNo, string Status)
        {
            try
            {
                var ImpLines = new List<StandingImprestLine>();
                var pageLine = "StandingImprestLines?$filter=No eq '" + DocNo + "'&$format=json";
                var httpResponseLine = Credentials.GetOdataData(pageLine);

                using (var streamReader = new StreamReader(httpResponseLine.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    var details = JObject.Parse(result);

                    foreach (JObject config in details["value"])
                    {
                        var ImLine = new StandingImprestLine
                        {
                            No = (string)config["No"],
                            Line_No = (int)config["Line_No"],
                            Bounced_Pv_No = (string)config["Bounced_Pv_No"],
                            Account_Type = (string)config["Account_Type"],
                            Account_No = (string)config["Account_No"],
                            Account_Name = (string)config["Account_Name"],
                            Description = (string)config["Description"],
                            Amount = (int)config["Amount"],
                            Shortcut_Dimension_1_Code = (string)config["Shortcut_Dimension_1_Code"],
                            Shortcut_Dimension_2_Code = (string)config["Shortcut_Dimension_2_Code"],
                            Vote_Item = (string)config["Vote_Item"],
                            Payee_Bank_Code = (string)config["Payee_Bank_Code"],
                            Payee_Bank_Name = (string)config["Payee_Bank_Name"],
                            Payee_Bank_Branch_Code = (string)config["Payee_Bank_Branch_Code"],
                            Payee_Bank_Branch_Name = (string)config["Payee_Bank_Branch_Name"],
                            Payee_Bank_Account_No = (string)config["Payee_Bank_Account_No"],
                            Payee_Bank_Acc_Name = (string)config["Payee_Bank_Acc_Name"],
                            rbankName = (string)config["rbankName"]

                        }
                    ;

                        ImpLines.Add(ImLine);
                    }
                }
                ViewBag.Status = Status;
                return PartialView("~/Views/StandingImprest/PartialViews/StandingImprestLines.cshtml", ImpLines);
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






        public ActionResult NewStandingImprestRequest()
        {
            try
            {
                if (Session["Username"] == null) return RedirectToAction("Login", "Login");
                var employeeView = Session["EmployeeData"] as EmployeeView;
                var StaffNo = Session["Username"].ToString();
                var NewStandingImprest = new StandingImprest();
                string Dim1 = employeeView.GlobalDimension1Code;
                string Dim2 = employeeView.GlobalDimension2Code;

                NewStandingImprest.Shortcut_Dimension_2_Code = employeeView.GlobalDimension2Code;

                #region Employee Data

                var pageData = "EmployeeList?$filter=No eq '" + StaffNo + "'&$format=json";

                var httpResponse = Credentials.GetOdataData(pageData);
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);

                    if (details["value"].Count() > 0)
                        foreach (JObject config in details["value"])
                        {
                            //Dim1 = (string)config["Global_Dimension_1_Code"];
                            // Dim2 = (string)config["GlobalDimension2Code"];
                        }
                }

                #endregion

                #region Region

                var Dim1List = new List<DimensionValues>();
                /*var pageDepartment =
                    "DimensionValueList?$filter=Global_Dimension_No eq 1 and Blocked eq false&$format=json";*/

                var pageDepartment = "DimensionValueList?$filter=Dimension_Code eq 'REGIONS' and Blocked eq false&$format=json";

                var httpResponseDepartment = Credentials.GetOdataData(pageDepartment);
                using (var streamReader = new StreamReader(httpResponseDepartment.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);


                    foreach (JObject config in details["value"])
                    {
                        var Department = new DimensionValues();
                        Department.Code = (string)config["Code"];
                        Department.Name = (string)config["Name"];
                        Dim1List.Add(Department);
                    }
                }

                #endregion

                #region Department

                var Dim2List = new List<DimensionValues>();
                /*var pageDivision ="DimensionValueList?$filter=Global_Dimension_No eq 2 and Blocked eq false&$format=json";*/
                var pageDivision = "DimensionValueList?$filter=Dimension_Code eq 'DEPARTMENT' and Blocked eq false&$format=json";

                var httpResponseDivision = Credentials.GetOdataData(pageDivision);
                using (var streamReader = new StreamReader(httpResponseDivision.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);


                    foreach (JObject config in details["value"])
                    {
                        var DList = new DimensionValues();
                        DList.Code = (string)config["Code"];
                        DList.Name = (string)config["Name"];
                        Dim2List.Add(DList);
                    }
                }

                #endregion

                #region StrategicPlan

                var StratPlanCList = new List<RespCenter>();
                var pageStratPlan = "AllCSPS?$format=json";

                var httpResponseStratPlan = Credentials.GetOdataData(pageStratPlan);
                using (var streamReader = new StreamReader(httpResponseStratPlan.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);


                    foreach (JObject config in details["value"])
                    {
                        var RCList = new RespCenter();
                        RCList.Code = (string)config["Code"];
                        RCList.Name = (string)config["Description"] + "-" + (string)config["Code"];
                        StratPlanCList.Add(RCList);
                    }
                }

                #endregion

                #region WorkplanActivities

                var WorkplanActivitiesList = new List<RespCenter>();
                var pageWorkplanActivities = "WorkplanActivities?$format=json";

                var httpWorkplanActivities = Credentials.GetOdataData(pageWorkplanActivities);
                using (var streamReader = new StreamReader(httpWorkplanActivities.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);


                    foreach (JObject config in details["value"])
                    {
                        var RCList = new RespCenter();
                        RCList.Code = (string)config["Code"];
                        RCList.Name = (string)config["Descriptions"] + "-" + (string)config["Code"];
                        WorkplanActivitiesList.Add(RCList);
                    }
                }
                #endregion

                #region ImplementationYears

                var ImplementationYearsList = new List<RespCenter>();
                var pageImplementationYears = "ImplementationYears?$format=json";

                var httpImplementationYears = Credentials.GetOdataData(pageImplementationYears);
                using (var streamReader = new StreamReader(httpImplementationYears.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);


                    foreach (JObject config in details["value"])
                    {
                        var RCList = new RespCenter();
                        RCList.Code = (string)config["Annual_Year_Code"];
                        RCList.Name = (string)config["Description"];
                        ImplementationYearsList.Add(RCList);
                    }
                }
                #endregion

                #region ExpenseRequisitions

                var ExpenseRequisitionsList = new List<RespCenter>();
                var pageExpenseRequisitions = "ExpenditureRequisitions?$format=json";

                var httpExpenseRequisitions = Credentials.GetOdataData(pageExpenseRequisitions);
                using (var streamReader = new StreamReader(httpExpenseRequisitions.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);


                    foreach (JObject config in details["value"])
                    {
                        var RCList = new RespCenter();
                        RCList.Code = (string)config["Annual_Year_Code"];
                        RCList.Name = (string)config["Description"];
                        ExpenseRequisitionsList.Add(RCList);
                    }
                }
                #endregion

                #region Responsibility

                var RespCList = new List<RespCenter>();
                var pageResC = "DimensionValueList?$format=json";

                var httpResponseResC3 = Credentials.GetOdataData(pageResC);
                using (var streamReader = new StreamReader(httpResponseResC3.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);


                    foreach (JObject config in details["value"])
                    {
                        var RCList = new RespCenter();
                        RCList.Code = (string)config["Code"];
                        RCList.Name = (string)config["Name"];
                        RespCList.Add(RCList);
                    }
                }

                #endregion

                #region ImprestDue

                // List<ImprestDuetype> ImprestDue = new List<ImprestDuetype>();
                var ImmpsDue = new List<ImpDuetyp>();
                var pageImprestDueType = "DimensionValueList?$format=json";

                var httpResponseImpDue = Credentials.GetOdataData(pageImprestDueType);
                using (var streamReader = new StreamReader(httpResponseImpDue.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);


                    foreach (JObject config in details["value"])
                    {
                        var impD = new ImpDuetyp();
                        impD.Code = (string)config["Code"];
                        impD.Name = (string)config["Code"];
                        ImmpsDue.Add(impD);
                    }
                }

                #endregion

                #region EmployeeList

                var employeeList = new List<EmployeeList>();
                /* var Department2 = CommonClass.EmployeeDepartment(StaffNo);

                 var Departments = CommonClass.EmployeeDepartment(StaffNo);*/
                var pageReliever = "EmployeeList?$format=json";


                var httpResponseReliever = Credentials.GetOdataData(pageReliever);
                using (var streamReader = new StreamReader(httpResponseReliever.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);
                    foreach (JObject config in details["value"])
                        if ((string)config["FirstName"] != "")
                        {
                            var Rlist = new EmployeeList();
                            Rlist.No = (string)config["No"];
                            Rlist.Name = (string)config["FirstName"] + " " + (string)config["MiddleName"] + " " +
                                         (string)config["LastName"];
                            employeeList.Add(Rlist);
                        }
                }

                #endregion

                NewStandingImprest = new StandingImprest
                {
                    Shortcut_Dimension_1_Code = Dim1,
                    Shortcut_Dimension_2_Code = Dim2,
                    ListOfDim1 = Dim1List.Select(x =>
                        new SelectListItem
                        {
                            Text = x.Name,
                            Value = x.Code
                        }).ToList(),
                    ListOfDim2 = Dim2List.Select(x =>
                        new SelectListItem
                        {
                            Text = x.Name,
                            Value = x.Code
                        }).ToList(),


                    ListOfStratPlans = StratPlanCList.Select(x =>
                       new SelectListItem
                       {
                           Text = x.Name,
                           Value = x.Code
                       }).ToList(),

                    ListOfWorkplanActivities = WorkplanActivitiesList.Select(x =>
                      new SelectListItem
                      {
                          Text = x.Name,
                          Value = x.Code
                      }).ToList(),


                    ListOfImplementationYears = ImplementationYearsList.Select(x =>
                     new SelectListItem
                     {
                         Text = x.Name,
                         Value = x.Code
                     }).ToList(),


                    ListOfExpenseRequisitions = ExpenseRequisitionsList.Select(x =>
                    new SelectListItem
                    {
                        Text = x.Name,
                        Value = x.Code
                    }).ToList(),



                    ListOfResponsibility = RespCList.Select(x =>
                        new SelectListItem
                        {
                            Text = x.Name,
                            Value = x.Code
                        }).ToList(),
                    ListOfEmployeeList = employeeList.Select(x =>
                        new SelectListItem
                        {
                            Text = x.Name,
                            Value = x.No
                        }).ToList(),
                    ListOfImprestDue = ImmpsDue.Select(x =>
                        new SelectListItem
                        {
                            Text = x.Name,
                            Value = x.Code
                        }).ToList()
                };
                return View("~/Views/StandingImprest/PArtialViews/NewStandingImprestRequest.cshtml", NewStandingImprest);

            }
            catch (Exception ex)
            {
                var erroMsg = new Error();
                erroMsg.Message = ex.Message.Replace("'", "");
                return View("~/Views/Common/ErrorMessange.cshtml", erroMsg);
            }
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SubmitStandingImprestRequisition(StandingImprest data)
        {
            var successVal = false;
            try
            {

                if (Session["UserID"] == null || Session["Username"] == null) return RedirectToAction("Login", "Login");

                var StaffNo = Session["Username"].ToString();
                var UserID = Session["UserID"].ToString();
                //var Start_Date = DateTime.ParseExact(data.Start_Date.Replace("-", "/"), "dd/MM/yyyy",
                //    CultureInfo.InvariantCulture);
                var DocNo = "";

                DocNo = Credentials.ObjNav.InsertStandingImprestRequisition(
                    StaffNo,
                    data.Payment_Narration,
                    "",
                   ""

                );

              



                if (DocNo == "")
                {

                    return Json(new { message = "Document not created. Try again", success = false }, JsonRequestBehavior.AllowGet);
                }
                else
                {

                    return Json(new { message = DocNo, success = true }, JsonRequestBehavior.AllowGet);
                }






            }
            catch (Exception ex)
            {
                if (successVal) Session["ErrorMsg"] = ex.Message.Replace("'", "");
                return Json(new { message = ex.Message.Replace("'", ""), success = false },
                    JsonRequestBehavior.AllowGet);
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult UpdateStandingImprestHeader(string DocNo, StandingImprest StandingImprestHeader)
        {
            try
            {


                var StaffNo = Session["Username"].ToString();
                var UserID = Session["UserID"].ToString();

                //var Start_Date = DateTime.ParseExact(Data.Start_Date.Replace("-", "/"), "dd/MM/yyyy",
                //    CultureInfo.InvariantCulture);

                var res = Credentials.ObjNav.ModifyStandingImprestRequisition(
                    DocNo,
                    StaffNo, 
                    StandingImprestHeader.Payment_Narration, 
                    StandingImprestHeader.Strategic_Plan,
                    StandingImprestHeader.Reporting_Year_Code
                );


                return Json(new { message = "Record Updated successfully", success = true },
                    JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message.Replace("'", ""), success = false },
                    JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult NewStandingImprestLine(string docNo)
        {
            try
            {
                var StandingImprestLine = new StandingImprestLine();

                var UserID = Session["UserID"].ToString();
                var StaffNo = Session["Username"].ToString();
                var employeeView = Session["EmployeeData"] as EmployeeView;

                #region Employees
                var EmpList = new List<DropdownList>();
                var pageEmp = "EmployeeList?$format=json";

                var httpResponseEmp = Credentials.GetOdataData(pageEmp);
                using (var streamReader = new StreamReader(httpResponseEmp.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    var details = JObject.Parse(result);
                    foreach (var jToken in details["value"])
                    {
                        var config1 = (JObject)jToken;
                        var dropdownList = new DropdownList
                        {
                            Text = (string)config1["First_Name"] + " " + (string)config1["Middle_Name"] + " " + (string)config1["Last_Name"] + " (" + (string)config1["No"] + ")",
                            Value = (string)config1["No"]
                        };
                        EmpList.Add(dropdownList);
                    }
                }

                #endregion

                #region Region

                var Dim1List = new List<DropdownList>();
                /*var pageDepartment =
                    "DimensionValueList?$filter=Global_Dimension_No eq 1 and Blocked eq false&$format=json";*/

                var pageDepartment = "DimensionValueList?$filter=Dimension_Code eq 'REGIONS' and Blocked eq false&$format=json";

                var httpResponseDepartment = Credentials.GetOdataData(pageDepartment);
                using (var streamReader = new StreamReader(httpResponseDepartment.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    var details = JObject.Parse(result);

                    foreach (var jToken in details["value"])
                    {
                        var config1 = (JObject)jToken;

                        var dropdownList = new DropdownList
                        {
                            Text = (string)config1["Name"] + " (" + (string)config1["Code"] + ")",
                            Value = (string)config1["Code"]
                        };

                        Dim1List.Add(dropdownList);
                    }
                }

                #endregion

                #region Department

                var Dim2List = new List<DropdownList>();

                var pageDivision = "DimensionValueList?$filter=Dimension_Code eq 'DEPARTMENT' and Blocked eq false&$format=json";

                var httpResponseDivision = Credentials.GetOdataData(pageDivision);
                using (var streamReader = new StreamReader(httpResponseDivision.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);


                    foreach (var jToken in details["value"])
                    {
                        var config1 = (JObject)jToken;

                        var dropdownList = new DropdownList
                        {
                            Text = (string)config1["Name"] + " (" + (string)config1["Code"] + ")",
                            Value = (string)config1["Code"]
                        };

                        Dim2List.Add(dropdownList);
                    }
                }

                #endregion

                #region GLAccounts

                var GlList = new List<DropdownList>();

                var pageGl = "GLAccounts?$format=json";

                var httpResponseGl = Credentials.GetOdataData(pageGl);
                using (var streamReader = new StreamReader(httpResponseGl.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);
                    foreach (var jToken in details["value"])
                    {
                        var config1 = (JObject)jToken;
                        var dropdownList = new DropdownList
                        {
                            Text = (string)config1["Name"] + " (" + (string)config1["No"] + ")",
                            Value = (string)config1["No"]
                        };
                        GlList.Add(dropdownList);
                    }
                }

                #endregion

                #region BankAccounts

                var BankList = new List<DropdownList>();

                var pageBank = "BankAccounts?$format=json";

                var httpResponseBank = Credentials.GetOdataData(pageBank);
                using (var streamReader = new StreamReader(httpResponseBank.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);


                    foreach (var jToken in details["value"])
                    {
                        var config1 = (JObject)jToken;

                        var dropdownList = new DropdownList
                        {
                            Text = (string)config1["BankName"] + " (" + (string)config1["BankCode"] + ")",
                            Value = (string)config1["BankCode"]
                        };

                        BankList.Add(dropdownList);
                    }

                }

                #endregion



                StandingImprestLine.ListOfEmployees = EmpList.Select(x =>
                      new SelectListItem
                      {
                          Text = x.Text,
                          Value = x.Value
                      }).ToList();


                StandingImprestLine.ListOfDim1 = Dim1List.Select(x =>
                      new SelectListItem
                      {
                          Text = x.Text,
                          Value = x.Value
                      }).ToList();

                StandingImprestLine.ListOfDim2 = Dim2List.Select(x =>
                      new SelectListItem
                      {
                          Text = x.Text,
                          Value = x.Value
                      }).ToList();
                StandingImprestLine.ListOfGl = GlList.Select(x =>
                      new SelectListItem
                      {
                          Text = x.Text,
                          Value = x.Value
                      }).ToList();
                StandingImprestLine.ListOfBanks = BankList.Select(x =>
                      new SelectListItem
                      {
                          Text = x.Text,
                          Value = x.Value
                      }).ToList();



                return PartialView("~/Views/StandingImprest/PartialViews/NewStandingImprestLine.cshtml", StandingImprestLine);
            }
            catch (Exception ex)
            {
                var erroMsg = new Error();
                erroMsg.Message = ex.Message.Replace("'", "");
                return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
            }
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult SubmitStandingImprestLine(string DocNo, StandingImprestLine ImpLine)
        {
            try
            {
                var UserID = Session["UserID"].ToString();
                var StaffNo = Session["Username"].ToString();
                var employeeView = Session["EmployeeData"] as EmployeeView;
                int res = 0;
                res = Credentials.ObjNav.InsertStandingImprestLine(
                    DocNo,
                    ImpLine.Account_No,
                    ImpLine.Description,
                   ImpLine.Amount
                 );
                //if (res == 1)
                //{
                    //var DocNetAmount = GetImpDocNetAmount(DocNo);
                    return Json(new { NetAmout = 0, message = "Record Added successfully", success = true }, JsonRequestBehavior.AllowGet);
                //}
                //else
                //{
                //    //var DocNetAmount = GetImpDocNetAmount(DocNo);
                //    return Json(new { NetAmout = 0, message = "Error creating record", success = false }, JsonRequestBehavior.AllowGet);
                //}

            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message.Replace("'", ""), success = false },
                    JsonRequestBehavior.AllowGet);
            }
        }


        public PartialViewResult OtherCosts(string DocNo, string Status)
        {
            try
            {
                var otherCostsLines = new List<OtherCosts>();
                var pageLine = "OtherCosts?$filter=Imprest_Memo_No eq '" + DocNo + "'&$format=json";
                var httpResponseLine = Credentials.GetOdataData(pageLine);

                using (var streamReader = new StreamReader(httpResponseLine.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    var details = JObject.Parse(result);

                    foreach (JObject config in details["value"])
                    {
                        var costLine = new OtherCosts
                        {
                            Imprest_Memo_No = (string)config["Imprest_Memo_No"],
                            Line_No = (int)config["Line_No"],
                            Type = (string)config["Type"],
                            Type_of_Expense = (string)config["Type_of_Expense"],
                            Description = (string)config["Description"],
                            No = (string)config["No"],
                            Required_For = (string)config["Required_For"],
                            Quantity_Required = (int)config["Quantity_Required"],
                            No_of_Days = (int)config["No_of_Days"],
                            Unit_Cost = (int)config["Unit_Cost"],
                            Line_Amount = (int)config["Line_Amount"],


                        }
                    ;

                        otherCostsLines.Add(costLine);
                    }
                }
                ViewBag.Status = Status;
                ViewBag.DocNo = DocNo;
                return PartialView(otherCostsLines);
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

        public PartialViewResult StandingImprestItems(string DocNo, string Status)
        {
            try
            {
                #region Imp Lines

                var ImpLines = new List<ImprestMemoLines>();
                var pageLine = "ImprestMemoLine?$filter=No eq '" + DocNo +
                               "' and ImprestType eq 'ItemCash' &$format=json";
                var httpResponseLine = Credentials.GetOdataData(pageLine);
                using (var streamReader = new StreamReader(httpResponseLine.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);
                    foreach (JObject config in details["value"])
                    {
                        var ImLine = new ImprestMemoLines();
                        ImLine.DocNo = (string)config["No"];
                        ImLine.AdvanceType = (string)config["AdvanceType"];
                        ImLine.Item = (string)config["AccountNo"];
                        ImLine.ItemDesc = (string)config["AccountName"];
                        ImLine.ItemDesc2 = (string)config["Purpose"];
                        ImLine.LnNo = (string)config["LineNo"];
                        ImLine.UoN = (string)config["UnitofMeasure"];
                        ImLine.Quantity = (string)config["Quantity"];
                        ImLine.UnitAmount = Convert.ToDecimal((string)config["UnitCostLCY"]).ToString("#,##0.00");
                        ImLine.Amount = Convert.ToDecimal((string)config["Amount"]).ToString("#,##0.00");
                        ImLine.EmployeeNo = (string)config["EmployeeName"];
                        ImpLines.Add(ImLine);
                    }
                }

                #endregion

                var Lines = new ImprestMemoItemsList
                {
                    Status = Status,
                    ListOfImprestMemoLines = ImpLines
                };
                return PartialView("~/Views/ImprestMemo/ImprestMemoItemsList.cshtml", Lines);
            }
            catch (Exception ex)
            {
                var erroMsg = new Error();
                erroMsg.Message = ex.Message.Replace("'", "");
                return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
            }
        }
        public PartialViewResult StandingImprestNonStaff(string DocNo, string Status)
        {
            try
            {
                #region Imp Lines

                var ImpLines = new List<ImprestMemoNonStaff>();
                var pageLine = "ImprestMemoNonStaff?$filter=ImprestMemoNo eq '" + DocNo + "'&$format=json";
                var httpResponseLine = Credentials.GetOdataData(pageLine);
                using (var streamReader = new StreamReader(httpResponseLine.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);
                    foreach (JObject config in details["value"])
                    {
                        var ImLine = new ImprestMemoNonStaff();
                        ImLine.ImprestMemoNo = (string)config["ImprestMemoNo"];
                        ImLine.Names = (string)config["Names"];
                        ImLine.Designation = (string)config["Designation"];
                        ImLine.Organization = (string)config["OrganizationInstitution"];
                        ImLine.LineNo = (string)config["LineNo"];
                        ImpLines.Add(ImLine);
                    }
                }

                #endregion

                var Lines = new ImprestMemoNonStaffList
                {
                    Status = Status,
                    ListOfNonstaff = ImpLines
                };
                return PartialView("~/Views/ImprestMemo/ImprestMemoNonStaffList.cshtml", Lines);
            }
            catch (Exception ex)
            {
                var erroMsg = new Error();
                erroMsg.Message = ex.Message.Replace("'", "");
                return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
            }
        }

        public ActionResult NewCostLine(string docNo)
        {
            try
            {
                var CostLine = new OtherCosts();

                var UserID = Session["UserID"].ToString();
                var StaffNo = Session["Username"].ToString();
                var employeeView = Session["EmployeeData"] as EmployeeView;


                #region VoteItems
                var itemList = new List<DropdownList>();
                var pageItem = "ReceiptsandPaymentTypes?$format=json";

                var httpResponseItem = Credentials.GetOdataData(pageItem);
                using (var streamReader = new StreamReader(httpResponseItem.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    var details = JObject.Parse(result);
                    foreach (var jToken in details["value"])
                    {
                        var config1 = (JObject)jToken;
                        var dropdownList = new DropdownList
                        {
                            Text = (string)config1["Code"] + " - " + (string)config1["Description"],
                            Value = (string)config1["Code"]
                        };
                        itemList.Add(dropdownList);
                    }
                }

                #endregion

                CostLine.ListOfVoteItems = itemList.Select(x =>
                      new SelectListItem
                      {
                          Text = x.Text,
                          Value = x.Value
                      }).ToList();



                return PartialView(CostLine);
            }
            catch (Exception ex)
            {
                var erroMsg = new Error();
                erroMsg.Message = ex.Message.Replace("'", "");
                return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
            }
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult SubmitCostLine(string DocNo, OtherCosts CostLine)
        {
            try
            {

                var UserID = Session["UserID"].ToString();
                var StaffNo = Session["Username"].ToString();
                var employeeView = Session["EmployeeData"] as EmployeeView;
                int res = Credentials.ObjNav.InsertOtherCost(
                   DocNo,
                   CostLine.Type_of_Expense,
                   CostLine.Unit_Cost,
                    CostLine.Required_For,
                    0,
                     CostLine.No_of_Days
                 );
                if (res == 0)
                {

                    return Json(new { message = "Record Not Added. Try again", success = false }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { message = "Record Added successfully", success = true }, JsonRequestBehavior.AllowGet);


                }


            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message.Replace("'", ""), success = false },
                    JsonRequestBehavior.AllowGet);
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult DeleteCostLine(string DocNo, string Line_No)
        {
            try
            {

                var UserID = Session["UserID"].ToString();
                var StaffNo = Session["Username"].ToString();
                var employeeView = Session["EmployeeData"] as EmployeeView;
                string res = "";
                res = Credentials.ObjNav.DeleteOtherCost(DocNo, int.Parse(Line_No));

                return Json(new { message = "Record deleted successfully", success = true }, JsonRequestBehavior.AllowGet);


            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message.Replace("'", ""), success = false },
                    JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult DeleteStandingImprestLine(string DocNo, int LineNo)
        {
            try
            {

                var UserID = Session["UserID"].ToString();
                var StaffNo = Session["Username"].ToString();
                var employeeView = Session["EmployeeData"] as EmployeeView;
                bool res = false;
                res = Credentials.ObjNav.DeleteStandingImprestLine(
                    LineNo,
                    DocNo
                 );
                if (res)
                {
                    var DocNetAmount = GetImpDocNetAmount(DocNo);
                    return Json(new { NetAmout = DocNetAmount, message = "Record Deleted successfully", success = true }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var DocNetAmount = GetImpDocNetAmount(DocNo);
                    return Json(new { NetAmout = DocNetAmount, message = "Error Deleting record Added", success = false }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message.Replace("'", ""), success = false },
                    JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult SendStandingImprestAppForApproval(string DocNo)
        {
            try
            {
                var UserID = Session["UserID"].ToString();
                var StaffNo = Session["Username"].ToString();
                var employeeView = Session["EmployeeData"] as EmployeeView;
                Credentials.ObjNav.SendDocForApprval( DocNo);

                Credentials.ObjNav.UpdateApprovalEntrySenderID(57000, DocNo, UserID);
                return Json(new { message = "Document successfully sent for approval ", success = true },
                    JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message.Replace("'", ""), success = false },
                    JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult CancelStandingImprestAppForApproval(string DocNo)
        {
            try
            {
                var UserID = Session["UserID"].ToString();
                var StaffNo = Session["Username"].ToString();
                var employeeView = Session["EmployeeData"] as EmployeeView;

                Credentials.ObjNav.CancelDocapproval(DocNo);
                return Json(new { message = "ImprestMemo Requisition approval cancelled Successfully", success = true },
                    JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message.Replace("'", ""), success = false },
                    JsonRequestBehavior.AllowGet);
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult SubmitStandingImprestItem(string DocNo, ImprestMemoLines ImprestMemoLine)
        {
            try
            {
                var StaffNo = Session["Username"].ToString();
                var item = ImprestMemoLine.Item.Trim();
                var itemDesc = ImprestMemoLine.ItemDesc.Trim();
                var amnt = ImprestMemoLine.Amount.Trim();
                //   string noofdays = ImprestMemoLine.Quantity.Trim();

                string UoN = "", Destination = "";

                // Credentials.ObjNav.InsertImprestMemoItems(DocNo, item, Convert.ToDecimal(amnt), StaffNo, Convert.ToDecimal(ImprestMemoLine.Quantity), UoN, itemDesc);
                var DocNetAmount = GetImpDocNetAmount(DocNo);
                return Json(
                    new { NetAmout = DocNetAmount, message = "ImprestMemo Line Added successfully", success = true },
                    JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message.Replace("'", ""), success = false },
                    JsonRequestBehavior.AllowGet);
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult SubmitStandingImprestNonStaff(string DocNo, string Names, string Organization, string Designation)
        {
            try
            {
                var StaffNo = Session["Username"].ToString();


                //  Credentials.ObjNav.InsertImprestMemoNonStaff(DocNo, Names, Organization, Designation);
                var DocNetAmount = GetImpDocNetAmount(DocNo);
                return Json(
                    new { NetAmout = DocNetAmount, message = "ImprestMemo Line Added successfully", success = true },
                    JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message.Replace("'", ""), success = false },
                    JsonRequestBehavior.AllowGet);
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult UpdateStandingImprestLine(string DocNo, ImprestMemoLines ImprestMemoLine)
        {
            try
            {
                var StaffNo = Session["Username"].ToString();
                var item = ImprestMemoLine.Item.Trim();
                var itemDesc = ImprestMemoLine.ItemDesc.Trim();
                var amnt = ImprestMemoLine.Amount.Trim();
                var LnNo = Convert.ToInt32(ImprestMemoLine.LnNo.Trim());
                var noofdays = ImprestMemoLine.Quantity.Trim();

                //  Credentials.ObjNav.ImprestMemoRequisitionLinesUpdate(DocNo, LnNo, Convert.ToDecimal(amnt), Convert.ToDecimal(ImprestMemoLine.Quantity), itemDesc, Convert.ToInt32(ImprestMemoLine.Quantity));
                var DocNetAmount = GetImpDocNetAmount(DocNo);
                return Json(
                    new { NetAmout = DocNetAmount, message = "ImprestMemo Line updated successfully", success = true },
                    JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message.Replace("'", ""), success = false },
                    JsonRequestBehavior.AllowGet);
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public PartialViewResult EditStandingImprestLine(string LnNo, string DocNo)
        {
            try
            {
                var ln = Convert.ToInt32(LnNo);

                #region ImprestMemo Lines

                var ImLine = new ImprestMemoLines();
                var pageLine = "ImprestMemoLines?$filter=No eq '" + DocNo + "' and Line_No eq " + ln + "&$format=json";
                var httpResponseLine = Credentials.GetOdataData(pageLine);
                using (var streamReader = new StreamReader(httpResponseLine.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);
                    foreach (JObject config in details["value"])
                    {
                        ImLine.DocNo = (string)config["No"];
                        ImLine.AdvanceType = (string)config["Advance_Type"];
                        ImLine.Item = (string)config["Account_No"];
                        ImLine.ItemDesc = (string)config["Account_Name"];
                        ImLine.ItemDesc2 = (string)config["Purpose"];
                        ImLine.LnNo = (string)config["Line_No"];
                        ImLine.UoN = (string)config["Unit_of_Measure"];
                        ImLine.Quantity = (string)config["Quantity"];
                        ImLine.UnitAmount = Convert.ToDecimal((string)config["Daily_Rate_Amount"]).ToString("#,##0.00");
                        ImLine.Amount = Convert.ToDecimal((string)config["Amount"]).ToString("#,##0.00");
                    }
                }

                #endregion

                #region ImprestMemo Type List

                var ImprestMemoTList = new List<ImprestMemoTypes>();
                var page = "ImprestTypes?$filter=Description ne ''&$format=json";

                var httpResponse = Credentials.GetOdataData(page);
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);

                    foreach (JObject config in details["value"])
                    {
                        var impList = new ImprestMemoTypes();
                        impList.Code = (string)config["Code"];
                        impList.Description = (string)config["Description"];
                        ImprestMemoTList.Add(impList);
                    }
                }

                #endregion

                #region UoM

                var UoMList = new List<DropdownList>();
                var pageUoM = "UnitOfMeasure?$format=json";

                var httpResponseUoM = Credentials.GetOdataData(pageUoM);
                using (var streamReader = new StreamReader(httpResponseUoM.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);


                    foreach (JObject config in details["value"])
                    {
                        var UoM = new DropdownList();
                        UoM.Value = (string)config["Code"];
                        UoM.Text = (string)config["Description"];
                        UoMList.Add(UoM);
                    }
                }

                #endregion

                #region Destinations

                var DestList = new List<DropdownList>();
                var pageDest = "DestinationList?$format=json";

                var httpResponseDest = Credentials.GetOdataData(pageDest);
                using (var streamReader = new StreamReader(httpResponseDest.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);


                    foreach (JObject config in details["value"])
                    {
                        var Dest = new DropdownList();
                        Dest.Value = (string)config["DestinationCode"];
                        Dest.Text = (string)config["DestinationName"];
                        DestList.Add(Dest);
                    }
                }

                #endregion

                var itemDetails = new ImprestMemoItemDetails
                {
                    ItemDetails = ImLine,
                    ListOfImprestTypes = ImprestMemoTList.Select(x =>
                        new SelectListItem
                        {
                            Text = x.Description,
                            Value = x.Code
                        }).ToList(),
                    ListOfUoM = UoMList.Select(x =>
                        new SelectListItem
                        {
                            Text = x.Text,
                            Value = x.Value
                        }).ToList(),
                    ListOfDestination = DestList.Select(x =>
                        new SelectListItem
                        {
                            Text = x.Text,
                            Value = x.Value
                        }).ToList()
                };
                return PartialView("~/Views/ImprestMemo/ImprestMemoEditItemForm.cshtml", itemDetails);
            }
            catch (Exception ex)
            {
                var erroMsg = new Error();
                erroMsg.Message = ex.Message;
                return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult RemoveStandingImprestLine(string DocNo, string LnNo)
        {
            try
            {
                //  Credentials.ObjNav.ImprestMemoRequsitionRemoveLine(Convert.ToInt32(LnNo), DocNo);
                var DocNetAmount = GetImpDocNetAmount(DocNo);
                return Json(
                    new { NetAmout = DocNetAmount, message = "ImprestMemo Line removed successfully", success = true },
                    JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message.Replace("'", ""), success = false },
                    JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult RemoveStandingImprestNonstaff(string DocNo, string LnNo)
        {
            try
            {
                //Credentials.ObjNav.DeleteImprestMemoNonStaff( DocNo, Convert.ToInt32(LnNo));
                var DocNetAmount = GetImpDocNetAmount(DocNo);
                return Json(
                    new { NetAmout = DocNetAmount, message = "ImprestMemo Line removed successfully", success = true },
                    JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message.Replace("'", ""), success = false },
                    JsonRequestBehavior.AllowGet);
            }
        }

        protected string GetImpDocNetAmount(string DocNo)
        {
            var amount = "";
            var page = "SafariImprest?$select=Total_Subsistence_Allowance&$filter=No eq '" + DocNo + "'&$format=json";
            var httpResponse = Credentials.GetOdataData(page);
            using var streamReader = new StreamReader(httpResponse.GetResponseStream());
            var result = streamReader.ReadToEnd();

            var details = JObject.Parse(result);
            if (details["value"].Count() > 0)
                foreach (JObject config in details["value"])
                    amount = Convert.ToDecimal((string)config["TotalNetAmount"]).ToString("#,##0.00");

            return amount;
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public PartialViewResult FileUploadForm()
        {
            return PartialView("~/Views/ImprestMemo/FileAttachmentForm.cshtml");
        }

        public PartialViewResult NewStandingImprestItem()
        {
            try
            {
                var locationList = new LocationList();


                #region Items List

                var ddlList = new List<DropdownList>();
                var page = "Item_List?$&orderby=Description&format=json";

                var httpResponse = Credentials.GetOdataData(page);
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();

                    var details = JObject.Parse(result);


                    foreach (JObject config in details["value"])
                    {
                        var dll = new DropdownList();
                        dll.Value = (string)config["No"];
                        dll.Text = (string)config["Description"];
                        ddlList.Add(dll);
                    }
                }

                #endregion

                locationList = new LocationList
                {
                    ListOfLocations = ddlList.Select(x =>
                        new SelectListItem
                        {
                            Text = x.Text,
                            Value = x.Value
                        }).ToList()
                };
                return PartialView("~/Views/ImprestMemo/ImprestMemoItemsAdd.cshtml", locationList);
            }
            catch (Exception ex)
            {
                var erroMsg = new Error();
                erroMsg.Message = ex.Message;
                return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
            }
        }

        public PartialViewResult NewStandingImprestNonStaff()
        {
            try
            {
                return PartialView("~/Views/ImprestMemo/ImprestMemoNonStaffAdd.cshtml");
            }
            catch (Exception ex)
            {
                var erroMsg = new Error();
                erroMsg.Message = ex.Message;
                return PartialView("~/Views/Shared/Partial Views/ErroMessangeView.cshtml", erroMsg);
            }
        }

        public string GetEmployeeJobID(string StaffNo)
        {
            try
            {
                var page = $"EmployeeHRCard?$filter=No eq '{StaffNo}'&$format=json";
                var httpResponse = Credentials.GetOdataData(page);
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    var details = JObject.Parse(result);

                    var items = details["value"] as JArray;
                    if (items != null && items.Count > 0)
                    {
                        var salaryScale = items[0]["Salary_Scale"]?.ToString();
                        return salaryScale ?? string.Empty;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in GetEmployeeJobID: " + ex.Message);
            }

            return string.Empty;
        }


        public JsonResult WorkplansJson(string Shortcut_Dimension_1_Code, string Shortcut_Dimension_2_Code, string Strategic_Plan, string Reporting_Year_Code)
        {
            try
            {
                List<object> workplan = new List<object>();

                var pageWorkplanActivities = $"DraftWorkPlans?$filter=Global_Dimension_1_Code eq '{Shortcut_Dimension_1_Code}' and Global_Dimension_2_Code eq '{Shortcut_Dimension_2_Code}' and Strategy_Plan_ID eq '{Strategic_Plan}' and Year_Reporting_Code eq '{Reporting_Year_Code}'&$format=json";

                HttpWebResponse httpResponse = Credentials.GetOdataData(pageWorkplanActivities);
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    var details = JObject.Parse(result);

                    foreach (JObject config in details["value"])
                    {
                        workplan.Add(new
                        {
                            Code = (string)config["No"],
                            Descriptions = (string)config["Description"]
                        });
                    }
                }

                return Json(workplan, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult WorkplanActivitiesJson(string WorkPlanCode)
        {
            try
            {
                List<object> workplan = new List<object>();

                var pageWorkplanActivities = $"StrategyWorkplanLines?$filter=No eq '{WorkPlanCode}'&$format=json";

                HttpWebResponse httpResponse = Credentials.GetOdataData(pageWorkplanActivities);
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    var details = JObject.Parse(result);

                    foreach (JObject config in details["value"])
                    {
                        workplan.Add(new
                        {
                            Code = (string)config["Resource_Req_No"],
                            Descriptions = (string)config["Description"]
                        });
                    }
                }

                return Json(workplan, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult BankBranchesJson(string BankCode)
        {
            try
            {
                List<object> branches = new List<object>();

                var pageWorkplanActivities = $"BankBranches?$filter=BankCode eq '{BankCode}'&$format=json";

                HttpWebResponse httpResponse = Credentials.GetOdataData(pageWorkplanActivities);
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    var details = JObject.Parse(result);

                    foreach (JObject config in details["value"])
                    {
                        branches.Add(new
                        {
                            Code = (string)config["BranchCode"],
                            Descriptions = (string)config["BranchName"]
                        });
                    }
                }

                return Json(branches, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }




    }
}