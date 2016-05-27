﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using SaveMyDate.Entities;

namespace CompanySimulator
{
    public class FreeAppointmentFinder
    {
        public List<FreeAppointmentCompany> FindFreeAppointmentByDay(DateTime dayToScedual, CompanyType companyType, string companySubType, string location)
        {
            List<FreeAppointmentCompany> freeAppointmentCompany;

            switch (companyType)
            {
                case CompanyType.Banks:
                {
                    freeAppointmentCompany = GetData("Bank", companySubType, dayToScedual, location);
                    break;
                }
                case CompanyType.MedicalClinic:
                {
                    freeAppointmentCompany = GetData("MedicalClinic", companySubType, dayToScedual, location);
                    break;
                }
                case CompanyType.PostOffice:
                {
                    freeAppointmentCompany = GetData("PostOffice", companySubType, dayToScedual, location);
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException(nameof(companyType), companyType, null);                  
            }

            return freeAppointmentCompany;
        }

        private List<FreeAppointmentCompany> GetData(string controllerName, string subType, DateTime dayToScedual, string location)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:61820/");

            // Add an Accept header for JSON format.
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = client.GetAsync("api/" + controllerName + "?SubType=" + subType + "&Location=" + location).Result;

            if (response.IsSuccessStatusCode)
            {
                var freeAppointmentCompanies = response.Content.ReadAsAsync<List<FreeAppointmentCompany>>().Result;
                foreach (var freeAppointmentCompany in freeAppointmentCompanies)
                {
                    freeAppointmentCompany.freeAppointments.RemoveAll(x => x.StartTime.ToShortDateString() != dayToScedual.ToShortDateString());
                }

                return freeAppointmentCompanies;

            }
            else
            {
                //MessageBox.Show("Error Code" +
                //response.StatusCode + " : Message - " + response.ReasonPhrase);
            }
            return new List<FreeAppointmentCompany>();
        }
    }
}