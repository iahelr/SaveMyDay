﻿using System.Collections.Generic;
using System.Web.Http;
using SaveMyDate.Entities;
using SaveMyDay.Algoritem;
using System.Web.Http.Cors;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using CompanySimulator;
using SaveMyDay.DistancesMatrix;

namespace PathFinder.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class PathCalculatorController : ApiController
    {
        [HttpPost]
        public object PathCalculator(JObject jsonParam)
        {
            // Example of parsing paramters.
            List<Constraint> constraintList = jsonParam["events"].ToObject<List<Constraint>>();
            string city = jsonParam["appointmentsCity"].ToObject<string>();
            string travel = jsonParam["travelWay"].ToObject<string>();
            DateTime selectedDate = jsonParam["selectedDate"].ToObject<DateTime>();
            List<JObject> appointmentsJobj = jsonParam["selectedAppointments"].ToObject<List<JObject>>();

            List<string[]> errands = new List<string[]>();
            List<CompanySubType> errandsForAlgo = new List<CompanySubType>();
            foreach (JObject appointment in appointmentsJobj)
            {
                string type = appointment["companyType"].ToObject<String>();
                string subType = appointment["SubType"].ToObject<String>();
                errands.Add(new string[] { type, subType } );
                errandsForAlgo.Add((CompanySubType)Enum.Parse(typeof(CompanySubType), subType));
            }

            // this is the call to company simulator for dror, then is the call to algorithem,need to fill up parameters.
            var appointments = FindAppointments(errands, selectedDate, city);
            var matrixDictionary = DistancesMatrixHandler.BuildDistancesMatrix(city, errandsForAlgo, constraintList);
            var algoritemRunner = new AlgoritemRunner();
            var succeed = algoritemRunner.Activate(errandsForAlgo, constraintList, appointments, matrixDictionary);


            List<Path> resultPaths = new List<Path>();
            if (succeed)
            {
                resultPaths = algoritemRunner.Results.ToList().GetRange(0, Math.Min(3, algoritemRunner.Results.Count));
                for (int i = 0; i < resultPaths.Count; i++)
                {
                    resultPaths[i].Appointments.Reverse();
                }
            }
            var paths = new List<Path>();

           // Send to the client
            var result = new { paths = resultPaths, DoesSuccedded = succeed };
            return Json(result);
        }

        private Dictionary<CompanySubType, List<Appointment>> FindAppointments(List<string[]> errands, DateTime selectedDate, string citySelected)
        {
            var freeAppointmentFinder = new FreeAppointmentFinder();
            var dbCompanyList = new List<DbAppointmentCompany>();
            var appointmentDictonary = new Dictionary<CompanySubType, List<Appointment>>();

            foreach (string[] errand in errands)
            {
                dbCompanyList.AddRange(freeAppointmentFinder.FindFreeAppointmentByDay(selectedDate, (CompanyType) Enum.Parse(typeof(CompanyType), errand[0]), (CompanySubType)Enum.Parse(typeof(CompanySubType), errand[1]), citySelected));
            }

            foreach (var dbCompany in dbCompanyList)
            {
                var appointmentList = dbCompany.ConvertToAppointments();
                if (appointmentDictonary.ContainsKey(dbCompany.Company.SubType))
                {
                    appointmentDictonary[dbCompany.Company.SubType].AddRange(appointmentList);
                }
                else
                {
                    appointmentDictonary.Add(dbCompany.Company.SubType, appointmentList);
                }
            }
          
            return appointmentDictonary;
        }
    }
}
